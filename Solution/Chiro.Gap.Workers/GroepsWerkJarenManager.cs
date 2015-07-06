/*
 * Copyright 2008-2015 the GAP developers. See the NOTICE file at the 
 * top-level directory of this distribution, and at
 * https://develop.chiro.be/gap/wiki/copyright
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
using System.Linq;
using Chiro.Gap.Domain;
using Chiro.Gap.Poco.Model;
using Chiro.Gap.Poco.Model.Exceptions;
using Chiro.Gap.WorkerInterfaces;
using Chiro.Gap.Workers.Properties;

namespace Chiro.Gap.Workers
{
    /// <summary>
    /// Worker die alle businesslogica i.v.m. groepswerkjaren bevat
    /// </summary>
    public class GroepsWerkJarenManager : IGroepsWerkJarenManager
    {
        private readonly IVeelGebruikt _veelGebruikt;

        public GroepsWerkJarenManager(IVeelGebruikt veelGebruikt)
        {
            _veelGebruikt = veelGebruikt;
        }


        /// <summary>
        /// Berekent de theoretische einddatum van het gegeven groepswerkjaar.
        /// </summary>
        /// <param name="groepsWerkJaar">
        /// Groepswerkjaar, met daaraan gekoppeld een werkjaarobject
        /// </param>
        /// <returns>
        /// Einddatum van het gekoppelde werkJaar.
        /// </returns>
        public DateTime EindDatum(GroepsWerkJaar groepsWerkJaar)
        {
            DateTime wjStart = Settings.Default.WerkjaarStartNationaal;
            return new DateTime(groepsWerkJaar.WerkJaar + 1, wjStart.Month, wjStart.Day).AddDays(-1);
        }

        /// <summary>
        /// Maakt een nieuw groepswerkjaar in het gevraagde werkJaar.
        /// Persisteert niet ;-P
        /// </summary>
        /// <param name="g">
        /// De groep waarvoor een groepswerkjaar aangemaakt moet worden
        /// </param>
        /// <returns>
        /// Het nieuwe groepswerkjaar
        /// </returns>
        /// <throws>OngeldigObjectException</throws>
        public GroepsWerkJaar VolgendGroepsWerkJaarMaken(Groep g)
        {
            // Bereken gewenste werkjaar
            // (die parameter g.ID is irrelevant, maar blijkbaar nodig voor een of andere
            // unit test.)
            int werkJaar = NieuweWerkJaar(g.ID);

            // Controle op dubbels moet gebeuren door data access.  (Zie #507)
            var resultaat = new GroepsWerkJaar { Groep = g, WerkJaar = werkJaar };
            g.GroepsWerkJaar.Add(resultaat);

            return resultaat;
        }

        /// <summary>
        /// Stelt afdelingsjaren voor voor de gegeven <paramref name="groep"/> en <paramref name="afdelingen"/>
        /// in het werkjaar <paramref name="nieuwWerkJaar"/> - <paramref name="nieuwWerkJaar"/>+1.
        /// </summary>
        /// <param name="groep">Groep waarvoor afdelingsjaren moeten worden voorgesteld, met daaraan gekoppeld
        /// het huidige groepswerkjaar, de huidige afdelingsjaren, en alle beschikbare afdelingen.</param>
        /// <param name="afdelingen">Afdelingen waarvoor afdelingsjaren moeten worden voorgesteld</param>
        /// <param name="nieuwWerkJaar">Bepaalt het werkjaar waarvoor de afdelingsjaren voorgesteld moeten worden.</param>
        /// <param name="standaardOfficieleAfdeling">Officiele afdeling die standaard voorgesteld moet worden als de
        /// afdeling het laatste afdelingsjaar niet in gebruik was.</param>
        /// <returns>Lijstje afdelingsjaren</returns>
        public IList<AfdelingsJaar> AfdelingsJarenVoorstellen(ChiroGroep groep, IList<Afdeling> afdelingen,
                                                              int nieuwWerkJaar,
                                                              OfficieleAfdeling standaardOfficieleAfdeling)
        {
            // TODO: als een afdeling vorig jaar niet bestond, dan maken we een afdelingsjaar op basis van de meegegeven
            // standaardOfficieleAfdeling. Dat kan beter. We zouden in eerste instantie kunnen nakijken of de afdeling
            // in een vroeger afdelingsjaar gebruikt was. En in tweede instantie zouden we kunnen gokken op basis van 
            // de naam.

            if (afdelingen.FirstOrDefault(afd => afd.ChiroGroep.ID != groep.ID) != null)
            {
                throw new FoutNummerException(FoutNummer.AfdelingNietVanGroep, Resources.AfdelingNietVanGroep);
            }

            var recentsteWerkjaar = groep.GroepsWerkJaar.OrderByDescending(gwj => gwj.WerkJaar).FirstOrDefault();

            if (recentsteWerkjaar == null)
            {
                // Eigenlijk gaan we ervan uit dat elke groep al wel een afdelingsjaar heeft.  Maar
                // moest het toch niet zo zijn, dan geven we gauw een domme suggestie terug

                return (from afd in afdelingen
                        select
                            new AfdelingsJaar
                                {
                                    Afdeling = afd,
                                    OfficieleAfdeling = standaardOfficieleAfdeling,
                                    Geslacht = GeslachtsType.Gemengd
                                }).ToList();
            }

            var werkJarenVerschil = nieuwWerkJaar - recentsteWerkjaar.WerkJaar;

            // We halen de afdelingsjaren van het huidige (oude) werkjaar op, zodat we op basis daarvan geboortejaren
            // en geslacht voor de nieuwe afdelingsjaren in het nieuwe werkjaar kunnen voorstellen.

            var huidigeAfdelingsJaren = recentsteWerkjaar.AfdelingsJaar;

            // Creeer een voorstel voor de nieuwe afdelingsjaren

            var nieuweAfdelingsJaren = new List<AfdelingsJaar>();

            foreach (var afdeling in afdelingen)
            {
                // geboortejaren en geslacht gewoon default values, passen we zo nodig
                // straks nog aan.

                var afdelingsJaar = new AfdelingsJaar
                                        {
                                            Afdeling = afdeling,
                                            GeboorteJaarVan = 0,
                                            GeboorteJaarTot = 0,
                                            Geslacht = GeslachtsType.Gemengd
                                        };


                nieuweAfdelingsJaren.Add(afdelingsJaar);

                // Als de afdeling dit jaar al actief was, kunnen we de details automatisch bepalen

                var bestaandAfdelingsJaar = (from aj in huidigeAfdelingsJaren
                                             where aj.Afdeling.ID == afdeling.ID
                                             select aj).FirstOrDefault();

                if (bestaandAfdelingsJaar != null)
                {
                    afdelingsJaar.OfficieleAfdeling = bestaandAfdelingsJaar.OfficieleAfdeling;
                    afdelingsJaar.Geslacht = bestaandAfdelingsJaar.Geslacht;
                    afdelingsJaar.GeboorteJaarTot = bestaandAfdelingsJaar.GeboorteJaarTot + werkJarenVerschil;
                    afdelingsJaar.GeboorteJaarVan = bestaandAfdelingsJaar.GeboorteJaarVan + werkJarenVerschil;

                    // 'Gemengd' werd geherdefinieerd sinds het gap het derde geslacht ondersteunt (#3814).
                    // De 'oude' interpretatie van gemengd moet nu vertaald worden naar M|V|X. (zie #3849).
                    if (afdelingsJaar.Geslacht == (GeslachtsType.Man | GeslachtsType.Vrouw))
                    {
                        afdelingsJaar.Geslacht = GeslachtsType.Gemengd;
                    }
                }
                else
                {
                    // Als officiële afdeling, geven we ribbels, om te vermijden dat de groepen te snel
                    // 'speciaal' zouden kiezen.
                    // TODO: Als een afdeling vorig jaar niet gebruikt werd, zou het niet slecht zijn moesten
                    // we opgezocht hebben of ze in een erder verleden wel gebruikt is geweest.

                    afdelingsJaar.OfficieleAfdeling = standaardOfficieleAfdeling;
                    afdelingsJaar.Geslacht = GeslachtsType.Gemengd;
                    afdelingsJaar.GeboorteJaarTot = nieuwWerkJaar - standaardOfficieleAfdeling.LeefTijdVan;
                    afdelingsJaar.GeboorteJaarVan = nieuwWerkJaar - standaardOfficieleAfdeling.LeefTijdTot;
                }
            }

            // Sorteer de afdelingsjaren: eerst die zonder geboortejaren, dan van jong naar oud
            var resultaat = (from a in nieuweAfdelingsJaren
                             orderby a.GeboorteJaarTot descending
                             orderby a.GeboorteJaarTot == 0 descending
                             select a).ToArray();
            return resultaat;
        }

        /// <summary>
        /// Levert het huidige werkjaar op, volgens 'nationaal'.
        /// </summary>
        /// <returns>Het jaartal waarin het huidige werkjaar begon</returns>
        public int HuidigWerkJaarNationaal()
        {
            var overgang = new DateTime(
                DateTime.Today.Year,
                Settings.Default.WerkjaarStartNationaal.Month,
                Settings.Default.WerkjaarStartNationaal.Day);

            if (overgang <= DateTime.Today)
            {
                return overgang.Year;
            }
            return overgang.Year - 1;
        }

        /// <summary>
        /// Levert de datum van vandaag op.
        /// </summary>
        /// <returns>De datum van vandaag.</returns>
        /// <remarks>
        /// Dit is een tamelijk domme functie. Maar ze is er om met de datum te kunnen
        /// foefelen in de unit tests.
        /// </remarks>
        public DateTime Vandaag()
        {
            return DateTime.Now.Date;
        }

        /// <summary>
        /// Bepaalt of in het gegeven <paramref name='werkJaar' /> op
	    /// het gegeven <paramref name='tijdstip' /> de jaarovergang al
	    /// kan doorgaan.
        /// </summary>
        /// <param name="tijdstip"> </param>
        /// <param name="werkJaar">
        /// Jaartal van het 'huidige' werkjaar (i.e. 2010 voor 2010-2011 enz)
        /// </param>
        /// <returns>
        /// Datum in het gegeven werkjaar vanaf wanneer het nieuwe aangemaakt mag worden
        /// </returns>
        public bool OvergangMogelijk(DateTime tijdstip, int werkJaar)
        {
#if JAAROVERGANGDEBUG
            return true;
#endif
            return tijdstip >= StartOvergang(werkJaar);
        }

        /// <summary>
        /// Berekent wat het nieuwe werkjaar zal zijn als op deze moment de jaarovergang zou gebeuren.
        /// </summary>
        /// <returns>
        /// Het jaar waarin dat nieuwe werkJaar begint
        /// </returns>
        /// <remarks>De paramter <paramref name="groepID"/> is er enkel voor debugging purposes</remarks>
        public int NieuweWerkJaar(int groepID)
        {
#if JAAROVERGANGDEBUG
            return RecentsteOphalen(groepID).WerkJaar+1;
#endif
            // Bereken gewenste werkjaar

            // Het algoritme kijkt het volgende na:
            // Stel dat de jaarovergang mogelijk wordt vanaf 1 augustus.
            // Als vandaag voor 1 augustus is, dan is het werkJaar vorig jaar begonnen => huidig jaar - 1
            // Als vandaag 1 augustus of later is, dan begint het werkJaar dit kalenderjaar => huidig jaar.
            int werkJaar;

            var startdate = new DateTime(
                DateTime.Today.Year,
                Settings.Default.BeginOvergangsPeriode.Month,
                Settings.Default.BeginOvergangsPeriode.Day);

            if (DateTime.Today < startdate)
            {
                // vroeger
                werkJaar = DateTime.Today.Year - 1;
            }
            else
            {
                werkJaar = DateTime.Today.Year;
            }

            return werkJaar;
        }

        /// <summary>
        /// Bepaalt de datum vanaf wanneer het volgende werkJaar begonnen kan worden
        /// </summary>
        /// <param name="werkJaar">
        /// Jaartal van het 'huidige' werkJaar (i.e. 2010 voor 2010-2011 enz)
        /// </param>
        /// <returns>
        /// Datum in het gegeven werkJaar vanaf wanneer het nieuwe aangemaakt mag worden
        /// </returns>
        public DateTime StartOvergang(int werkJaar)
        {
            var datum = Settings.Default.BeginOvergangsPeriode;
            return new DateTime(werkJaar + 1, datum.Month, datum.Day);
        }

        /// <summary>
        /// Controleert of de datum <paramref name="dateTime"/> zich in het werkJaar <paramref name="p"/> bevindt.
        /// </summary>
        /// <param name="dateTime">
        /// Te controleren datum
        /// </param>
        /// <param name="p">
        /// Werkjaar.  (2010 voor 2010-2011 enz.)
        /// </param>
        /// <returns>
        /// <c>True</c> als <paramref name="dateTime"/> zich in het werkJaar bevindt; anders <c>false</c>.
        /// </returns>
        public bool DatumInWerkJaar(DateTime dateTime, int p)
        {
            var werkJaarStart = new DateTime(
                p,
                Settings.Default.WerkjaarStartNationaal.Month,
                Settings.Default.WerkjaarStartNationaal.Day);

            DateTime werkJaarStop = new DateTime(
                p + 1,
                Settings.Default.WerkjaarStartNationaal.Month,
                Settings.Default.WerkjaarStartNationaal.Day).AddDays(-1);

            return werkJaarStart <= dateTime && dateTime <= werkJaarStop;
        }
    }
}
