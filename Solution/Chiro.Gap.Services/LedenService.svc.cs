// <copyright company="Chirojeugd-Vlaanderen vzw">
// Copyright (c) 2007-2010
// Mail naar informatica@chiro.be voor alle info over deze broncode
// </copyright>

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.Serialization;
using System.Security.Permissions;
using System.ServiceModel;
using System.Text;

using AutoMapper;

using Chiro.Cdf.Ioc;
using Chiro.Gap.Fouten;
using Chiro.Gap.Fouten.Exceptions;
using Chiro.Gap.Orm;
using Chiro.Gap.ServiceContracts;
using Chiro.Gap.ServiceContracts.FaultContracts;
using Chiro.Gap.ServiceContracts.Mappers;
using Chiro.Gap.Workers;

namespace Chiro.Gap.Services
{
	// OPM: als je de naam van de class "LedenService" hier verandert, moet je ook de sectie "Services" in web.config aanpassen.
	public class LedenService : ILedenService
	{
		#region Manager Injection

		private readonly GelieerdePersonenManager _gelieerdePersonenMgr;
		private readonly GroepenManager _groepenMgr;
		private readonly LedenManager _ledenMgr;
		private readonly FunctiesManager _functiesMgr;
		private readonly AfdelingsJaarManager _afdelingsJaarMgr;

		public LedenService(
			GelieerdePersonenManager gpm, 
			LedenManager lm, 
			GroepenManager grm,
			FunctiesManager fm,
			AfdelingsJaarManager ajm)
		{
			_gelieerdePersonenMgr = gpm;
			_ledenMgr = lm;
			_groepenMgr = grm;
			_functiesMgr = fm;
			_afdelingsJaarMgr = ajm;
		}

		#endregion

		/* zie #273 */
		// [PrincipalPermission(SecurityAction.Demand, Role = SecurityGroepen.Gebruikers)]
		public IEnumerable<int> LedenMakenEnBewaren(IEnumerable<int> gelieerdePersoonIDs)
		{
			String result = String.Empty;
			IList<Lid> leden = new List<Lid>();
			foreach (int gpID in gelieerdePersoonIDs)
			{
				GelieerdePersoon gp = _gelieerdePersonenMgr.DetailsOphalen(gpID);

				try
				{
					Lid l = _ledenMgr.KindMaken(gp);
					leden.Add(l);
				}
				catch (BlokkerendeObjectenException<BestaatAlFoutCode, Lid>)
				{
					/*code is reentrant*/
				}
				catch (OngeldigeActieException ex)
				{
					result += ex.Message + "\n";
				}
			}
			if (!result.Equals(String.Empty))
			{
				throw new FaultException<OngeldigeActieException>(new OngeldigeActieException(result));
			}

			foreach (Lid l in leden)
			{
				_ledenMgr.LidBewaren(l);
			}
			return (from l in leden
					select l.ID).ToList<int>();
		}

		/* zie #273 */
		// [PrincipalPermission(SecurityAction.Demand, Role = SecurityGroepen.Gebruikers)]
		public IEnumerable<int> LeidingMakenEnBewaren(IEnumerable<int> gelieerdePersoonIDs)
		{
			String result = String.Empty;
			IList<Lid> leden = new List<Lid>();
			foreach (int gpID in gelieerdePersoonIDs)
			{
				GelieerdePersoon gp = _gelieerdePersonenMgr.DetailsOphalen(gpID);

				try
				{
					Lid l = _ledenMgr.LeidingMaken(gp);
					leden.Add(l);
				}
				catch (BlokkerendeObjectenException<BestaatAlFoutCode, Lid>)
				{
					/*code is reentrant*/
				}
				catch (OngeldigeActieException ex)
				{
					result += ex.Message + "\n";
				}
			}
			if (!result.Equals(String.Empty))
			{
				throw new OngeldigeActieException(result);
			}

			foreach (Lid l in leden)
			{
				_ledenMgr.LidBewaren(l);
			}
			return (from l in leden
					select l.ID).ToList<int>();
		}

		/// <summary>
		/// Slaat veranderingen op aan de eigenschappen van het lidobject zelf. Creëert of verwijdert geen leden, en leden
		/// kunnen ook niet van werkjaar of van gelieerdepersoon veranderen.
		/// </summary>
		/// <param name="lid">te bewaren lid</param>
		public void Bewaren(LidInfo lidinfo)
		{
			Lid lid = _ledenMgr.Ophalen(lidinfo.LidID, LidExtras.Geen);

			_ledenMgr.InfoOvernemen(lidinfo, lid);
			_ledenMgr.LidBewaren(lid);
		}

		/// <summary>
		/// Verwijdert het lid met ID <paramref name="lidID"/>
		/// </summary>
		/// <param name="lidID">ID van het te verwijderen lid</param>
		public void Verwijderen(int lidID)
		{
			Lid l = _ledenMgr.Ophalen(lidID, LidExtras.Groep | LidExtras.Afdelingen);
			_ledenMgr.Verwijderen(l);	// verwijderen persisteert meteen
		}

		/* zie #273 */
		// [PrincipalPermission(SecurityAction.Demand, Role = SecurityGroepen.Gebruikers)]
		public IList<LidInfo> PaginaOphalen(int groepsWerkJaarID, out int paginas)
		{
			var result = _ledenMgr.PaginaOphalen(groepsWerkJaarID, out paginas);
			return Mapper.Map<IList<Lid>, IList<LidInfo>>(result);
		}

		/* zie #273 */
		// [PrincipalPermission(SecurityAction.Demand, Role = SecurityGroepen.Gebruikers)]
		public IList<LidInfo> PaginaOphalenVolgensAfdeling(int groepsWerkJaarID, int afdelingsID, out int paginas)
		{
			IList<Lid> result = _ledenMgr.PaginaOphalenVolgensAfdeling(groepsWerkJaarID, afdelingsID, out paginas);
			return Mapper.Map<IList<Lid>, IList<LidInfo>>(result);
		}

		/// <summary>
		/// Haalt lid op, inclusief gelieerde persoon en persoon
		/// </summary>
		/// <param name="lidID">ID op te halen lid</param>
		/// <param name="extras">geeft aan welke extra entiteiten mee opgehaald moeten worden</param>
		/// <returns>Lidinfo met gelieerdepersoon en persoon</returns>
		public LidInfo Ophalen(int lidID, LidExtras extras)
		{
			if ((extras & LidExtras.AlleAfdelingen) != 0)
			{
				throw new NotImplementedException("Het datacontract LidInfo is niet voorzien op " +
					"het opleveren van alle afdelingen.");
			}
			return Mapper.Map<Lid, LidInfo>(_ledenMgr.Ophalen(lidID, extras));
		}

		/// <summary>
		/// Vervangt de functies van het lid bepaald door <paramref name="lidID"/> door de functies
		/// met ID's <paramref name="functieIDs"/>
		/// </summary>
		/// <param name="lidID">ID van lid met te vervangen functies</param>
		/// <param name="functieIDs">IDs van nieuwe functies voor het lid</param>
		public void FunctiesVervangen(int lidID, IEnumerable<int> functieIDs)
		{
			Lid lid = _ledenMgr.Ophalen(lidID, LidExtras.Groep | LidExtras.Functies);
			IList<Functie> functies;

			if (functieIDs != null && functieIDs.Count() > 0)
			{
				functies = _functiesMgr.Ophalen(functieIDs);
			}
			else
			{
				functies = new List<Functie>();
			}

			// Probleem is hier dat de functies en de groepen daaraan gekoppeld uit 'functies'
			// mogelijk dezelfde zijn als de functies en de groep van 'lid', hoewel het verschillende
			// objecten zijn.
			//
			// Laat ons dus hopen dat volgende call hierop geen problemen geeft:

			_functiesMgr.Vervangen(lid, functies);
		}

		/// <summary>
		/// Haalt de ID's van de groepswerkjaren van een lid op.
		/// </summary>
		/// <param name="lidID">ID van het lid waarin we geinteresseerd zijn</param>
		/// <returns>Een LidAfdelingInfo-object</returns>
		public LidAfdelingInfo AfdelingenOphalen(int lidID)
		{
			LidAfdelingInfo resultaat = new LidAfdelingInfo();

			Lid l = _ledenMgr.Ophalen(lidID, LidExtras.Afdelingen | LidExtras.Persoon);
			resultaat.VolledigeNaam = String.Format(
				"{0} {1}", 
				l.GelieerdePersoon.Persoon.VoorNaam,
				l.GelieerdePersoon.Persoon.Naam);
			resultaat.Type = l.Type;

			if (l is Kind)
			{
				resultaat.AfdelingsJaarIDs = new List<int>();
				resultaat.AfdelingsJaarIDs.Add((l as Kind).AfdelingsJaar.ID);
			}
			else
			{
				resultaat.AfdelingsJaarIDs = (from aj in (l as Leiding).AfdelingsJaar
							      select aj.ID).ToList();
			}

			return resultaat;
		}

		/// <summary>
		/// Vervangt de afdelingen van het lid met ID <paramref name="lidID"/> door de afdelingen
		/// met AFDELINGSJAARIDs gegeven door <paramref name="afdelingsJaarIDs"/>.
		/// </summary>
		/// <param name="lidID">Lid dat nieuwe afdelingen moest krijgen</param>
		/// <param name="afdelingsJaarIDs">ID's van de te koppelen afdelingsjaren</param>
		/// <returns>De GelieerdePersoonID van het lid</returns>
		public int AfdelingenVervangen(int lidID, IEnumerable<int> afdelingsJaarIDs)
		{
			Lid l = _ledenMgr.Ophalen(
				lidID, 
				LidExtras.Groep | LidExtras.Afdelingen | LidExtras.AlleAfdelingen | LidExtras.Persoon);

			var afdelingsjaren = from aj in l.GroepsWerkJaar.AfdelingsJaar
					     where afdelingsJaarIDs.Contains(aj.ID)
					     select aj;

			if (afdelingsJaarIDs.Count() != afdelingsjaren.Count())
			{
				// waarschijnlijk afdelingsjaren die niet gekoppeld zijn aan het groepswerkjaar.
				// Dat wil zeggen dat de user aan het prutsen is.

				throw new InvalidOperationException(Properties.Resources.AccessDenied);
			}
			_afdelingsJaarMgr.Vervangen(l, afdelingsjaren);

			return l.GelieerdePersoon.ID;
		}


		/* zie #273 */
		// [PrincipalPermission(SecurityAction.Demand, Role = SecurityGroepen.Gebruikers)]
		public LidInfo BewarenMetVrijeVelden(LidInfo lid)
		{
			// TODO
			throw new NotImplementedException();
		}
	}
}