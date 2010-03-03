// <copyright company="Chirojeugd-Vlaanderen vzw">
// Copyright (c) 2007-2010
// Mail naar informatica@chiro.be voor alle info over deze broncode
// </copyright>

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

using Chiro.Gap.Fouten.Exceptions;
using Chiro.Gap.Orm;
using Chiro.Gap.Orm.DataInterfaces;
using Chiro.Gap.Workers.Properties;

namespace Chiro.Gap.Workers
{
    /// <summary>
    /// Worker die alle businesslogica i.v.m. groepswerkjaren bevat
    /// </summary>
	public class GroepsWerkJaarManager
	{
		private IGroepsWerkJaarDao _groepsWjDao;
		private IGroepenDao _groepenDao;
		private IAutorisatieManager _autorisatieMgr;
		private IAfdelingenDao _afdelingenDao;

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
		/// samen met alle info over het AfdelingsJaar, de Afdeling, de gelinkte
		/// OfficieleAfdeling, de Kinderen en de Leiding, ...
		/// </summary>
		/// <param name="groepsWerkJaarID">ID van het gevraagde GroepsWerkJaar</param>
		/// <returns>Gevraagde groepswerkjaar</returns>
		public GroepsWerkJaar OphalenMetLeden(int groepsWerkJaarID)
		{
			if (_autorisatieMgr.IsGavGroepsWerkJaar(groepsWerkJaarID))
			{
				GroepsWerkJaar resultaat = _groepsWjDao.Ophalen(
					groepsWerkJaarID,
					gwj => gwj.Groep,
					gwj => gwj.AfdelingsJaar.First().Afdeling,
					gwj => gwj.AfdelingsJaar.First().OfficieleAfdeling,
					gwj => gwj.AfdelingsJaar.First().Kind,
					gwj => gwj.AfdelingsJaar.First().Leiding);

				return resultaat;
			}
			else
			{
				throw new GeenGavException(Resources.GeenGavGroep);
			}
		}

		/// <summary>
		/// Haalt een groepswerkjaar op, samen met gekoppelde afdelingsjaren, afdelingen en officiële afdelingen.
		/// </summary>
		/// <param name="groepsWerkJaarID">ID op te vragen groepswerkjaar</param>
		/// <returns>
		/// Groepswerkjaar, samen met gekoppelde afdelingsjaren, afdelingen en officiële afdelingen
		/// </returns>
		public GroepsWerkJaar OphalenMetAfdelingen(int groepsWerkJaarID)
		{
			if (_autorisatieMgr.IsGavGroepsWerkJaar(groepsWerkJaarID))
			{
				GroepsWerkJaar resultaat = _groepsWjDao.Ophalen(
					groepsWerkJaarID,
					gwj => gwj.AfdelingsJaar.First().Afdeling,
					gwj => gwj.AfdelingsJaar.First().OfficieleAfdeling);
				return resultaat;
			}
			else
			{
				throw new GeenGavException(Resources.GeenGavGroepsWerkJaar);
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
				throw new GeenGavException(Properties.Resources.GeenGavGroepsWerkJaar);
			}
			if (!_autorisatieMgr.IsGavAfdeling(afd.ID))
			{
				throw new GeenGavException(Properties.Resources.GeenGavAfdeling);
			}

			Debug.Assert(gwj.Groep != null);
			Debug.Assert(afd.Groep != null);

			// FIXME: Eigenlijk zou onderstaande if ook moeten
			// werken zonder de .ID's, want ik heb equals geoverload.
			// Maar dat is blijkbaar niet zo evident.

			if (!gwj.Groep.Equals(afd.Groep))
			{
				throw new FoutieveGroepException("De afdeling is niet gekoppeld aan de groep van het groepswerkjaar.");
			}

			AfdelingsJaar resultaat = new AfdelingsJaar();

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
		/// Haalt recentste groepswerkjaar voor een groep op, inclusief afdelingsjaren
		/// </summary>
		/// <param name="groepID">ID gevraagde groep</param>
		/// <returns>Het recentste Groepswerkjaar voor de opgegeven groep</returns>
		public GroepsWerkJaar RecentsteGroepsWerkJaarGet(int groepID)
		{
			if (_autorisatieMgr.IsGavGroep(groepID))
			{
				// TODO: cachen, want dit gaan we veel nodig hebben (Zie #251)
				return _groepenDao.RecentsteGroepsWerkJaarGet(groepID);
			}
			else
			{
				throw new GeenGavException(Resources.GeenGavGroep);
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
				return _groepenDao.RecentsteGroepsWerkJaarGet(groepID).ID;
			}
			else
			{
				throw new GeenGavException(Properties.Resources.GeenGavGroep);
			}
		}

		/// <summary>
		/// Haalt het huidige werkjaar op (beginjaar) voor een bepaalde groep
		/// </summary>
		/// <param name="groepID">ID van de groep</param>
		/// <returns>Beginjaar van het huidige werkjaar voor die bepaalde groep</returns>
		public int HuidigWerkJaarGet(int groepID)
		{
			// TODO: Beter documenteren!

			if (_autorisatieMgr.IsGavGroep(groepID))
			{
				var begindatumnieuwwerkjaar = Properties.Settings.Default.WerkjaarStartNationaal;
				var deadlinenieuwwerkjaar = Properties.Settings.Default.WerkjaarVerplichteOvergang;
				var huidigedatum = System.DateTime.Today;

				if (compare(huidigedatum.Day, huidigedatum.Month, begindatumnieuwwerkjaar.Day, begindatumnieuwwerkjaar.Month) < 0)
				{
					return huidigedatum.Year;
				}
				else
				{
					if (compare(deadlinenieuwwerkjaar.Day, deadlinenieuwwerkjaar.Month, huidigedatum.Day, huidigedatum.Month) < 0)
					{
						return huidigedatum.Year;
					}
					else
					{
						int werkjaar = _groepenDao.RecentsteGroepsWerkJaarGet(groepID).WerkJaar;
						Debug.Assert(huidigedatum.Year == werkjaar || werkjaar + 1 == huidigedatum.Year);
						return werkjaar;
					}
				}
			}
			else
			{
				throw new GeenGavException(Properties.Resources.GeenGavGroep);
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
	}
}
