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

		public LedenService(
			GelieerdePersonenManager gpm, 
			LedenManager lm, 
			GroepenManager grm,
			FunctiesManager fm)
		{
			this._gelieerdePersonenMgr = gpm;
			this._ledenMgr = lm;
			this._groepenMgr = grm;
			this._functiesMgr = fm;
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
		/// Slaat veranderingen op aan de eigenschappen van het lidobject zelf. Creëert of verwijdert geen leden, en leden
		/// kunnen ook niet van werkjaar of van gelieerdepersoon veranderen. Ook de afdelingen worden aangepast.
		/// </summary>
		/// <param name="lid">te bewaren lid</param>
		[PrincipalPermission(SecurityAction.Demand, Role = SecurityGroepen.Gebruikers)]
		public void BewarenMetAfdelingen(LidInfo lidinfo)
		{
			// Haal oorspronkelijk lid op.
			Lid lid = _ledenMgr.Ophalen(
				lidinfo.LidID, 
				LidExtras.Groep | LidExtras.Afdelingen | LidExtras.AlleAfdelingen);

			// Neem gegevens uit lidinfo over.  

			_ledenMgr.InfoOvernemen(lidinfo, lid);
		
			try
			{
				// persisteert meteen

				_ledenMgr.AanpassenAfdelingenVanLid(lid, lidinfo.AfdelingIdLijst);
			}
			catch (OngeldigeActieException)
			{
				// TODO
				throw;
			}
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
		public void BewarenMetAfdelingen(int lidID, IList<int> afdelingsIDs)
		{
			Lid l = _ledenMgr.Ophalen(lidID, LidExtras.Groep | LidExtras.Afdelingen);
			_ledenMgr.AanpassenAfdelingenVanLid(l, afdelingsIDs);
			_ledenMgr.LidBewaren(l);
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
			var functies = _functiesMgr.Ophalen(functieIDs);

			// Probleem is hier dat de functies en de groepen daaraan gekoppeld uit 'functies'
			// mogelijk dezelfde zijn als de functies en de groep van 'lid', hoewel het verschillende
			// objecten zijn.
			//
			// Laat ons dus hopen dat volgende call hierop geen problemen geeft:

			_functiesMgr.Vervangen(lid, functies);
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