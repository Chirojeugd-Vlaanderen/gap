// <copyright company="Chirojeugd-Vlaanderen vzw">
// Copyright (c) 2007-2010
// Mail naar informatica@chiro.be voor alle info over deze broncode
// </copyright>

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using AutoMapper;
using Chiro.Gap.Domain;
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
		private readonly GroepsWerkJaarManager _groepwsWjMgr;
		private readonly VerzekeringenManager _verzekeringenMgr;

		public LedenService(
			GelieerdePersonenManager gpm, 
			LedenManager lm, 
			GroepenManager grm,
			FunctiesManager fm,
			AfdelingsJaarManager ajm,
			GroepsWerkJaarManager gwjm,
			VerzekeringenManager vrzm)
		{
			_gelieerdePersonenMgr = gpm;
			_ledenMgr = lm;
			_groepenMgr = grm;
			_functiesMgr = fm;
			_afdelingsJaarMgr = ajm;
			_groepwsWjMgr = gwjm;
			_verzekeringenMgr = vrzm;
		}

		#endregion

		/// <summary>
		/// Gaat een gelieerde persoon ophalen en maakt die lid in het huidige werkjaar.  Als het om kindleden gaat,
		/// krjgen ze meteen een afdeling die overeenkomt met leeftijd en geslacht.
		/// </summary>
		/// <param name="gelieerdePersoonIDs">ID's van de gelieerde personen</param>
		/// <param name="type">Bepaalt of de personen als kind of als leiding lid worden.</param>
		/// <param name="foutBerichten">Als er sommige personen geen lid gemaakt werden, bevat foutBerichten een
		/// string waarin wat uitleg staat.  TODO: beter systeem vinden voor deze feedback.</param>
		/// <returns>De LidIDs van de personen die lid zijn gemaakt</returns>
		/// <remarks>
		/// Als er met bepaalde gelieerde personen een probleem is (geen geboortedatum,...), dan worden
		/// de personen die geen problemen vertonen *toch* lid gemaakt. 
		/// </remarks>
		public IEnumerable<int> LedenMaken(IEnumerable<int> gelieerdePersoonIDs, LidType type, out string foutBerichten)
		{
			var lidIDs = new List<int>();
			StringBuilder foutBerichtenBuilder = new StringBuilder();

			// Haal meteen alle gelieerde personen op, gecombineerd met hun groep

			var gelieerdePersonen = _gelieerdePersonenMgr.Ophalen(gelieerdePersoonIDs, PersoonsExtras.Groep);

			// Mogelijk horen de gelieerde personen tot verschillende groepen.  Dat kan, als de GAV GAV is van
			// al die groepen.  Is dat niet het geval, dan werd hierboven al een exception gethrowd.

			var groepen = (from gp in gelieerdePersonen select gp.Groep).Distinct();
			
			// Ter controle bij debuggen even kijken of de distinct goed werkt.

			var groepIDs = (from gp in gelieerdePersonen select gp.Groep.ID).Distinct();
			Debug.Assert(groepen.Count() == groepIDs.Count());

			foreach (Groep g in groepen)
			{
				// Per groep lid maken.
				// Zoek eerst recentste groepswerkjaar.

				var gwj = _groepwsWjMgr.RecentsteOphalen(
					g.ID, 
					GroepsWerkJaarExtras.Afdelingen|GroepsWerkJaarExtras.Groep);

                                foreach (GelieerdePersoon gp in g.GelieerdePersoon)
                                {
                                	Lid l = null;

                                	try
                                	{
						switch (type)
						{
							case LidType.Kind: 
								l = _ledenMgr.KindMaken(gp, gwj);
								break;
							case LidType.Leiding:
								l = _ledenMgr.LeidingMaken(gp, gwj);
								break;
							default:
								throw new NotSupportedException(Properties.Resources.OngeldigLidType);
						}
						
                                	}
                                	catch (InvalidOperationException ex)
                                	{
						// TODO: beter systeem voor feedback
                                		foutBerichtenBuilder.AppendLine(String.Format(
							"Fout voor {0}: {1}", 
							gp.Persoon.VolledigeNaam,
                                		        ex.Message));
                                	}

					// Bewaar leden 1 voor 1, en niet allemaal tegelijk, om te vermijden dat 1 dubbel lid
					// verhindert dat de rest bewaard wordt.

					if (l != null)
					{
						try
						{
							l = _ledenMgr.LidBewaren(l);
							lidIDs.Add(l.ID);
						}
						catch (BestaatAlException<Kind>)
						{
							foutBerichtenBuilder.AppendLine(String.Format(
								Properties.Resources.WasAlLid,
								gp.Persoon.VolledigeNaam));
						}
						catch (BestaatAlException<Leiding>)
						{
							foutBerichtenBuilder.AppendLine(String.Format(
								Properties.Resources.WasAlLeiding,
								gp.Persoon.VolledigeNaam));							
						}
					}

                                }

			}

			foutBerichten = foutBerichtenBuilder.ToString();

			return lidIDs;
		}

		/// <summary>
		/// Slaat veranderingen op aan de eigenschappen van het lidobject zelf. Creëert of verwijdert geen leden, en leden kunnen ook niet van werkjaar of van gelieerdepersoon veranderen.
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

		/// <summary>
		/// Verzekert lid met ID <paramref name="lidID"/> tegen loonverlies
		/// </summary>
		/// <param name="lidID">ID van te verzekeren lid</param>
		/// <returns>GelieerdePersoonID van het verzekerde lid</returns>
		/// <remarks>Dit is nogal een specifieke method.  In ons domain model is gegeven dat verzekeringen gekoppeld zijn aan
		/// personen, voor een bepaalde periode.  Maar in eerste instantie zal alleen de verzekering loonverlies gebruikt worden,
		/// die per definitie enkel voor leden bestaat.</remarks>
		public int LoonVerliesVerzekeren(int lidID)
		{
			Lid l = _ledenMgr.Ophalen(lidID, LidExtras.Verzekeringen);
			VerzekeringsType verz = _verzekeringenMgr.Ophalen(Verzekering.LoonVerlies);

			var verzekering = _verzekeringenMgr.Verzekeren(
				l, 
				verz, 
				DateTime.Today, GroepsWerkJaarManager.EindDatum(l.GroepsWerkJaar));

			_verzekeringenMgr.PersoonsVerzekeringBewaren(verzekering);

			return l.GelieerdePersoon.ID;
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

		#region Ophalen

		/* zie #273 */
		// [PrincipalPermission(SecurityAction.Demand, Role = SecurityGroepen.Gebruikers)]
		public IList<PersoonLidInfo> PaginaOphalen(int groepsWerkJaarID, LedenSorteringsEnum sortering)
		{
			var result = _ledenMgr.PaginaOphalen(groepsWerkJaarID, sortering);
			return Mapper.Map<IList<Lid>, IList<PersoonLidInfo>>(result);
		}

		/* zie #273 */
		// [PrincipalPermission(SecurityAction.Demand, Role = SecurityGroepen.Gebruikers)]
		public IList<PersoonLidInfo> PaginaOphalenVolgensAfdeling(int groepsWerkJaarID, int afdelingsID, LedenSorteringsEnum sortering)
		{
			IList<Lid> result = _ledenMgr.PaginaOphalenVolgensAfdeling(groepsWerkJaarID, afdelingsID, sortering);
			return Mapper.Map<IList<Lid>, IList<PersoonLidInfo>>(result);
		}

		// [PrincipalPermission(SecurityAction.Demand, Role = SecurityGroepen.Gebruikers)]
		public IList<PersoonLidInfo> PaginaOphalenVolgensFunctie(int groepsWerkJaarID, int functiID, LedenSorteringsEnum sortering)
		{
			IList<Lid> result = _ledenMgr.PaginaOphalenVolgensFunctie(groepsWerkJaarID, functiID, sortering);
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
				LidExtras.Groep | LidExtras.Afdelingen | LidExtras.Functies | LidExtras.Persoon));
		}

		/// <summary>
		/// Haalt informatie op over alle leden uit het groepswerkjaar bepaald door <paramref name="groepsWerkJaarID"/>
		/// die lid zijn in de afdeling bepaald door <paramref name="afdID"/>.
		/// </summary>
		/// <param name="groepsWerkJaarID">ID van het groepswerkjaar waaruit de leden opgehaald moeten worden</param>
		/// <param name="afdID">ID van de afdeling waaruit de leden opgehaald moeten worden.</param>
		/// <returns>Een rij 'LidOverzicht'-objecten met informatie over de betreffende leden.</returns>
		public IList<LidOverzicht> OphalenUitAfdelingsJaar(int groepsWerkJaarID, int afdID)
		{
			IList<Lid> result = _ledenMgr.PaginaOphalenVolgensAfdeling(
				groepsWerkJaarID, 
				afdID,
				LidExtras.Persoon | LidExtras.Afdelingen | LidExtras.Functies | LidExtras.Adressen | LidExtras.Communicatie).ToList();
			return Mapper.Map<IEnumerable<Lid>, IList<LidOverzicht>>(result);

		}

		/// <summary>
		/// Haalt informatie op over alle leden uit het groepswerkjaar bepaald door 
		/// <paramref name="groepsWerkJaarID"/> die de functie bepaald door <paramref name="functieID"/> hebben.
		/// </summary>
		/// <param name="groepsWerkJaarID">ID van groepswerkjaar waaruit leden moeten worden opgehaald</param>
		/// <param name="functieID">ID van functie die opgehaalde leden moeten hebben</param>
		/// <returns>Een rij `LidOverzicht'-objecten met informatie over de betreffende leden.</returns>
		public IList<LidOverzicht> OphalenUitFunctie(int groepsWerkJaarID, int functieID)
		{
			throw new NotImplementedException();
		}

		/// <summary>
		/// Haalt informatie op over alle leden uit een gegeven groepswerkjaar
		/// </summary>
		/// <param name="groepsWerkJaarID">ID van het groepswerkjaar waaruit de leden moeten worden opgehaald</param>
		/// <returns>Een rij `LidOverzicht'-objecten met informatie over de betreffende leden.</returns>
		public IList<LidOverzicht> OphalenUitGroepsWerkJaar(int groepsWerkJaarID)
		{
			var leden = _ledenMgr.PaginaOphalen(
				groepsWerkJaarID, 
				LidExtras.Persoon|LidExtras.Afdelingen|LidExtras.Functies|LidExtras.Adressen|LidExtras.Communicatie);
			var resultaat = Mapper.Map<IEnumerable<Lid>, IList<LidOverzicht>>(leden);

			return resultaat;
		}

		#endregion
	}
}