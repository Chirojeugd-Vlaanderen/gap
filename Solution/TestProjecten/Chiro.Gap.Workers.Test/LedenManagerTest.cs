/*
 * Copyright 2008-2013 the GAP developers. See the NOTICE file at the 
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
using Chiro.Cdf.Ioc;
using Chiro.Gap.Domain;
using Chiro.Gap.Poco.Model;
using Chiro.Gap.Poco.Model.Exceptions;
using Chiro.Gap.SyncInterfaces;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using Moq;

namespace Chiro.Gap.Workers.Test.CustomIoc
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

        private ChiroGroep _groep;
        private GelieerdePersoon _gp;
        private GroepsWerkJaar _gwj;
        private LidVoorstel _voorstel;
        private AfdelingsJaar _afd1;
        private AfdelingsJaar _afd2;

        // FIXME: Een aantal van onderstaande tests gaan _gp in- en uitschrijven als lid en leiding.
        // Het is echter niet gezegd dat de tests in volgorde uitgevoerd worden; het kan goed zijn
        // dat sommige tests tegelijkertijd lopen.  In die gevallen gaan ze elkaar in de weg lopen,
        // en failen.

        private void Setup()
        {
            // Creeer een aantal dummygegevens om op te testen.
            _groep = new ChiroGroep { ID = 493 };
            _gp = new GelieerdePersoon
            {
                Persoon = new Persoon
                {
                    Geslacht = GeslachtsType.Man,
                    GeboorteDatum = new DateTime(1992, 3, 7)
                },
                Groep = _groep
            };
            _gwj = new GroepsWerkJaar
            {
                Groep = _groep,
                WerkJaar = 2011
            };
            _voorstel = new LidVoorstel
            {
                AfdelingsJaarIDs = new int[0],
                AfdelingsJarenIrrelevant = false,
                LeidingMaken = true
            };
            _afd1 = new AfdelingsJaar { ID = 1 };
            _afd2 = new AfdelingsJaar { ID = 2 };
            _gwj.AfdelingsJaar.Add(new AfdelingsJaar { ID = 0 });
            _gwj.AfdelingsJaar.Add(_afd1);
            _gwj.AfdelingsJaar.Add(_afd2);
        }

        ///<summary>
        ///Kijkt na of we een leid(st)er kunnen inschrijven zonder afdelingen
        ///</summary>
        [TestMethod()]
        public void InschrijvenTest()
        {
            //// LedenManager_Accessor, zodat we ook private members kunnen testen.
            //var target = Factory.Maak<LedenManager>();
            //Setup();

            //// act
            //var actual = target.NieuwInschrijven(_gp, _gwj, false, _voorstel) as Leiding;

            //// assert
            //Assert.IsNotNull(actual);
            //Assert.AreEqual(0, actual.AfdelingsJaar.Count);
            throw new NotImplementedException(NIEUWEBACKEND.Info);
        }

        ///<summary>
        /// Test herinschrijven van leiding als lid en in een andere afdeling
        ///</summary>
        [TestMethod()]
        public void HerInschrijvenAlsLidTest()
        {
            //var kindDaoMock = new Mock<IKindDao>();
            //kindDaoMock.Setup(src => src.Bewaren(It.IsAny<Kind>(), It.IsAny<LidExtras>())).Returns<Kind, LidExtras>((kind, extra) => kind);

            //// we verwachten dat het kind bewaard zal worden

            //Factory.InstantieRegistreren(kindDaoMock.Object);

            //var target = Factory.Maak<LedenManager>();
            //Setup();
            
            //// act
            //var actual = target.NieuwInschrijven(_gp, _gwj, false, _voorstel) as Leiding;
            //actual.UitschrijfDatum = DateTime.Today;
            
            //var voorstel2 = new LidVoorstel
            //{
            //    AfdelingsJaarIDs = new[]{1},
            //    AfdelingsJarenIrrelevant = false,
            //    LeidingMaken = false
            //};

            //var newlid = target.Wijzigen(actual, voorstel2) as Kind;

            //// assert
            //Assert.IsNotNull(newlid);
            //Assert.AreEqual(_afd1, newlid.AfdelingsJaar);
            //Assert.IsNull(newlid.UitschrijfDatum);
            throw new NotImplementedException(NIEUWEBACKEND.Info);
        }

        ///<summary>
        /// Test herinschrijven van lid als leiding met 2 afdelingen
        ///</summary>
        [TestMethod()]
        public void HerInschrijvenAlsLeidingTest()
        {
            //var leidingDaoMock = new Mock<ILeidingDao>();
            //leidingDaoMock.Setup(src => src.Bewaren(It.IsAny<Leiding>(), It.IsAny<LidExtras>())).Returns<Leiding, LidExtras>((ld, extra) => ld);

            //// we verwachten dat de leid(st)er bewaard zal worden

            //Factory.InstantieRegistreren(leidingDaoMock.Object);

            //var target = Factory.Maak<LedenManager>();
            //Setup();

            //// act
            //_voorstel.LeidingMaken = false;
            //_voorstel.AfdelingsJaarIDs = new[] {0};
            //var actual = target.NieuwInschrijven(_gp, _gwj, false, _voorstel) as Kind;
            //actual.UitschrijfDatum = DateTime.Today;

            //var voorstel2 = new LidVoorstel
            //{
            //    AfdelingsJaarIDs = new[] { 1, 2 },
            //    AfdelingsJarenIrrelevant = false,
            //    LeidingMaken = true
            //};

            //var newlid = target.Wijzigen(actual, voorstel2) as Leiding;

            //// assert
            //Assert.IsNotNull(newlid);
            //Assert.IsTrue(newlid.AfdelingsJaar.Contains(_afd1));
            //Assert.IsTrue(newlid.AfdelingsJaar.Contains(_afd2));
            //Assert.IsNull(newlid.UitschrijfDatum);
            throw new NotImplementedException(NIEUWEBACKEND.Info);
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
                                                  }
                                      };

            var gwj = new GroepsWerkJaar {WerkJaar = 2011};
            gwj.AfdelingsJaar.Add(new AfdelingsJaar
                                      {
                                          ID = 1,
                                          GeboorteJaarVan = 1996,
                                          GeboorteJaarTot = 1997,
                                          Geslacht = GeslachtsType.Man,
                                          OfficieleAfdeling = new OfficieleAfdeling()
                                      });
                        gwj.AfdelingsJaar.Add(new AfdelingsJaar
                                      {
                                          ID = 2,
                                          GeboorteJaarVan = 1996,
                                          GeboorteJaarTot = 1997,
                                          Geslacht = GeslachtsType.Vrouw,
                                          OfficieleAfdeling = new OfficieleAfdeling()
                                      });


            var target = Factory.Maak<LedenManager>();

            // Act
            
            var actual = target.InschrijvingVoorstellen(gp, gwj, false);

            // Assert

            Assert.AreEqual(actual.AfdelingsJaarIDs[0], 2);
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
                    }
            };

            var gwj = new GroepsWerkJaar {WerkJaar = 2011};

            gwj.AfdelingsJaar.Add(new AfdelingsJaar
            {
                ID = 1,
                GeboorteJaarVan = 1999,
                GeboorteJaarTot = 2000,
                Geslacht = GeslachtsType.Gemengd,
                OfficieleAfdeling = new OfficieleAfdeling()
            });
            gwj.AfdelingsJaar.Add(new AfdelingsJaar
            {
                ID = 2,
                GeboorteJaarVan = 1997,
                GeboorteJaarTot = 1998,
                Geslacht = GeslachtsType.Gemengd,
                OfficieleAfdeling = new OfficieleAfdeling()
            });


            var target = Factory.Maak<LedenManager>();

            // Act

            var actual = target.InschrijvingVoorstellen(gp, gwj, false);

            // Assert

            Assert.AreEqual(actual.AfdelingsJaarIDs[0], 2);
        }

        /// <summary>
        /// InschrijvingVoorstellen moet weigeren een voorstel te doen voor kleuters.
        ///</summary>
        [TestMethod()]
        [ExpectedException(typeof(FoutNummerException))]
        public void InschrijvingVoorstellenTest2()
        {
            // Arrange
            // Ik maak een persoon, en een werkjaar met 2 afdelingen.  Maar dat is hier
            // niet relevant. Wat wel van belang is, is dat de persoon te jong is om
            // lid te worden. We verwachten dat het maken van een lidvoorstel crasht.

            var gwj = new GroepsWerkJaar { WerkJaar = 2012 };  // Werkjaar 2012-2013

            var gp = new GelieerdePersoon
            {
                Persoon =
                    new Persoon
                    {
                        GeboorteDatum = new DateTime(2010, 06, 21),  // Geboren in 2010
                        Geslacht = GeslachtsType.Vrouw,
                    }
            };

            gwj.AfdelingsJaar.Add(new AfdelingsJaar
            {
                ID = 1,
                GeboorteJaarVan = 1999,
                GeboorteJaarTot = 2000,
                Geslacht = GeslachtsType.Gemengd,
                OfficieleAfdeling = new OfficieleAfdeling()
            });

            gwj.AfdelingsJaar.Add(new AfdelingsJaar
            {
                ID = 2,
                GeboorteJaarVan = 1997,
                GeboorteJaarTot = 1998,
                Geslacht = GeslachtsType.Gemengd,
                OfficieleAfdeling = new OfficieleAfdeling()
            });

            var target = Factory.Maak<LedenManager>();

            // Act

            target.InschrijvingVoorstellen(gp, gwj, false);

            // Assert

            Assert.Fail();  // Als we hier zonder problemen geraken, is het niet OK
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

            var target = new LedenManager_Accessor(accessor);

            // Act
            target.LidMaken(gp, gwj, LidType.Kind, false);

            //Assert
            Assert.Fail();  // Als we hier komen zonder exception, dan is het mislukt.
        }
    }
}
