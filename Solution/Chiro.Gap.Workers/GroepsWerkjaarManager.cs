// <copyright company="Chirojeugd-Vlaanderen vzw">
// Copyright (c) 2007-2010
// Mail naar informatica@chiro.be voor alle info over deze broncode
// </copyright>

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;

using Chiro.Gap.Domain;
using Chiro.Gap.Orm;
using Chiro.Gap.Orm.DataInterfaces;
using Chiro.Gap.Workers.Exceptions;

namespace Chiro.Gap.Workers
{
	/// <summary>
	/// Worker die alle businesslogica i.v.m. groepswerkjaren bevat
	/// </summary>
	public class GroepsWerkJaarManager
	{
		private readonly IGroepsWerkJaarDao _groepsWjDao;
		private readonly IGroepenDao _groepenDao;
		private readonly IAutorisatieManager _autorisatieMgr;
		private readonly IAfdelingenDao _afdelingenDao;

		/// <summary>
		/// Creëert een GroepsWerkJaarManager
		/// </summary>
		/// <param name="groepsWjDao">Repository voor groepswerkjaren</param>
		/// <param name="groepenDao">Repository voor groepen</param>
		/// <param name="afdelingenDao">Repository voor afdelingen</param>
		/// <param name="autorisatieMgr">Worker die autorisatie regelt</param>
		public GroepsWerkJaarManager(
			IGroepsWerkJaarDao groepsWjDao,
			IGroepenDao groepenDao,
			IAfdelingenDao afdelingenDao,
			IAutorisatieManager autorisatieMgr)
		{
			_groepsWjDao = groepsWjDao;
			_groepenDao = groepenDao;
			_autorisatieMgr = autorisatieMgr;
			_afdelingenDao = afdelingenDao;
		}

		/// <summary>
		/// Haalt het groepswerkjaar op bij een gegeven <paramref name="groepsWerkJaarID"/>
		/// </summary>
		/// <param name="groepsWerkJaarID">ID van het gevraagde GroepsWerkJaar</param>
		/// <param name="extras">Bepaalt op te halen gerelateerde entiteiten</param>
		/// <returns>Gevraagde groepswerkjaar</returns>
		public GroepsWerkJaar Ophalen(int groepsWerkJaarID, GroepsWerkJaarExtras extras)
		{
			if (_autorisatieMgr.IsGavGroepsWerkJaar(groepsWerkJaarID))
			{
				GroepsWerkJaar resultaat = _groepsWjDao.Ophalen(
					groepsWerkJaarID,
					ExtrasNaarLambdas(extras));

				return resultaat;
			}
			else
			{
				throw new GeenGavException(Properties.Resources.GeenGav);
			}
		}

		/// <summary>
		/// Haalt de afdelingen van een groep op die niet gebruikt zijn in een gegeven 
		/// groepswerkjaar, op basis van een <paramref name="groepsWerkJaarID"/>
		/// </summary>
		/// <param name="groepsWerkJaarID">ID van het groepswerkjaar waarvoor de niet-gebruikte afdelingen
		/// opgezocht moeten worden.</param>
		/// <returns>De ongebruikte afdelingen van een groep in het gegeven groepswerkjaar</returns>
		public IList<Afdeling> OngebruikteAfdelingenOphalen(int groepsWerkJaarID)
		{
			return _afdelingenDao.OngebruikteOphalen(groepsWerkJaarID);
		}

		/// <summary>
		/// Maakt een nieuw afdelingsjaar op basis van groepswerkjaar,
		/// afdeling en officiële afdeling.
		/// </summary>
		/// <param name="gwj">Groepswerkjaar voor afdelingsjaar</param>
		/// <param name="afd">Afdeling voor afdelingswerkjaar</param>
		/// <param name="oa">Corresponderende officiële afdeling voor afd</param>
		/// <param name="jaarVan">Startpunt interval geboortejaren</param>
		/// <param name="jaarTot">Eindpunt interval geboortejaren</param>
		/// <returns>Afdelingsjaar met daaraan gekoppeld groepswerkjaar
		/// , afdeling en officiële afdeling.</returns>
		/// <remarks>gwj.Groep en afd.Groep mogen niet null zijn</remarks>
		public AfdelingsJaar AfdelingsJaarMaken(GroepsWerkJaar gwj, Afdeling afd, OfficieleAfdeling oa, int jaarVan, int jaarTot)
		{
			if (!_autorisatieMgr.IsGavGroepsWerkJaar(gwj.ID))
			{
				throw new GeenGavException(Properties.Resources.GeenGav);
			}
			if (!_autorisatieMgr.IsGavAfdeling(afd.ID))
			{
				throw new GeenGavException(Properties.Resources.GeenGav);
			}

			Debug.Assert(gwj.Groep != null);
			Debug.Assert(afd.Groep != null);

			// FIXME: Eigenlijk zou onderstaande if ook moeten
			// werken zonder de .ID's, want ik heb equals geoverload.
			// Maar dat is blijkbaar niet zo evident.

			if (!gwj.Groep.Equals(afd.Groep))
			{
				throw new GapException(
						FoutNummer.AfdelingNietVanGroep,
						"De afdeling is niet gekoppeld aan de groep van het groepswerkjaar.");
			}

			var resultaat = new AfdelingsJaar();

			resultaat.GeboorteJaarVan = jaarVan;
			resultaat.GeboorteJaarTot = jaarTot;

			resultaat.GroepsWerkJaar = gwj;
			resultaat.Afdeling = afd;
			resultaat.OfficieleAfdeling = oa;

			gwj.AfdelingsJaar.Add(resultaat);
			afd.AfdelingsJaar.Add(resultaat);
			oa.AfdelingsJaar.Add(resultaat);

			return resultaat;
		}

		/// <summary>
		/// Haalt recentste groepswerkjaar voor een groep op.
		/// </summary>
		/// <param name="groepID">ID gevraagde groep</param>
		/// <returns>Het recentste Groepswerkjaar voor de opgegeven groep</returns>
		public GroepsWerkJaar RecentsteOphalen(int groepID)
		{
			return RecentsteOphalen(groepID, GroepsWerkJaarExtras.Geen);
		}

		/// <summary>
		/// Haalt recentste groepswerkjaar voor een groep op.
		/// </summary>
		/// <param name="groepID">ID gevraagde groep</param>
		/// <param name="extras">Bepaalt eventuele mee op te halen gekoppelde entiteiten</param>
		/// <returns>Het recentste Groepswerkjaar voor de opgegeven groep</returns>
		public GroepsWerkJaar RecentsteOphalen(int groepID, GroepsWerkJaarExtras extras)
		{
			if (_autorisatieMgr.IsGavGroep(groepID))
			{
				// TODO: cachen, want dit gaan we veel nodig hebben (Zie #251)
				return _groepsWjDao.RecentsteOphalen(groepID, ExtrasNaarLambdas(extras));
			}
			else
			{
				throw new GeenGavException(Properties.Resources.GeenGav);
			}
		}

		/// <summary>
		/// Bepaalt ID van het recentste GroepsWerkJaar gemaakt voor een
		/// gegeven groep.
		/// </summary>
		/// <param name="groepID">ID van Groep</param>
		/// <returns>ID van het recentste GroepsWerkJaar</returns>
		public int RecentsteGroepsWerkJaarIDGet(int groepID)
		{
			if (_autorisatieMgr.IsGavGroep(groepID))
			{
				// TODO: cachen, want dit gaan we veel nodig hebben (Zie #251)
				return _groepsWjDao.RecentsteOphalen(groepID).ID;
			}
			else
			{
				throw new GeenGavException(Properties.Resources.GeenGav);
			}
		}

		// WTF???
		private int compare(int dag1, int maand1, int dag2, int maand2)
		{
			if (maand1 < maand2 || (maand1 == maand2 && dag1 < dag2))
			{
				return -1;
			}
			else
			{
				if (maand1 > maand2 || (maand1 == maand2 && dag1 > dag2))
				{
					return 1;
				}
				else
				{
					return 0;
				}
			}
		}

		/// <summary>
		/// Converteert de GroepsWerkJaarExtras <paramref name="extras"/> naar lambda-expressies die mee naar 
		/// de data access moeten om de extra's daadwerkelijk op te halen.
		/// </summary>
		/// <param name="extras">Te converteren groepsextra's</param>
		/// <returns>Lambda-expressies geschikt voor onze DAO's</returns>
		private Expression<Func<GroepsWerkJaar, object>>[] ExtrasNaarLambdas(GroepsWerkJaarExtras extras)
		{
			var paths = new List<Expression<Func<GroepsWerkJaar, object>>>();

			if ((extras & GroepsWerkJaarExtras.Afdelingen) != 0)
			{
				paths.Add(gwj => gwj.AfdelingsJaar.First().Afdeling);
				paths.Add(gwj => gwj.AfdelingsJaar.First().OfficieleAfdeling);
			}

			if ((extras & GroepsWerkJaarExtras.LidFuncties) != 0)
			{
				paths.Add(gwj => gwj.Lid.First().Functie);
				paths.Add(gwj => gwj.AfdelingsJaar.First().Kind.First().Functie);
				paths.Add(gwj => gwj.AfdelingsJaar.First().Leiding.First().Functie);
			}
			else if ((extras & GroepsWerkJaarExtras.Leden) != 0)
			{
				paths.Add(gwj => gwj.AfdelingsJaar.First().Kind);
				paths.Add(gwj => gwj.AfdelingsJaar.First().Leiding);
			}

			if ((extras & GroepsWerkJaarExtras.GroepsFuncties) != 0)
			{
				paths.Add(gwj => gwj.Groep.Functie);
			}

			return paths.ToArray();
		}
	}
}
