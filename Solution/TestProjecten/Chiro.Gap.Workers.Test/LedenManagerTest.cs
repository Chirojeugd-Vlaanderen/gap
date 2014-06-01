/*
 * Copyright 2008-2014 the GAP developers. See the NOTICE file at the 
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

using System.Collections.Generic;
using Chiro.Cdf.Ioc;
using Chiro.Gap.Domain;
using Chiro.Gap.Poco.Model;
using Chiro.Gap.Poco.Model.Exceptions;
using Chiro.Gap.TestAttributes;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using Chiro.Gap.WorkerInterfaces;

namespace Chiro.Gap.Workers.Test
{
    /// <summary>
    /// Dit is een testclass voor LedenManagerTest,
    ///to contain all LedenManagerTest Unit Tests
    /// </summary>
    [TestClass]
    public class LedenManagerTest
    {

        #region Additional test attributes
        // 
        //You can use the following additional attributes as you write your tests:
        //
        //Use ClassInitialize to run code before running the first test in the class
        //[ClassInitialize]
        //public static void MyClassInitialize(TestContext testContext)
        //{
        //}
        //
        //Use ClassCleanup to run code after all tests in a class have run
        //[ClassCleanup()]
        //public static void MyClassCleanup()
        //{
        //}
        //

        /// <summary>
        /// Run code before running each test
        /// </summary>
        [TestInitialize]
        public void MyTestInitialize()
        {
            Factory.ContainerInit();
        }

        //
        //Use TestCleanup to run code after each test has run
        //[TestCleanup()]
        //public void MyTestCleanup()
        //{
        //}
        //
        #endregion


        ///<summary>
        ///Kijkt na of we een leid(st)er kunnen inschrijven zonder afdelingen
        ///</summary>
        [TestMethod()]
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
        [TestMethod()]
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
        [TestMethod()]
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
        [TestMethod()]
        [ExpectedException(typeof(FoutNummerException))]
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

            var ledenMgr = Factory.Maak<LedenManager>();
            var accessor = new PrivateObject(ledenMgr);

            var target = new LedenManager();

            // Act
            target.NieuwInschrijven(
                new LidVoorstel
                {
                    GelieerdePersoon = gp,
                    GroepsWerkJaar = gwj,
                    AfdelingsJarenIrrelevant = true,
                    LeidingMaken = false
                }, false);

            //Assert
            Assert.Fail();  // Als we hier komen zonder exception, dan is het mislukt.
        }

        /// <summary>
        /// Als je groep als gestopt staat geregistreerd, dan mag je leden niet meer naar
        /// een andere afdeling verhuizen.
        ///</summary>
        [TestMethod()]
        [ExpectedFoutNummer(typeof(FoutNummerException), FoutNummer.GroepInactief)]
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

            target.AfdelingsJarenVervangen(lid, afdelingsJaren);
        }

        /// <summary>
        /// Lid/Leiding omwisselen mag niet in een inactieve groep
        /// </summary>
        [TestMethod()]
        [ExpectedFoutNummer(typeof(FoutNummerException), FoutNummer.GroepInactief)]
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
            
            target.TypeToggle(origineelLid);
        }

        /// <summary>
        /// NieuwInschrijven mag niet inschrijven als leiding als de persoon daar te jong voor is.
        ///</summary>
        [TestMethod()]
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
        [TestMethod()]
        [ExpectedFoutNummer(typeof(FoutNummerException), FoutNummer.AdresOntbreekt)]
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

            // ACT

            target.NieuwInschrijven(new LidVoorstel
            {
                GelieerdePersoon = gelieerdePersoon,
                GroepsWerkJaar = groepsWerkJaar,
                AfdelingsJarenIrrelevant = false,
                LeidingMaken = true
            }, false);

            // We don't assert anything. We just expect an exception.
        }
    }
}
