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
﻿using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
﻿using System.Web.UI.MobileControls;
﻿using Chiro.Gap.Dummies;
using Chiro.Gap.Poco.Model;
﻿using Chiro.Gap.ServiceContracts.FaultContracts;
﻿using Chiro.Gap.SyncInterfaces;
﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using Chiro.Gap.ServiceContracts;
using Chiro.Cdf.Ioc;
using Chiro.Gap.ServiceContracts.Mappers;
﻿using System.Security.Principal;
using System.Threading;
using Chiro.Gap.ServiceContracts.DataContracts;
using System;
using Chiro.Cdf.Poco;
using Chiro.Gap.Domain;
using Moq;
using Microsoft.VisualStudio.TestTools.UnitTesting.Web;
using Chiro.Gap.WorkerInterfaces;
﻿using GebruikersRecht = Chiro.Gap.Poco.Model.GebruikersRecht;
using Chiro.Gap.Services;

namespace Chiro.Gap.Services.Test
{
    /// <summary>
    /// Test op groepenservice
    /// </summary>
    /// <remarks>Blijkbaar heeft iemand mijn mocks voor de DAO weggehaald.  Dan testen we maar
    /// heel de flow.</remarks>
    [TestClass]
    public class GroepenServiceTest
    {
        public GroepenServiceTest()
        {
        }


        #region Additional test attributes

        //
        // You can use the following additional attributes as you write your tests:
        //
        // Use ClassInitialize to run code before running the first test in the class
        // [ClassInitialize]
        // public static void MyClassInitialize(TestContext testContext) { }
        //
        // Use ClassCleanup to run code after all tests in a class have run
        // [ClassCleanup()]
        // public static void MyClassCleanup() { }
        //
        // Use TestInitialize to run code before running each test 
        // [TestInitialize]
        // public void MyTestInitialize() { }
        //
        // Use TestCleanup to run code after each test has run
        // [TestCleanup()]
        // public void MyTestCleanup() { }
        //

        #endregion



        [ClassInitialize]
        public static void InitialiseerTests(TestContext tc)
        {
            Factory.ContainerInit();
            MappingHelper.MappingsDefinieren();
        }

        [ClassCleanup]
        public static void AfsluitenTests()
        {
        }


        /// <summary>
        /// Deze functie zorgt ervoor dat de PrincipalPermissionAttributes op de service methods
        /// geen excepties genereren, door te doen alsof de service aangeroepen is met de goede
        /// security
        /// </summary>
        [TestInitialize]
        public void VoorElkeTest()
        {
            var identity = new GenericIdentity(Properties.Settings.Default.TestUser);
            var roles = new[] {Properties.Settings.Default.TestSecurityGroep};
            var principal = new GenericPrincipal(identity, roles);
            Thread.CurrentPrincipal = principal;
        }


        /// <summary>
        /// Ophalen groepsinfo (zonder categorieën of afdelingen)
        /// </summary>
        [TestMethod]
        public void GroepOphalen()
        {
            // Flauwe test die groep met gegeven ID opvraagt, en dan gewoon
            // het ID checkt.

            #region Arrange

            var groep = new ChiroGroep{ID = 1};

            var dummyRepositoryProvider = new Mock<IRepositoryProvider>();
            dummyRepositoryProvider.Setup(src => src.RepositoryGet<Groep>())
                                   .Returns(new DummyRepo<Groep>(new List<Groep> {groep}));
            Factory.InstantieRegistreren(dummyRepositoryProvider.Object);

            IGroepenService svc = Factory.Maak<GroepenService>();

            #endregion

            #region Act

            GroepInfo g = svc.InfoOphalen(groep.ID);

            #endregion

            #region Assert

            Assert.IsTrue(g.ID == groep.ID);

            #endregion
        }

        /// <summary>
        /// Ophalen groepsinfo; controleert of categorieën en afdelingen meekomen.
        /// </summary>
        [TestMethod]
        public void GroepDetailOphalen()
        {
            #region Arrange

            var afdeling = new Afdeling {ID = 11};
            var categorie = new Categorie {ID = 21};
            var groepsWerkJaar = new GroepsWerkJaar {AfdelingsJaar = new[] {new AfdelingsJaar {Afdeling = afdeling}}};
            var groep = new ChiroGroep
                            {
                                ID = 1,
                                Categorie = new[] {categorie},
                                GroepsWerkJaar = new[] {groepsWerkJaar}
                            };
            groepsWerkJaar.Groep = groep;

            var dummyGroepenRepo = new DummyRepo<Groep>(new[] {groep});
            var repoProviderMock = new Mock<IRepositoryProvider>();
            repoProviderMock.Setup(src => src.RepositoryGet<Groep>()).Returns(dummyGroepenRepo);
            Factory.InstantieRegistreren(repoProviderMock.Object);

            IGroepenService svc = Factory.Maak<GroepenService>();

            #endregion

            #region Act

            GroepDetail g = svc.DetailOphalen(groep.ID);

            CategorieInfo catInfo = (from cat in g.Categorieen
                                     where cat.ID == categorie.ID
                                     select cat).FirstOrDefault();

            AfdelingsJaarDetail afdInfo = (from afd in g.Afdelingen
                                           where afd.AfdelingID == afdeling.ID
                                           select afd).FirstOrDefault();

            #endregion

            #region Assert

            Assert.IsTrue(g.ID == groep.ID);
            Assert.IsTrue(categorie != null);
            Assert.IsTrue(afdeling != null);

            #endregion
        }

        ///<summary>
        /// Kijkt na of er een foutmelding komt als een gebruiker probeert een functie bij te maken met een code
        /// die al bestaat voor een nationale functie (in dit geval 'CP')
        ///</summary>
        [TestMethod()]
        [ExpectedException(typeof (FaultException<BestaatAlFault<FunctieInfo>>))]
        public void FunctieToevoegenNationaleCode()
        {
            // Arrange.

            // De groep:
            int groepID = 123; // arbitraire groepID
            var groep = new ChiroGroep {ID = groepID};

            // De bestaande nationale functie:

            var bestaandeFunctie = new Functie
                                       {
                                           Code = "CP",
                                           IsNationaal = true
                                       };


            // Mocking opzetten. Maak een groepenrepository en een functiesrepository die enkel
            // opleveren wat we hierboven creeerden.

            var groepenRepositoryMock = new Mock<IRepository<Groep>>();
            groepenRepositoryMock.Setup(src => src.ByID(It.IsAny<int>())).Returns(groep);
            var functieRepositoryMock = new Mock<IRepository<Functie>>();
            functieRepositoryMock.Setup(src => src.Select()).Returns((new[] {bestaandeFunctie}).AsQueryable());

            // Vervolgens configureren we een repositoryprovider, die de gemockte repository oplevert.

            var repositoryProviderMock = new Mock<IRepositoryProvider>();
            repositoryProviderMock.Setup(src => src.RepositoryGet<Groep>()).Returns(groepenRepositoryMock.Object);
            repositoryProviderMock.Setup(src => src.RepositoryGet<Functie>()).Returns(functieRepositoryMock.Object);

            // Tenslotte configureren we de IOC-container om de gemockte repository provider 
            // te gebruiken.

            Factory.InstantieRegistreren(repositoryProviderMock.Object);

            // Nu kunnen we de service die we willen testen creeren:

            var target = Factory.Maak<GroepenService>();

            // Act.

            // gegevens van de nieuwe functie. Op de code na arbitrair
            string naam = "Championplukker"; // naam van nieuwe functie
            string code = bestaandeFunctie.Code; // code die toevallig gelijk is aan die van de bestaande functie
            Nullable<int> maxAantal = null; // geen max. aantal voor de functie
            int minAantal = 0; // ook geen min. aantal
            LidType lidType = LidType.Alles; // lidtype irrelevant voor deze functie
            Nullable<int> werkJaarVan = 2012; // in gebruik vanaf 2012-2013

            target.FunctieToevoegen(groepID, naam, code, maxAantal, minAantal, lidType, werkJaarVan);

            // Assert

            Assert.Fail(); // De bedoeling is dat we hier niet komen, maar dat een exception werd gethrowd.
        }


        /// <summary>
        ///Kijkt na of 'functiesOphalen' al minstens de nationale functies ophaalt.
        ///</summary>
        [TestMethod]
        public void FunctiesOphalenTest()
        {
            // groepswerkjaar
            var groepsWerkJaar = new GroepsWerkJaar
                                     {
                                         ID = 1,
                                         Groep = new ChiroGroep(),
                                         WerkJaar = 2012
                                     };

            // nationale functie
            var nationaleFunctie = new Functie
                                       {
                                           ID = 2,
                                           IsNationaal = true,
                                           Type = LidType.Alles,
                                           Niveau = Niveau.Alles
                                       };

            // zet repositoryprovider zodanig op dat ons testgroepswerkjaar wordt opgeleverd
            // door de GroepsWerkJarenRepository, en onze testnationalefunctie door de FunctiesRepository

            var gwjRepo = new DummyRepo<GroepsWerkJaar>(new[] {groepsWerkJaar});
            var funRepo = new DummyRepo<Functie>(new[] {nationaleFunctie});


            var repoProviderMock = new Mock<IRepositoryProvider>();
            repoProviderMock.Setup(src => src.RepositoryGet<GroepsWerkJaar>()).Returns(gwjRepo);
            repoProviderMock.Setup(src => src.RepositoryGet<Functie>()).Returns(funRepo);

            Factory.InstantieRegistreren(repoProviderMock.Object);

            var target = Factory.Maak<GroepenService>();

            // Haal alle functies op relevant voor het groepswerkjaar met ID 1.
            var actual = target.FunctiesOphalen(1, LidType.Alles);

            Assert.AreEqual(actual.First().ID, 2);
        }

        ///<summary>
        ///Controleert of een groepsnaam die begint met 'chiro ' bijgewerkt wordt bij het saven.
        ///</summary>
        [TestMethod()]
        public void BewarenGroepsNaamChiroTest()
        {
            // (Dit is een referentie-implementatie van een eenvoudige unit test voor de services)

            // ARRANGE

            // eenvoudig modelleke
            var groep = new ChiroGroep {ID = 1, Naam = "Blabla"};

            // repositoryprovider opzetten met dummy-groepenrepo
            var dummyGroepenRepo = new DummyRepo<Groep>(new[] {groep});
            var repoProviderMock = new Mock<IRepositoryProvider>();
            repoProviderMock.Setup(src => src.RepositoryGet<Groep>()).Returns(dummyGroepenRepo);
            Factory.InstantieRegistreren(repoProviderMock.Object);

            // te testen service construeren
            var target = Factory.Maak<GroepenService>();

            // ACT

            target.Bewaren(new GroepInfo {ID = 1, Naam = " Chiro  Blibli"});

            // ASSERT

            Assert.AreEqual(groep.Naam, "Blibli", true);
        }

        ///<summary>
        /// Controleert of een groepsnaam die begint met 'chiro' (zonder spatie daarna)
        /// correct wordt gesaved
        ///</summary>
        [TestMethod()]
        public void BewarenGroepsNaamChiroTest2()
        {
            // (Dit is een referentie-implementatie van een eenvoudige unit test voor de services)

            // ARRANGE

            // eenvoudig modelleke
            var groep = new ChiroGroep {ID = 1, Naam = "Blabla"};

            // repositoryprovider opzetten met dummy-groepenrepo
            var dummyGroepenRepo = new DummyRepo<Groep>(new[] {groep});
            var repoProviderMock = new Mock<IRepositoryProvider>();
            repoProviderMock.Setup(src => src.RepositoryGet<Groep>()).Returns(dummyGroepenRepo);
            Factory.InstantieRegistreren(repoProviderMock.Object);

            // te testen service construeren
            var target = Factory.Maak<GroepenService>();

            // ACT

            target.Bewaren(new GroepInfo {ID = 1, Naam = "Chirokidoki"});

            // ASSERT

            Assert.AreEqual(groep.Naam, "Chirokidoki", true);
        }


        /// <summary>
        ///Test of FunctieToevoegen wel echt een functie toevoegt. (Flauw)
        ///</summary>
        [TestMethod()]
        public void FunctieToevoegenTest()
        {
            // ARRANGE

            // eenvoudig modelletje met data
            var groep = new ChiroGroep
                            {
                                ID = 1,
                                Functie = new List<Functie>(),
                                GroepsWerkJaar = new[] {new GroepsWerkJaar()}
                            };

            // repositoryprovider registreren, die de groep oplevert
            var dummyGroepenRepo = new DummyRepo<Groep>(new[] {groep});
            var dummyFunctiesRepo = new DummyRepo<Functie>(new List<Functie>());

            var repoProviderMock = new Mock<IRepositoryProvider>();
            repoProviderMock.Setup(src => src.RepositoryGet<Groep>()).Returns(dummyGroepenRepo);
            repoProviderMock.Setup(src => src.RepositoryGet<Functie>()).Returns(dummyFunctiesRepo);
            Factory.InstantieRegistreren(repoProviderMock.Object);

            // de te testen groepenservice
            var target = Factory.Maak<GroepenService>();

            // ACT

            target.FunctieToevoegen(1, "MijnFunctie", "mfn", null, 0, LidType.Alles, null);

            // ASSERT

            Assert.AreEqual(groep.Functie.Count, 1);
            Assert.AreEqual(groep.Functie.First().Code, "mfn", true);
        }

        /// <summary>
        ///Verwacht faultexception bij verwijderen van functie dit jaar in gebruik.
        ///</summary>
        [TestMethod()]
        [ExpectedException(typeof (FaultException<BlokkerendeObjectenFault<PersoonLidInfo>>))]
        public void FunctieVerwijderenInGebruikTest()
        {
            // ARRANGE

            // testsituatie creeren
            var functie = new Functie {ID = 21};
            var groepswerkjaar = new GroepsWerkJaar
                                     {
                                         ID = 11,
                                         Groep =
                                             new ChiroGroep
                                                 {
                                                     ID = 1,
                                                     GroepsWerkJaar = new List<GroepsWerkJaar>()
                                                 }
                                     };
            groepswerkjaar.Groep.GroepsWerkJaar.Add(groepswerkjaar);
            functie.Groep = groepswerkjaar.Groep;
            var lid = new Leiding {Functie = new List<Functie>() {functie}, GroepsWerkJaar = groepswerkjaar};
            functie.Lid.Add(lid);

            // repository opzetten
            var dummyFunctiesRepo = new DummyRepo<Functie>(new List<Functie> {functie});
            var repoProviderMock = new Mock<IRepositoryProvider>();
            repoProviderMock.Setup(src => src.RepositoryGet<Functie>()).Returns(dummyFunctiesRepo);
            Factory.InstantieRegistreren(repoProviderMock.Object);

            var target = Factory.Maak<GroepenService>();

            // ACT

            target.FunctieVerwijderen(functie.ID, false);

            // ASSERT
            Assert.Fail(); // we moeten een faultexception gekregen hebben.
        }

        /// <summary>
        ///Als een functie vroeger gebruikt werd, moet verwijderen enkel het 'werkjaar-tot' zetten.
        ///</summary>
        [TestMethod()]
        public void FunctieVerwijderenOoitGebruiktTest()
        {
            // ARRANGE

            // testsituatie creeren
            // een lid uit vorig werkjaar heeft de te verwijderen functie
            var functie = new Functie {ID = 21};
            var groep = new ChiroGroep
                            {
                                ID = 1,
                                GroepsWerkJaar = new List<GroepsWerkJaar>()
                            };

            var groepswerkjaar = new GroepsWerkJaar
                                     {
                                         WerkJaar = 2012,
                                         ID = 12,
                                         Groep = groep
                                     };
            var oudGroepswerkjaar = new GroepsWerkJaar
                                        {
                                            WerkJaar = 2011,
                                            ID = 11,
                                            Groep = groep
                                        };

            groep.GroepsWerkJaar.Add(oudGroepswerkjaar);
            groep.GroepsWerkJaar.Add(groepswerkjaar);
            functie.Groep = groep;
            var lid = new Leiding {Functie = new List<Functie>() {functie}, GroepsWerkJaar = oudGroepswerkjaar};
            functie.Lid.Add(lid);

            // repository opzetten
            var dummyFunctiesRepo = new DummyRepo<Functie>(new List<Functie> {functie});
            var repoProviderMock = new Mock<IRepositoryProvider>();
            repoProviderMock.Setup(src => src.RepositoryGet<Functie>()).Returns(dummyFunctiesRepo);
            Factory.InstantieRegistreren(repoProviderMock.Object);

            var target = Factory.Maak<GroepenService>();

            // ACT
            target.FunctieVerwijderen(functie.ID, false);

            // ASSERT
            Assert.AreEqual(functie.WerkJaarTot, oudGroepswerkjaar.WerkJaar);
        }

        /// <summary>
        ///Controleert of 'MijnGroepenOphalen' rekening houdt met vervallen gebruikresrecht.
        ///</summary>
        [TestMethod()]
        public void MijnGroepenOphalenTest()
        {
            // ARRANGE

            // testgroep; toegang met mijnLogin net vervallen.
            const string mijnLogin = "MijnLogin";

            Groep groep = new ChiroGroep
            {
                GebruikersRecht = new[]
                                                        {
                                                            new GebruikersRecht
                                                                {
                                                                    Gav = new Gav {Login = "MijnLogin"},
                                                                    VervalDatum = DateTime.Today // net vervallen
                                                                }
                                                        }
            };

            // Zet mock op voor het opleveren van gebruikersnaam, en voor data-access groepen
            var authenticatieManagerMock = new Mock<IAuthenticatieManager>();
            authenticatieManagerMock.Setup(src => src.GebruikersNaamGet()).Returns(mijnLogin);
            Factory.InstantieRegistreren(authenticatieManagerMock.Object);

            var repositoryProviderMock = new Mock<IRepositoryProvider>();
            repositoryProviderMock.Setup(src => src.RepositoryGet<Groep>())
                                  .Returns(new DummyRepo<Groep>(new List<Groep> {groep}));
            Factory.InstantieRegistreren(repositoryProviderMock.Object);


            // ACT

            var target = Factory.Maak<GroepenService>();
            var actual = target.MijnGroepenOphalen();

            // ASSERT

            Assert.IsNull(actual.FirstOrDefault());
        }

        ///<summary>
        ///Controleert of de service verhindert dat een stamnummer wordt gewijzigd.
        ///</summary>
        [TestMethod()]
        public void BewarenStamNrTest()
        {
            // ARRANGE

            // testmodel
            bool gevangen = false;
            var groep = new ChiroGroep {Code = "BLA/0000", ID = 2};

            // mocking van data access
            var repositoryProvider = new Mock<IRepositoryProvider>();
            repositoryProvider.Setup(src => src.RepositoryGet<Groep>())
                              .Returns(new DummyRepo<Groep>(new List<Groep> {groep}));
            Factory.InstantieRegistreren(repositoryProvider.Object);


            // ACT
            var target = Factory.Maak<GroepenService>();

            var groepInfo = new GroepInfo
                                      {
                                          ID = groep.ID,
                                          StamNummer = "BLI/0001" // we proberen dit te wijzigen
                                      };

            try
            {
                target.Bewaren(groepInfo);
            }
            catch (FaultException<FoutNummerFault> ex)
            {
                // ASSERT

                Assert.AreEqual(ex.Detail.FoutNummer, FoutNummer.GeenGav);
                gevangen = true;
            }
            
            Assert.IsTrue(gevangen);
        }

        /// <summary>
        ///A test for JaarOvergangUitvoeren
        ///</summary>
        [TestMethod()]
        public void JaarovergangUitvoerenTest()
        {
            // ARRANGE

            // model

            var groep = new ChiroGroep();

            var groepsWerkJaar = new GroepsWerkJaar {Groep = groep, WerkJaar = 2011};
            groep.GroepsWerkJaar.Add(groepsWerkJaar);

            groep.Afdeling = new List<Afdeling>
                                 {
                                     new Afdeling {ID = 4, ChiroGroep = groep},
                                     new Afdeling {ID = 5, ChiroGroep = groep},
                                     new Afdeling {ID = 6, ChiroGroep = groep}
                                 };

            var officieleAfdelingen = new List<OfficieleAfdeling>
                                          {
                                              new OfficieleAfdeling {ID = 1},
                                              new OfficieleAfdeling {ID = 2},
                                              new OfficieleAfdeling {ID = 3}
                                          };

            var teActiveren = new List<AfdelingsJaarDetail>
                                  {
                                      new AfdelingsJaarDetail
                                          {
                                              AfdelingID = 4,
                                              GeboorteJaarVan = 1993,
                                              GeboorteJaarTot = 1994,
                                              OfficieleAfdelingID = 3,
                                              Geslacht = GeslachtsType.Gemengd
                                          },
                                      new AfdelingsJaarDetail
                                          {
                                              AfdelingID = 5,
                                              GeboorteJaarVan = 1994,
                                              GeboorteJaarTot = 1995,
                                              OfficieleAfdelingID = 2,
                                              Geslacht = GeslachtsType.Gemengd
                                          },
                                  };

            // dependency injection

            // We prutsen wat, zodat het volgende werkjaar altijd 2012 is.
            var groepsWerkJarenManagerMock = new Mock<IGroepsWerkJarenManager>();
            groepsWerkJarenManagerMock.Setup(src => src.NieuweWerkJaar(groep.ID)).Returns(2012);
            groepsWerkJarenManagerMock.Setup(src => src.OvergangMogelijk(It.IsAny<DateTime>(), It.IsAny<int>())).Returns(true);
            Factory.InstantieRegistreren(groepsWerkJarenManagerMock.Object);

            var dummyRepositoryProvider = new Mock<IRepositoryProvider>();
            dummyRepositoryProvider.Setup(src => src.RepositoryGet<Groep>())
                                   .Returns(new DummyRepo<Groep>(new List<Groep> {groep}));
            dummyRepositoryProvider.Setup(src => src.RepositoryGet<OfficieleAfdeling>())
                                   .Returns(new DummyRepo<OfficieleAfdeling>(officieleAfdelingen));

            Factory.InstantieRegistreren(dummyRepositoryProvider.Object);

            // ACT

            var target = Factory.Maak<GroepenService>();
            target.JaarOvergangUitvoeren(teActiveren, groep.ID);

            // ASSERT

            var nieuwGwj = (from gwj in groep.GroepsWerkJaar
                            orderby gwj.WerkJaar descending
                            select gwj).First();

            Assert.AreEqual(nieuwGwj.WerkJaar, 2012);
            Assert.AreEqual(nieuwGwj.AfdelingsJaar.Count, 2);
        }

        /// <summary>
        ///Controleert of JaarOvergangUitvoeren rekening houdt met de minimumleeftijd
        /// bij de afdelingsindeling.
        ///</summary>
        [TestMethod()]
        public void JaarovergangUitvoerenMinimumLeeftijdTest()
        {
            // ARRANGE

            // model

            bool gedetecteerd = false;
            var groep = new ChiroGroep();

            var groepsWerkJaar = new GroepsWerkJaar { Groep = groep, WerkJaar = 2011 };
            groep.GroepsWerkJaar.Add(groepsWerkJaar);

            groep.Afdeling = new List<Afdeling>
                                 {
                                     new Afdeling {ID = 4, ChiroGroep = groep},
                                     new Afdeling {ID = 5, ChiroGroep = groep},
                                     new Afdeling {ID = 6, ChiroGroep = groep}
                                 };

            var officieleAfdelingen = new List<OfficieleAfdeling>
                                          {
                                              new OfficieleAfdeling {ID = 1},
                                              new OfficieleAfdeling {ID = 2},
                                              new OfficieleAfdeling {ID = 3}
                                          };

            var teActiveren = new List<AfdelingsJaarDetail>
                                  {
                                      new AfdelingsJaarDetail
                                          {
                                              AfdelingID = 4,
                                              GeboorteJaarVan = 1993,
                                              GeboorteJaarTot = 1994,
                                              OfficieleAfdelingID = 3,
                                              Geslacht = GeslachtsType.Gemengd
                                          },
                                      new AfdelingsJaarDetail
                                          {
                                              AfdelingID = 5,
                                              GeboorteJaarVan = 1994,
                                              GeboorteJaarTot = 2007,   // kleuter in 2012
                                              OfficieleAfdelingID = 2,
                                              Geslacht = GeslachtsType.Gemengd
                                          },
                                  };

            // dependency injection

            // We prutsen wat, zodat het volgende werkjaar altijd 2012 is.
            var groepsWerkJarenManagerMock = new Mock<IGroepsWerkJarenManager>();
            groepsWerkJarenManagerMock.Setup(src => src.NieuweWerkJaar(groep.ID)).Returns(2012);
            groepsWerkJarenManagerMock.Setup(src => src.OvergangMogelijk(It.IsAny<DateTime>(), It.IsAny<int>())).Returns(true);
            Factory.InstantieRegistreren(groepsWerkJarenManagerMock.Object);

            // Data access faken
            var dummyRepositoryProvider = new Mock<IRepositoryProvider>();
            dummyRepositoryProvider.Setup(src => src.RepositoryGet<Groep>())
                                   .Returns(new DummyRepo<Groep>(new List<Groep> { groep }));
            dummyRepositoryProvider.Setup(src => src.RepositoryGet<OfficieleAfdeling>())
                                   .Returns(new DummyRepo<OfficieleAfdeling>(officieleAfdelingen));

            Factory.InstantieRegistreren(dummyRepositoryProvider.Object);

            // ACT

            var target = Factory.Maak<GroepenService>();
            try
            {
                target.JaarOvergangUitvoeren(teActiveren, groep.ID);
            }
            catch (FaultException<FoutNummerFault> ex)
            {
                gedetecteerd = ex.Detail.FoutNummer == FoutNummer.OngeldigeGeboorteJarenVoorAfdeling;
            }
            

            // ASSERT

            var nieuwGwj = (from gwj in groep.GroepsWerkJaar
                            orderby gwj.WerkJaar descending
                            select gwj).First();

            Assert.IsTrue(gedetecteerd);
        }

        ///<summary>
        /// Controleert of AfdelingsJaarBewaren rekening houdt met de minimumleeftijd.
        ///</summary>
        [TestMethod()]
        public void AfdelingsJaarBewarenMinimumLeeftijdTest()
        {
            // ARRANGE
            
            // model om op te testen.
            
            var groep = new ChiroGroep();
            var groepsWerkJaar = new GroepsWerkJaar {Groep = groep, WerkJaar = 2012};
            groep.GroepsWerkJaar.Add(groepsWerkJaar);
            var afdelingsJaar = new AfdelingsJaar
                                    {
                                        Afdeling = new Afdeling {ID = 2, ChiroGroep = groep},
                                        OfficieleAfdeling = new OfficieleAfdeling {ID = 3},
                                        GroepsWerkJaar = groepsWerkJaar,
                                        GeboorteJaarTot = 2006,
                                        GeboorteJaarVan = 2004,
                                        ID = 1
                                    };

            // dependency injection opzetten voor data access

            var repositoryProviderMock = new Mock<IRepositoryProvider>();
            repositoryProviderMock.Setup(src => src.RepositoryGet<AfdelingsJaar>())
                                  .Returns(new DummyRepo<AfdelingsJaar>(new List<AfdelingsJaar> {afdelingsJaar}));
            repositoryProviderMock.Setup(src => src.RepositoryGet<Afdeling>())
                                  .Returns(new DummyRepo<Afdeling>(new List<Afdeling> {afdelingsJaar.Afdeling}));
            repositoryProviderMock.Setup(src => src.RepositoryGet<OfficieleAfdeling>())
                                  .Returns(
                                      new DummyRepo<OfficieleAfdeling>(new List<OfficieleAfdeling>
                                                                           {
                                                                               afdelingsJaar
                                                                                   .OfficieleAfdeling
                                                                           }));

            Factory.InstantieRegistreren(repositoryProviderMock.Object);

            // ACT

            bool gedetecteerd = false;

            var target = Factory.Maak<GroepenService>();
            try
            {
                target.AfdelingsJaarBewaren(new AfdelingsJaarDetail
                {
                    AfdelingsJaarID = afdelingsJaar.ID,
                    GeboorteJaarTot = 2007,
                    GeboorteJaarVan = 2004,
                    AfdelingID = afdelingsJaar.Afdeling.ID,
                    Geslacht = GeslachtsType.Gemengd
                });
            }
            catch (FaultException<FoutNummerFault> ex)
            {
                gedetecteerd = ex.Detail.FoutNummer == FoutNummer.OngeldigeGeboorteJarenVoorAfdeling;
            }
            
            // ASSERT

            Assert.IsTrue(gedetecteerd);
        }

        ///<summary>
        ///OngebruikteAfdelingenOphalen mag enkel de afdelingen ophalen die in het
        /// gevraagde werkjaar ongebruikt zijn.
        ///</summary>
        [TestMethod()]
        public void OngebruikteAfdelingenOphalenTest()
        {
            // ARRANGE

            // het model

            var groep = new ChiroGroep();
            var vorigWerkJaar = new GroepsWerkJaar {Groep = groep, WerkJaar = 2011, ID = 1};
            var ditWerkJaar = new GroepsWerkJaar {Groep = groep, WerkJaar = 2012, ID = 2};

            var afdeling = new Afdeling {ChiroGroep = groep, ID = 3};
            groep.Afdeling.Add(afdeling);
            var vorigAfdelingsJaar = new AfdelingsJaar {GroepsWerkJaar = vorigWerkJaar, Afdeling = afdeling};
            afdeling.AfdelingsJaar.Add(vorigAfdelingsJaar);

            // dependency injection voor data acces

            var repositoryProviderMock = new Mock<IRepositoryProvider>();
            repositoryProviderMock.Setup(src => src.RepositoryGet<GroepsWerkJaar>())
                                  .Returns(
                                      new DummyRepo<GroepsWerkJaar>(new List<GroepsWerkJaar>
                                                                        {
                                                                            vorigWerkJaar,
                                                                            ditWerkJaar
                                                                        }));
            repositoryProviderMock.Setup(src => src.RepositoryGet<Afdeling>())
                                  .Returns(new DummyRepo<Afdeling>(new List<Afdeling> {afdeling}));

            Factory.InstantieRegistreren(repositoryProviderMock.Object);

            var target = Factory.Maak<GroepenService>();

            var actual = target.OngebruikteAfdelingenOphalen(ditWerkJaar.ID);
            Assert.AreEqual(actual.Count, 1);
            Assert.AreEqual(actual.First().ID, afdeling.ID);
        }

        /// <summary>
        ///Controleer of AlleAfdelingenOphalen ook inactieve afdelingen ophaalt.
        ///</summary>
        [TestMethod()]
        public void AlleAfdelingenOphalenTest()
        {
            // ARRANGE

            // het model

            var groep = new ChiroGroep();
            var ditWerkJaar = new GroepsWerkJaar { Groep = groep, WerkJaar = 2012, ID = 2 }; // we hebben een werkjaar
            groep.GroepsWerkJaar.Add(ditWerkJaar);

            var afdeling = new Afdeling { ChiroGroep = groep, ID = 3 };
            groep.Afdeling.Add(afdeling); // en een afdeling, die inactief is

            // dependency injection voor data acces

            var repositoryProviderMock = new Mock<IRepositoryProvider>();
            repositoryProviderMock.Setup(src => src.RepositoryGet<Groep>())
                                  .Returns(new DummyRepo<Groep>(new List<Groep> { groep }));

            Factory.InstantieRegistreren(repositoryProviderMock.Object);

            var target = Factory.Maak<GroepenService>();

            var actual = target.AlleAfdelingenOphalen(groep.ID);
            Assert.AreEqual(actual.Count, 1);
            Assert.AreEqual(actual.First().ID, afdeling.ID);
        }

        /// <summary>
        ///Kijkt na of uitgeschreven leden zonder adres niet als fout worden aangegeven
        ///</summary>
        [TestMethod()]
        public void LedenControlerenZonderAdresEnUitgeschrevenTest()
        {
            // ARRANGE

            // model: uitgeschreven lid zonder adres

            var groep = new ChiroGroep {ID = 1};
            var groepsWerkJaar = new GroepsWerkJaar {Groep = groep};
            groep.GroepsWerkJaar.Add(groepsWerkJaar);

            var lid = new Lid
                          {
                              GelieerdePersoon =
                                  new GelieerdePersoon {Groep = groep, PersoonsAdres = null},
                              UitschrijfDatum = DateTime.Today.AddDays(-1),
                              GroepsWerkJaar = groepsWerkJaar
                          };
            groepsWerkJaar.Lid.Add(lid);

            // dependency injection voor data access
            var repositoryProviderMock = new Mock<IRepositoryProvider>();
            repositoryProviderMock.Setup(src => src.RepositoryGet<Groep>())
                                  .Returns(new DummyRepo<Groep>(new List<Groep> { groep }));
            repositoryProviderMock.Setup(src => src.RepositoryGet<Lid>())
                                  .Returns(new DummyRepo<Lid>(new List<Lid> {lid}));

            Factory.InstantieRegistreren(repositoryProviderMock.Object);

            // ACT
            var target = Factory.Maak<GroepenService>();
            var actual = target.LedenControleren(groep.ID);

            // ASSERT
            var aantalLedenZonderAdres = (from probleem in actual
                                          where probleem.Probleem == LidProbleem.AdresOntbreekt
                                          select probleem);
            Assert.IsFalse(aantalLedenZonderAdres.Any());
        }

        /// <summary>
        ///Kijkt na of uitgeschreven leden zonder telefoonnummer niet als fout worden aangegeven
        ///</summary>
        [TestMethod()]
        public void LedenControlerenZonderTelNrEnUitgeschrevenTest()
        {
            // ARRANGE

            // model: uitgeschreven lid zonder adres

            var groep = new ChiroGroep { ID = 1 };
            var groepsWerkJaar = new GroepsWerkJaar { Groep = groep };
            groep.GroepsWerkJaar.Add(groepsWerkJaar);

            var lid = new Lid
            {
                GelieerdePersoon =
                    new GelieerdePersoon { Groep = groep, PersoonsAdres = null },
                UitschrijfDatum = DateTime.Today.AddDays(-1),
                GroepsWerkJaar = groepsWerkJaar
            };
            groepsWerkJaar.Lid.Add(lid);

            // dependency injection voor data access
            var repositoryProviderMock = new Mock<IRepositoryProvider>();
            repositoryProviderMock.Setup(src => src.RepositoryGet<Groep>())
                                  .Returns(new DummyRepo<Groep>(new List<Groep> { groep }));
            repositoryProviderMock.Setup(src => src.RepositoryGet<Lid>())
                                  .Returns(new DummyRepo<Lid>(new List<Lid> { lid }));

            Factory.InstantieRegistreren(repositoryProviderMock.Object);

            // ACT
            var target = Factory.Maak<GroepenService>();
            var actual = target.LedenControleren(groep.ID);

            // ASSERT
            var aantalLedenZonderTelNr = (from probleem in actual
                                          where probleem.Probleem == LidProbleem.TelefoonNummerOntbreekt
                                          select probleem);
            Assert.IsFalse(aantalLedenZonderTelNr.Any());
        }

        /// <summary>
        ///Kijkt na of uitgeschreven leden zonder e-mailadres niet als fout worden aangegeven
        ///</summary>
        [TestMethod()]
        public void LeidingControlerenZonderEmailEnUitgeschrevenTest()
        {
            // ARRANGE

            // model: uitgeschreven lid zonder adres

            var groep = new ChiroGroep { ID = 1 };
            var groepsWerkJaar = new GroepsWerkJaar { Groep = groep };
            groep.GroepsWerkJaar.Add(groepsWerkJaar);

            var lid = new Lid
            {
                GelieerdePersoon =
                    new GelieerdePersoon { Groep = groep, PersoonsAdres = null },
                UitschrijfDatum = DateTime.Today.AddDays(-1),
                GroepsWerkJaar = groepsWerkJaar
            };
            groepsWerkJaar.Lid.Add(lid);

            // dependency injection voor data access
            var repositoryProviderMock = new Mock<IRepositoryProvider>();
            repositoryProviderMock.Setup(src => src.RepositoryGet<Groep>())
                                  .Returns(new DummyRepo<Groep>(new List<Groep> { groep }));
            repositoryProviderMock.Setup(src => src.RepositoryGet<Lid>())
                                  .Returns(new DummyRepo<Lid>(new List<Lid> { lid }));

            Factory.InstantieRegistreren(repositoryProviderMock.Object);

            // ACT
            var target = Factory.Maak<GroepenService>();
            var actual = target.LedenControleren(groep.ID);

            // ASSERT
            var aantalLedenZonderTelNr = (from probleem in actual
                                          where probleem.Probleem == LidProbleem.EmailOntbreekt
                                          select probleem);
            Assert.IsFalse(aantalLedenZonderTelNr.Any());
        }


        /// <summary>
        /// Als een functie die enkel dit jaar wordt gebruikt (geforceerd) wordt verwijderd,
        /// moet die helemaal verdwijnen (i.e. niet enkel een stopdatum krijgen)
        /// (test ter vervanging van Workers.Test.FunctieEnkelDitJaarInGebruikGeforceerdVerwijderenTest
        /// </summary>
        [TestMethod()]
        public void FunctieEnkelDitJaarInGebruikGeforceerdVerwijderenTest()
        {
            // ARRANGE

            // datamodel
            var groep = new ChiroGroep();
            var groepsWerkJaar = new GroepsWerkJaar { Groep = groep };
            groep.GroepsWerkJaar.Add(groepsWerkJaar);
            var functie = new Functie { Groep = groep, ID = 1 };
            groep.Functie.Add(functie);
            var leider = new Leiding { GroepsWerkJaar = groepsWerkJaar };
            leider.Functie.Add(functie);
            functie.Lid.Add(leider);

            var alleFuncties = new List<Functie> {functie};

            // ioc
            var repositoryProviderMock = new Mock<IRepositoryProvider>();
            repositoryProviderMock.Setup(src => src.RepositoryGet<Functie>())
                                  .Returns(new DummyRepo<Functie>(alleFuncties));
            Factory.InstantieRegistreren(repositoryProviderMock.Object);

            // ACT

            var target = Factory.Maak<GroepenService>();
            target.FunctieVerwijderen(functie.ID, true);

            // ASSERT

            Assert.IsFalse(alleFuncties.Contains(functie));
        }

        ///<summary>
        /// Controleer of de groepswerkjaarcache wordt gecleard na een jaarovergang.
        ///</summary>
        [TestMethod()]
        public void JaarOvergangUitvoerenCacheClearTest()
        {
            // ARRANGE

            // testdata

            var groep = new KaderGroep {ID = 1};   // geen gedoe met afdelingen
            groep.GroepsWerkJaar.Add(new GroepsWerkJaar {WerkJaar = 2011});

            // invesion of control

            var repositoryProvider = new Mock<IRepositoryProvider>();
            repositoryProvider.Setup(src => src.RepositoryGet<Groep>())
                              .Returns(new DummyRepo<Groep>(new List<Groep> {groep}));
            Factory.InstantieRegistreren(repositoryProvider.Object);

            var veelGebruiktMock = new Mock<IVeelGebruikt>();
            veelGebruiktMock.Setup(vgb => vgb.WerkJaarInvalideren(groep)).Verifiable();
            Factory.InstantieRegistreren(veelGebruiktMock.Object);

            // ACT

            var target = Factory.Maak<GroepenService>();
            target.JaarOvergangUitvoeren(new List<AfdelingsJaarDetail>(), groep.ID);

            // ASSERT
            
            veelGebruiktMock.Verify(vgb => vgb.WerkJaarInvalideren(groep), Times.Once());
        }

        /// <summary>
        /// Controleert of het bewaren van een groep die gegevens synct met kipadmin.
        /// </summary>
        [TestMethod()]
        public void BewarenTest()
        {
            // ARRANGE

            var groep = new ChiroGroep {ID = 1, Naam = "Chiro Jos"};
            var groepInfo = new GroepInfo {ID = 1, Naam = "Chiro Jim"};

            // dependency injection voor synchronisatie

            var groepenSyncMock = new Mock<IGroepenSync>();
            groepenSyncMock.Setup(src => src.Bewaren(It.IsAny<Groep>())).Verifiable();
            Factory.InstantieRegistreren(groepenSyncMock.Object);

            // dependency injection voor data access

            var repositoryProviderMock = new Mock<IRepositoryProvider>();
            repositoryProviderMock.Setup(src => src.RepositoryGet<Groep>())
                                  .Returns(new DummyRepo<Groep>(new List<Groep> {groep}));
            Factory.InstantieRegistreren(repositoryProviderMock.Object);

            // ACT

            var target = Factory.Maak<GroepenService>();
            target.Bewaren(groepInfo);

            // ASSERT

            groepenSyncMock.Verify(src=>src.Bewaren(It.IsAny<Groep>()), Times.Once());
        }

        /// <summary>
        /// Een test op het verwijderen van een eigen functie zonder speciallekes
        /// </summary>
        [TestMethod()]
        public void FunctieVerwijderenGewoonTest()
        {
            // ARRANGE

            var functie = new Functie
                              {
                                  ID = 1,
                                  Groep =
                                      new ChiroGroep
                                          {
                                              GroepsWerkJaar =
                                                  new List<GroepsWerkJaar> {new GroepsWerkJaar()}
                                          },
                                  IsNationaal = false
                              };
            var alleFuncties = new List<Functie> {functie};

            var repositoryProviderMock = new Mock<IRepositoryProvider>();
            repositoryProviderMock.Setup(src => src.RepositoryGet<Functie>())
                                  .Returns(new DummyRepo<Functie>(alleFuncties));
            Factory.InstantieRegistreren(repositoryProviderMock.Object);

            // ACT

            var target = Factory.Maak<GroepenService>();
            target.FunctieVerwijderen(functie.ID, false);

            // ASSERT

            Assert.IsNull(alleFuncties.FirstOrDefault());
        }

        /// <summary>
        /// A test for FunctiesControleren
        /// </summary>
        [TestMethod()]
        public void FunctiesControlerenTest()
        {
            // ARRANGE

            var groepsWerkJaar = new GroepsWerkJaar {Groep = new ChiroGroep()};
            groepsWerkJaar.Groep.GroepsWerkJaar.Add(groepsWerkJaar);
            var verplichteFunctie = new Functie {Code = "bla", IsNationaal = true, MinAantal = 1};

            // een groep met groepswerkjaar zonder leden. We verwachten dat er gemeld wordt dat 
            // de verplichte functie niet is ingevuld.

            var repositoryProviderMock = new Mock<IRepositoryProvider>();
            repositoryProviderMock.Setup(src => src.RepositoryGet<Functie>())
                .Returns(new DummyRepo<Functie>(new List<Functie> {verplichteFunctie}));
            repositoryProviderMock.Setup(src => src.RepositoryGet<Groep>())
                .Returns(new DummyRepo<Groep>(new List<Groep> {groepsWerkJaar.Groep}));
            Factory.InstantieRegistreren(repositoryProviderMock.Object);

            // ACT

            var target = Factory.Maak<GroepenService>();
            var actual = target.FunctiesControleren(groepsWerkJaar.Groep.ID);

            // ASSERT

            Assert.IsNotNull(actual.Select(prob => prob.Code = verplichteFunctie.Code).FirstOrDefault());
        }

        /// <summary>
        /// Controleert of het niveau van een functie goed bewaard wordt door FunctieBewerken.
        /// </summary>
        [TestMethod()]
        public void FunctieBewerkenNiveauTest()
        {
            // ARRANGE

            var functie = new Functie
            {
                ID = 1,
                Groep =
                    new ChiroGroep
                    {
                        GroepsWerkJaar =
                            new List<GroepsWerkJaar> { new GroepsWerkJaar() }
                    },
                IsNationaal = false,
                Niveau = Niveau.LeidingInGroep
            };
            var alleFuncties = new List<Functie> { functie };

            var repositoryProviderMock = new Mock<IRepositoryProvider>();
            repositoryProviderMock.Setup(src => src.RepositoryGet<Functie>())
                                  .Returns(new DummyRepo<Functie>(alleFuncties));
            Factory.InstantieRegistreren(repositoryProviderMock.Object);

            // ACT

            var target = Factory.Maak<GroepenService>();
            target.FunctieBewerken(new FunctieDetail {ID = functie.ID, IsNationaal = functie.IsNationaal, Type = LidType.Kind});

            // ASSERT

            Assert.AreEqual(Niveau.LidInGroep, functie.Niveau);
        }

        /// <summary>
        /// Test of GroepenService.FunctiesBewerken controleert op dubbele namen of codes
        /// </summary>
        [TestMethod()]
        [ExpectedException(typeof(FaultException<BestaatAlFault<FunctieInfo>>))]
        public void FunctieBewerkenCheckDubbelTest()
        {
            // ARRANGE

            var functie = new Functie
            {
                ID = 1,
                Groep =
                    new ChiroGroep
                    {
                        GroepsWerkJaar =
                            new List<GroepsWerkJaar> { new GroepsWerkJaar() }
                    },
                IsNationaal = false,
                Niveau = Niveau.LeidingInGroep,
                Code = "MF",
                Naam = "Mijn Functie"
            };
            var nationaleFunctie = new Functie
                                       {
                                           ID = 2,
                                           IsNationaal = true,
                                           Niveau = Niveau.LeidingInGroep,
                                           Code = "NF",
                                           Naam = "Nationale Functie"
                                       };

            var alleFuncties = new List<Functie> { functie, nationaleFunctie };

            var repositoryProviderMock = new Mock<IRepositoryProvider>();
            repositoryProviderMock.Setup(src => src.RepositoryGet<Functie>())
                                  .Returns(new DummyRepo<Functie>(alleFuncties));
            Factory.InstantieRegistreren(repositoryProviderMock.Object);

            // ACT

            var target = Factory.Maak<GroepenService>();
            target.FunctieBewerken(new FunctieDetail
                                       {
                                           ID = functie.ID,
                                           IsNationaal = functie.IsNationaal,
                                           Type = LidType.Leiding,
                                           Code = nationaleFunctie.Code
                                       });

            // ASSERT

            // niks te assert. Hopelijk hebben we een exception gecatcht.
        }

        /// <summary>
        /// OngebruikteAfdelingenOphalen moet een lege lijst opleveren voor een kadergroep.
        /// </summary>
        [TestMethod()]
        public void OngebruikteAfdelingenOphalenKaderGroepTest()
        {
            // ARRANGE

            // het model

            var groep = new KaderGroep();
            var ditWerkJaar = new GroepsWerkJaar { Groep = groep, WerkJaar = 2012, ID = 2 };

            // dependency injection voor data acces

            var repositoryProviderMock = new Mock<IRepositoryProvider>();
            repositoryProviderMock.Setup(src => src.RepositoryGet<GroepsWerkJaar>())
                                  .Returns(
                                      new DummyRepo<GroepsWerkJaar>(new List<GroepsWerkJaar>
                                                                        {
                                                                            ditWerkJaar
                                                                        }));

            Factory.InstantieRegistreren(repositoryProviderMock.Object);

            var target = Factory.Maak<GroepenService>();

            var actual = target.OngebruikteAfdelingenOphalen(ditWerkJaar.ID);
            Assert.AreEqual(actual.Count, 0);
        }
    }
}
