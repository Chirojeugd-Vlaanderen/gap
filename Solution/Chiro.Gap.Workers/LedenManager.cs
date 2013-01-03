// <copyright company="Chirojeugd-Vlaanderen vzw" file="LedenManager.cs">
//   Copyright (c) 2007-2012 Mail naar informatica@chiro.be voor alle info over deze broncode
// </copyright>

#if KIPDORP
using System.Transactions;
#endif
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

using Chiro.Cdf.Data;
using Chiro.Gap.Domain;
using Chiro.Gap.Orm;
using Chiro.Gap.Orm.DataInterfaces;
using Chiro.Gap.Orm.SyncInterfaces;
using Chiro.Gap.WorkerInterfaces;
using Chiro.Gap.Workers.Exceptions;
using Chiro.Gap.Workers.Properties;

namespace Chiro.Gap.Workers
{
    /// <summary>
    /// Klasse met extra methode om het einde van de jaarovergang in een groepswerkjaar op te vragen.
    /// </summary>
    public static class GroepsWerkJaarHelper
    {
        /// <summary>
        /// Berekent aan de hand van een gegeven werkJaar de datum van het verplichte einde van de instapperiode in dat jaar.
        /// Belangrijk =&gt; volgens de HUIDIGE settings van dat werkjaareinde (moest dat in de toekomst veranderen en we hebben dat van vroeger nodig)
        /// </summary>
        /// <param name="gwj">
        /// Het groepswerkjaar waarvoor we het einde van de jaarovergang willen berekenen
        /// </param>
        /// <returns>
        /// De datum waarom de jaarovergang eindigt
        /// </returns>
        public static DateTime GetEindeJaarovergang(this GroepsWerkJaar gwj)
        {
            // Haal de einddatum voor de overgang/aansluiting uit de settings, en bereken wanneer die datum valt in dit werkJaar
            var dt = Settings.Default.WerkjaarVerplichteOvergang;
            return new DateTime(gwj.WerkJaar, dt.Month, dt.Day);
        }
    }

    /// <summary>
    /// Worker die alle businesslogica i.v.m. leden bevat
    /// </summary>
    public class LedenManager : ILedenManager
    {
        private readonly LedenDaoCollectie _daos;
        private readonly IAutorisatieManager _autorisatieMgr;
        private readonly ILedenSync _sync;

        /// <summary>
        /// Maakt een nieuwe ledenmanager aan
        /// </summary>
        /// <param name="daos">
        /// Een hele reeks van IDao-objecten, nodig
        /// voor data access.
        /// </param>
        /// <param name="autorisatie">
        /// Een IAuthorisatieManager, die
        /// de GAV-permissies van de huidige user controleert.
        /// </param>
        /// <param name="sync">
        /// Zorgt voor synchronisate van adressen naar KipAdmin
        /// </param>
        public LedenManager(LedenDaoCollectie daos,
                            IAutorisatieManager autorisatie,
                            ILedenSync sync)
        {
            _daos = daos;
            _autorisatieMgr = autorisatie;
            _sync = sync;
        }

        /// <summary>
        /// Maakt een gelieerde persoon <paramref name="gp"/> lid in groepswerkjaar <paramref name="gwj"/>,
        /// met lidtype <paramref name="type"/>, persisteert niet.
        /// </summary>
        /// <param name="gp">
        /// Lid te maken gelieerde persoon, gekoppeld met groep en persoon
        /// </param>
        /// <param name="gwj">
        /// Groepswerkjaar waarin de gelieerde persoon lid moet worden
        /// </param>
        /// <param name="type">
        /// LidType.Kind of LidType.Leiding
        /// </param>
        /// <param name="isJaarOvergang">
        /// Als deze true is, is einde probeerperiode steeds 
        /// 15 oktober als het nog geen 15 oktober is
        /// </param>
        /// <remarks>
        /// Private; andere lagen moeten via 'Inschrijven' gaan.
        /// <para>
        /// </para>
        /// Deze method test niet of het groepswerkjaar wel het recentste is.  (Voor de unit tests moeten
        /// we ook leden kunnen maken in oude groepswerkjaren.)
        /// Roep deze method ook niet rechtstreeks aan, maar wel via KindMaken of LeidingMaken
        /// </remarks>
        /// <returns>
        /// Het aangepaste Lid-object
        /// </returns>
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
                throw new GeenGavException(Resources.GeenGav);
            }

            if (gp.Groep.ID != gwj.Groep.ID)
            {
                throw new FoutNummerException(FoutNummer.GroepsWerkJaarNietVanGroep,
                                              Resources.GroepsWerkJaarNietVanGroep);
            }

            // Geboortedatum is verplicht als je lid wilt worden
            if (!gp.GebDatumMetChiroLeefTijd.HasValue)
            {
                throw new InvalidOperationException(Resources.GeboorteDatumOntbreekt);
            }

            // Je moet oud genoeg zijn
            if (gwj.WerkJaar - gp.GebDatumMetChiroLeefTijd.Value.Year < Properties.Settings.Default.MinLidLeefTijd)
            {
                throw new FoutNummerException(FoutNummer.LidTeJong, Properties.Resources.MinimumLeeftijd);
            }

            // en nog leven ook
            if (gp.Persoon.SterfDatum.HasValue)
            {
                throw new InvalidOperationException(Resources.PersoonIsOverleden);
            }

            // GroepsWerkJaar en GelieerdePersoon invullen
            lid.GroepsWerkJaar = gwj;
            lid.GelieerdePersoon = gp;
            gp.Lid.Add(lid);
            gwj.Lid.Add(lid);

            var stdProbeerPeriode = DateTime.Today.AddDays(Settings.Default.LengteProbeerPeriode);

            if ((gp.Groep.Niveau & (Niveau.Gewest | Niveau.Verbond)) != 0)
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
        /// Maakt gelieerde persoon een kind (lid) voor het gegeven werkJaar.
        /// <para>
        /// </para>
        /// Dit komt neer op 
        /// 		Automatisch een afdeling voor het kind bepalen. Een exception als dit niet mogelijk is.
        /// 		De probeerperiode zetten op binnen 3 weken als het een nieuw lid is, en op 15 oktober als de persoon vorig jaar al lid was.
        /// </summary>
        /// <param name="gp">
        /// Gelieerde persoon, gekoppeld aan groep en persoon
        /// </param>
        /// <param name="gwj">
        /// Groepswerkjaar waarin lid te maken, gekoppeld met afdelingsjaren
        /// </param>
        /// <param name="isJaarOvergang">
        /// Geeft aan of het lid gemaakt wordt voor de automatische jaarovergang; relevant
        /// voor probeerperiode.
        /// </param>
        /// <param name="voorgesteldeAfdeling">
        /// Voorstel tot toe te kennen afdeling. Checks hierop moeten al gebeurd zijn
        /// </param>
        /// <returns>
        /// Nieuw kindobject, niet gepersisteerd
        /// </returns>
        /// <remarks>
        /// Private; andere lagen moeten via 'Inschrijven' gaan.
        /// <para>
        /// </para>
        /// De user zal nooit zelf mogen kiezen in welk groepswerkjaar een kind lid wordt.  Maar 
        /// om testdata voor unit tests op te bouwen, hebben we deze functionaliteit wel nodig.
        /// <para>
        /// </para>
        /// Voorlopig gaan we ervan uit dat aan een gelieerde persoon al zijn vorige lidobjecten met
        /// groepswerkjaren gekoppeld zijn.  Dit wordt gebruikt in LidMaken
        /// om na te kijken of een gelieerde persoon al eerder
        /// lid was.  Dit lijkt me echter niet nodig; zie de commentaar aldaar.
        /// </remarks>
        /// <throws>FoutNummerException</throws>
        /// <throws>GeenGavException</throws>
        /// <throws>InvalidOperationException</throws>
        private Kind KindMaken(GelieerdePersoon gp,
                               GroepsWerkJaar gwj,
                               bool isJaarOvergang,
                               AfdelingsJaar voorgesteldeAfdeling)
        {
            // LidMaken doet de nodige checks ivm GAV-schap, enz.
            var k = LidMaken(gp, gwj, LidType.Kind, isJaarOvergang) as Kind;

            // Probeer nu afdeling te vinden.
            if (gwj.AfdelingsJaar.Count == 0)
            {
                throw new InvalidOperationException(Resources.InschrijvenZonderAfdelingen);
            }

            // allemaal om resharper blij tehouden:
            Debug.Assert(k != null);
            Debug.Assert(gp.GebDatumMetChiroLeefTijd != null);

            // Afdeling bepalen
            AfdelingsJaar gekozenAfdeling;
            if (voorgesteldeAfdeling != null)
            {
                // Checks hierop zijn al gebeurd in de aanroepende methode
                gekozenAfdeling = voorgesteldeAfdeling;
            }
            else
            {
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
                               && a.OfficieleAfdeling.ID == (int)NationaleAfdeling.Speciaal
                         select a).ToList();
                }

                if (afdelingsjaren.Count == 0)
                {
                    throw new InvalidOperationException(Resources.GeenAfdelingVoorLeeftijd);
                }

                // Kijk of er een afdeling is met een overeenkomend geslacht
                var aj = (from a in afdelingsjaren
                          where a.Geslacht == gp.Persoon.Geslacht || a.Geslacht == GeslachtsType.Gemengd
                          select a).FirstOrDefault();

                // Als dit niet zo is, kies dan de eerste afdeling die voldoet aan de leeftijdsgrenzen.
                gekozenAfdeling = aj ?? afdelingsjaren.First();
            }

            k.AfdelingsJaar = gekozenAfdeling;
            gekozenAfdeling.Kind.Add(k);

            return k;
        }

        /// <summary>
        /// Maakt gelieerde persoon leiding voor het gegeven werkJaar.
        /// <returns>
        /// Nieuw leidingsobject; niet gepersisteerd
        /// </returns>
        /// <remarks>
        /// Private; andere lagen moeten via 'Inschrijven' gaan.
        /// <para>
        /// </para>
        /// Deze method mag niet geexposed worden via de services, omdat een gebruiker uiteraard enkel in het huidige groepswerkjaar leden
        /// kan maken.
        /// <para>
        /// </para>
        /// Voorlopig gaan we ervan uit dat aan een gelieerde persoon al zijn vorige lidobjecten met
        /// groepswerkjaren gekoppeld zijn.  Dit wordt gebruikt in LidMaken
        /// om na te kijken of een gelieerde persoon al eerder lid was.  Dit lijkt me echter niet nodig; zie de commentaar aldaar.
        /// </remarks>
        /// <throws>FoutNummerException</throws>
        /// <throws>GeenGavException</throws>
        /// </summary>
        /// <param name="gp">
        /// Gelieerde persoon, gekoppeld met groep en persoon
        /// </param>
        /// <param name="gwj">
        /// Groepswerkjaar waarin leiding te maken
        /// </param>
        /// <param name="isJaarovergang">
        /// Geeft aan of het over de automatische jaarovergang gaat.
        /// (relevant voor probeerperiode)
        /// </param>
        /// <param name="afdelingsJaren">
        /// De afdelingsjaren waarin de nieuwe leiding zeker moet ingeschreven worden
        /// </param>
        /// <returns>
        /// Het aangemaakte Leiding-object
        /// </returns>
        private Leiding LeidingMaken(GelieerdePersoon gp,
                                     GroepsWerkJaar gwj,
                                     bool isJaarovergang,
                                     IEnumerable<AfdelingsJaar> afdelingsJaren)
        {
            Debug.Assert(gp != null && gp.GebDatumMetChiroLeefTijd != null);

            var leiding = LidMaken(gp, gwj, LidType.Leiding, isJaarovergang) as Leiding;

            if (!KanLeidingWorden(gp, gwj))
            {
                throw new GapException(Resources.TeJongVoorLeidingEnGeenGepasteAfdeling);
            }

            Debug.Assert(leiding != null);

            if (afdelingsJaren != null)
            {
                foreach (var a in afdelingsJaren)
                {
                    leiding.AfdelingsJaar.Add(a);
                    a.Leiding.Add(leiding);
                }
            }

            return leiding;
        }

        /// <summary>
        /// Geeft <c>true</c> als de gelieerde persoon <paramref name="gp"/> leiding kan worden in 
        /// groepswerkjaar <paramref name="gwj"/>
        /// </summary>
        /// <param name="gp">Gelieerde persoon, waarvan nagegaan moet worden of hij/zij leiding kan worden</param>
        /// <param name="gwj">Groepswerkjaar waarvoor gecontroleerd moet worden</param>
        /// <returns><c>true</c> als de gelieerde persoon <paramref name="gp"/> leiding kan worden in 
        /// groepswerkjaar <paramref name="gwj"/></returns>
        private static bool KanLeidingWorden(GelieerdePersoon gp, GroepsWerkJaar gwj)
        {
            // private, want we doen hier geen check op GAV-schap.

            Debug.Assert(gp.GebDatumMetChiroLeefTijd != null, "gp.GebDatumMetChiroLeefTijd != null");
            return gwj.WerkJaar - gp.GebDatumMetChiroLeefTijd.Value.Year >= Settings.Default.MinLeidingLeefTijd;
        }

        /// <summary>
        /// Doet een voorstel voor de inschrijving van de gegeven gelieerdepersoon <paramref name="gp"/> in groepswerkjaar 
        /// <paramref name="gwj"/>
        /// <para />
        /// Als de persoon in een afdeling past, krijgt hij die afdeling. Als er meerdere passen, wordt er een gekozen.
        /// Als de persoon niet in een afdeling past, en <paramref name="leidingIndienMogelijk"/> <c>true</c> is, 
        /// wordt hij leiding als hij oud genoeg is.
        /// Anders wordt een afdeling gekozen die het dichtst aanleunt bij de leeftijd van de persoon.
        /// Zijn er geen afdelingen, dan wordt een exception opgeworpen.
        /// </summary>
        /// <param name="gp">
        /// De persoon om in te schrijven, gekoppeld met groep en persoon
        /// </param>
        /// <param name="gwj">
        /// Het groepswerkjaar waarin moet worden ingeschreven, gekoppeld met afdelingsjaren
        /// </param>
        /// <param name="leidingIndienMogelijk">Als deze <c>true</c> is, dan stelt de method voor om een persoon
        /// leiding te maken als er geen geschikte afdeling is, en hij/zij oud genoeg is.</param>
        /// <returns>
        /// Voorstel tot inschrijving
        /// </returns>
        public LidVoorstel InschrijvingVoorstellen(GelieerdePersoon gp, GroepsWerkJaar gwj, bool leidingIndienMogelijk)
        {
            if (!_autorisatieMgr.IsGavGelieerdePersoon(gp.ID) || !_autorisatieMgr.IsGavGroepsWerkJaar(gwj.ID))
            {
                throw new GeenGavException(Properties.Resources.GeenGav);
            }

            // We moeten kunnen bepalen hoe oud iemand is, om hem/haar ofwel in een afdeling te steken,
            // of te kijken of hij/zij oud genoeg is om leiding te zijn.

            if (!gp.GebDatumMetChiroLeefTijd.HasValue)
            {
                // TODO: FoutnummerException van maken
                throw new GapException("De geboortedatum moet ingevuld zijn voor je iemand lid kunt maken.");
            }

            var resultaat = new LidVoorstel();
            var geboortejaar = gp.GebDatumMetChiroLeefTijd.Value.Year;

            if (gwj.WerkJaar - geboortejaar < Properties.Settings.Default.MinLidLeefTijd)
            {
                throw new FoutNummerException(FoutNummer.LidTeJong, Properties.Resources.MinimumLeeftijd);
            }

            // Bestaat er een afdeling waar de gelieerde persoon als kind in zou passen?
            // (Als er meerdere mogelijkheden zijn zullen we gewoon de eerste kiezen, maar we sorteren op
            // overenkomst geslacht)

            var mogelijkeAfdelingsJaren =
                gwj.AfdelingsJaar.Where(a => a.OfficieleAfdeling.ID != (int) NationaleAfdeling.Speciaal &&
                                             geboortejaar <= a.GeboorteJaarTot &&
                                             a.GeboorteJaarVan <= geboortejaar).OrderByDescending(
                                                 a => (gp.Persoon.Geslacht & a.Geslacht)).ToArray();

            if (mogelijkeAfdelingsJaren.Any())
            {
                // Als we lid kunnen maken: doen
                resultaat.AfdelingsJaarIDs = new [] {mogelijkeAfdelingsJaren.First().ID};
                resultaat.AfdelingsJarenIrrelevant = false;
                resultaat.LeidingMaken = false;
            }
            else if (leidingIndienMogelijk && KanLeidingWorden(gp, gwj))
            {
                // Als oud genoeg om leiding te worden: OK
                resultaat.AfdelingsJarenIrrelevant = true;
                resultaat.LeidingMaken = true;
            }
            else
            {               
                // Sorteer eerst aflopend op persoonsgeslacht|afdelingsgeslacht, zodat de
                // afdelingen met overeenkomstig geslacht eerst staan. Daarna sorteren we
                // op verschil geboortejaar-afdelingsgeboortejaar
                var geschiktsteAfdelingsjaar =
                    gwj.AfdelingsJaar.OrderByDescending(a => (gp.Persoon.Geslacht & a.Geslacht)).ThenBy(
                        aj => (Math.Abs(geboortejaar - aj.GeboorteJaarTot))).FirstOrDefault();

                // Als niet gevonden, proberen we nog eens zonder geslacht:

                if (geschiktsteAfdelingsjaar == null)
                {
                    throw new FoutNummerException(FoutNummer.AfdelingNietBeschikbaar,
                                                  Properties.Resources.InschrijvenZonderAfdelingen);
                }

                resultaat.AfdelingsJaarIDs = new[] {geschiktsteAfdelingsjaar.ID};
                resultaat.AfdelingsJarenIrrelevant = false;
                resultaat.LeidingMaken = false;
            }

            return resultaat;
        }

        /// <summary>
        /// Wijzigt een bestaand lid, op basis van de gegevens in <paramref name="voorstellid"/>.
        /// (in praktijk wordt het lid verwijderd en terug aangemaakt.  Wat op zich zo geen ramp is,
        /// maar wel tot problemen kan leiden, omdat het ID daardoor verandert.)
        /// 
        /// Deze method persisteert.  Dat is belangrijk, want het kan zijn dat er entities
        /// verdwijnen.  (Bijv. als er nieuwe afdelingen gegeven zijn.)
        /// </summary>
        /// <param name="lid">
        /// Het lidobject van de inschrijving, met gekoppeld gelieerderpersoon, groepswerkjaar, afdelingsjaren
        /// </param>
        /// <param name="voorstellid">
        /// Bevat de afdelingen waar het nieuwe lidobject aan gekoppeld moet worden
        /// en heeft aan of de gelieerde persoon leiding is.
        /// </param>
        /// <returns>
        /// Het lidobject met de gegevens van de nieuwe inschrijving
        /// </returns>
        /// <exception cref="GeenGavException">
        /// Komt voor als de gebruiker geen GAV-rechten heeft op het <paramref name="lid" />.
        /// </exception>
        public Lid Wijzigen(Lid lid, LidVoorstel voorstellid)
        {
            var gelieerdePersoon = lid.GelieerdePersoon;
            var groepsWerkJaar = lid.GroepsWerkJaar;

            if (!_autorisatieMgr.IsGavLid(lid.ID))
            {
                throw new GeenGavException(Resources.GeenGav);
            }

            Lid nieuwLid;  // Deze declaratie moet buiten de TransactionScope staan,
                           // anders compileert de solution niet met transactions enabled! (Zie #1336)

#if KIPDORP
            using (var tx = new TransactionScope())
            {
#endif

            // Voor 't gemak eerst verwijderen, en dan terug aanmaken.

            // als type wisselt, dan functies deleten die het andere type niet mag hebben
            if (lid is Kind && voorstellid.LeidingMaken)
            {
                foreach (var fn in lid.Functie)
                {
                    // TODO check whether it is a function which the new type does not have
                    // {
                    fn.TeVerwijderen = true;

                    // }
                    // TODO reassign functies to nieuw lid
                }
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
            nieuwLid = NieuwInschrijven(gelieerdePersoon, groepsWerkJaar, false, voorstellid);
            // de 'false' hierboven geeft aan dat het niet om een jaarovergang gaat.  Bij een jaarovergang worden
            // dan ook geen bestaande leden gewijzigd, enkel nieuwe gemaakt.

            nieuwLid.EindeInstapPeriode = lid.EindeInstapPeriode;
            nieuwLid = Bewaren(nieuwLid, LidExtras.Afdelingen | LidExtras.Persoon, true);
            // bewaren gaat voor ons de sync oproepen

#if KIPDORP
                tx.Complete();
            }
#endif
            return nieuwLid;
        }

        /// <summary>
        /// Schrijft een gelieerde persoon in, persisteert niet.  Er mag nog geen lidobject (ook geen inactief) voor de
        /// gelieerde persoon bestaan.
        /// </summary>
        /// <param name="gp">
        /// De persoon om in te schrijven, gekoppeld met groep en persoon
        /// </param>
        /// <param name="gwj">
        /// Het groepswerkjaar waarin moet worden ingeschreven, gekoppeld met afdelingsjaren
        /// </param>
        /// <param name="isJaarOvergang">
        /// Geeft aan of het over de automatische jaarovergang gaat; relevant voor de
        /// probeerperiode
        /// </param>
        /// <param name="voorstellid">
        /// Voorstel voor de eigenschappen van het in te schrijven lid.
        /// </param>
        /// <returns>
        /// Het aangemaakte lid object
        /// </returns>
        public Lid NieuwInschrijven(GelieerdePersoon gp, GroepsWerkJaar gwj, bool isJaarOvergang, LidVoorstel voorstellid)
        {
            // We gaan ervan uit dat er een voorstel is. Je kunt het voorstel automatisch laten 
            // genereren via InschrijvingVoorstellen.

            Debug.Assert(voorstellid != null);

            // Lid maken zonder geboortedatum is geen probleem meer, aangezien de afdeling
            // bij in het voorstel zit. (en dus niet op dit moment bepaald moet worden.)

            // Geslacht is wel verplicht; kipadmin kan geen onzijdige mensen aan.
            if (gp.Persoon.Geslacht != GeslachtsType.Man && gp.Persoon.Geslacht != GeslachtsType.Vrouw)
            {
                // FIXME: (#530) De boodschap in onderstaande exception wordt getoond aan de user,
                // terwijl dit eigenlijk een technische boodschap voor de developer moet zijn.
                throw new FoutNummerException(
                    FoutNummer.OnbekendGeslachtFout, Resources.GeslachtVerplicht);
            }

            List<AfdelingsJaar> afdelingsJaren = null;

            if (voorstellid.AfdelingsJarenIrrelevant && !voorstellid.LeidingMaken)
            {
                // Lid maken en zelf afdeling bepalen

                int afdelingsJaarID = InschrijvingVoorstellen(gp, gwj, false).AfdelingsJaarIDs.FirstOrDefault();
                afdelingsJaren = (from aj in gwj.AfdelingsJaar
                                 where aj.ID == afdelingsJaarID
                                 select aj).ToList();
            }
            else
            {
                // Eerst even checken of we geen lid proberen te maken met een ongeldig aantal afdelingen.

                if (!voorstellid.LeidingMaken && voorstellid.AfdelingsJaarIDs.Count() != 1)
                {
                    throw new GapException("Een kind moet exact 1 afdeling krijgen bij het inschrijven.");
                }

                if (voorstellid.AfdelingsJaarIDs != null)
                {
                    // Als er afdelingsjaarID's meegegeven zijn, dan zoeken we die op in het huidige
                    // groepswerkjaar.

                    afdelingsJaren = (from a in gwj.AfdelingsJaar
                                     where
                                         voorstellid.AfdelingsJaarIDs.Contains(a.ID)
                                     select a).ToList();

                    if (afdelingsJaren.Count() != voorstellid.AfdelingsJaarIDs.Count())
                    {
                        throw new FoutNummerException(FoutNummer.AfdelingNietBeschikbaar,
                                                      Properties.Resources.AfdelingNietBeschikbaar);
                    }
                }
            }

            Lid nieuwlid;
            if (voorstellid.LeidingMaken)
            {
                nieuwlid = LeidingMaken(gp, gwj, isJaarOvergang, afdelingsJaren);
            }
            else
            {
                Debug.Assert(afdelingsJaren != null);
                nieuwlid = KindMaken(gp, gwj, isJaarOvergang, afdelingsJaren.First());
            }

            return nieuwlid;
        }

        /// <summary>
        /// Persisteert een lid met de gekoppelde entiteiten bepaald door <paramref name="extras"/>.
        /// </summary>
        /// <param name="lid">
        /// Het <paramref name="lid"/> dat bewaard moet worden
        /// </param>
        /// <param name="extras">
        /// De gekoppelde entiteiten
        /// </param>
        /// <param name="syncen">
        /// Als <c>true</c>, dan wordt het lid gesynct met Kipadmin.
        /// </param>
        /// <returns>
        /// Een kloon van het lid en de extra's, met eventuele nieuwe ID's ingevuld
        /// </returns>
        /// <remarks>
        /// De parameter <paramref name="syncen"/> heeft als doel een sync te vermijden als een
        /// irrelevante wijziging zoals 'lidgeld betaald' wordt bewaard.
        /// </remarks>
        public Lid Bewaren(Lid lid, LidExtras extras, bool syncen)
        {
            if (!_autorisatieMgr.IsGavLid(lid.ID))
            {
                throw new GeenGavException(Resources.GeenGav);
            }

            Lid bewaardLid;

            if (lid is Kind)
            {
                try
                {
#if KIPDORP
                    using (var tx = new TransactionScope())
                    {
#endif
                    if (syncen)
                    {
                        if (lid.UitschrijfDatum == null)
                        {
                            // Actieve leden altijd syncen
                            _sync.Bewaren(lid);
                        }
                        else if (lid.EindeInstapPeriode > DateTime.Now)
                        {
                            // Verwijderen tijdens probeerperiode mag natuurlijk nog wel
                            _sync.Verwijderen(lid);
                        }
                    }

                    bewaardLid = _daos.KindDao.Bewaren((Kind)lid, extras);
#if KIPDORP
                        tx.Complete();
                    }
#endif
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
#if KIPDORP
                    using (var tx = new TransactionScope())
                    {
#endif
                    if (syncen)
                    {
                        if (lid.UitschrijfDatum == null)
                        {
                            // Actieve leden altijd syncen
                            _sync.Bewaren(lid);
                        }
                        else if (lid.EindeInstapPeriode > DateTime.Now || lid.Niveau > Niveau.Groep)
                        {
                            // verwijderen uit Kipadmin enkel in een van deze gevallen:
                            // * instapperiode is nog niet voorbij (voor gewone groepen)
                            // * kaderleden.  Deze hebben namelijk geen instapperiode, en het lidgeld is onafhankelijk
                            // van het aantal ingeschreven personen.
                            _sync.Verwijderen(lid);
                        }
                    }

                    bewaardLid = _daos.LeidingDao.Bewaren((Leiding)lid, extras);
#if KIPDORP
                        tx.Complete();
                    }
#endif
                }
                catch (Exception)
                {
                    throw new BestaatAlException<Leiding>(lid as Leiding);
                }
            }
            else
            {
                throw new NotSupportedException(Resources.OngeldigLidType);
            }

            return bewaardLid;
        }

        /// <summary>
        /// Haalt leden op, op basis van de <paramref name="lidIDs"/>
        /// </summary>
        /// <param name="lidIDs">
        /// ID gevraagde leden
        /// </param>
        /// <param name="lidExtras">
        /// Geeft aan welke gekoppelde entiteiten mee opgehaald moeten worden
        /// </param>
        /// <returns>
        /// Kinderen of leiding met gevraagde <paramref name="lidExtras"/>.
        /// </returns>
        /// <remarks>
        /// ID's van leden waarvoor de user geen GAV is, worden genegeerd
        /// </remarks>
        public IEnumerable<Lid> Ophalen(IEnumerable<int> lidIDs, LidExtras lidExtras)
        {
            var eigenLidIDs = _autorisatieMgr.IsSuperGav() ? lidIDs : _autorisatieMgr.EnkelMijnLeden(lidIDs);
            return _daos.LedenDao.Ophalen(eigenLidIDs, lidExtras);
        }

        /// <summary>
        /// Haalt lid op, op basis van zijn <paramref name="lidID"/>
        /// </summary>
        /// <param name="lidID">
        /// ID gevraagde lid
        /// </param>
        /// <param name="extras">
        /// Geeft aan welke gekoppelde entiteiten mee opgehaald moeten worden
        /// </param>
        /// <returns>
        /// Kind of Leiding met gevraagde <paramref name="extras"/>.
        /// </returns>
        public Lid Ophalen(int lidID, LidExtras extras)
        {
            if (!_autorisatieMgr.IsGavLid(lidID))
            {
                throw new GeenGavException(Resources.GeenGav);
            }

            return Ophalen(new[] { lidID }, extras).FirstOrDefault();
        }

        /// <summary>
        /// Haalt lid en gekoppelde persoon op, op basis van <paramref name="lidID"/>
        /// </summary>
        /// <param name="lidID">
        /// ID op te halen lid
        /// </param>
        /// <returns>
        /// Lid, met daaraan gekoppeld gelieerde persoon en persoon.
        /// </returns>
        public Lid Ophalen(int lidID)
        {
            return Ophalen(lidID, LidExtras.Geen);
        }

        /// <summary>
        /// Haalt het lid op bepaald door <paramref name="gelieerdePersoonID"/> en
        /// <paramref name="groepsWerkJaarID"/>, inclusief de relevante details om het lid naar Kipadmin te krijgen:
        /// persoon, afdelingen, officiële afdelingen, functies, groepswerkjaar, groep
        /// </summary>
        /// <param name="gelieerdePersoonID">
        /// ID van de gelieerde persoon waarvoor het lidobject gevraagd is.
        /// </param>
        /// <param name="groepsWerkJaarID">
        /// ID van groepswerkjaar in hetwelke het lidobject gevraagd is
        /// </param>
        /// <returns>
        /// Het lid bepaald door <paramref name="gelieerdePersoonID"/> en
        /// <paramref name="groepsWerkJaarID"/>, inclusief de relevante details om het lid naar Kipadmin te krijgen
        /// </returns>
        public Lid OphalenViaPersoon(int gelieerdePersoonID, int groepsWerkJaarID)
        {
            if (!_autorisatieMgr.IsGavGroepsWerkJaar(groepsWerkJaarID) ||
                !_autorisatieMgr.IsGavGelieerdePersoon(gelieerdePersoonID))
            {
                throw new GeenGavException(Resources.GeenGav);
            }

            var lid = _daos.LedenDao.OphalenViaPersoon(gelieerdePersoonID, groepsWerkJaarID);

            if (lid != null)
            {
                // We weten dat lid.GroepsWerkJaar.Groep steeds gelijk is aan lid.GelieerdePersoon.Groep.
                // Die laatste is echter niet opgehaald, maar het is erg gemakkelijk om die extra informatie
                // hier mee te geven:

                lid.GelieerdePersoon.Groep = lid.GroepsWerkJaar.Groep;
            }

            return lid;
        }

        /// <summary>
        /// Haalt leden op uit het groepswerkjaar met gegeven ID, inclusief persoonsgegevens,
        /// voorkeursadressen, functies en afdelingen.  (Geen communicatiemiddelen)
        /// </summary>
        /// <param name="gwjID">
        /// ID van het gevraagde groepswerkjaar
        /// </param>
        /// <param name="ookInactief">
        /// Geef hier <c>true</c> als ook de niet-actieve leden opgehaald
        /// moeten worden.
        /// </param>
        /// <returns>
        /// De lijst van leden
        /// </returns>
        public IEnumerable<Lid> OphalenUitGroepsWerkJaar(int gwjID, bool ookInactief)
        {
            if (_autorisatieMgr.IsSuperGav() || _autorisatieMgr.IsGavGroepsWerkJaar(gwjID))
            {
                return _daos.LedenDao.OphalenUitGroepsWerkJaar(gwjID, ookInactief);
            }
            else
            {
                throw new GeenGavException(Resources.GeenGav);
            }
        }

        /// <summary>
        /// Zoekt leden op, op basis van de gegeven <paramref name="filter"/>.
        /// </summary>
        /// <param name="filter">
        /// De niet-nulle properties van de filter
        /// bepalen waarop gezocht moet worden
        /// </param>
        /// <param name="extras">
        /// Bepaalt de mee op te halen gekoppelde entiteiten. 
        /// (Adressen ophalen vertraagt aanzienlijk.)
        /// </param>
        /// <returns>
        /// Lijst met info over gevonden leden
        /// </returns>
        /// <remarks>
        /// Er worden enkel actieve leden opgehaald
        /// </remarks>
        public IEnumerable<Lid> Zoeken(LidFilter filter, LidExtras extras)
        {
            // Hieronder een hele hoop voorwaarden voor de GeenGavException.
            // Ik denk dat het duidelijker zou zijn moest deze if geinverteerd worden,
            // en dat er dus zou staan wanneer er wél gezocht mag worden.
            if (!_autorisatieMgr.IsSuperGav() &&
                (filter.GroepID != null && !_autorisatieMgr.IsGavGroep(filter.GroepID.Value) ||
                 filter.GroepsWerkJaarID != null && !_autorisatieMgr.IsGavGroepsWerkJaar(filter.GroepsWerkJaarID.Value) ||
                 filter.AfdelingID != null && !_autorisatieMgr.IsGavAfdeling(filter.AfdelingID.Value) ||
                 filter.FunctieID != null && !_autorisatieMgr.IsGavFunctie(filter.FunctieID.Value)))
            {
                throw new GeenGavException(Resources.GeenGav);
            }

            var kinderen = _daos.KindDao.Zoeken(filter, extras);
            IEnumerable<Lid> leiding = _daos.LeidingDao.Zoeken(filter, extras);

            // Sorteren doen we hier niet; dat is presentatie :)
            // Voeg kinderen en leiding samen, en haal de inactieve er uit
            var alles = kinderen.Union(leiding);
            return alles.Where(ld => ld.NonActief == false).ToArray();
        }

        /// <summary>
        /// Haalt alle leden op uit groepswerkjaar met gegeven <paramref name="groepsWerkJaarID"/> en gegeven
        /// <paramref name="nationaleFunctie"/>, met daaraan gekoppeld de gelieerde personen.
        /// </summary>
        /// <param name="groepsWerkJaarID">ID van een groepswerkjaar</param>
        /// <param name="nationaleFunctie">een nationale functie</param>
        /// <returns>alle leden op uit groepswerkjaar met gegeven <paramref name="groepsWerkJaarID"/> en gegeven
        /// <paramref name="nationaleFunctie"/>, met daaraan gekoppeld de gelieerde personen.</returns>
        public List<Lid> Ophalen(int groepsWerkJaarID, NationaleFunctie nationaleFunctie)
        {
            return _daos.LedenDao.OphalenUitFunctie((int) nationaleFunctie, groepsWerkJaarID, ld => ld.GelieerdePersoon);
        }
    }
}
