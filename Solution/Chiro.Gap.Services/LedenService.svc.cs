// <copyright company="Chirojeugd-Vlaanderen vzw">
// Copyright (c) 2007-2010
// Mail naar informatica@chiro.be voor alle info over deze broncode
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;

using AutoMapper;

using Chiro.Gap.Orm;
using Chiro.Gap.ServiceContracts;
using Chiro.Gap.ServiceContracts.DataContracts;
using Chiro.Gap.Workers;
using Chiro.Gap.Workers.Exceptions;

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
				//TODO moet hier ook geen try rond staan?
				GelieerdePersoon gp = _gelieerdePersonenMgr.DetailsOphalen(gpID);

				try
				{
					Lid l = _ledenMgr.KindMaken(gp);
					leden.Add(l);
				}
				catch (BestaatAlException<Lid>)
				{
					/*code is reentrant*/
				}
				catch (InvalidOperationException ex)
				{
					result += "Fout voor " + gp.Persoon.VolledigeNaam + ": " + ex.Message + Environment.NewLine;
				}
			}
			if (!result.Equals(String.Empty))
			{
				// Ne string als faultcontract.  Nog niet geweldig, maar al beter als een
				// exception.  Zie #463.

				throw new FaultException<string>("Kon niet alle personen lid maken", result);
			}

			foreach (Lid l in leden)
			{
				_ledenMgr.LidBewaren(l);
			}
			return (from l in leden 
					select l.ID).ToList();
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
				catch (BestaatAlException<Lid>)
				{
					/*code is reentrant*/
				}
				catch (InvalidOperationException ex)
				{
					result += ex.Message + "\n";
				}
			}
			if (!result.Equals(String.Empty))
			{
				throw new InvalidOperationException(result);
			}

			foreach (Lid l in leden)
			{
				_ledenMgr.LidBewaren(l);
			}
			return (from l in leden
					select l.ID).ToList();
		}

		/// <summary>
		/// Slaat veranderingen op aan de eigenschappen van het lidobject zelf. Creëert of verwijdert geen leden, en leden
		/// kunnen ook niet van werkjaar of van gelieerdepersoon veranderen.
		/// </summary>
		/// <param name="lidinfo">Te bewaren lid</param>
		public void Bewaren(PersoonLidInfo lidinfo)
		{
			Lid lid = _ledenMgr.Ophalen(lidinfo.LidInfo.LidID, LidExtras.Geen);

			_ledenMgr.InfoOvernemen(lidinfo.LidInfo, lid);
			_ledenMgr.LidBewaren(lid);
		}

		public void NonActiefMaken(int lidID)
		{
			Lid l = _ledenMgr.Ophalen(lidID, LidExtras.Groep);
			_ledenMgr.NonActiefMaken(l);	// verwijderen persisteert meteen
		}

		public void ActiefMaken(int lidID)
		{
			Lid l = _ledenMgr.Ophalen(lidID);
			l.NonActief = false;
			_ledenMgr.LidBewaren(l);	// verwijderen persisteert meteen
		}

		/* zie #273 */
		// [PrincipalPermission(SecurityAction.Demand, Role = SecurityGroepen.Gebruikers)]
		public IList<PersoonLidInfo> PaginaOphalen(int groepsWerkJaarID, out int paginas)
		{
			var result = _ledenMgr.PaginaOphalen(groepsWerkJaarID, out paginas);
			return Mapper.Map<IList<Lid>, IList<PersoonLidInfo>>(result);
		}

		/* zie #273 */
		// [PrincipalPermission(SecurityAction.Demand, Role = SecurityGroepen.Gebruikers)]
		public IList<PersoonLidInfo> PaginaOphalenVolgensAfdeling(int groepsWerkJaarID, int afdelingsID, out int paginas)
		{
			IList<Lid> result = _ledenMgr.PaginaOphalenVolgensAfdeling(groepsWerkJaarID, afdelingsID, out paginas);
			return Mapper.Map<IList<Lid>, IList<PersoonLidInfo>>(result);
		}

		/// <summary>
		/// Haalt lid op, inclusief gelieerde persoon, persoon, groep, afdelingen en functies
		/// </summary>
		/// <param name="lidID">ID op te halen lid</param>
		/// <returns>Lidinfo; bevat info over gelieerde persoon, persoon, groep, afdelingen 
		/// en functies </returns>
		public PersoonLidInfo DetailsOphalen(int lidID)
		{
			return Mapper.Map<Lid, PersoonLidInfo>(_ledenMgr.Ophalen(
				lidID,
				LidExtras.Groep|LidExtras.Afdelingen|LidExtras.Functies|LidExtras.Persoon));
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
			var resultaat = new LidAfdelingInfo();

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
			else if(l is Leiding)
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
		public PersoonLidInfo BewarenMetVrijeVelden(PersoonLidInfo lid)
		{
			// TODO
			throw new NotImplementedException();
		}
	}
}