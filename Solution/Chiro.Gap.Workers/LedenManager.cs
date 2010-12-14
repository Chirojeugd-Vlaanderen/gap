// <copyright company="Chirojeugd-Vlaanderen vzw">
// Copyright (c) 2007-2010
// Mail naar informatica@chiro.be voor alle info over deze broncode
// </copyright>

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
#if KIPDORP
using System.Threading;
using System.Transactions;
#endif

using Chiro.Cdf.Data;
using Chiro.Gap.Domain;
using Chiro.Gap.Orm;
using Chiro.Gap.Orm.DataInterfaces;
using Chiro.Gap.Orm.SyncInterfaces;
using Chiro.Gap.ServiceContracts.DataContracts;
using Chiro.Gap.Workers.Exceptions;

namespace Chiro.Gap.Workers
{
	/// <summary>
	/// Klasse met extra methode om het einde van de jaarovergang in een groepswerkjaar op te vragen.
	/// </summary>
	public static class GroepsWerkJaarHelper
	{
		/// <summary>
		/// Berekend aan de hand van een gegeven werkjaar de datum van het verplichte einde van de instapperiode in dat jaar.
		/// Belangrijk => volgens de HUIDIGE settings van dat werkjaareinde (moest dat in de toekomst veranderen en we hebben dat van vroeger nodig)
		/// </summary>
		/// <param name="gwj">Het groepswerkjaar waarvoor we het einde van de jaarovergang willen berekenen</param>
		/// <returns>De datum waarom de jaarovergang eindigt</returns>
		public static DateTime GetEindeJaarovergang(this GroepsWerkJaar gwj)
		{
			// Haal de einddatum voor de overgang/aansluiting uit de settings, en bereken wanneer die datum valt in dit werkjaar
			var dt = Properties.Settings.Default.WerkjaarVerplichteOvergang;
			return new DateTime(gwj.WerkJaar, dt.Month, dt.Day);
		}
	}

	/// <summary>
	/// Worker die alle businesslogica i.v.m. leden bevat
	/// </summary>
	public class LedenManager
	{
		private readonly LedenDaoCollectie _daos;
		private readonly IAutorisatieManager _autorisatieMgr;
		private readonly ILedenSync _sync;

		/// <summary>
		/// Maakt een nieuwe ledenmanager aan
		/// </summary>
		/// <param name="daos">Een hele reeks van IDao-objecten, nodig
		/// voor data access.</param>
		/// <param name="autorisatie">Een IAuthorisatieManager, die
		/// de GAV-permissies van de huidige user controleert.</param>
		/// <param name="sync">Zorgt voor synchronisate van adressen naar KipAdmin</param>
		public LedenManager(LedenDaoCollectie daos, IAutorisatieManager autorisatie, ILedenSync sync)
		{
			_daos = daos;
			_autorisatieMgr = autorisatie;
			_sync = sync;
		}

		/// <summary>
		/// Maakt een gelieerde persoon <paramref name="gp"/> lid in groepswerkjaar <paramref name="gwj"/>,
		/// met lidtype <paramref name="type"/>, persisteert niet.
		/// </summary>
		/// <param name="gp">Lid te maken gelieerde persoon, gekoppeld met groep en persoon</param>
		/// <param name="gwj">Groepswerkjaar waarin de gelieerde persoon lid moet worden</param>
		/// <param name="type">LidType.Kind of LidType.Leiding</param>
		/// <param name="isJaarOvergang">Als deze true is, is einde probeerperiode steeds 
		/// 15 oktober als het nog geen 15 oktober is</param>
		/// <remarks>
		/// Private; andere lagen moeten via 'Inschrijven' gaan.
		/// <para/>
		/// Deze method test niet
		/// of het groepswerkjaar wel het recentste is.  (Voor de unit tests moeten
		/// we ook leden kunnen maken in oude groepswerkjaren.)
		/// Roep deze method ook niet rechtstreeks aan, maar wel via KindMaken of LeidingMaken
		/// </remarks>
		/// <returns>Het aangepaste Lid-object</returns>
		/// <throws>FoutNummerException</throws>
		/// <throws>GeenGavException</throws>
		/// <throws>InvalidOperationException</throws>
		private Lid LidMaken(GelieerdePersoon gp, GroepsWerkJaar gwj, LidType type, bool isJaarOvergang)
		{
			Lid lid;

			switch (type)
			{
				case LidType.Kind:
					lid = new Kind();
					break;
				case LidType.Leiding:
					lid = new Leiding();
					break;
				default:
					lid = new Lid();
					break;
			}

			if (!_autorisatieMgr.IsGavGelieerdePersoon(gp.ID) || !_autorisatieMgr.IsGavGroepsWerkJaar(gwj.ID))
			{
				throw new GeenGavException(Properties.Resources.GeenGav);
			}

			if (gp.Groep.ID != gwj.Groep.ID)
			{
				throw new FoutNummerException(FoutNummer.GroepsWerkJaarNietVanGroep, Properties.Resources.GroepsWerkJaarNietVanGroep);
			}

			// Geboortedatum is verplicht als je lid wilt worden
			if (!gp.GebDatumMetChiroLeefTijd.HasValue)
			{
				throw new InvalidOperationException(Properties.Resources.GeboorteDatumOntbreekt);
			}

			// Nog leven ook
			if (gp.Persoon.SterfDatum.HasValue)
			{
				throw new InvalidOperationException(Properties.Resources.PersoonIsOverleden);
			}

			// GroepsWerkJaar en GelieerdePersoon invullen
			lid.GroepsWerkJaar = gwj;
			lid.GelieerdePersoon = gp;
			gp.Lid.Add(lid);
			gwj.Lid.Add(lid);

			var stdProbeerPeriode = DateTime.Today.AddDays(Properties.Settings.Default.LengteProbeerPeriode);

			if ((gp.Groep.Niveau & (Niveau.Gewest|Niveau.Verbond)) != 0)
			{
				// Gewesten en verbonden: instapperiode enkel vandaag
				// ! NIET RECHTSTREEKS SYNCEN, ZODAT DE GROEPN NOG EVEN TIJD HEEFT OM
				// FOUT INGESCHREVEN LEDEN OPNIEUW UIT TE SCHRIJVEN.

				lid.EindeInstapPeriode = DateTime.Today;
			}
			else if (!isJaarOvergang)
			{
				// Standaardinstapperiode indien niet in jaarovergang

				lid.EindeInstapPeriode = gwj.GetEindeJaarovergang() >= stdProbeerPeriode
				                         	? gwj.GetEindeJaarovergang()
				                         	: stdProbeerPeriode;
			}
			else
			{
				// Voor jaarovergang: vaste deadline (gek idee, maar blijkbaar in de specs)

				lid.EindeInstapPeriode = gwj.GetEindeJaarovergang() >= DateTime.Now
								? gwj.GetEindeJaarovergang()
								: stdProbeerPeriode;
				
			}

			return lid;
		}

		/// <summary>
		/// Maakt gelieerde persoon een kind (lid) voor het gegeven werkjaar.
		/// <para />
		/// Dit komt neer op 
		///		Automatisch een afdeling voor het kind bepalen. Een exception als dit niet mogelijk is.
		///		De probeerperiode zetten op binnen 3 weken als het een nieuw lid is, en op 15 oktober als de persoon vorig jaar al lid was.
		/// </summary>
		/// <param name="gp">Gelieerde persoon, gekoppeld aan groep en persoon</param>
		/// <param name="gwj">Groepswerkjaar waarin lid te maken, gekoppeld met afdelingsjaren</param>
		///<param name="isJaarOvergang">Geeft aan of het lid gemaakt wordt voor de automatische jaarovergang; relevant
		/// voor probeerperiode.</param>
		///<returns>Nieuw kindobject, niet gepersisteerd</returns>
		/// <remarks>
		/// Private; andere lagen moeten via 'Inschrijven' gaan.
		/// <para/>
		/// De user zal nooit zelf mogen kiezen in welk groepswerkjaar een kind lid wordt.  Maar 
		/// om testdata voor unit tests op te bouwen, hebben we deze functionaliteit wel nodig.
		/// <para/>
		/// Voorlopig gaan we ervan uit dat aan een gelieerde persoon al zijn vorige lidobjecten met
		/// groepswerkjaren gekoppeld zijn.  Dit wordt gebruikt in LidMaken
		/// om na te kijken of een gelieerde persoon al eerder
		/// lid was.  Dit lijkt me echter niet nodig; zie de commentaar aldaar.
		/// </remarks>
		/// <throws>FoutNummerException</throws>
		/// <throws>GeenGavException</throws>
		/// <throws>InvalidOperationException</throws>
		private Kind KindMaken(GelieerdePersoon gp, GroepsWerkJaar gwj, bool isJaarOvergang)
		{
			// LidMaken doet de nodige checks ivm GAV-schap, enz.
			var k = LidMaken(gp, gwj, LidType.Kind, isJaarOvergang) as Kind;

			// Probeer nu afdeling te vinden.
			if (gwj.AfdelingsJaar.Count == 0)
			{
				throw new InvalidOperationException(Properties.Resources.InschrijvenZonderAfdelingen);
			}

			// Afdeling automatisch bepalen

			Debug.Assert(gp.GebDatumMetChiroLeefTijd != null);
			// Om resharper blij te houden, is al gecontroleerd.
			var geboortejaar = gp.GebDatumMetChiroLeefTijd.Value.Year;

			// Relevante afdelingsjaren opzoeken.  Afdelingen met speciale officiele afdeling
			// worden in eerste instantie uitgesloten van de automatische verdeling.

			var afdelingsjaren =
				(from a in gwj.AfdelingsJaar
				 where a.GeboorteJaarVan <= geboortejaar && geboortejaar <= a.GeboorteJaarTot
					   && a.OfficieleAfdeling.ID != (int)NationaleAfdeling.Speciaal
				 select a).ToList();

			if (afdelingsjaren.Count == 0)
			{
				// Is er geen geschikte 'normale' afdeling gevonden, probeer dan de speciale eens.

				afdelingsjaren =
					(from a in gwj.AfdelingsJaar
					 where a.GeboorteJaarVan <= geboortejaar && geboortejaar <= a.GeboorteJaarTot
					       && a.OfficieleAfdeling.ID == (int) NationaleAfdeling.Speciaal
					 select a).ToList();
			}

			if (afdelingsjaren.Count == 0)
			{
				throw new InvalidOperationException(Properties.Resources.GeenAfdelingVoorLeeftijd);
			}

			// Kijk of er een afdeling is met een overeenkomend geslacht
			var aj = (from a in afdelingsjaren
					  where a.Geslacht == gp.Persoon.Geslacht || a.Geslacht == GeslachtsType.Gemengd
					  select a).FirstOrDefault();

			// Als dit niet zo is, kies dan de eerste afdeling die voldoet aan de leeftijdsgrenzen.
			aj = aj ?? afdelingsjaren.First();

			// Elk scenario waarbij k null is, leidt tot een exception, maar dat gebeurt deels in LidMaken, dus ziet ReSharper dat niet
			// Je kan de foutmelding van resharper dan als volgt vermijden:
			Debug.Assert(k != null);

			k.AfdelingsJaar = aj;
			aj.Kind.Add(k);

			return k;
		}

		/// <summary>
		/// Maakt gelieerde persoon leiding voor het gegeven werkjaar.
		/// </summary>
		/// <param name="gp">Gelieerde persoon, gekoppeld met groep en persoon</param>
		/// <param name="gwj">Groepswerkjaar waarin leiding te maken</param>
		/// <param name="isJaarovergang">geeft aan of het over de automatische jaarovergang gaat.
		/// (relevant voor probeerperiode)</param>
		/// <returns>Nieuw leidingsobject; niet gepersisteerd</returns>
		/// <remarks>
		/// Private; andere lagen moeten via 'Inschrijven' gaan.
		/// <para/>
		/// Deze method mag niet geexposed worden via de services, omdat
		/// een gebruiker uiteraard enkel in het huidige groepswerkjaar leden
		/// kan maken.
		/// <para/>
		/// Voorlopig gaan we ervan uit dat aan een gelieerde persoon al zijn vorige lidobjecten met
		/// groepswerkjaren gekoppeld zijn.  Dit wordt gebruikt in LidMaken
		///  om na te kijken of een gelieerde persoon al eerder
		/// lid was.  Dit lijkt me echter niet nodig; zie de commentaar aldaar.
		/// </remarks>
		/// <throws>FoutNummerException</throws>
		/// <throws>GeenGavException</throws>
		/// <throws>InvalidOperationException</throws>
		private Leiding LeidingMaken(GelieerdePersoon gp, GroepsWerkJaar gwj, bool isJaarovergang)
		{
			// LidMaken doet de nodige checks ivm GAV-schap enz.
			var resultaat = LidMaken(gp, gwj, LidType.Leiding, isJaarovergang) as Leiding;

			Debug.Assert(gp.GebDatumMetChiroLeefTijd != null);  // Anders was er al wel een exception geworpen

			if (gwj.WerkJaar - gp.GebDatumMetChiroLeefTijd.Value.Year < Properties.Settings.Default.MinLeidingLeefTijd)
			{
				// Throw hier een InvalidOperationException, niet omdat dat goed is, maar wel om
				// consistent te blijven met KindMaken.
				// TODO #761
				throw new InvalidOperationException(Properties.Resources.TeJongVoorLeiding);
			}

			return resultaat;
		}

		/// <summary>
		/// Schrijft een gelieerde persoon zo automatisch mogelijk in, persisteert niet.
		///	<para />
		/// Als de persoon in een afdeling past, krijgt hij die afdeling. Als er meerdere passen, wordt er een gekozen.
		///	Als de persoon niet in een afdeling past, wordt hij leiding als hij oud genoeg is.
		///	Anders wordt een foutmelding gegeven.
		/// </summary>
		/// <param name="gp">De persoon om in te schrijven, gekoppeld met groep en persoon</param>
		/// <param name="gwj">Het groepswerkjaar waarin moet worden ingeschreven, gekoppeld met afdelingsjaren</param>
		///<param name="isJaarOvergang">geeft aan of het over de automatische jaarovergang gaat; relevant voor de
		/// probeerperiode</param>
		///<returns>Het aangemaakte lid object</returns>
		public Lid Inschrijven(GelieerdePersoon gp, GroepsWerkJaar gwj, bool isJaarOvergang)
		{
			if (!gp.GebDatumMetChiroLeefTijd.HasValue)
			{
				throw new GapException("De geboortedatum moet ingevuld zijn voor je iemand lid kunt maken.");
			}
			if (gp.Persoon.Geslacht != GeslachtsType.Man && gp.Persoon.Geslacht != GeslachtsType.Vrouw)
			{
				// FIXME: (#530) De boodschap in onderstaande exception wordt getoond aan de user,
				// terwijl dit eigenlijk een technische boodschap voor de developer moet zijn.

				throw new FoutNummerException(
					FoutNummer.OnbekendGeslachtFout, Properties.Resources.GeslachtVerplicht);
			}

			// Bepaal of het een kind of leiding wordt.  Als de persoon qua leeftijd in een niet-speciale
			// afdeling valt, wordt het een kind.

			// Stop de geboortedatum in een lokale variabele [wiki:VeelVoorkomendeWaarschuwingen#PossibleInvalidOperationinLinq-statement]
			var geboortejaar = gp.GebDatumMetChiroLeefTijd.Value.Year;
			var afdelingsjaar = (from a in gwj.AfdelingsJaar
								 where (a.OfficieleAfdeling.ID != (int)NationaleAfdeling.Speciaal) &&
									   (geboortejaar <= a.GeboorteJaarTot
										&& a.GeboorteJaarVan <= geboortejaar)
								 select a).FirstOrDefault();

			Lid nieuwlid;
			// Kijk of er een passend afdelingsjaar is
			if (afdelingsjaar != null)
			{
				nieuwlid = KindMaken(gp, gwj, isJaarOvergang);
			}
			// Kijk of de persoon oud genoeg is om leiding te worden
			else if (gwj.WerkJaar - gp.GebDatumMetChiroLeefTijd.Value.Year >= Properties.Settings.Default.MinLeidingLeefTijd)
			{
				nieuwlid = LeidingMaken(gp, gwj, isJaarOvergang);
			}
			else
			{
				throw new GapException("Je groep heeft geen afdeling voor die leeftijd en de persoon is te jong om leiding te worden.");
			}

			return nieuwlid;
		}

		/// <summary>
		/// Zet kinderen en leiding op non-actief. Geen van beide kunnen ooit verwijderd worden!!!
		/// </summary>
		/// <param name="lid">Het lid dat we non-actief willen maken</param>
		/// <remarks>Het <paramref name="lid"/> moet via het groepswerkjaar gekoppeld
		/// aan zijn groep.  Als het om leiding gaat, moeten ook de afdelingen gekoppeld zijn.</remarks>
		public void NonActiefMaken(Lid lid)
		{
			Debug.Assert(lid.GroepsWerkJaar != null);
			Debug.Assert(lid.GroepsWerkJaar.Groep != null);

			if (!_autorisatieMgr.IsGavLid(lid.ID))
			{
				throw new GeenGavException(Properties.Resources.GeenGav);
			}

			// checks:
			if (lid.GroepsWerkJaar.ID != _daos.GroepsWerkJaarDao.RecentsteOphalen(lid.GroepsWerkJaar.Groep.ID).ID)
			{
				throw new FoutNummerException(
					FoutNummer.GroepsWerkJaarNietBeschikbaar,
					Properties.Resources.GroepsWerkJaarVoorbij);
			}

			lid.NonActief = true;
			// TODO (#683): functies afpakken
			_daos.LedenDao.Bewaren(lid);
		}

		/// <summary>
		/// Persisteert een lid met de gekoppelde entiteiten bepaald door <paramref name="extras"/>.
		/// </summary>
		/// <param name="lid">Het <paramref name="lid"/> dat bewaard moet worden</param>
		/// <param name="extras">De gekoppelde entiteiten</param>
		/// <returns>Een kloon van het lid en de extra's, met eventuele nieuwe ID's ingevuld</returns>
		/// <remarks>BELANGRIJK! Een nieuw lid mag niet direct gesynct worden naar Kipadmin! Dit gebeurt
		/// pas bij het verstrijken van de probeerperiode, zie de method 'OverZettenNaProbeerPeriode'.  Enkel wijzigingen
		/// van reeds bestaande leden mogen hier naar de queue geduwd worden.</remarks>
		public Lid Bewaren(Lid lid, LidExtras extras)
		{
			if (!_autorisatieMgr.IsGavLid(lid.ID))
			{
				throw new GeenGavException(Properties.Resources.GeenGav);
			}

			Lid nieuwlid;
			if (lid is Kind)
			{
				try
				{
					nieuwlid = _daos.KindDao.Bewaren((Kind)lid, ExtrasNaarLambdasKind(extras));
				}
				catch (DubbeleEntiteitException<Kind>)
				{
					throw new BestaatAlException<Kind>(lid as Kind);
				}
			}
			else if (lid is Leiding)
			{
				try
				{
					nieuwlid = _daos.LeidingDao.Bewaren((Leiding)lid, ExtrasNaarLambdasLeiding(extras));
				}
				catch (Exception)
				{
					throw new BestaatAlException<Leiding>(lid as Leiding);
				}
			}
			else
			{
				throw new NotSupportedException(Properties.Resources.OngeldigLidType);
			}

			return nieuwlid;
		}

		/// <summary>
		/// Haalt lid op, op basis van zijn <paramref name="lidID"/>
		/// </summary>
		/// <param name="lidID">ID gevraagde lid</param>
		/// <param name="extras">Geeft aan welke gekoppelde entiteiten mee opgehaald moeten worden</param>
		/// <returns>Kind of Leiding met gevraagde <paramref name="extras"/>.</returns>
		public Lid Ophalen(int lidID, LidExtras extras)
		{
			if (!_autorisatieMgr.IsGavLid(lidID))
			{
				throw new GeenGavException(Properties.Resources.GeenGav);
			}

			Lid lid;
			if (_daos.LedenDao.IsLeiding(lidID))
			{
				lid = _daos.LeidingDao.Ophalen(lidID, ExtrasNaarLambdasLeiding(extras));
			}
			else
			{
				lid = _daos.KindDao.Ophalen(lidID, ExtrasNaarLambdasKind(extras));
			}
			return lid;
		}

		/// <summary>
		/// Haalt lid en gekoppelde persoon op, op basis van <paramref name="lidID"/>
		/// </summary>
		/// <param name="lidID">ID op te halen lid</param>
		/// <returns>Lid, met daaraan gekoppeld gelieerde persoon en persoon.</returns>
		public Lid Ophalen(int lidID)
		{
			return Ophalen(lidID, LidExtras.Geen);
		}

		/// <summary>
		/// Haalt het lid op bepaald door <paramref name="gelieerdePersoonID"/> en
		/// <paramref name="groepsWerkJaarID"/>, inclusief persoon, afdelingen, functies, groepswerkjaar
		/// </summary>
		/// <param name="gelieerdePersoonID">ID van de gelieerde persoon waarvoor het lidobject gevraagd is.</param>
		/// <param name="groepsWerkJaarID">ID van groepswerkjaar in hetwelke het lidobject gevraagd is</param>
		/// <returns>
		/// Het lid bepaald door <paramref name="gelieerdePersoonID"/> en
		/// <paramref name="groepsWerkJaarID"/>, inclusief persoon, afdelingen, functies, groepswerkjaar
		/// </returns>
		public Lid OphalenViaPersoon(int gelieerdePersoonID, int groepsWerkJaarID)
		{
			if (!_autorisatieMgr.IsGavGroepsWerkJaar(groepsWerkJaarID) || !_autorisatieMgr.IsGavGelieerdePersoon(gelieerdePersoonID))
			{
				throw new GeenGavException(Properties.Resources.GeenGav);
			}

			return _daos.LedenDao.OphalenViaPersoon(gelieerdePersoonID, groepsWerkJaarID);
		}

		/// <summary>
		/// Neemt de info uit <paramref name="lidInfo"/> over in <paramref name="lid"/>
		/// </summary>
		/// <param name="lidInfo">LidInfo om over te nemen in <paramref name="lid"/></param>
		/// <param name="lid">Lid dat <paramref name="lidInfo"/> moet krijgen</param>
		public void InfoOvernemen(LidInfo lidInfo, Lid lid)
		{
			Debug.Assert(lid is Leiding || lid is Kind);

			if (!_autorisatieMgr.IsGavLid(lid.ID))
			{
				throw new GeenGavException(Properties.Resources.GeenGav);
			}

			if (!_daos.GroepsWerkJaarDao.IsRecentste(lid.GroepsWerkJaar.ID))
			{
				throw new FoutNummerException(
					FoutNummer.GroepsWerkJaarNietBeschikbaar,
					Properties.Resources.GroepsWerkJaarVoorbij);
			}

			if (lid is Kind && lidInfo.Type == LidType.Leiding)
			{
				throw new NotImplementedException();
			}
			if (lid is Leiding && lidInfo.Type == LidType.Kind)
			{
				throw new NotImplementedException();
			}

			if (lid is Kind)
			{
				var kind = (Kind)lid;
				kind.LidgeldBetaald = lidInfo.LidgeldBetaald;
				kind.NonActief = lidInfo.NonActief;
			}
			else
			{
				var leiding = (Leiding)lid;
				leiding.NonActief = lidInfo.NonActief;
			}
		}

		/// <summary>
		/// Geeft een lijst terug van alle afdelingen waaraan het lid gegeven gekoppeld is.
		/// </summary>
		/// <param name="l">Het gegeven lid</param>
		/// <returns>Lijst met afdelingen</returns>
		/// <remarks>Een kind is hoogstens aan 1 afdeling gekoppeld</remarks>
		public static IList<int> AfdelingIdLijstGet(Lid l)
		{
			IList<int> result = new List<int>();
			if (l is Kind)
			{
				if ((l as Kind).AfdelingsJaar != null)
				{
					result.Add((l as Kind).AfdelingsJaar.Afdeling.ID);
				}
			}
			else if (l is Leiding)
			{
				// ReSharper disable LoopCanBeConvertedToQuery
				// Als ReShaper hier een Linq-query van maakt, staat er result.AddRange, en dat wordt niet herkend
				foreach (AfdelingsJaar aj in (l as Leiding).AfdelingsJaar)
				{
					result.Add(aj.Afdeling.ID);
				}
				// ReSharper restore LoopCanBeConvertedToQuery
			}
			else
			{
				Debug.Assert(false, "Lid moet kind of leiding zijn.");
			}

			return result;
		}

		/// <summary>
		/// Controleert of de datum <paramref name="dateTime"/> zich in het werkjaar <paramref name="p"/> bevindt.
		/// </summary>
		/// <param name="dateTime">Te controleren datum</param>
		/// <param name="p">Werkjaar.  (2010 voor 2010-2011 enz.)</param>
		/// <returns><c>true</c> als <paramref name="dateTime"/> zich in het werkjaar bevindt; anders <c>false</c>.</returns>
		public static bool DatumInWerkJaar(DateTime dateTime, int p)
		{
			var werkJaarStart = new DateTime(
				p,
				Properties.Settings.Default.WerkjaarStartNationaal.Month,
				Properties.Settings.Default.WerkjaarStartNationaal.Day);

			DateTime werkJaarStop = new DateTime(
				p + 1,
				Properties.Settings.Default.WerkjaarStartNationaal.Month,
				Properties.Settings.Default.WerkjaarStartNationaal.Day).AddDays(-1);

			return werkJaarStart <= dateTime && dateTime <= werkJaarStop;
		}

		/// <summary>
		/// Converteert lidextras <paramref name="extras"/> naar lambda-expresses voor een
		/// KindDao
		/// </summary>
		/// <param name="extras">Te converteren lidextras</param>
		/// <returns>Lambda-expresses voor een KindDao</returns>
		private static Expression<Func<Kind, object>>[] ExtrasNaarLambdasKind(LidExtras extras)
		{
			var paths = ExtrasNaarLambdas<Kind>(extras & ~LidExtras.Afdelingen);

			if ((extras & LidExtras.Afdelingen) != 0)
			{
				paths.Add(ld => ld.AfdelingsJaar.Afdeling.WithoutUpdate());
			}

			return paths.ToArray();
		}

		/// <summary>
		/// Converteert lidextras <paramref name="extras"/> naar lambda-expresses voor een
		/// LeidingDao.
		/// </summary>
		/// <param name="extras">Te converteren lidextras</param>
		/// <returns>Lambda-expresses voor een LeidingDao</returns>
		private static Expression<Func<Leiding, object>>[] ExtrasNaarLambdasLeiding(LidExtras extras)
		{
			var paths = ExtrasNaarLambdas<Leiding>(extras & ~LidExtras.Afdelingen);

			if ((extras & LidExtras.Afdelingen) != 0)
			{
				paths.Add(ld => ld.AfdelingsJaar.First().Afdeling.WithoutUpdate());
			}

			return paths.ToArray();
		}

		/// <summary>
		/// Converteert lidextras <paramref name="extras"/> naar lambda-expresses voor een
		/// LedenDao.
		/// </summary>
		/// <param name="extras">Te converteren lidextras</param>
		/// <returns>Lambda-expresses voor een LedenDao</returns>
		private static IEnumerable<Expression<Func<Lid, object>>> ExtrasNaarLambdasLid(LidExtras extras)
		{
			return ExtrasNaarLambdas<Lid>(extras & ~LidExtras.Afdelingen).ToArray();
		}

		/// <summary>
		/// Converteert LidExtra's naar lambda-expressies voor de data-access
		/// </summary>
		/// <param name="extras">Te converteren lidextra's</param>
		/// <typeparam name="T"></typeparam>
		/// <returns>Lijst lambda-expressies geschikt voor de LedenDAO</returns>
		private static IList<Expression<Func<T, object>>> ExtrasNaarLambdas<T>(LidExtras extras) where T : Lid
		{
			var paths = new List<Expression<Func<T, object>>> { ld => ld.GroepsWerkJaar.WithoutUpdate() };

			if ((extras & LidExtras.VoorkeurAdres) != 0)
			{
				// enkel voorkeursadres
				paths.Add(ld => ld.GelieerdePersoon.PersoonsAdres.Adres.WoonPlaats.WithoutUpdate());
				paths.Add(ld => ld.GelieerdePersoon.PersoonsAdres.Adres.StraatNaam.WithoutUpdate());
			}

			if ((extras & LidExtras.Adressen) != 0)
			{
				// alle adressen
				paths.Add(ld => ld.GelieerdePersoon.Persoon.PersoonsAdres.First().Adres.WoonPlaats.WithoutUpdate());
				paths.Add(ld => ld.GelieerdePersoon.Persoon.PersoonsAdres.First().Adres.StraatNaam.WithoutUpdate());

				// link naar standaardadres
				paths.Add(ld => ld.GelieerdePersoon.PersoonsAdres.Adres.WithoutUpdate());
			}
			else if ((extras & LidExtras.Persoon) != 0)
			{
				paths.Add(ld => ld.GelieerdePersoon.Persoon);
			}

			if ((extras & LidExtras.Communicatie) != 0)
			{
				paths.Add(ld => ld.GelieerdePersoon.Communicatie.First().CommunicatieType.WithoutUpdate());
			}

			if ((extras & LidExtras.Groep) != 0)
			{
				paths.Add(ld => ld.GroepsWerkJaar.Groep.WithoutUpdate());
			}
			if ((extras & LidExtras.Afdelingen) != 0)
			{
				//// Onderstaande had cool geweest; dan hadden we de generieke <T> niet nodig. 
				//// Maar helaas lukt dat (nog??) niet met AttachObjectGraph:

				// paths.Add(ld => ld is Kind ? (ld as Kind).AfdelingsJaar.Afdeling : ld is Leiding ? (ld as Leiding).AfdelingsJaar.First().Afdeling : null);

				//// Dus:
				throw new NotSupportedException();
			}
			if ((extras & LidExtras.Functies) != 0)
			{
				// FIXME (#116): Hieronder zou 'WithoutUpdate' gebruikt moeten worden, maar owv #116 kan dat nog niet.
				paths.Add(ld => ld.Functie.First());
			}
			if ((extras & LidExtras.AlleAfdelingen) != 0)
			{
				paths.Add(ld => ld.GroepsWerkJaar.AfdelingsJaar.First().Afdeling.WithoutUpdate());
				paths.Add(ld => ld.GroepsWerkJaar.AfdelingsJaar.First().OfficieleAfdeling.WithoutUpdate());
			}
			if ((extras & LidExtras.Verzekeringen) != 0)
			{
				paths.Add(ld => ld.GelieerdePersoon.Persoon.PersoonsVerzekering.First().VerzekeringsType.WithoutUpdate());
			}
			return paths;
		}

		/// <summary>
		/// Zoekt de niet-overgezette leden op wier probeerperiode voorbij is, en stuurt diens gegevens
		/// naar KipSync.
		/// </summary>
		public void OverZettenNaProbeerPeriode()
		{
			if (!_autorisatieMgr.IsSuperGav())
			{
				throw new GeenGavException(Properties.Resources.GeenGav);
			}

			// Begrens het aantal op te halen leden, omdat in op 15 oktober naar schatting 75000 leden
			// over te zetten zullen zijn.

			IEnumerable<Lid> teSyncen = _daos.LedenDao.OverTeZettenOphalen(Properties.Settings.Default.AantalLedenOverzettenPerKeer);

			while (teSyncen.FirstOrDefault() != null)
			{
				foreach (Lid l in teSyncen)
				{
#if KIPDORP
					using (var tx = new TransactionScope())
					{
#endif
						_sync.Bewaren(l);
						l.IsOvergezet = true;
						_daos.LedenDao.Bewaren(l);
#if KIPDORP
						tx.Complete();
					}
#endif
				}
				teSyncen = _daos.LedenDao.OverTeZettenOphalen(Properties.Settings.Default.AantalLedenOverzettenPerKeer);
			}
		}

		/// <summary>
		/// Maakt van een kindlid een leid(st)er of omgekeerd.  Persisteert.
		/// Functies gaan voor het gemak verloren.
		/// Gekoppelde afdelingen gaan verloren; een lid krijgt meteen
		/// een juiste afdeling.
		/// </summary>
		/// <param name="lid">Lid waarvan type moet worden veranderd</param>
		public Lid TypeToggle(Lid lid)
		{
			if (!_autorisatieMgr.IsGavLid(lid.ID))
			{
				throw new GeenGavException(Properties.Resources.GeenGav);
			}

			var gelieerdePersoon = lid.GelieerdePersoon;
			var groepsWerkJaar = lid.GroepsWerkJaar;
			var nieuwType = LidType.Alles & (~lid.Type);

			Lid nieuwLid;

#if KIPDORP
			using (var tx = new TransactionScope())
			{
#endif
				// Voor 't gemak eerst verwijderen, en dan terug aanmaken.

				foreach (var fn in lid.Functie)
				{
					fn.TeVerwijderen = true;
				}

                                if (lid is Kind)
                                {
                                	var kind = lid as Kind;
					kind.TeVerwijderen = true;
					_daos.KindDao.Bewaren(kind, knd => knd.AfdelingsJaar, knd => knd.Functie);	

                                }
                                else
                                {
                                	var leiding = lid as Leiding;

                                	Debug.Assert(leiding != null);

					foreach (var aj in leiding.AfdelingsJaar)
					{
						aj.TeVerwijderen = true;
					}
                                	leiding.TeVerwijderen = true;
                                	_daos.LeidingDao.Bewaren(leiding, ld => ld.AfdelingsJaar, ld => ld.Functie);
                                }
				
				// Met heel dat 'TeVerwijderen'-gedoe, is het domein typisch
				// niet meer consistent na iets te verwijderen.

				gelieerdePersoon.Lid.Clear();
				groepsWerkJaar.Lid.Clear();
				foreach (var aj in groepsWerkJaar.AfdelingsJaar)
				{
					aj.TeVerwijderen = false;
				}

				// Maak opnieuw lid

				if (nieuwType == LidType.Kind)
				{
					nieuwLid = KindMaken(gelieerdePersoon, groepsWerkJaar, false);
					nieuwLid.EindeInstapPeriode = lid.EindeInstapPeriode;
					nieuwLid.IsOvergezet = lid.IsOvergezet;
					nieuwLid = _daos.KindDao.Bewaren(
						nieuwLid as Kind,
						ld => ld.GroepsWerkJaar.WithoutUpdate(),
						ld => ld.GelieerdePersoon.WithoutUpdate(),
						ld => ld.AfdelingsJaar.WithoutUpdate());
				}
				else
				{
					nieuwLid = LeidingMaken(gelieerdePersoon, groepsWerkJaar, false);
					nieuwLid.EindeInstapPeriode = lid.EindeInstapPeriode;
					nieuwLid.IsOvergezet = lid.IsOvergezet;
					nieuwLid = _daos.LeidingDao.Bewaren(
						nieuwLid as Leiding,
						ld => ld.GroepsWerkJaar.WithoutUpdate(),
						ld => ld.GelieerdePersoon.WithoutUpdate());
				}
				if (lid.IsOvergezet)
				{
					// In 2 keer syncen; pragmatische aanpak voor TODO #762
					_sync.TypeUpdaten(nieuwLid);
					_sync.AfdelingenUpdaten(nieuwLid);
				}



#if KIPDORP
				tx.Complete();
			}
#endif
			return nieuwLid;

		}

		/// <summary>
		/// Zoekt leden op, op basis van de gegeven <paramref name="filter"/>.
		/// </summary>
		/// <param name="filter">De niet-nulle properties van de filter
		/// bepalen waarop gezocht moet worden</param>
		/// <param name="extras">Bepaalt de mee op te halen gekoppelde entiteiten. 
		/// (Adressen ophalen vertraagt aanzienlijk.)
		/// </param>
		/// <returns>Lijst met info over gevonden leden</returns>
		/// <remarks>
		/// Er worden enkel actieve leden opgehaald
		/// </remarks>
		public IEnumerable<Lid> Zoeken(LidFilter filter, LidExtras extras)
		{
			if (filter.GroepID != null && !_autorisatieMgr.IsGavGroep(filter.GroepID.Value) ||
				filter.GroepsWerkJaarID != null && !_autorisatieMgr.IsGavGroepsWerkJaar(filter.GroepsWerkJaarID.Value) ||
				filter.AfdelingID != null && !_autorisatieMgr.IsGavAfdeling(filter.AfdelingID.Value) ||
				filter.FunctieID != null && !_autorisatieMgr.IsGavFunctie(filter.FunctieID.Value))
			{
				throw new GeenGavException(Properties.Resources.GeenGav);
			}

			// TODO: Nog lang niet alles is ondersteund wat betreft ledenlijsten filteren.
			// De bedoeling is dat er ook in de data-access een zoekfunctie komt die met LidFilter werkt.
			// Voorlopig is deze method een rommeltje, dat de huidige methods van LedenDao gebruikt.

			IEnumerable<Lid> kinderen = null;
			IEnumerable<Lid> leiding = _daos.LeidingDao.Zoeken(filter, ExtrasNaarLambdasLeiding(extras)).Cast<Lid>();
			IEnumerable<Lid> alles = null;

			if (filter.HeeftEmailAdres != null || 
				filter.HeeftTelefoonNummer != null || 
				filter.HeeftEmailAdres != null)
			{
				throw new NotImplementedException();
			}

			if (filter.ProbeerPeriodeNa != null)
			{
				if (filter.ProbeerPeriodeNa.Value.Date != DateTime.Now.Date ||
					filter.AfdelingID != null ||
					filter.FunctieID != null ||
					filter.GroepsWerkJaarID != null)
				{
					throw new NotImplementedException();
				}
				kinderen = _daos.KindDao.ProbeerLedenOphalen(
					filter.GroepID.Value, 
					ExtrasNaarLambdasKind(extras)).Cast<Lid>();
			}
			else if (filter.AfdelingID != null && filter.FunctieID != null)
			{
				throw new NotImplementedException();
			}
			else if (filter.AfdelingID != null)
			{
				if (filter.GroepsWerkJaarID == null)
				{
					throw new NotImplementedException();
				}
				kinderen = _daos.KindDao.OphalenUitAfdelingsJaar(
					filter.GroepsWerkJaarID.Value,
					filter.AfdelingID.Value,
					ExtrasNaarLambdasKind(extras)).Cast<Lid>();
			}
			else if (filter.FunctieID != null)
			{
				if (filter.GroepsWerkJaarID == null)
				{
					throw new NotImplementedException();
				}
				kinderen = _daos.KindDao.OphalenUitFunctie(
					filter.GroepsWerkJaarID.Value,
					filter.FunctieID.Value,
					ExtrasNaarLambdasKind(extras)).Cast<Lid>();
			}
			else if (filter.GroepsWerkJaarID != null)
			{
				kinderen = _daos.KindDao.OphalenUitGroepsWerkJaar(
					filter.GroepsWerkJaarID.Value,
					ExtrasNaarLambdasKind(extras)).Cast<Lid>();				
			}
			else
			{
				throw new NotImplementedException();
			}

			// Sorteren doen we hier niet; dat is presentatie :)

			// Voeg kinderen en leiding samen, en haal de inactieve er uit

			alles = kinderen.Union(leiding);

			return alles.Where(ld => ld.NonActief == false).ToArray();
		}
	}
}
