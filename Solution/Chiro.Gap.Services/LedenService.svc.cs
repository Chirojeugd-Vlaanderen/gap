// <copyright company="Chirojeugd-Vlaanderen vzw">
// Copyright (c) 2007-2010
// Mail naar informatica@chiro.be voor alle info over deze broncode
// </copyright>

using System;
using System.Collections.Generic;
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

	// *BELANGRIJK*: Als het debuggen hier stopt owv een autorisatiefout, kijk dan na of de gebruiker waarmee
	// je aangemeld bent, op je lokale computer in de groep CgUsers zit.

	/// <summary>
	/// Service voor operaties op leden en leiding
	/// </summary>
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

		/// <summary>
		/// Constructor met via IoC toegekende workers
		/// </summary>
		/// <param name="gpm">De worker voor GelieerdePersonen</param>
		/// <param name="lm">De worker voor Leden</param>
		/// <param name="grm">De worker voor Groepen</param>
		/// <param name="fm">De worker voor Functies</param>
		/// <param name="ajm">De worker voor AfdelingsJaren</param>
		/// <param name="gwjm">De worker voor GroepsWerkJaren</param>
		/// <param name="vrzm">De worker voor Verzekeringen</param>
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

		#region leden managen

		/// <summary>
		/// Gegeven een lijst van IDs van gelieerde personen.
		/// Haal al die gelieerde personen op en probeer ze in het huidige werkjaar lid te maken in het gegeven lidtype.
		/// <para />
		/// Gaat een gelieerde persoon ophalen en maakt die lid in het huidige werkjaar.  Als het om kindleden gaat,
		/// krjgen ze meteen een afdeling die overeenkomt met leeftijd en geslacht.
		/// </summary>
		/// <param name="gelieerdePersoonIDs">ID's van de gelieerde personen</param>
		/// <param name="type">Bepaalt of de personen als kind of als leiding lid worden.</param>
		/// <param name="foutBerichten">Als er sommige personen geen lid gemaakt werden, bevat foutBerichten een
		/// string waarin wat uitleg staat. TODO: beter systeem vinden voor deze feedback.</param>
		/// <returns>De LidID's van de personen die lid zijn gemaakt</returns>
		/// <remarks>
		/// Iedereen die kan lid gemaakt worden, wordt lid, zelfs als dit voor andere personen niet lukt. Voor die personen worden dan foutberichten
		/// teruggegeven.
		/// </remarks>
		/// <throws>NotSupportedException</throws> // TODO handle
		public IEnumerable<int> Inschrijven(IEnumerable<int> gelieerdePersoonIDs, LidType type, out string foutBerichten)
		{
			try
			{
				if (type != LidType.Kind && type != LidType.Leiding)
				{
					throw new NotSupportedException(Properties.Resources.OngeldigLidType);
				}

				var lidIDs = new List<int>();
				var foutBerichtenBuilder = new StringBuilder();

				// Haal meteen alle gelieerde personen op, gecombineerd met hun groep
				// Ik haal nu ook de groepswerkjaren mee op, omdat 'LidMaken' daar straks in zal kijken.
				// TODO: Dat kan volgens mij ook zonder, maar daarvoor moet LedenManager.LidMaken aangepast wdn

				var gelieerdePersonen = _gelieerdePersonenMgr.Ophalen(gelieerdePersoonIDs,
				                                                      PersoonsExtras.Groep | PersoonsExtras.GroepsWerkJaren);

				// Mogelijk horen de gelieerde personen tot verschillende groepen.  Dat kan, als de GAV GAV is van
				// al die groepen. Als hij geen GAV is van de IDs, dan werd er al een exception gethrowd natuurlijk.
				var groepen = (from gp in gelieerdePersonen select gp.Groep).Distinct();

				foreach (var g in groepen)
				{
					// Per groep lid maken.
					// Zoek eerst recentste groepswerkjaar.
					var gwj = _groepwsWjMgr.RecentsteOphalen(g.ID, GroepsWerkJaarExtras.Afdelingen | GroepsWerkJaarExtras.Groep);

					foreach (var gp in g.GelieerdePersoon)
					{
						try
						{
							var l = _ledenMgr.OphalenViaPersoon(gp.ID, gwj.ID);

							// TODO: hier moet nog ergens een controle gebeuren of de persoon niet overleden is - zie ticket #632

							if (l != null) // uitgeschreven
							{
								if (l.Type != type)
								{
									// TODO maak ander lidobject aan #
									throw new NotImplementedException();
								}
								if (!l.NonActief)
								{
									foutBerichtenBuilder.AppendLine(String.Format(Properties.Resources.IsNogIngeschreven, gp.Persoon.VolledigeNaam));
									continue;
								}
								l.NonActief = false;
							}
							else // nieuw lid
							{
								if (type == LidType.Kind)
								{
									l = _ledenMgr.KindMaken(gp, gwj);
								}
								else
								{
									l = _ledenMgr.LeidingMaken(gp, gwj);
								}
							}

							// Bewaar leden 1 voor 1, en niet allemaal tegelijk, om te vermijden dat 1 dubbel lid
							// verhindert dat de rest bewaard wordt.
							if (l != null)
							{
								l = _ledenMgr.LidBewaren(l, LidExtras.Persoon|LidExtras.Afdelingen);
								lidIDs.Add(l.ID);
							}
						}
						catch (InvalidOperationException ex)
						{
							foutBerichtenBuilder.AppendLine(String.Format("Fout voor {0}: {1}", gp.Persoon.VolledigeNaam, ex.Message));
						}
						catch (GeenGavException ex)
						{
							foutBerichtenBuilder.AppendLine(String.Format("Fout voor {0}: {1}", gp.Persoon.VolledigeNaam, ex.Message));
						}
						catch (FoutNummerException ex)
						{
							foutBerichtenBuilder.AppendLine(String.Format("Fout voor {0}: {1}", gp.Persoon.VolledigeNaam, ex.Message));
						}
						catch (BestaatAlException<Kind>)
						{
							foutBerichtenBuilder.AppendLine(String.Format(Properties.Resources.WasAlLid, gp.Persoon.VolledigeNaam));
						}
						catch (BestaatAlException<Leiding>)
						{
							foutBerichtenBuilder.AppendLine(String.Format(Properties.Resources.WasAlLeiding, gp.Persoon.VolledigeNaam));
						}
					}
				}

				foutBerichten = foutBerichtenBuilder.ToString();

				return lidIDs;
			}
			catch (Exception ex)
			{
				FoutAfhandelaar.FoutAfhandelen(ex);
				foutBerichten = null;
				return null;
			}
		}

		/// <summary>
		/// Gegeven een lijst van IDs van gelieerde personen.
		/// Haal al die gelieerde personen op en probeer ze in het huidige werkjaar lid te maken in het gegeven lidtype.
		/// <para />
		/// Gaat een gelieerde persoon ophalen en maakt die lid op de plaats die overeenkomt met hun leeftijd in het huidige werkjaar.
		/// </summary>
		/// <param name="gelieerdePersoonIDs">ID's van de gelieerde personen</param>
		/// <param name="foutBerichten">Als er sommige personen geen lid gemaakt werden, bevat foutBerichten een
		/// string waarin wat uitleg staat. TODO: beter systeem vinden voor deze feedback.</param>
		/// <returns>De LidID's van de personen die lid zijn gemaakt</returns>
		/// <remarks>
		/// Iedereen die kan lid gemaakt worden, wordt lid, zelfs als dit voor andere personen niet lukt. Voor die personen worden dan foutberichten
		/// teruggegeven.
		/// </remarks>
		/// <throws>NotSupportedException</throws> // TODO handle
		public IEnumerable<int> AutomatischInschrijven(IEnumerable<int> gelieerdePersoonIDs, out string foutBerichten)
		{
			try
			{
				var lidIDs = new List<int>();
				var foutBerichtenBuilder = new StringBuilder();

				// Haal meteen alle gelieerde personen op, gecombineerd met hun groep
				// Ik haal nu ook de groepswerkjaren mee op, omdat 'LidMaken' daar straks in zal kijken.
				// TODO: Dat kan volgens mij ook zonder, maar daarvoor moet LedenManager.LidMaken aangepast wdn

				var gelieerdePersonen = _gelieerdePersonenMgr.Ophalen(gelieerdePersoonIDs,
																	  PersoonsExtras.Groep | PersoonsExtras.GroepsWerkJaren);

				// Mogelijk horen de gelieerde personen tot verschillende groepen.  Dat kan, als de GAV GAV is van
				// al die groepen. Als hij geen GAV is van de IDs, dan werd er al een exception gethrowd natuurlijk.
				var groepen = (from gp in gelieerdePersonen select gp.Groep).Distinct();

				foreach (var g in groepen)
				{
					// Per groep lid maken.
					// Zoek eerst recentste groepswerkjaar.
					var gwj = _groepwsWjMgr.RecentsteOphalen(g.ID, GroepsWerkJaarExtras.Afdelingen | GroepsWerkJaarExtras.Groep);

					foreach (var gp in g.GelieerdePersoon)
					{
						try
						{
							var l = _ledenMgr.OphalenViaPersoon(gp.ID, gwj.ID);

							if (l != null) //uitgeschreven
							{
								if (!l.NonActief)
								{
									foutBerichtenBuilder.AppendLine(String.Format(Properties.Resources.IsNogIngeschreven, gp.Persoon.VolledigeNaam));
									continue;
								}
								l.NonActief = false;
							}
							else //nieuw lid
							{
								l = _ledenMgr.AutomatischLidMaken(gp, gwj);
							}

							// Bewaar leden 1 voor 1, en niet allemaal tegelijk, om te vermijden dat 1 dubbel lid
							// verhindert dat de rest bewaard wordt.
							if (l != null)
							{
								l = _ledenMgr.LidBewaren(l, LidExtras.Afdelingen);
								lidIDs.Add(l.ID);
							}
						}
						catch (InvalidOperationException ex)
						{
							foutBerichtenBuilder.AppendLine(String.Format("Fout voor {0}: {1}", gp.Persoon.VolledigeNaam, ex.Message));
						}
						catch (GeenGavException ex)
						{
							foutBerichtenBuilder.AppendLine(String.Format("Fout voor {0}: {1}", gp.Persoon.VolledigeNaam, ex.Message));
						}
						catch (FoutNummerException ex)
						{
							foutBerichtenBuilder.AppendLine(String.Format("Fout voor {0}: {1}", gp.Persoon.VolledigeNaam, ex.Message));
						}
						catch (BestaatAlException<Kind>)
						{
							foutBerichtenBuilder.AppendLine(String.Format(Properties.Resources.WasAlLid, gp.Persoon.VolledigeNaam));
						}
						catch (BestaatAlException<Leiding>)
						{
							foutBerichtenBuilder.AppendLine(String.Format(Properties.Resources.WasAlLeiding, gp.Persoon.VolledigeNaam));
						}
					}
				}

				foutBerichten = foutBerichtenBuilder.ToString();

				return lidIDs;
			}
			catch (Exception ex)
			{
				FoutAfhandelaar.FoutAfhandelen(ex);
				foutBerichten = null;
				return null;
			}
		}

		/// <summary>
		/// Maakt lid met gegeven ID nonactief
		/// </summary>
		/// <param name="gelieerdePersoonIDs">ID's van de gelieerde personen</param>
		/// <param name="foutBerichten">Als voor sommige personen die actie mislukte, bevat foutBerichten een
		/// string waarin wat uitleg staat.  TODO: beter systeem vinden voor deze feedback.</param>
		public void Uitschrijven(IEnumerable<int> gelieerdePersoonIDs, out string foutBerichten)
		{
			try
			{
				var foutBerichtenBuilder = new StringBuilder();

				var gelieerdePersonen = _gelieerdePersonenMgr.Ophalen(gelieerdePersoonIDs, PersoonsExtras.Groep);
				var groepen = (from gp in gelieerdePersonen select gp.Groep).Distinct();

				foreach (var g in groepen)
				{
					var gwj = _groepwsWjMgr.RecentsteOphalen(g.ID, GroepsWerkJaarExtras.Afdelingen | GroepsWerkJaarExtras.Groep);

					foreach (var gp in g.GelieerdePersoon)
					{
						var l = _ledenMgr.OphalenViaPersoon(gp.ID, gwj.ID);

						// TODO: Is dit niet eerder iets voor 'den business':

						if (l == null)
						{
							foutBerichtenBuilder.AppendLine(String.Format(Properties.Resources.IsNogNietIngeschreven, gp.Persoon.VolledigeNaam));
							continue;
						}
						if (l.NonActief)
						{
							foutBerichtenBuilder.AppendLine(String.Format(Properties.Resources.IsAlUitgeschreven, gp.Persoon.VolledigeNaam));
							continue;
						}

						l.NonActief = true;

						foreach (var fn in l.Functie)
						{
							l.TeVerwijderen = true;
						}

						_ledenMgr.LidBewaren(l, LidExtras.Functies);
					}
				}

				foutBerichten = foutBerichtenBuilder.ToString();
			}
			catch (Exception ex)
			{
				FoutAfhandelaar.FoutAfhandelen(ex);
				foutBerichten = null;
			}
		}

		#endregion

		#region bewaren

		/// <summary>
		/// Slaat veranderingen op aan de eigenschappen van het lidobject zelf. 
		/// Creëert of verwijdert geen leden, en leden kunnen ook niet van werkjaar of van gelieerdepersoon veranderen.
		/// </summary>
		/// <param name="lidinfo">Te bewaren lid</param>
		public void Bewaren(PersoonLidInfo lidinfo)
		{
			try
			{
				Lid lid = _ledenMgr.Ophalen(lidinfo.LidInfo.LidID, LidExtras.Geen);

				_ledenMgr.InfoOvernemen(lidinfo.LidInfo, lid);
				_ledenMgr.LidBewaren(lid, LidExtras.Geen);
			}
			catch (Exception ex)
			{
				FoutAfhandelaar.FoutAfhandelen(ex);
			}
		}

		/// <summary>
		/// Bewaart lidinfo, inclusief wat in vrije velden ingevuld werd
		/// </summary>
		/// <param name="lid">De info die opgeslagen moet worden</param>
		/// <returns>De bijgewerkte lidinfo</returns>
		/* zie #273 */
		// [PrincipalPermission(SecurityAction.Demand, Role = SecurityGroepen.Gebruikers)]
		public PersoonLidInfo BewarenMetVrijeVelden(PersoonLidInfo lid)
		{
			// TODO
			throw new NotImplementedException();
		}

		#endregion

		#region verzekeren

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
			try
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
			catch (Exception ex)
			{
				FoutAfhandelaar.FoutAfhandelen(ex);
				return 0;
			}
		}

		#endregion

		#region vervangen

		/// <summary>
		/// Vervangt de functies van het lid bepaald door <paramref name="lidID"/> door de functies
		/// met ID's <paramref name="functieIDs"/>
		/// </summary>
		/// <param name="lidID">ID van lid met te vervangen functies</param>
		/// <param name="functieIDs">IDs van nieuwe functies voor het lid</param>
		public void FunctiesVervangen(int lidID, IEnumerable<int> functieIDs)
		{
			try
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
			catch (Exception ex)
			{
				FoutAfhandelaar.FoutAfhandelen(ex);
			}
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
			try
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
			catch (Exception ex)
			{
				FoutAfhandelaar.FoutAfhandelen(ex);
				return 0;
			}
		}

		#endregion

		#region Ophalen

		/// <summary>
		/// Haalt relevante info op van personen die lid zijn in het opgegeven GroepsWerkJaar,
		/// gesorteerd op de opgegeven parameter
		/// </summary>
		/// <param name="groepsWerkJaarID">De ID van het GroepsWerkJaar waarvan we info willen</param>
		/// <param name="sortering">De parameter waarop het resultaat gesorteerd moet worden</param>
		/// <returns>Een lijst met relevante gegevens over personen die lid zijn/waren in het
		/// opgegeven GroepsWerkJaar</returns>
		/* zie #273 */
		// [PrincipalPermission(SecurityAction.Demand, Role = SecurityGroepen.Gebruikers)]
		public IList<PersoonLidInfo> PaginaOphalen(int groepsWerkJaarID, LedenSorteringsEnum sortering)
		{
			try
			{
				var result = _ledenMgr.PaginaOphalen(groepsWerkJaarID, sortering);
				return Mapper.Map<IList<Lid>, IList<PersoonLidInfo>>(result);
			}
			catch (Exception ex)
			{
				FoutAfhandelaar.FoutAfhandelen(ex);
				return null;
			}
		}

		/// <summary>
		/// Haalt relevante info op van personen die in het opgegeven GroepsWerkJaar
		/// lid zijn/waren van de opgegeven afdeling, gesorteerd op de opgegeven parameter
		/// </summary>
		/// <param name="groepsWerkJaarID">De ID van het GroepsWerkJaar waarvan we info willen</param>
		/// <param name="afdelingsID">De ID van de afdeling waarvan de personen lid moeten zijn</param>
		/// <param name="sortering">De parameter waarop het resultaat gesorteerd moet worden</param>
		/// <returns>Een lijst met relevante gegevens over personen die lid zijn/waren in het
		/// opgegeven GroepsWerkJaar</returns>
		/* zie #273 */
		// [PrincipalPermission(SecurityAction.Demand, Role = SecurityGroepen.Gebruikers)]
		public IList<PersoonLidInfo> PaginaOphalenVolgensAfdeling(int groepsWerkJaarID, int afdelingsID, LedenSorteringsEnum sortering)
		{
			try
			{
				IList<Lid> result = _ledenMgr.PaginaOphalenVolgensAfdeling(groepsWerkJaarID, afdelingsID, sortering);
				return Mapper.Map<IList<Lid>, IList<PersoonLidInfo>>(result);
			}
			catch (Exception ex)
			{
				FoutAfhandelaar.FoutAfhandelen(ex);
				return null;
			}
		}

		/// <summary>
		/// Haalt relevante info op van personen die in het opgegeven GroepsWerkJaar
		/// lid zijn/waren en de opgegeven functie hadden, gesorteerd op de opgegeven parameter
		/// </summary>
		/// <param name="groepsWerkJaarID">De ID van het GroepsWerkJaar waarvan we info willen</param>
		/// <param name="functieID">De ID van de functie die de personen moeten hebben</param>
		/// <param name="sortering">De parameter waarop het resultaat gesorteerd moet worden</param>
		/// <returns>Een lijst met relevante gegevens over personen die lid zijn/waren in het
		/// opgegeven GroepsWerkJaar</returns>
		/* zie #273 */
		// [PrincipalPermission(SecurityAction.Demand, Role = SecurityGroepen.Gebruikers)]
		public IList<PersoonLidInfo> PaginaOphalenVolgensFunctie(int groepsWerkJaarID, int functieID, LedenSorteringsEnum sortering)
		{
			try
			{
				IList<Lid> result = _ledenMgr.PaginaOphalenVolgensFunctie(groepsWerkJaarID, functieID, sortering);
				return Mapper.Map<IList<Lid>, IList<PersoonLidInfo>>(result);
			}
			catch (Exception ex)
			{
				FoutAfhandelaar.FoutAfhandelen(ex);
				return null;
			}
		}

		/// <summary>
		/// Haalt lid op, inclusief gelieerde persoon, persoon, groep, afdelingen en functies
		/// </summary>
		/// <param name="lidID">ID op te halen lid</param>
		/// <returns>Lidinfo; bevat info over gelieerde persoon, persoon, groep, afdelingen 
		/// en functies </returns>
		public PersoonLidInfo DetailsOphalen(int lidID)
		{
			try
			{
				return Mapper.Map<Lid, PersoonLidInfo>(_ledenMgr.Ophalen(
								lidID,
								LidExtras.Groep | LidExtras.Afdelingen | LidExtras.Functies | LidExtras.Persoon));
			}
			catch (Exception ex)
			{
				FoutAfhandelaar.FoutAfhandelen(ex);
				return null;
			}
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
			try
			{
				IList<Lid> result = _ledenMgr.PaginaOphalenVolgensAfdeling(
					groepsWerkJaarID,
					afdID,
					LidExtras.Persoon | LidExtras.Afdelingen | LidExtras.Functies | LidExtras.Adressen | LidExtras.Communicatie).ToList();
				return Mapper.Map<IEnumerable<Lid>, IList<LidOverzicht>>(result);
			}
			catch (Exception ex)
			{
				FoutAfhandelaar.FoutAfhandelen(ex);
				return null;
			}
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
			try
			{
				var leden = _ledenMgr.PaginaOphalen(
					groepsWerkJaarID,
					LidExtras.Persoon | LidExtras.Afdelingen | LidExtras.Functies | LidExtras.Adressen | LidExtras.Communicatie);
				var resultaat = Mapper.Map<IEnumerable<Lid>, IList<LidOverzicht>>(leden);

				return resultaat;
			}
			catch (Exception ex)
			{
				FoutAfhandelaar.FoutAfhandelen(ex);
				return null;
			}
		}

		/// <summary>
		/// Haalt de ID's van de groepswerkjaren van een lid op.
		/// </summary>
		/// <param name="lidID">ID van het lid waarin we geinteresseerd zijn</param>
		/// <returns>Een LidAfdelingInfo-object</returns>
		public LidAfdelingInfo AfdelingenOphalen(int lidID)
		{
			try
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
				else if (l is Leiding)
				{
					resultaat.AfdelingsJaarIDs = (from aj in (l as Leiding).AfdelingsJaar
												  select aj.ID).ToList();
				}

				return resultaat;
			}
			catch (Exception ex)
			{
				FoutAfhandelaar.FoutAfhandelen(ex);
				return null;
			}
		}

		#endregion
	}
}