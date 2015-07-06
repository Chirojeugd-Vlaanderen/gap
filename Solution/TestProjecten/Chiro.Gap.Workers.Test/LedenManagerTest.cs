/*
 * Copyright 2008-2014, 2017 the GAP developers. See the NOTICE file at the
 * top-level directory of this distribution, and at
 * https://gapwiki.chiro.be/copyright
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
using Chiro.Gap.Test;
using Chiro.Gap.WorkerInterfaces;
using NUnit.Framework;

namespace Chiro.Gap.Workers.Test
{
    /// <summary>
    /// Dit is een testclass voor LedenManagerTest,
    ///to contain all LedenManagerTest Unit Tests
    /// </summary>
    [TestFixture]
    public class LedenManagerTest: ChiroTest
    {
        ///<summary>
        ///Kijkt na of we een leid(st)er kunnen inschrijven zonder afdelingen
        ///</summary>
        [Test]
        public void InschrijvenLeidingZonderAfdelingTest()
        {
            // ARRANGE

            var groep = new ChiroGroep();
            var groepsWerkJaar = new GroepsWerkJaar {Groep = groep, WerkJaar = 2012};
            var gelieerdePersoon = new GelieerdePersoon
            {
                Groep = groep,
                ChiroLeefTijd = 0,
                Persoon =
                    new Persoon
                    {
                        Geslacht = GeslachtsType.Vrouw,
                        GeboorteDatum = new DateTime(1990, 04, 23)
                    },
                // verplichte velden voor inschrijving (#1786)
                PersoonsAdres = new PersoonsAdres(),
                Communicatie =
                    new List<CommunicatieVorm>
                    {
                        new CommunicatieVorm
                        {
                            CommunicatieType = new CommunicatieType {ID = (int) CommunicatieTypeEnum.TelefoonNummer}
                        },
                        new CommunicatieVorm
                        {
                            CommunicatieType = new CommunicatieType {ID = (int) CommunicatieTypeEnum.Email}
                        }
                    }
            };

            var target = Factory.Maak<LedenManager>();

            // ACT

            var actual =
                target.NieuwInschrijven(new LidVoorstel
                {
                    GroepsWerkJaar = groepsWerkJaar,
                    GelieerdePersoon = gelieerdePersoon,
                    AfdelingsJarenIrrelevant = false,
                    LeidingMaken = true
                }, false) as Leiding;

            // ASSERT

            Assert.IsNotNull(actual);
            Assert.AreEqual(0, actual.AfdelingsJaar.Count);
        }

        /// <summary>
        /// Test of 'LedenManager.InschrijvingVoorstellen' rekening houdt met het geslacht van een persoon.
        ///</summary>
        [Test]
        public void InschrijvingVoorstellenTest()
        {
            // Arrange

            var gp = new GelieerdePersoon
                                      {
                                          Persoon =
                                              new Persoon
                                                  {
                                                      GeboorteDatum = new DateTime(1996, 03, 07),
                                                      Geslacht = GeslachtsType.Vrouw,
                                                  },
                                          PersoonsAdres = new PersoonsAdres(),
                                      };

            var gwj = new GroepsWerkJaar {WerkJaar = 2011, Groep = new ChiroGroep()};
            gp.Groep = gwj.Groep;

            var afdelingsJaar1 = new AfdelingsJaar
                                     {
                                         ID = 1,
                                         GeboorteJaarVan = 1996,
                                         GeboorteJaarTot = 1997,
                                         Geslacht = GeslachtsType.Man,
                                         OfficieleAfdeling = new OfficieleAfdeling()
                                     };
            var afdelingsJaar2 = new AfdelingsJaar
                                     {
                                         ID = 2,
                                         GeboorteJaarVan = 1996,
                                         GeboorteJaarTot = 1997,
                                         Geslacht = GeslachtsType.Vrouw,
                                         OfficieleAfdeling = new OfficieleAfdeling()
                                     };
            gwj.AfdelingsJaar.Add(afdelingsJaar1);
            gwj.AfdelingsJaar.Add(afdelingsJaar2);

            var target = Factory.Maak<LedenManager>();

            // Act
            
            var actual = target.InschrijvingVoorstellen(gp, gwj, false);

            // Assert

            Assert.AreEqual(actual.AfdelingsJaren[0], afdelingsJaar2);
        }

        /// <summary>
        /// Test of er een 'redelijke' afdeling wordt voorgesteld als er voor een persoon
        /// geen afdeling wordt gevonden waarin die 'natuurlijk' past.
        ///</summary>
        [Test]
        public void InschrijvingVoorstellenTest1()
        {
            // Arrange
            // Ik maak een persoon, en een werkjaar met 2 afdelingen, zodanig dat
            // de geboortedatum van die persoon buiten de afdelingen valt. We verwachten
            // de afdeling die het meest logisch is (kleinste verschil met geboortedatum)

            var gp = new GelieerdePersoon
            {
                Persoon =
                    new Persoon
                    {
                        GeboorteDatum = new DateTime(1996, 03, 07),
                        Geslacht = GeslachtsType.Vrouw,
                    },
                PersoonsAdres = new PersoonsAdres(),
            };

            var gwj = new GroepsWerkJaar {WerkJaar = 2011, Groep = new ChiroGroep()};
            gp.Groep = gwj.Groep;

            var afdelingsJaar1 = new AfdelingsJaar
            {
                ID = 1,
                GeboorteJaarVan = 1999,
                GeboorteJaarTot = 2000,
                Geslacht = GeslachtsType.Gemengd,
                OfficieleAfdeling = new OfficieleAfdeling(),
                GroepsWerkJaar = gwj
            };
            var afdelingsJaar2 = new AfdelingsJaar
            {
                ID = 2,
                GeboorteJaarVan = 1997,
                GeboorteJaarTot = 1998,
                Geslacht = GeslachtsType.Gemengd,
                OfficieleAfdeling = new OfficieleAfdeling(),
                GroepsWerkJaar = gwj
            };
            gwj.AfdelingsJaar.Add(afdelingsJaar1);
            gwj.AfdelingsJaar.Add(afdelingsJaar2);


            var target = Factory.Maak<LedenManager>();

            // Act

            var actual = target.InschrijvingVoorstellen(gp, gwj, false);

            // Assert

            Assert.AreEqual(actual.AfdelingsJaren[0], afdelingsJaar2);
        }

        /// <summary>
        /// LidMaken moet weigeren kleuters in te schrijven.
        ///</summary>
        [Test]
        public void LidMakenTest()
        {
            // Arrange
            // Ik probeer iemand lid te maken die te jong is, en verwacht een exception.

            var groep = new ChiroGroep() { ID = 1 };

            var gwj = new GroepsWerkJaar { WerkJaar = 2012, Groep = groep };  // Werkjaar 2012-2013

            var gp = new GelieerdePersoon
            {
                Persoon =
                    new Persoon
                    {
                        GeboorteDatum = new DateTime(2010, 06, 21), // Geboren in 2010
                        Geslacht = GeslachtsType.Vrouw,
                    },
                Groep = groep
            };

            var target = new LedenManager();

            // Assert
            Assert.Throws<FoutNummerException>(() => target.NieuwInschrijven(
                new LidVoorstel
                {
                    GelieerdePersoon = gp,
                    GroepsWerkJaar = gwj,
                    AfdelingsJarenIrrelevant = true,
                    LeidingMaken = false
                }, false));
        }

        /// <summary>
        /// Als je groep als gestopt staat geregistreerd, dan mag je leden niet meer naar
        /// een andere afdeling verhuizen.
        ///</summary>
        [Test]
        public void AfdelingsJarenVervangenGestoptTest()
        {
            var target = new LedenManager();

            var groepsWerkJaar = new GroepsWerkJaar
                                     {
                                         Groep =
                                             new ChiroGroep
                                                 {
                                                     StopDatum = DateTime.Now.AddMonths(-1)
                                                 }
                                     };

            var lid = new Kind
                          {
                              GroepsWerkJaar = groepsWerkJaar                                 
                          };

            IList<AfdelingsJaar> afdelingsJaren = new List<AfdelingsJaar>
                                                      {
                                                          new AfdelingsJaar
                                                              {
                                                                  GroepsWerkJaar =
                                                                      groepsWerkJaar
                                                              }
                                                      };

            var ex = Assert.Throws<FoutNummerException>(() => target.AfdelingsJarenVervangen(lid, afdelingsJaren));
            Assert.That(ex.FoutNummer == FoutNummer.GroepInactief);
        }

        /// <summary>
        /// Lid/Leiding omwisselen mag niet in een inactieve groep
        /// </summary>
        [Test]
        public void TypeToggleGestoptTest()
        {
            var target = new LedenManager();

            Lid origineelLid = new Kind
                                   {
                                       GroepsWerkJaar =
                                           new GroepsWerkJaar
                                               {
                                                   Groep =
                                                       new ChiroGroep
                                                           {
                                                               StopDatum =
                                                                   DateTime.Now.AddMonths(-1)
                                                           }
                                               }
                                   };

            var ex = Assert.Throws<FoutNummerException>(() => target.TypeToggle(origineelLid));
            Assert.That(ex.FoutNummer == FoutNummer.GroepInactief);
        }

        /// <summary>
        /// Maak geen aansluitingen meer aan voor het oude werkjaar, als de ploeg in het nieuwe werkjaar werkt.
        /// </summary>
        [Test]
        public void GeenAansluitingenOudWerkjaarTest()
        {
            // Stel: het is juli 2016.
            var datum = new DateTime(2016, 7, 15);

            var groep = new ChiroGroep
            {
                GroepsWerkJaar = new List<GroepsWerkJaar>()
            };
            var gwj1 = new GroepsWerkJaar {Groep = groep, WerkJaar = 2016};
            // De groep heeft zijn jaarovergang al gedaan, en er bestaat dus een werkjaar 2017-2018.
            var gwj2 = new GroepsWerkJaar {Groep = groep, WerkJaar = 2017};
            groep.GroepsWerkJaar.Add(gwj1);
            groep.GroepsWerkJaar.Add(gwj2);

            var lid = new Leiding
            {
                GroepsWerkJaar = gwj1,
                EindeInstapPeriode = datum.AddDays(-5),
                GelieerdePersoon = new GelieerdePersoon
                {
                    Groep = groep,
                    Persoon = new Persoon()
                }
            };
            lid.GelieerdePersoon.Persoon.GelieerdePersoon.Add(lid.GelieerdePersoon);

            gwj1.Lid = new List<Lid> {lid};

            var target = new LedenManager();

            // Nu proberen we uit te zoeken welke leden er nog aangesloten moeten worden in 2016-2017.
            var result = target.AanTeSluitenLedenOphalen(groep.GroepsWerkJaar.SelectMany(gwj => gwj.Lid).AsQueryable(),
                2016, datum, null);

            Assert.IsEmpty(result);
        }

        /// <summary>
        /// Als de property <c>LaatsteMembership</c> van een persoon <c>null</c> is, dan mag dat niet verhinderen dat
        /// een persoon aangesloten wordt.
        /// </summary>
        [Test]
        public void NooitAangeslotenLedenAansluitenTest()
        {
            // ARRANGE
            const int huidigWerkjaar = 2014;
            DateTime vandaagZoGezegd = new DateTime(2015, 02, 23);

            // We hebben 1 leidster, die niet meer in haar probeerperiode zit.
            var leidster = new Leiding
            {
                ID = 1,
                EindeInstapPeriode = vandaagZoGezegd.AddDays(-7),
                GelieerdePersoon = new GelieerdePersoon
                {
                    ID = 2,
                    Persoon = new Persoon
                    {
                        ID = 3,
                        VoorNaam = "Kelly",
                        Naam = "Pfaff"
                    }
                },
                GroepsWerkJaar = new GroepsWerkJaar
                {
                    ID = 4,
                    WerkJaar = huidigWerkjaar,
                    Groep = new ChiroGroep { ID = 5 }
                }
            };
            leidster.GroepsWerkJaar.Groep.GroepsWerkJaar = new List<GroepsWerkJaar>{leidster.GroepsWerkJaar};

            // ACT
            var target = new LedenManager();
            var result = target.AanTeSluitenLedenOphalen((new List<Lid> {leidster}).AsQueryable(), huidigWerkjaar,
                vandaagZoGezegd, null);

            // ASSERT
            Assert.IsNotEmpty(result);
        }

        /// <summary>
        /// Als iemand al een gratis membership heeft via een kaderploeg, maar nu ook lid is van
        /// een plaatselijke groep, moet het bestaande membership betalend worden. (#4519)
        /// </summary>
        [Test]
        public void VanGratisNaarBetalendMembershipTest()
        {
            // ARRANGE
            const int huidigWerkjaar = 2015;
            DateTime vandaagZoGezegd = new DateTime(2016, 1, 7);

            var gewest = new KaderGroep { ID = 6 };

            // We hebben 1 leidster, die ook in het gewest actief is.
            var leidster = new Leiding
            {
                ID = 1,
                EindeInstapPeriode = vandaagZoGezegd.AddDays(-7),
                GelieerdePersoon = new GelieerdePersoon
                {
                    ID = 2,
                    Persoon = new Persoon
                    {
                        ID = 3,
                        VoorNaam = "Kelly",
                        Naam = "Pfaff"
                    }
                },
                GroepsWerkJaar = new GroepsWerkJaar
                {
                    ID = 4,
                    WerkJaar = huidigWerkjaar,
                    Groep = new ChiroGroep { ID = 5 }
                }
            };
            leidster.GroepsWerkJaar.Groep.GroepsWerkJaar = new List<GroepsWerkJaar>{leidster.GroepsWerkJaar};

            leidster.GelieerdePersoon.Persoon.GelieerdePersoon.Add(new GelieerdePersoon
            {
                ID = 5,
                Groep = gewest,
                Lid = new [] {new Leiding
                {
                    ID = 7,
                    IsAangesloten = true,
                    GroepsWerkJaar = new GroepsWerkJaar
                    {
                        ID = 8,
                        WerkJaar = huidigWerkjaar,
                        Groep = gewest
                    }
                } }
            });

            // ACT
            var target = new LedenManager();
            var result = target.AanTeSluitenLedenOphalen((new List<Lid> {leidster}).AsQueryable(), huidigWerkjaar,
                vandaagZoGezegd, null);

            // ASSERT
            Assert.IsNotEmpty(result);
        }

        /// <summary>
        /// NieuwInschrijven mag niet inschrijven als leiding als de persoon daar te jong voor is.
        ///</summary>
        [Test]
        public void NieuwInschrijvenLeidingLeeftijd()
        {
            // ARRANGE

            var groep = new ChiroGroep();
            var groepsWerkJaar = new GroepsWerkJaar { Groep = groep, WerkJaar = 2012 };
            var gelieerdePersoon = new GelieerdePersoon
            {
                Groep = groep,
                ChiroLeefTijd = 0,
                Persoon =
                    new Persoon
                    {
                        Geslacht = GeslachtsType.Vrouw,
                        GeboorteDatum = new DateTime(2000, 04, 23)  // wat jong om leiding te worden
                    }
            };

            var target = Factory.Maak<LedenManager>();

            // ACT

            try
            {
                target.NieuwInschrijven(new LidVoorstel
                {
                    GelieerdePersoon = gelieerdePersoon,
                    GroepsWerkJaar = groepsWerkJaar,
                    AfdelingsJarenIrrelevant = false,
                    LeidingMaken = true
                }, false);
            }
            catch (GapException)
            {
                // We verwachten weliswaar een exception, maar we verwachten ook
                // dat het lid niet gemaakt is.
            }

            // ASSERT

            Assert.AreEqual(0, gelieerdePersoon.Lid.Count);
        }

        /// <summary>
        /// NieuwInschrijven mag geen leden inschrijven zonder adres.
        ///</summary>
        [Test]
        public void NieuwInschrijvenLidAdres()
        {
            // ARRANGE

            var groep = new ChiroGroep();
            var groepsWerkJaar = new GroepsWerkJaar {Groep = groep, WerkJaar = 2014};
            var gelieerdePersoon = new GelieerdePersoon
            {
                Groep = groep,
                ChiroLeefTijd = 0,
                Persoon =
                    new Persoon
                    {
                        Geslacht = GeslachtsType.Vrouw,
                        GeboorteDatum = new DateTime(1994, 04, 23)
                    }
            };

            var target = Factory.Maak<LedenManager>();

            // Assert

            var ex = Assert.Throws<FoutNummerException>(() => target.NieuwInschrijven(new LidVoorstel
            {
                GelieerdePersoon = gelieerdePersoon,
                GroepsWerkJaar = groepsWerkJaar,
                AfdelingsJarenIrrelevant = false,
                LeidingMaken = true
            }, false));
            Assert.AreEqual(FoutNummer.AdresOntbreekt, ex.FoutNummer);
        }
    }
}
