// <copyright company="Chirojeugd-Vlaanderen vzw">
// Copyright (c) 2007-2010
// Mail naar informatica@chiro.be voor alle info over deze broncode
// </copyright>

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

using Chiro.Cdf.Data;
using Chiro.Gap.Data.Ef;
using Chiro.Gap.Fouten;
using Chiro.Gap.Fouten.Exceptions;
using Chiro.Gap.Orm;
using Chiro.Gap.Orm.DataInterfaces;
using Chiro.Gap.Workers.Properties;

namespace Chiro.Gap.Workers
{
	/// <summary>
	/// Worker die alle businesslogica i.v.m. afdelingsjaren bevat
	/// </summary>
	public class AfdelingsJaarManager
	{
		private IAfdelingsJarenDao _dao;
		private IAutorisatieManager _autorisatieMgr;

		/// <summary>
		/// Deze constructor laat toe om een alternatieve repository voor
		/// de groepen te gebruiken.  Nuttig voor mocking en testing.
		/// </summary>
		/// <param name="dao">Alternatieve dao</param>
		/// <param name="autorisatieMgr">Alternatieve autorisatiemanager</param>
		public AfdelingsJaarManager(IAfdelingsJarenDao dao, IAutorisatieManager autorisatieMgr)
		{
			_dao = dao;
			_autorisatieMgr = autorisatieMgr;
		}

		/// <summary>
		/// Op basis van een ID een afdelingsjaar ophalen
		/// </summary>
		/// <param name="afdelingsJaarID">De ID van het afdelingsjaar</param>
		/// <param name="extras">Bepaalt welke gerelateerde entiteiten mee opgehaald moeten worden.</param>
		/// <returns>Het afdelingsjaar met de opgegeven ID</returns>
		public AfdelingsJaar Ophalen(int afdelingsJaarID, AfdelingsJaarExtras extras)
		{
			var paths = new List<Expression<Func<AfdelingsJaar, object>>>();

			if ((extras & AfdelingsJaarExtras.Afdeling) != 0)
			{
				paths.Add(aj => aj.Afdeling);
			}
			if ((extras & AfdelingsJaarExtras.GroepsWerkJaar) != 0)
			{
				paths.Add(aj => aj.GroepsWerkJaar);
			}
			if ((extras & AfdelingsJaarExtras.Leden) != 0)
			{
				paths.Add(aj => aj.Kind);
				paths.Add(aj => aj.Leiding);
			}
			if ((extras & AfdelingsJaarExtras.OfficieleAfdeling) != 0)
			{
				paths.Add(aj => aj.OfficieleAfdeling);
			}

			if (_autorisatieMgr.IsGavAfdelingsJaar(afdelingsJaarID))
			{
				return _dao.Ophalen(afdelingsJaarID, paths.ToArray());
			}
			else
			{
				throw new GeenGavException(GeenGavFoutCode.Groep, Resources.GeenGavGroep);
			}
		}

		/// <summary>
		/// Verwijdert AfdelingsJaar uit database
		/// </summary>
		/// <param name="afdelingsJaarID">ID van het AfdelingsJaar</param>
		/// <returns><c>True</c> on successful</returns>
		public bool Verwijderen(int afdelingsJaarID)
		{
			AfdelingsJaar aj = _dao.Ophalen(afdelingsJaarID, a => a.Afdeling, a => a.Leiding, a => a.Kind);

			if (_autorisatieMgr.IsGavAfdeling(aj.Afdeling.ID))
			{
				if (aj.Kind.Count != 0 || aj.Leiding.Count != 0)
				{
					throw new InvalidOperationException("AfdelingsJaar kan niet verwijderd worden omdat er nog leden of leiding in deze afdeling zitten.");
				}
				else
				{
					aj.TeVerwijderen = true;
					_dao.Bewaren(aj);
					return true;
				}
			}
			else
			{
				throw new GeenGavException(GeenGavFoutCode.Groep, Resources.GeenGavGroep);
			}
		}

		/// <summary>
		/// Het opgegeven afdelingsjaar opslaan
		/// </summary>
		/// <param name="aj">Het afdelingsjaar dat opgeslagen moet worden</param>
		public void Bewaren(AfdelingsJaar aj)
		{
			if (_autorisatieMgr.IsGavAfdeling(aj.Afdeling.ID))
			{
				_dao.Bewaren(aj);
			}
			else
			{
				throw new GeenGavException(GeenGavFoutCode.Lid, Properties.Resources.GeenGavLid);
			}
		}

		/// <summary>
		/// Wijzigt de property's van <paramref name="afdelingsJaar"/>
		/// </summary>
		/// <param name="afdelingsJaar">Te wijzigen afdelingsjaar</param>
		/// <param name="officieleAfdeling">Officiele afdeling</param>
		/// <param name="geboorteJaarVan">Ondergrens geboortejaar</param>
		/// <param name="geboorteJaarTot">Bovengrens geboortejaar</param>
		/// <param name="geslachtsType">Jongensafdeling, meisjesafdeling of gemengde afdeling</param>
		/// <param name="versieString">Versiestring uit database voor concurrency controle</param>
		/// <remarks>Groepswerkjaar en afdeling kunnen niet gewijzigd worden.  Verwijder hiervoor het
		/// afdelingsjaar, en maak een nieuw.</remarks>
		public void Wijzigen(
			AfdelingsJaar afdelingsJaar, 
			OfficieleAfdeling officieleAfdeling, 
			int geboorteJaarVan, 
			int geboorteJaarTot, 
			GeslachtsType geslachtsType, 
			string versieString)
		{
			if (_autorisatieMgr.IsGavAfdelingsJaar(afdelingsJaar.ID))
			{
				if (officieleAfdeling.ID != afdelingsJaar.OfficieleAfdeling.ID)
				{
					// vervang officiele afdeling.  Een beetje gepruts om in de mate van het
					// mogelijke de state van de entity's consistent te houden.

					// verwijder link vorige off. afdeling - afdelingsjaar
					afdelingsJaar.OfficieleAfdeling.AfdelingsJaar.Remove(afdelingsJaar);

					// nieuwe link
					afdelingsJaar.OfficieleAfdeling = officieleAfdeling;
					officieleAfdeling.AfdelingsJaar.Add(afdelingsJaar);
				}
				afdelingsJaar.GeboorteJaarVan = geboorteJaarVan;
				afdelingsJaar.GeboorteJaarTot = geboorteJaarTot;
				afdelingsJaar.Geslacht = geslachtsType;
				afdelingsJaar.VersieString = versieString;
			}
			else
			{
				throw new GeenGavException(GeenGavFoutCode.Afdeling, "Dit is geen afdeling van jouw groep(en).");
			}
		}
	}
}
