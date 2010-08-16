// <copyright company="Chirojeugd-Vlaanderen vzw">
// Copyright (c) 2007-2010
// Mail naar informatica@chiro.be voor alle info over deze broncode
// </copyright>

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;

using Chiro.Cdf.Data;
using Chiro.Gap.Domain;
using Chiro.Gap.Orm;
using Chiro.Gap.Orm.DataInterfaces;
using Chiro.Gap.ServiceContracts.DataContracts;
using Chiro.Gap.Workers.Exceptions;

namespace Chiro.Gap.Workers
{
	/// <summary>
	/// DONE (#190): Documenteren
	/// Klasse met extra methode om het einde van de jaarovergang in een groepswerkjaar op te vragen.
	/// </summary>
	public static class GroepsWerkJaarHelper
	{
		/// <summary>
		/// DONE (#190): Documenteren
		/// Berekend aan de hand van een gegeven werkjaar de datum van het verplichte einde van de instapperiode in dat jaar.
		/// Belangrijk => volgens de HUIDIGE settings van dat werkjaareinde (moest dat in de toekomst veranderen en we hebben dat van vroeger nodig)
		/// </summary>
		/// <param name="gwj">Het groepswerkjaar waarvoor we het einde van de jaarovergang willen berekenen</param>
		/// <returns>De datum waarom de jaarovergang eindigt</returns>
		public static DateTime GetEindeJaarovergang(this GroepsWerkJaar gwj)
		{
			var dt = Properties.Settings.Default.WerkjaarVerplichteOvergang;
			dt.AddYears(gwj.WerkJaar - dt.Year);
			return dt; // DONE (#610): 15 oktober mag niet hardgecodeerd zijn, dat hoort thuis in de settings
		}
	}

	/// <summary>
	/// Worker die alle businesslogica i.v.m. leden bevat
	/// </summary>
	public class LedenManager
	{
		private readonly LedenDaoCollectie _daos;
		private readonly IAutorisatieManager _autorisatieMgr;

		/// <summary>
		/// Maakt een nieuwe ledenmanager aan
		/// </summary>
		/// <param name="daos">Een hele reeks van IDao-objecten, nodig
		/// voor data access.</param>
		/// <param name="autorisatie">Een IAuthorisatieManager, die
		/// de GAV-permissies van de huidige user controleert.</param>
		public LedenManager(LedenDaoCollectie daos, IAutorisatieManager autorisatie)
		{
			_daos = daos;
			_autorisatieMgr = autorisatie;
		}

		/// <summary>
		/// Maakt een gelieerde persoon <paramref name="gp"/> lid in groepswerkjaar <paramref name="gwj"/>,
		/// met lidtype <paramref name="type"/>
		/// </summary>
		/// <param name="gp">Lid te maken gelieerde persoon</param>
		/// <param name="gwj">Groepswerkjaar waarin de gelieerde persoon lid moet worden</param>
		/// <param name="type">LidType.Kind of LidType.Leiding</param>
		/// <remarks>
		/// Deze method kent geen afdelingen toe.  Ze test ook niet
		/// of het groepswerkjaar wel het recentste is.  (Voor de unit tests moeten
		/// we ook leden kunnen maken in oude groepswerkjaren.)
		/// <para/>
		/// Voorlopig gaan we ervan uit dat aan een gelieerde persoon al zijn/haar vorige lidobjecten met
		/// groepswerkjaren gekoppeld zijn.  Dit wordt gebruikt om na te kijken of een gelieerde persoon al eerder
		/// lid was.  Dit lijkt me echter niet nodig; zie de commentaar verderop.
		/// </remarks>
		/// <returns>Het aangepaste Lid-object</returns>
		/// <throws>FoutNummerException</throws>
		/// <throws>GeenGavException</throws>
		/// <throws>InvalidOperationException</throws>
		private Lid LidMaken(GelieerdePersoon gp, GroepsWerkJaar gwj, LidType type)
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
			if (!gp.LeefTijd.HasValue)
			{
				throw new InvalidOperationException(Properties.Resources.GeboorteDatumOntbreekt);
			}

			// GroepsWerkJaar en GelieerdePersoon invullen
			lid.GroepsWerkJaar = gwj;
			lid.GelieerdePersoon = gp;
			gp.Lid.Add(lid);
			gwj.Lid.Add(lid);

			// Instapperiode invullen
			// Haal alle groepswerkjaren op // TODO in de toekomst niet efficient genoeg!
			// en selecteert het werkjaar van een jaar geleden als het bestaat

			var voriggwj = (from ld in gp.Lid
							where ld.GroepsWerkJaar.WerkJaar == gwj.WerkJaar - 1
							select ld.GroepsWerkJaar).FirstOrDefault();

			// Als er vorig jaar een werkjaar was en de persoon was toen lid, dan zal zijn probeerperiode maximum tot 15 oktober zijn, 
			// eender wanneer de persoon lid wordt.
			if (voriggwj != null)
			{
				lid.EindeInstapPeriode = gwj.GetEindeJaarovergang();
			}
			else
			{
				// In het andere geval was de persoon vorig jaar geen lid (of was er geen vorig jaar), dus krijgt hij de standaard periode om te bedenken.
				// Of als de overgang nog verder in de toekomst ligt, wordt dat de datum
				DateTime een = gwj.GetEindeJaarovergang();
				DateTime twee = DateTime.Today.AddDays(Properties.Settings.Default.LengteProbeerPeriode);
				lid.EindeInstapPeriode = een.CompareTo(twee) < 0 ? twee : een;
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
		/// <param name="gp">Gelieerde persoon, gekoppeld aan groep</param>
		/// <param name="gwj">Groepswerkjaar waarin lid te maken</param>
		/// <returns>Nieuw kindobject, niet gepersisteerd</returns>
		/// <remarks>De user zal nooit zelf mogen kiezen in welk groepswerkjaar een kind lid wordt.  Maar 
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
		public Kind KindMaken(GelieerdePersoon gp, GroepsWerkJaar gwj)
		{
			// LidMaken doet de nodige checks ivm GAV-schap, enz.
			var k = LidMaken(gp, gwj, LidType.Kind) as Kind;

			// Probeer nu afdeling te vinden.
			if (gwj.AfdelingsJaar.Count == 0)
			{
				throw new InvalidOperationException(Properties.Resources.InschrijvenZonderAfdelingen);
			}

			// Afdeling automatisch bepalen
			// Bepaal het geboortejaar, aangepast volgens de Chiroleeftijd.
// ReSharper disable PossibleInvalidOperationException
			// Controle of geboortedatum null is, gebeurde al in LidMaken
			var geboortejaar = gp.LeefTijd.Value.Year;
// ReSharper restore PossibleInvalidOperationException

			// Relevante afdelingsjaren opzoeken
			var afdelingsjaren =
				(from a in gwj.AfdelingsJaar
				 where a.GeboorteJaarVan <= geboortejaar && geboortejaar <= a.GeboorteJaarTot
				 select a).ToList();

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

// ReSharper disable PossibleNullReferenceException
			// Elk scenario waarbij k null is, leidt tot een exception, maar dat gebeurt deels in LidMaken, dus ziet ReSharper dat niet
			k.AfdelingsJaar = aj;
// ReSharper restore PossibleNullReferenceException
			aj.Kind.Add(k);

			return k;
		}

		/// <summary>
		/// Maakt gelieerde persoon leiding voor het gegeven werkjaar.
		/// </summary>
		/// <param name="gp">Gelieerde persoon</param>
		/// <param name="gwj">Groepswerkjaar waarin leiding te maken</param>
		/// <returns>Nieuw leidingsobject; niet gepersisteerd</returns>
		/// <remarks>Deze method mag niet geexposed worden via de services, omdat
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
		public Leiding LeidingMaken(GelieerdePersoon gp, GroepsWerkJaar gwj)
		{
			// LidMaken doet de nodige checks ivm GAV-schap enz.
			return LidMaken(gp, gwj, LidType.Leiding) as Leiding;
		}

		public Lid AutomatischLidMaken(GelieerdePersoon gp, GroepsWerkJaar gwj)
		{
			if (!gp.LeefTijd.HasValue)
			{
				throw new OngeldigObjectException("De geboortedatum moet ingevuld zijn voor je iemand lid kunt maken.");
			}

			// Bepaal of het een kind of leiding wordt. 

			// Stop de geboortedatum in een lokale variabele voor gebruik in Linq-statement. 
			// (zie [wiki:VeelVoorkomendeWaarschuwingen#PossibleInvalidOperationinLinq-statement])
			var geboortejaar = gp.LeefTijd.Value.Year;
			var afdeling = (from a in gwj.AfdelingsJaar
							where
								(geboortejaar <= a.GeboorteJaarTot
								&& a.GeboorteJaarVan <= geboortejaar)
							select a).FirstOrDefault();

			Lid nieuwlid;
			if (afdeling != null) //Er is een afdeling met de gewenste leeftijd
			{
				nieuwlid = KindMaken(gp, gwj);
			} 
			else if (gwj.WerkJaar - gp.LeefTijd.Value.Year >= Properties.Settings.Default.MinLeidingLeefTijd) 
			{
				nieuwlid = LeidingMaken(gp, gwj);
			} 
			else
			{
				throw new OngeldigObjectException("De persoon is te jong om leiding te worden en je groep heeft geen afdeling voor die leeftijd.");
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
			_daos.LedenDao.Bewaren(lid);
		}

		/// <summary>
		/// Haal een pagina op met leden van een groepswerkjaar.
		/// </summary>
		/// <param name="groepsWerkJaarID">ID van het groepswerkjaar</param>
		/// <param name="sortering">De parameter waarop de gegevens gesorteerd moeten worden</param>
		/// <returns>Lijst met alle leden uit het gevraagde groepswerkjaar.</returns>
		public IList<Lid> PaginaOphalen(int groepsWerkJaarID, LedenSorteringsEnum sortering)
		{
			// TODO: deze functie mergen met PaginaOphalen(groepsWerkJaarID, extras).
			// Ik wacht hier nog mee tot Broes' sorteercode op punt staat.

			if (!_autorisatieMgr.IsGavGroepsWerkJaar(groepsWerkJaarID))
			{
				throw new GeenGavException(Properties.Resources.GeenGav);
			}

			var list = _daos.LedenDao.ActieveLedenOphalen(groepsWerkJaarID, sortering);
			return list;
		}

		/// <summary>
		/// Haal een pagina op met leden van een groepswerkjaar.
		/// </summary>
		/// <param name="groepsWerkJaarID">ID van het groepswerkjaar</param>
		/// <param name="extras">Bepaalt welke extra entiteiten mee opgevraagd worden</param>
		/// <returns>Lijst met alle leden uit het gevraagde groepswerkjaar.</returns>
		public IList<Lid> PaginaOphalen(int groepsWerkJaarID, LidExtras extras)
		{
			if (!_autorisatieMgr.IsGavGroepsWerkJaar(groepsWerkJaarID))
			{
				throw new GeenGavException(Properties.Resources.GeenGav);
			}

			// Ik haal leden en leiding apart op, omdat de lambda-expressies verschillend zijn als er
			// afdelingen bij in de 'extra's' zitten.

			IEnumerable<Lid> kindLijst = _daos.KindDao.OphalenUitGroepsWerkJaar(groepsWerkJaarID, ExtrasNaarLambdasKind(extras)).Cast<Lid>();
			IEnumerable<Lid> leidingLijst = _daos.LeidingDao.OphalenUitGroepsWerkJaar(groepsWerkJaarID, ExtrasNaarLambdasLeiding(extras)).Cast<Lid>();

			var list = kindLijst.Union(leidingLijst);

			// TODO: lijst sorteren, maar ik wacht hier nog mee tot Broes' sorteercode wat stabiliseert
			// voorlopig sorteer ik gewoon op naam

			return
				list.OrderBy(ld => ld.GelieerdePersoon.Persoon.Naam).ThenBy(ld => ld.GelieerdePersoon.Persoon.VoorNaam).ToList();
		}

		/// <summary>
		/// Haalt een 'pagina' op met leden uit een bepaald GroepsWerkJaar
		/// </summary>
		/// <param name="groepsWerkJaarID">ID gevraagde GroepsWerkJaar</param>
		/// <param name="afdelingsID">ID gevraagde afdeling</param>
		/// <param name="sortering">De parameter waarop de gegevens gesorteerd moeten worden</param>
		/// <returns>De 'pagina' (collectie) met leden</returns>
		public IList<Lid> PaginaOphalenVolgensAfdeling(int groepsWerkJaarID, int afdelingsID, LedenSorteringsEnum sortering)
		{
			// TODO: Mergen met PaginaOphalenVolgensAfdelng(groepsWerkJaarID, afdelingsID, extras)
			// (Ik wacht hier even mee tot Broes' sortering stabiel is)

			if (!_autorisatieMgr.IsGavGroepsWerkJaar(groepsWerkJaarID))
			{
				throw new GeenGavException(Properties.Resources.GeenGav);
			}

			return _daos.LedenDao.PaginaOphalenVolgensAfdeling(groepsWerkJaarID, afdelingsID, sortering);
		}

		/// <summary>
		/// Haalt een 'pagina' op met leden uit een bepaald GroepsWerkJaar
		/// </summary>
		/// <param name="groepsWerkJaarID">ID gevraagde GroepsWerkJaar</param>
		/// <param name="afdelingID">ID gevraagde afdeling</param>
		/// <param name="extras">Beschrijving van de extra gegevens die opgehaald moeten worden</param>
		/// <returns>De 'pagina' (collectie) met leden</returns>
		public IEnumerable<Lid> PaginaOphalenVolgensAfdeling(int groepsWerkJaarID, int afdelingID, LidExtras extras)
		{
			if (!_autorisatieMgr.IsGavGroepsWerkJaar(groepsWerkJaarID))
			{
				throw new GeenGavException(Properties.Resources.GeenGav);
			}

			// Ik haal leden en leiding apart op, omdat de lambda-expressies verschillend zijn als er
			// afdelingen bij in de 'extra's' zitten.

			IEnumerable<Lid> kindLijst = _daos.KindDao.OphalenUitAfdelingsJaar(groepsWerkJaarID, afdelingID, ExtrasNaarLambdasKind(extras)).Cast<Lid>();
			IEnumerable<Lid> leidingLijst = _daos.LeidingDao.OphalenUitAfdelingsJaar(groepsWerkJaarID, afdelingID, ExtrasNaarLambdasLeiding(extras)).Cast<Lid>();

			var list = kindLijst.Union(leidingLijst);

			// TODO: lijst sorteren, maar ik wacht hier nog mee tot Broes' sorteercode wat stabiliseert
			// voorlopig sorteer ik gewoon op naam

			return
				list.OrderBy(ld => ld.GelieerdePersoon.Persoon.Naam).ThenBy(ld => ld.GelieerdePersoon.Persoon.VoorNaam).ToList();
		}

		/// <summary>
		/// Haalt een 'pagina' op met leden uit een bepaald GroepsWerkJaar
		/// </summary>
		/// <param name="groepsWerkJaarID">ID gevraagde GroepsWerkJaar</param>
		/// <param name="functieID">ID gevraagde functie</param>
		/// <param name="sortering">De parameter waarop de gegevens gesorteerd moeten worden</param>
		/// <returns>De lijst van leden die in het opgegeven GroepsWerkJaar de opgegeven functie hadden/hebben,
		/// gesorteerd volgens de opgegeven parameter</returns>
		public IList<Lid> PaginaOphalenVolgensFunctie(int groepsWerkJaarID, int functieID, LedenSorteringsEnum sortering)
		{
			if (!_autorisatieMgr.IsGavGroepsWerkJaar(groepsWerkJaarID))
			{
				throw new GeenGavException(Properties.Resources.GeenGav);
			}

			return _daos.LedenDao.PaginaOphalenVolgensFunctie(groepsWerkJaarID, functieID, sortering);
		}

		/// <summary>
		/// Persisteert een lid met de gekoppelde entiteiten bepaald door <paramref name="extras"/>.
		/// </summary>
		/// <param name="lid">Het <paramref name="lid"/> dat bewaard moet worden</param>
		/// <param name="extras">De gekoppelde entiteiten</param>
		/// <returns>Een kloon van het lid en de extra's, met eventuele nieuwe ID's ingevuld</returns>
		public Lid LidBewaren(Lid lid, LidExtras extras)
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
				catch (KeyViolationException<Kind>)
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
		/// Haalt leden op uit een bepaald groepswerkjaar met een gegeven functie
		/// </summary>
		/// <param name="functieID">ID van de functie</param>
		/// <param name="groepsWerkJaarID">ID van het groepswerkjaar</param>
		/// <param name="extras">Geeft aan welke gekoppelde entiteiten mee opgehaald moeten worden
		/// met de leden</param>
		/// <returns>Lijst leden uit het groepswerkjaar met de gegeven functie</returns>
		/// <remarks>Persoonsgegevens worden standaard mee opgehaald met lid.</remarks>
		public IList<Lid> Ophalen(int functieID, int groepsWerkJaarID, LidExtras extras)
		{
			if (!_autorisatieMgr.IsGavGroepsWerkJaar(groepsWerkJaarID))
			{
				throw new GeenGavException(Properties.Resources.GeenGav);
			}
			if (!_autorisatieMgr.IsGavFunctie(functieID))
			{
				throw new GeenGavException(Properties.Resources.GeenGav);
			}
			var paths = ExtrasNaarLambdasLid(extras);

			return _daos.LedenDao.OphalenUitFunctie(
				functieID,
				groepsWerkJaarID,
				paths.ToArray());
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

			return _daos.LedenDao.OphalenViaPersoon(
				gelieerdePersoonID,
				groepsWerkJaarID);
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
			var paths = new List<Expression<Func<T, object>>> {ld => ld.GroepsWerkJaar.WithoutUpdate()};

			if ((extras & LidExtras.Adressen) != 0)
			{
				// alle adressen
				paths.Add(ld => ld.GelieerdePersoon.Persoon.PersoonsAdres.First().Adres.WoonPlaats.WithoutUpdate());
				paths.Add(ld => ld.GelieerdePersoon.Persoon.PersoonsAdres.First().Adres.StraatNaam.WithoutUpdate());

				// link naar standaardadres
				paths.Add(ld => ld.GelieerdePersoon.PersoonsAdres.Adres);
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
				paths.Add(ld => ld.Functie.First().WithoutUpdate());
			}
			if ((extras & LidExtras.AlleAfdelingen) != 0)
			{
				paths.Add(ld => ld.GroepsWerkJaar.AfdelingsJaar.First().Afdeling.WithoutUpdate());
			}
			if ((extras & LidExtras.Verzekeringen) != 0)
			{
				paths.Add(ld => ld.GelieerdePersoon.Persoon.PersoonsVerzekering.First().VerzekeringsType.WithoutUpdate());
			}
			return paths;
		}
	}
}
