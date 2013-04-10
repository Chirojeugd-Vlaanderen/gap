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
using Chiro.Gap.Dummies;
using Chiro.Gap.Poco.Model;
using Chiro.Gap.ServiceContracts.FaultContracts;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Chiro.Gap.ServiceContracts;
using Chiro.Cdf.Ioc;
using Chiro.Gap.ServiceContracts.Mappers;
using Chiro.Gap.TestDbInfo;
using System.Security.Principal;
using System.Threading;
using Chiro.Gap.ServiceContracts.DataContracts;
using System;
using Chiro.Cdf.Poco;
using Chiro.Gap.Domain;
using Moq;
using Microsoft.VisualStudio.TestTools.UnitTesting.Web;
using Chiro.Gap.WorkerInterfaces;
using Chiro.Gap.Services;
using GebruikersRecht = Chiro.Gap.Poco.Model.GebruikersRecht;

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
        public void FunctieVerwijderenTest()
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
    }
}
