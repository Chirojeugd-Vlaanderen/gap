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
using Chiro.Gap.Domain;
using Chiro.Gap.Poco.Model;
using Chiro.Gap.Poco.Model.Exceptions;
using Chiro.Gap.SyncInterfaces;
using Chiro.Gap.WorkerInterfaces;
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
        private readonly IAutorisatieManager _autorisatieMgr;
        private readonly ILedenSync _sync;

        public LedenManager(IAutorisatieManager autorisatie,
                            ILedenSync sync)
        {
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

            if (!_autorisatieMgr.IsGav(gp) || !_autorisatieMgr.IsGav(gwj))
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
            if (!_autorisatieMgr.IsGav(gp) || !_autorisatieMgr.IsGav(gwj))
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

            // TODO: Bekijken of we 'AfdelingsJaarVoorstellen' niet kunnen hergebruiken.

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
            throw new NotImplementedException(NIEUWEBACKEND.Info);
//            var gelieerdePersoon = lid.GelieerdePersoon;
//            var groepsWerkJaar = lid.GroepsWerkJaar;

//            if (!_autorisatieMgr.IsGavLid(lid.ID))
//            {
//                throw new GeenGavException(Resources.GeenGav);
//            }

//            Lid nieuwLid;  // Deze declaratie moet buiten de TransactionScope staan,
//                           // anders compileert de solution niet met transactions enabled! (Zie #1336)

//#if KIPDORP
//            using (var tx = new TransactionScope())
//            {
//#endif

//            // Voor 't gemak eerst verwijderen, en dan terug aanmaken.

//            // als type wisselt, dan functies deleten die het andere type niet mag hebben
//            if (lid is Kind && voorstellid.LeidingMaken)
//            {
//                foreach (var fn in lid.Functie)
//                {
//                    // TODO check whether it is a function which the new type does not have
//                    // {
//                    fn.TeVerwijderen = true;

//                    // }
//                    // TODO reassign functies to nieuw lid
//                }
//            }

//            if (lid is Kind)
//            {
//                var kind = lid as Kind;
//                kind.TeVerwijderen = true;
//                _daos.KindDao.Bewaren(kind, knd => knd.AfdelingsJaar, knd => knd.Functie);
//            }
//            else
//            {
//                var leiding = lid as Leiding;
//                Debug.Assert(leiding != null);
//                foreach (var aj in leiding.AfdelingsJaar)
//                {
//                    aj.TeVerwijderen = true;
//                }

//                leiding.TeVerwijderen = true;
//                _daos.LeidingDao.Bewaren(leiding, ld => ld.AfdelingsJaar, ld => ld.Functie);
//            }

//            // Met heel dat 'TeVerwijderen'-gedoe, is het domein typisch
//            // niet meer consistent na iets te verwijderen.
//            gelieerdePersoon.Lid.Clear();
//            groepsWerkJaar.Lid.Clear();
//            foreach (var aj in groepsWerkJaar.AfdelingsJaar)
//            {
//                aj.TeVerwijderen = false;
//            }

//            // Maak opnieuw lid
//            nieuwLid = NieuwInschrijven(gelieerdePersoon, groepsWerkJaar, false, voorstellid);
//            // de 'false' hierboven geeft aan dat het niet om een jaarovergang gaat.  Bij een jaarovergang worden
//            // dan ook geen bestaande leden gewijzigd, enkel nieuwe gemaakt.

//            nieuwLid.EindeInstapPeriode = lid.EindeInstapPeriode;
//            nieuwLid = Bewaren(nieuwLid, LidExtras.Afdelingen | LidExtras.Persoon, true);
//            // bewaren gaat voor ons de sync oproepen

//#if KIPDORP
//                tx.Complete();
//            }
//#endif
//            return nieuwLid;
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
        /// Geeft <c>true</c> als de gegeven <paramref name="gelieerdePersoon"/> in zijn recentste groepswerkjaar
        /// lid kan worden, d.w.z. dat hij qua (Chiro)leeftijd in een afdeling past.
        /// </summary>
        /// <param name="gelieerdePersoon">een gelieerde persoon</param>
        /// <returns><c>true</c> als de gegeven <paramref name="gelieerdePersoon"/> in zijn recentste groepswerkjaar
        /// lid kan worden, d.w.z. dat hij qua (Chiro)leeftijd in een afdeling past.</returns>
        public bool KanInschrijvenAlsKind(GelieerdePersoon gelieerdePersoon)
        {
            return AfdelingsJaarVoorstellen(gelieerdePersoon) != null;
        }

        /// <summary>
        /// Geeft <c>true</c> als de gegeven <paramref name="gelieerdePersoon"/> in zijn recentste groepswerkjaar
        /// leiding kan worden. Dit hangt eigenlijk enkel van de leeftijd af.
        /// </summary>
        /// <param name="gelieerdePersoon">een gelieerde persoon</param>
        /// <returns><c>true</c> als de gegeven <paramref name="gelieerdePersoon"/> in zijn recentste groepswerkjaar
        /// leiding kan worden.</returns>
        public bool KanInschrijvenAlsLeiding(GelieerdePersoon gelieerdePersoon)
        {
            return KanLeidingWorden(gelieerdePersoon,
                                    gelieerdePersoon.Groep.GroepsWerkJaar.OrderByDescending(gwj => gwj.WerkJaar)
                                                    .FirstOrDefault());
        }

        /// <summary>
        /// Als de gegeven <paramref name="gelieerdePersoon"/> lid is in het huidige werkjaar van zijn groep, dan
        /// levert deze method het overeenkomstige lidobject op. In het andere geval <c>null</c>.
        /// </summary>
        /// <param name="gelieerdePersoon">Een gelieerde persoon</param>
        /// <returns>
        /// Als de gegeven <paramref name="gelieerdePersoon"/> lid is in het huidige werkjaar van zijn groep, dan
        /// levert deze method het overeenkomstige lidobject op. In het andere geval <c>null</c>.
        /// </returns>
        public Lid HuidigLidGet(GelieerdePersoon gelieerdePersoon)
        {
            return
                gelieerdePersoon.Groep.GroepsWerkJaar.OrderByDescending(gwj => gwj.WerkJaar)
                                .First()
                                .Lid.Where(ld => !ld.NonActief)
                                .FirstOrDefault(ld => ld.GelieerdePersoon.ID == gelieerdePersoon.ID);
        }

        /// <summary>
        /// Zoekt een afdelingsjaar van het recentste groepswerkjaar, waarin de gegeven 
        /// <paramref name="gelieerdePersoon"/> (kind)lid zou kunnen worden. <c>null</c> als er zo geen
        /// bestaat.
        /// </summary>
        /// <param name="gelieerdePersoon">gelieerde persoon waarvoor we een afdeling zoeken</param>
        /// <returns>een afdelingsjaar van het recentste groepswerkjaar, waarin de gegeven 
        /// <paramref name="gelieerdePersoon"/> lid zou kunnen worden. <c>null</c> als er zo geen
        /// bestaat.</returns>
        public AfdelingsJaar AfdelingsJaarVoorstellen(GelieerdePersoon gelieerdePersoon)
        {
            return AfdelingsJaarVoorstellen(gelieerdePersoon,
                                            gelieerdePersoon.Groep.GroepsWerkJaar.OrderByDescending(gwj => gwj.WerkJaar)
                                                            .First());
        }

        /// <summary>
        /// Geeft <c>true</c> als de gegeven <paramref name="gelieerdePersoon"/> ingeschreven is als
        /// (kind)lid in het huidige werkjaar van zijn groep. Anders <c>false</c>.
        /// </summary>
        /// <param name="gelieerdePersoon">Een gelieerde persoon</param>
        /// <returns>
        /// <c>true</c> als de gegeven <paramref name="gelieerdePersoon"/> ingeschreven is als
        /// (kind)lid in het huidige werkjaar van zijn groep. Anders <c>false</c>.
        /// </returns>
        public bool IsActiefKind(GelieerdePersoon gelieerdePersoon)
        {
            var lid = HuidigLidGet(gelieerdePersoon);
            return lid != null && lid is Kind;
        }

        /// <summary>
        /// Geeft <c>true</c> als de gegeven <paramref name="gelieerdePersoon"/> ingeschreven is als
        /// leiding in het huidige werkjaar van zijn groep. Anders <c>false</c>.
        /// </summary>
        /// <param name="gelieerdePersoon">Een gelieerde persoon</param>
        /// <returns>
        /// <c>true</c> als de gegeven <paramref name="gelieerdePersoon"/> ingeschreven is als
        /// leiding in het huidige werkjaar van zijn groep. Anders <c>false</c>.
        /// </returns>
        public bool IsActieveLeiding(GelieerdePersoon gelieerdePersoon)
        {
            var lid = HuidigLidGet(gelieerdePersoon);
            return lid != null && lid is Leiding;
        }

        /// <summary>
        /// Als de gegeven <paramref name="gelieerdePersoon"/> lid is in het huidige werkjaar van zijn
        /// groep, wordt het lidID opgeleverd, zo niet <c>null</c>.
        /// </summary>
        /// <param name="gelieerdePersoon">Een gelieerde persoon</param>
        /// <returns>
        /// Het lidID als de gegeven <paramref name="gelieerdePersoon"/> lid is in het huidige werkjaar
        /// van zijn groep, anders <c>null</c>.
        /// </returns>
        public int? LidIDGet(GelieerdePersoon gelieerdePersoon)
        {
            var lid = HuidigLidGet(gelieerdePersoon);
            return lid == null ? null : (int?)lid.ID;
        }

        /// <summary>
        /// Zoekt een afdelingsjaar van gegeven <paramref name="groepsWerkJaar"/>, waarin de gegeven 
        /// <paramref name="gelieerdePersoon"/> (kind)lid zou kunnen worden. <c>null</c> als er zo geen
        /// bestaat.
        /// </summary>
        /// <param name="gelieerdePersoon">gelieerde persoon waarvoor we een afdeling zoeken</param>
        /// <param name="groepsWerkJaar">groepswerkjaar waarin we zoeken naar een afdeling</param>
        /// <returns>een afdelingsjaar van het recentste groepswerkjaar, waarin de gegeven 
        /// <paramref name="gelieerdePersoon"/> lid zou kunnen worden. <c>null</c> als er zo geen
        /// bestaat.</returns>
        private AfdelingsJaar AfdelingsJaarVoorstellen(GelieerdePersoon gelieerdePersoon, GroepsWerkJaar groepsWerkJaar)
        {
            if (gelieerdePersoon.Persoon.GeboorteDatum == null)
            {
                return null;
            }

            var geboortejaar = gelieerdePersoon.GebDatumMetChiroLeefTijd.Value.Year;

            if (groepsWerkJaar.WerkJaar - geboortejaar < Settings.Default.MinLidLeefTijd)
            {
                throw new FoutNummerException(FoutNummer.LidTeJong, Resources.MinimumLeeftijd);
            }

            // Bestaat er een afdeling waar de gelieerde persoon als kind in zou passen?
            // (Als er meerdere mogelijkheden zijn zullen we gewoon de eerste kiezen, maar we sorteren op
            // overenkomst geslacht)

            var mogelijkeAfdelingsJaren =
                groepsWerkJaar.AfdelingsJaar.Where(a => a.OfficieleAfdeling.ID != (int)NationaleAfdeling.Speciaal &&
                                             geboortejaar <= a.GeboorteJaarTot &&
                                             a.GeboorteJaarVan <= geboortejaar).OrderByDescending(
                                                 a => (gelieerdePersoon.Persoon.Geslacht & a.Geslacht)).ToArray();

            return mogelijkeAfdelingsJaren.FirstOrDefault();
        }
    }
}
