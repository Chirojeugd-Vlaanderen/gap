/*
 * Copyright 2008-2013, 2015, 2016, 2017 the GAP developers.
 * See the NOTICE file at the top-level directory of this distribution, 
 * and at https://gapwiki.chiro.be/copyright
 * 
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 * 
 *     http://www.apache.org/licenses/LICENSE-2.0
 * 
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.ServiceModel.Channels;
using Chiro.Gap.Domain;
using Chiro.Gap.Poco.Model;
using Chiro.Gap.Poco.Model.Exceptions;
using Chiro.Gap.Validatie;
using Chiro.Gap.WorkerInterfaces;
using Chiro.Gap.Workers.Properties;

namespace Chiro.Gap.Workers
{
    /// <summary>
    /// Worker die alle businesslogica i.v.m. leden bevat
    /// </summary>
    public class LedenManager : ILedenManager
    {
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

            // GroepsWerkJaar en GelieerdePersoon invullen
            lid.GroepsWerkJaar = gwj;
            lid.GelieerdePersoon = gp;
            gp.Lid.Add(lid);
            gwj.Lid.Add(lid);

            var stdProbeerPeriode = DateTime.Today.AddDays(Settings.Default.LengteProbeerPeriode);

            var eindeJaarOvergang = Settings.Default.WerkjaarVerplichteOvergang;
            eindeJaarOvergang = new DateTime(gwj.WerkJaar, eindeJaarOvergang.Month, eindeJaarOvergang.Day);

            if ((gp.Groep.Niveau & (Niveau.Gewest | Niveau.Verbond)) != 0)
            {
                lid.EindeInstapPeriode = DateTime.Today;
            }
            else if (!isJaarOvergang)
            {
                // Standaardinstapperiode indien niet in jaarovergang
                lid.EindeInstapPeriode = eindeJaarOvergang >= stdProbeerPeriode
                                             ? eindeJaarOvergang
                                             : stdProbeerPeriode;
            }
            else
            {
                // Voor jaarovergang: vaste deadline (gek idee, maar blijkbaar in de specs)
                lid.EindeInstapPeriode = eindeJaarOvergang >= DateTime.Now
                                             ? eindeJaarOvergang
                                             : stdProbeerPeriode;
            }

            FoutNummer? fout = new LidValidator().FoutNummer(lid);

            if (fout == FoutNummer.GroepsWerkJaarNietVanGroep)
            {
                throw new FoutNummerException(FoutNummer.GroepsWerkJaarNietVanGroep,
                                              Resources.GroepsWerkJaarNietVanGroep);
            }

            // Geboortedatum is verplicht als je lid wilt worden
            if (fout == FoutNummer.GeboorteDatumOntbreekt)
            {
                throw new FoutNummerException(FoutNummer.GeboorteDatumOntbreekt, Resources.GeboorteDatumOntbreekt);
            }

            // Je moet oud genoeg zijn
            if (fout == FoutNummer.LidTeJong)
            {
                throw new FoutNummerException(FoutNummer.LidTeJong, Resources.MinimumLeeftijd);
            }

            // en nog leven ook
            if (fout == FoutNummer.PersoonOverleden)
            {
                throw new FoutNummerException(FoutNummer.PersoonOverleden, Resources.PersoonIsOverleden);
            }


            return lid;
        }

        /// <summary>
        /// Verandert het lidtype van <paramref name="origineelLid"/> van
        /// <c>Kind</c> naar <c>Leiding</c> of omgekeerd
        /// </summary>
        /// <param name="origineelLid">Lid waarvan type veranderd moet worden</param>
        /// <returns>Nieuw lid, met ander type</returns>
        /// <remarks>Het origineel lid moet door de caller zelf uit de repository verwijderd worden.</remarks>
        public Lid TypeToggle(Lid origineelLid)
        {
            if (origineelLid.GroepsWerkJaar.Groep.StopDatum != null &&
                origineelLid.GroepsWerkJaar.Groep.StopDatum < DateTime.Now)
            {
                throw new FoutNummerException(FoutNummer.GroepInactief, Resources.GroepInactief);
            }

            DateTime? eindeInstap = origineelLid.EindeInstapPeriode;
            var nieuwNiveau = (origineelLid is Kind) ? Niveau.LeidingInGroep : Niveau.LidInGroep;

            var gelieerdePersoon = origineelLid.GelieerdePersoon;
            var groepsWerkJaar = origineelLid.GroepsWerkJaar;

            // Een bestaand object van type wisselen, is niet mogelijk (denk ik)
            // dus we verwijderen het bestaande lid, en maken een nieuw aan.

            // Zaken uit de repository verwijderen, kan moeilijk tot niet in de workers,
            // dus doen we het hier.

            if (!groepsWerkJaar.Groep.Niveau.HasFlag(Niveau.Groep))
            {
                throw new FoutNummerException(FoutNummer.LidTypeVerkeerd, Resources.FoutiefLidTypeFunctie);
            }

            // Behoud bestaande functies die straks nog van 
            // toepassing zijn, om opnieuw te kunnen toekennen.

            var teBewarenFuncties = (from f in origineelLid.Functie
                                     where f.Niveau.HasFlag(nieuwNiveau)
                                     select f).ToList();


            // Koppel de bestaande functies en afdelingen los van het lid, en verwijder het bestaande lid.

            origineelLid.Functie.Clear();
            if (origineelLid is Leiding)
            {
                (origineelLid as Leiding).AfdelingsJaar.Clear();
            }

            gelieerdePersoon.Lid.Remove(origineelLid);
            groepsWerkJaar.Lid.Remove(origineelLid);

            var voorstelLid = new LidVoorstel
            {
                AfdelingsJarenIrrelevant = true,
                LeidingMaken = (nieuwNiveau == Niveau.LeidingInGroep),
                GelieerdePersoon = gelieerdePersoon,
                GroepsWerkJaar = groepsWerkJaar
            };

            var nieuwLid = NieuwInschrijven(voorstelLid, false);

            nieuwLid.EindeInstapPeriode = eindeInstap;

            foreach (var f in teBewarenFuncties)
            {
                nieuwLid.Functie.Add(f);
            }
            return nieuwLid;
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
                throw new FoutNummerException(FoutNummer.AfdelingKindVerplicht,
                    Resources.InschrijvenZonderAfdelingen);
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
                    throw new FoutNummerException(FoutNummer.AfdelingNietBeschikbaar, Resources.GeenAfdelingVoorLeeftijd);
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

            if (!KanLeidingWorden(gp, gwj))
            {
                throw new FoutNummerException(FoutNummer.LeidingTeJong, Resources.TeJongVoorLeiding);
            }

            var leiding = LidMaken(gp, gwj, LidType.Leiding, isJaarovergang) as Leiding;

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
        public bool KanLeidingWorden(GelieerdePersoon gp, GroepsWerkJaar gwj)
        {
            if (gp.Persoon.GeboorteDatum == null)
            {
                return false;
            }
            // Gooi exception als groepswerkjaar van andere groep als gelieerde persoon.
            if (!gp.Groep.Equals(gwj.Groep))
            {
                throw new FoutNummerException(FoutNummer.GroepsWerkJaarNietVanGroep, Resources.GroepsWerkJaarNietVanGroep);
            }

            Debug.Assert(gp.GebDatumMetChiroLeefTijd != null, "gp.GebDatumMetChiroLeefTijd != null");
            return KanLeidingWorden(gp.GebDatumMetChiroLeefTijd.Value, gwj);
        }

        /// <summary>
        /// Geeft <c>true</c> als een persoon met gegeven geboortedatum (met Chiroleeftijd) leiding mag worden in
        /// groepswerkjaar <paramref name="gwj"/>
        /// </summary>
        /// <param name="geboorteDatum">Geboortedatum van persoon (Chiroleeftijd ingerekend)</param>
        /// <param name="gwj">Groepswerkjaar waarvoor gecontroleerd moet worden</param>
        /// <returns><c>true</c> als een persoon met gegeven geboortedatum (met Chiroleeftijd) leiding mag worden in
        /// groepswerkjaar <paramref name="gwj"/></returns>
        public bool KanLeidingWorden(DateTime geboorteDatum, GroepsWerkJaar gwj)
        {
            return gwj.WerkJaar - geboorteDatum.Year >= Settings.Default.MinLeidingLeefTijd;
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
        /// <remarks>
        /// Dit voorstel is een best effort. Het wordt hier niet gevalideerd; dat moet je elders doen.
        /// </remarks>
        public LidVoorstel InschrijvingVoorstellen(GelieerdePersoon gp, GroepsWerkJaar gwj, bool leidingIndienMogelijk)
        {
            var resultaat = new LidVoorstel { GelieerdePersoon = gp, GroepsWerkJaar = gwj };

            if (!gp.GebDatumMetChiroLeefTijd.HasValue)
            {
                // Als we geen geboortedatum hebben, kunnen we niets doen, want het minste dat we willen is
                // valideren of de persoon wel oud genoeg is om leiding te kunnen worden.
                throw new FoutNummerException(FoutNummer.GeboorteDatumOntbreekt, Resources.GeboorteDatumOntbreekt);
            }

            // TODO: Bekijken of we 'AfdelingsJaarVoorstellen' niet kunnen hergebruiken.

            // Als we een geboortejaar hebben, kunnen we een afdeling voorstellen.
            var geboortejaar = gp.GebDatumMetChiroLeefTijd.Value.Year;

            // Bestaat er een afdeling waar de gelieerde persoon als kind in zou passen?
            // (Als er meerdere mogelijkheden zijn zullen we gewoon de eerste kiezen, maar we sorteren op
            // overenkomst geslacht)

            var mogelijkeAfdelingsJaren =
                gwj.AfdelingsJaar.Where(a => a.OfficieleAfdeling.ID != (int)NationaleAfdeling.Speciaal &&
                                             geboortejaar <= a.GeboorteJaarTot &&
                                             a.GeboorteJaarVan <= geboortejaar).OrderByDescending(
                                                 a => (gp.Persoon.Geslacht & a.Geslacht)).ToArray();

            if (mogelijkeAfdelingsJaren.Any())
            {
                // Als we lid kunnen maken: doen.
                // Een lid heeft steeds maar 1 afdeling, vandaar: 'First'.
                resultaat.AfdelingsJaren = new List<AfdelingsJaar> { mogelijkeAfdelingsJaren.First() };
                resultaat.LeidingMaken = false;
            }
            else if (leidingIndienMogelijk && KanLeidingWorden(gp, gwj))
            {
                // Geen afdeling gevonden. Als oud genoeg om leiding te worden: OK
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
                        aj => (Math.Abs(geboortejaar - aj.GeboorteJaarTot))).FirstOrDefault() ??
                    // als bovenstaande expressie null is, proberen we onderstaande. Onderstaande houdt
                    // geen rekening met geslacht.
                    gwj.AfdelingsJaar.OrderBy(
                            aj => (Math.Abs(geboortejaar - aj.GeboorteJaarTot))).FirstOrDefault();

                resultaat.AfdelingsJaren = new List<AfdelingsJaar> { geschiktsteAfdelingsjaar };
                resultaat.LeidingMaken = false;
            }

            return resultaat;
        }

        /// <summary>
        /// Schrijft een gelieerde persoon in, persisteert niet.  Er mag nog geen lidobject (ook geen inactief) voor de
        /// gelieerde persoon bestaan.
        /// </summary>
        /// <param name="voorstelLid">
        ///     Voorstel voor de eigenschappen van het in te schrijven lid.
        /// </param>
        /// <param name="isJaarOvergang">
        ///     Geeft aan of het over de automatische jaarovergang gaat; relevant voor de
        ///     probeerperiode
        /// </param>
        /// <returns>
        /// Het aangemaakte lid object
        /// </returns>
        public Lid NieuwInschrijven(LidVoorstel voorstelLid, bool isJaarOvergang)
        {
            var validator = new LidVoorstelValidator();
            var gp = voorstelLid.GelieerdePersoon;
            var gwj = voorstelLid.GroepsWerkJaar;

            // We gaan ervan uit dat er een voorstel is. Je kunt het voorstel automatisch laten 
            // genereren via InschrijvingVoorstellen.

            Debug.Assert(voorstelLid != null);

            var foutNummer = validator.FoutNummer(voorstelLid);

            if (foutNummer != null)
            {
                if (foutNummer.Value == FoutNummer.OnbekendGeslacht)
                {
                    // Al de rest zit in client side validation, maar dit is een complex geval. Geslacht is verplicht, 
                    // maar als het niet ingevuld wordt, krijg je standaard 'Onbekend', waardoor het toch ingevuld is.
                    throw new FoutNummerException(foutNummer.Value, Resources.GeslachtVerplicht);
                }
                else
                {
                    throw new FoutNummerException(foutNummer.Value, Resources.LidProbleem);
                }
            }

            if (voorstelLid.AfdelingsJarenIrrelevant && !voorstelLid.LeidingMaken)
            {
                // Lid maken en zelf afdeling bepalen
                voorstelLid.AfdelingsJaren = InschrijvingVoorstellen(gp, gwj, false).AfdelingsJaren;

                if (!voorstelLid.AfdelingsJaren.Any())
                {
                    throw new FoutNummerException(FoutNummer.AfdelingNietBeschikbaar, Resources.GeenAfdelingVoorLeeftijd);
                }
            }

            Lid nieuwlid;
            if (voorstelLid.LeidingMaken)
            {
                nieuwlid = LeidingMaken(gp, gwj, isJaarOvergang, voorstelLid.AfdelingsJaren);
            }
            else
            {
                nieuwlid = KindMaken(gp, gwj, isJaarOvergang, voorstelLid.AfdelingsJaren.First());
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
            try
            {
                return AfdelingsJaarVoorstellen(gelieerdePersoon) != null;
            }
            catch (FoutNummerException)
            {
                return false;
            }

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
            if (!gelieerdePersoon.Groep.GroepsWerkJaar.Any())
            {
                // Als er geen groepswerkjaar is, dan is er ook geen lid.
                return null;
            }
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
            if (!gelieerdePersoon.Groep.GroepsWerkJaar.Any())
            {
                return null;
            }
            return AfdelingsJaarVoorstellen(gelieerdePersoon,
                                            gelieerdePersoon.Groep.GroepsWerkJaar.OrderByDescending(gwj => gwj.WerkJaar)
                                                            .First());
        }


        /// <summary>
        /// Geeft <c>true</c> als de gegeven <paramref name="gelieerdePersoon"/> ingeschreven is als
        /// kind of leiding in het huidige werkjaar van zijn groep. Anders <c>false</c>.
        /// </summary>
        /// <param name="gelieerdePersoon">Een gelieerde persoon</param>
        /// <returns>
        /// <c>true</c> als de gegeven <paramref name="gelieerdePersoon"/> ingeschreven is als kind
        /// of leiding in het huidige werkjaar van zijn groep. Anders <c>false</c>.
        /// </returns>
        public bool IsActiefLid(GelieerdePersoon gelieerdePersoon)
        {
            return HuidigLidGet(gelieerdePersoon) != null;
        }

        /// <summary>
        /// Levert <c>true</c> als het gegeven <paramref name="lid"/> een lid is van het huidige werkjaar
        /// van zijn groep.
        /// </summary>
        /// <param name="lid"></param>
        /// <returns>
        /// Levert <c>true</c> als het gegeven <paramref name="lid"/> een lid is van het huidige werkjaar
        /// van zijn groep.
        /// </returns>
        public bool IsVanHuidigWerkjaar(Lid lid)
        {
            var huidigGwj = lid.GroepsWerkJaar.Groep.GroepsWerkJaar.OrderByDescending(gwj => gwj.WerkJaar)
                .FirstOrDefault();
            Debug.Assert(huidigGwj != null);
            int huidigWj = huidigGwj.WerkJaar;
            return (lid.GroepsWerkJaar.WerkJaar == huidigWj);
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
        public AfdelingsJaar AfdelingsJaarVoorstellen(GelieerdePersoon gelieerdePersoon, GroepsWerkJaar groepsWerkJaar)
        {
            // Gooi exception als groepswerkjaar van andere groep als gelieerde persoon.
            if (!gelieerdePersoon.Groep.Equals(groepsWerkJaar.Groep))
            {
                throw new FoutNummerException(FoutNummer.GroepsWerkJaarNietVanGroep, Resources.GroepsWerkJaarNietVanGroep);
            }

            if (gelieerdePersoon.Persoon.GeboorteDatum == null || gelieerdePersoon.Persoon.SterfDatum != null)
            {
                return null;
            }

            Debug.Assert(gelieerdePersoon.GebDatumMetChiroLeefTijd != null);
            // Als er een geboortedatum is, dan moet er ook een geboortedatum met Chiroleeftijd zijn.

            return AfdelingsJaarVoorstellen(gelieerdePersoon.GebDatumMetChiroLeefTijd.Value, gelieerdePersoon.Persoon.Geslacht,
                                            groepsWerkJaar);
        }

        /// <summary>
        /// Vervang de afdelingsjaren van gegeven <paramref name="lid"/> door de 
        /// gegeven <paramref name="afdelingsJaren"/>/
        /// </summary>
        /// <param name="lid">lid waarvan afdelingsjaren te vervangen</param>
        /// <param name="afdelingsJaren">nieuwe afdelingsjaren voor <paramref name="lid"/></param>
        /// <remarks>als <paramref name="lid"/> een kindlid is, dan moet <paramref name="afdelingsJaren"/>
        /// precies 1 afdelingsjaar bevatten.</remarks>
        public void AfdelingsJarenVervangen(Lid lid, IList<AfdelingsJaar> afdelingsJaren)
        {
            if (lid.GroepsWerkJaar.Groep.StopDatum != null && lid.GroepsWerkJaar.Groep.StopDatum < DateTime.Now)
            {
                throw new FoutNummerException(FoutNummer.GroepInactief, Resources.GroepInactief);
            }

            var query = from aj in afdelingsJaren
                        where !Equals(aj.GroepsWerkJaar, lid.GroepsWerkJaar)
                        select aj;

            if (query.Any())
            {
                throw new FoutNummerException(FoutNummer.AfdelingNietVanGroep, Resources.AfdelingNietVanGroep);
            }

            var kind = lid as Kind;
            if (kind != null)
            {
                // lid is kind.
                if (afdelingsJaren.Count != 1)
                {
                    throw new FoutNummerException(FoutNummer.AlgemeneKindFout,
                                                          Resources.AfdelingKindVerplicht);
                }
                kind.AfdelingsJaar = afdelingsJaren.First();
            }
            else
            {
                // lid is leiding
                var leiding = (Leiding)lid;

                // hmmm. Dat zijn hier precies nogal veel loops.
                // Gelukkig zijn het kleine loopjes (loopen over afdelingsjaren)

                // te verwijderen afdelingsjaren verwijderen
                var teVerwijderen = (from aj in leiding.AfdelingsJaar
                                     where !afdelingsJaren.Contains(aj)
                                     select aj).ToList();
                foreach (var aj in teVerwijderen)
                {
                    leiding.AfdelingsJaar.Remove(aj);
                    aj.Leiding.Remove(leiding);
                }

                // toe te voegen afdelingsjaren toevoegen

                foreach (var aj in afdelingsJaren)
                {
                    if (!leiding.AfdelingsJaar.Contains(aj))
                    {
                        leiding.AfdelingsJaar.Add(aj);
                        aj.Leiding.Add(leiding);
                    }
                }
            }
        }

        /// <summary>
        /// Geeft <c>true</c> als de probeerperiode van het gegeven <paramref name="lid"/> voorbij is,
        /// anders <c>false</c>.
        /// </summary>
        /// <param name="lid">Een lid waarvan de probeerperiode getest moet worden.</param>
        /// <returns><c>true</c> als de probeerperiode van het gegeven <paramref name="lid"/> voorbij is,
        /// anders <c>false</c>.</returns>
        public bool ProbeerPeriodeVoorbij(Lid lid)
        {
            return (lid.EindeInstapPeriode < DateTime.Now);
        }

        /// <summary>
        /// Bepaalt of een lid gratis kan aansluiten.
        /// </summary>
        /// <param name="lid">Een lid.</param>
        /// <returns><c>true</c> als dat lid gratis aangesloten kan worden.</returns>
        public bool GratisAansluiting(Lid lid)
        {
            return lid.GroepsWerkJaar.Groep is KaderGroep;
        }

        /// <summary>
        /// Levert het stamnummer van een lid op.
        /// </summary>
        /// <param name="lid">Lid waarvan we het stamnummer willen weten.</param>
        /// <returns>Het stamnummer van dat lid.</returns>
        public string StamNummer(Lid lid)
        {
            return lid.GroepsWerkJaar.Groep.Code;
        }

        /// <summary>
        /// Zoekt een afdelingsjaar uit het gegeven <paramref name="groepsWerkJaar"/>, waarin een persoon
        /// met gegeven <paramref name="geslacht"/> en <paramref name="geboorteDatum"/> lid zou kunnen worden,
        /// of <c>null</c> als er zo geen bestaat.
        /// </summary>
        /// <param name="geboorteDatum">een geboortedatum</param>
        /// <param name="geslacht">een geslacht</param>
        /// <param name="groepsWerkJaar">een groepswerkjaar waarin we naar een afdeling zoeken</param>
        /// <returns>Afdelingsjaar uit het gegeven <paramref name="groepsWerkJaar"/>, waarin een persoon
        /// met gegeven <paramref name="geslacht"/> en <paramref name="geboorteDatum"/> lid zou kunnen worden,
        /// of <c>null</c> als er zo geen bestaat.</returns>
        public AfdelingsJaar AfdelingsJaarVoorstellen(DateTime geboorteDatum, GeslachtsType geslacht, GroepsWerkJaar groepsWerkJaar)
        {
            var geboortejaar = geboorteDatum.Year;

            // Bestaat er een afdeling waar de gelieerde persoon als kind in zou passen?
            // (Als er meerdere mogelijkheden zijn zullen we gewoon de eerste kiezen, maar we sorteren op
            // overenkomst geslacht)

            var mogelijkeAfdelingsJaren =
                groepsWerkJaar.AfdelingsJaar.Where(a => a.OfficieleAfdeling.ID != (int)NationaleAfdeling.Speciaal &&
                                             geboortejaar <= a.GeboorteJaarTot &&
                                             a.GeboorteJaarVan <= geboortejaar).OrderByDescending(
                                                 a => (geslacht & a.Geslacht)).ToArray();

            return mogelijkeAfdelingsJaren.FirstOrDefault();
        }

        /// <summary>
        /// Geeft <c>true</c> als er in de Civi een betalende aansluiting bestaat voor het gegeven lid
        /// <paramref name="l"/> (eventueel via dezelfde persoon in een andere groep).
        /// </summary>
        /// <param name="l">Een lid</param>
        /// <returns><c>true</c> als er in de Civi een betalende aansluiting bestaat voor het gegeven lid
        /// <paramref name="l"/> (eventueel via dezelfde persoon in een andere groep).</returns>
        /// <remarks>Dit wordt bepaald o.m. op basis van het IsAangesloten-veld van de leden.</remarks>
        public bool IsBetalendAangesloten(Lid l)
        {
            //where l.GelieerdePersoon.Persoon.GelieerdePersoon
            //.Where(gp => gp.Lid.Any(l2 => l2.GroepsWerkJaar.WerkJaar == huidigWerkJaar && l2.IsAangesloten)).FirstOrDefault() == null

            var betalendAangeslotenGelieerdePersonen = (from gp in l.GelieerdePersoon.Persoon.GelieerdePersoon
                                                        where gp.Lid.Any(l2 => l2.GroepsWerkJaar.WerkJaar == l.GroepsWerkJaar.WerkJaar && l2.IsAangesloten && !GratisAansluiting(l2))
                                                        select gp);

            return betalendAangeslotenGelieerdePersonen.FirstOrDefault() != null;
        }

        /// <summary>
        /// Geeft <c>true</c> als er in de Civi al een aansluiting bestaat voor het gegeven <paramref name="lid"/>
        /// (typisch via dezelfde persoon in een andere groep).
        /// </summary>
        /// <param name="lid">Een lid</param>
        /// <returns>
        /// <c>true</c> als er in de Civi al een aansluiting bestaat voor het gegeven <paramref name="lid"/>.
        /// </returns>
        /// <remarks>We gaan hiervoor niet in Civi kijken, maar wel naar het IsAangesloten-veld van de leden.</remarks>
        public bool IsAangesloten(Lid lid)
        {
            // TODO: Dit lijkt te hard op de functie hierboven.

            var aangeslotenGelieerdePersonen = (from gp in lid.GelieerdePersoon.Persoon.GelieerdePersoon
                where gp.Lid.Any(l2 => l2.GroepsWerkJaar.WerkJaar == lid.GroepsWerkJaar.WerkJaar && l2.IsAangesloten)
                select gp);

            return aangeslotenGelieerdePersonen.FirstOrDefault() != null;
        }

        /// <summary>
        /// Haal alle leden op die nog aangesloten moeten worden voor werkjaar <paramref name="werkjaar"/>
        /// op de dag gegeven in <paramref name="vandaag"/>.
        /// </summary>
        /// <param name="lidQueryable">Queryable om leden in te zoeken.</param>
        /// <param name="werkjaar">Werkjaar waarvoor leden te zoeken.</param>
        /// <param name="vandaag">De datum van vandaag.</param>
        /// <param name="limit">Begrens het aantal leden tot de gegeven limiet.</param>
        /// <returns>Een array met leden.</returns>
        public Lid[] AanTeSluitenLedenOphalen(IQueryable<Lid> lidQueryable, int werkjaar, DateTime vandaag, int? limit)
        {
            var query = (from l in lidQueryable
                where
                    // maak memberships voor niet-aangesloten leden
                    !l.IsAangesloten &&
                    // maak enkel memberships voor huidig werkjaar
                    l.GroepsWerkJaar.WerkJaar == werkjaar &&
                    // actieve leden waarvan de instapperiode voorbij is
                    l.EindeInstapPeriode < vandaag && !l.NonActief &&
                    // enkel als de groep nog actief was wanneer instapperiode verviel (#4528)
                    (l.GroepsWerkJaar.Groep.StopDatum == null || l.GroepsWerkJaar.Groep.StopDatum > l.EindeInstapPeriode)
                select l);
            if (limit.HasValue)
            {
                query = query.Take(limit.Value);
            }
            var nietAangeslotenLeden = query.ToArray();

            // Overloop de gevonden leden, en kijk na in hoeverre ze naar de Civi moeten.
            // Ik weet niet meer waarom ik twee aparte linq query's gebruikt. Misschien om al
            // op tijd die limit toe te kunnen passen?

            // TODO: In de 'continue'-gevallen van onderstaande loop, kunnen we de leden markeren
            // met IsAangesloten = true. Dan worden ze in de toekomst niet iedere keer opnieuw bekeken.
            // TODO: Fix issue #4966
            return (from lid in nietAangeslotenLeden
            // Als er al betaald is voor het membership, dan gaat het membership niet opnieuw naar CiviCRM, zodat in
            // het membership de aanvrager dezelfde blijft als de betaler.
                where !IsBetalendAangesloten(lid)
            // Als deze aansluiting gratis is, en het lid is al ergens anders aangesloten, dan moet er niets meer
            // gebeuren aan het membership in CiviCRM.
                where !GratisAansluiting(lid) || !IsAangesloten(lid)
            // Als de groep al een werkjaar heeft dat recenter is dan dat van het lid, dan sluiten we het lid niet
            // meer aan in het oude werkjaar.
                where IsVanHuidigWerkjaar(lid)
                select lid).ToArray();
        }
    }
}
