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
// <summary>
//   Dit is een testclass voor Unit Tests van GelieerdePersonenServiceTest
// </summary>

using System;
using System.Collections.Generic;
using System.Data.Objects.DataClasses;
using System.Linq;
using Chiro.Cdf.Ioc;
using Chiro.Cdf.Poco;
using Chiro.Gap.Domain;
using Chiro.Gap.Dummies;
using Chiro.Gap.Poco.Model;
using Chiro.Gap.ServiceContracts.DataContracts;
using Chiro.Gap.ServiceContracts.Mappers;
using Chiro.Gap.SyncInterfaces;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using Moq;
using System.ServiceModel;
using Chiro.Gap.ServiceContracts.FaultContracts;
using Chiro.Gap.TestDbInfo;
using Chiro.Gap.WorkerInterfaces;
using Chiro.Gap.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting.Web;

namespace Chiro.Gap.Services.Test
{
    /// <summary>
    /// Dit is een testclass voor Unit Tests van GelieerdePersonenServiceTest
    /// </summary>
    [TestClass]
    public class GelieerdePersonenServiceTest
    {
        private TestContext testContextInstance;

        /// <summary>
        /// Gets or sets the test context which provides
        /// information about and functionality for the current test run.
        /// </summary>
        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }
            set
            {
                testContextInstance = value;
            }
        }

        /// <summary>
        /// Run code before running first test
        /// </summary>
        [ClassInitialize]
        static public void MyClassInitialize(TestContext context)
        {
            MappingHelper.MappingsDefinieren();
        }

        /// <summary>
        /// Run code before running each test
        /// </summary>
        [TestInitialize]
        public void MyTestInitialize()
        {
            // Restore IoC settings

            // (Ik had hier ook alle nodige mocks kunnen maken, en dan
            // een voor een registreren.  Maar ContainerInit herleest gewoon
            // de configuratiefile.)

            Factory.ContainerInit();
        }

        /// <summary>
        /// Probeer een communicatievorm toe te voegen die niet valideert.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(FaultException<FoutNummerFault>))]
        public void CommunicatieVormToevoegenTestOngeldig()
        {
            // ARRANGE

            // testdata
            var gelieerdePersoon = new GelieerdePersoon {ID = 1, Persoon = new Persoon()};
            var communicatieType = new CommunicatieType {ID = 2, Validatie = "^[0-9]*$"};

            // dependency injection
            var repositoryProviderMock = new Mock<IRepositoryProvider>();
            repositoryProviderMock.Setup(src => src.RepositoryGet<GelieerdePersoon>())
                                  .Returns(new DummyRepo<GelieerdePersoon>(new List<GelieerdePersoon> {gelieerdePersoon}));
            repositoryProviderMock.Setup(src => src.RepositoryGet<CommunicatieType>())
                                  .Returns(new DummyRepo<CommunicatieType>(new List<CommunicatieType> {communicatieType}));
            Factory.InstantieRegistreren(repositoryProviderMock.Object);

            // ACT

            var target = Factory.Maak<GelieerdePersonenService>();

            var commInfo = new CommunicatieDetail()
            {
                CommunicatieTypeID = communicatieType.ID,
                Nummer = "BLA"
            };

            // ASSERT

            target.CommunicatieVormToevoegen(gelieerdePersoon.ID, commInfo);
            Assert.IsTrue(false);
        }

        ///<summary>
        ///Toevoegen van een geldig telefoonnr
        /// </summary>
        [TestMethod]
        public void CommunicatieVormToevoegenTestGeldig()
        {
            // ARRANGE

            // modelletje

            var gelieerdePersoon = new GelieerdePersoon {ID = 1, Persoon = new Persoon()};
            var telefoonNr = new CommunicatieType
                                 {
                                     ID = 1,
                                     IsOptIn = false,
                                     Omschrijving = "Telefoonnummer",
                                     Validatie =
                                         @"(^0[0-9]{1,2}\-[0-9]{2,3}\s?[0-9]{2}\s?[0-9]{2}$|^04[0-9]{2}\-[0-9]{2,3}\s?[0-9]{2}\s?[0-9]{2}$)|^\+[0-9]*$"
                                 };

            // mocking opzetten

            var repositoryProviderMock = new Mock<IRepositoryProvider>();
            repositoryProviderMock.Setup(src => src.RepositoryGet<GelieerdePersoon>())
                                  .Returns(new DummyRepo<GelieerdePersoon>(new List<GelieerdePersoon> {gelieerdePersoon}));
            repositoryProviderMock.Setup(src => src.RepositoryGet<CommunicatieType>())
                                  .Returns(new DummyRepo<CommunicatieType>(new List<CommunicatieType> {telefoonNr}));

            Factory.InstantieRegistreren(repositoryProviderMock.Object);

            // ACT

            var target = Factory.Maak<GelieerdePersonenService>();

            var commInfo = new CommunicatieDetail()
            {
                CommunicatieTypeID = telefoonNr.ID,
                Voorkeur = true,
                Nummer = "03-484 53 32" // geldig nummer
            };

            target.CommunicatieVormToevoegen(gelieerdePersoon.ID, commInfo);

            // ASSERT

            Assert.IsTrue(true);	// al blij als er geen exception optreedt
        }

        /// <summary>
        /// Kijkt na of CommunicatieVormToevoegen enkel de nieuwe communicatievorm
        /// naar Kipadmin synct, ipv alle communicatie in kipadmin te vervangen  
        /// </summary>
        [TestMethod]
        public void CommunicatieVormToevoegenTest()
        {
            // ARRANGE

            var gelieerdePersoon = new GelieerdePersoon {ID = 1, Persoon = new Persoon {AdInAanvraag = true}};
            var email = new CommunicatieType {ID = 3, Validatie = ".*"};

            var commDetail = new CommunicatieInfo
            {
                CommunicatieTypeID = email.ID,    // e-mail
                ID = 4,    // verschillend van nul, in een poging de communicatiemanager te misleiden.
                Nummer = "johan@linux.be"  // arbitrair
            };

            // dependency injection voor synchronisatie:
            // verwacht dat CommunicatieSync.Toevoegen wordt aangeroepen.

            var communicatieSyncMock = new Mock<ICommunicatieSync>();
            communicatieSyncMock.Setup(snc => snc.Toevoegen(It.IsAny<CommunicatieVorm>())).Verifiable();

            Factory.InstantieRegistreren(communicatieSyncMock.Object);

            // dependency injection voor data access

            var repositoryProviderMock = new Mock<IRepositoryProvider>();
            repositoryProviderMock.Setup(src => src.RepositoryGet<GelieerdePersoon>())
                                  .Returns(new DummyRepo<GelieerdePersoon>(new List<GelieerdePersoon> {gelieerdePersoon}));
            repositoryProviderMock.Setup(src => src.RepositoryGet<CommunicatieType>())
                                  .Returns(new DummyRepo<CommunicatieType>(new List<CommunicatieType> {email}));
            Factory.InstantieRegistreren(repositoryProviderMock.Object);

            // ACT

            var target = Factory.Maak<GelieerdePersonenService>();
            target.CommunicatieVormToevoegen(gelieerdePersoon.ID, commDetail);

            // ASSERT
            communicatieSyncMock.Verify(snc => snc.Toevoegen(It.IsAny<CommunicatieVorm>()), Times.Once());
        }

        /// <summary>
        /// Controleren of 'communicatievormaanpassen' wel degelijk een communicatievorm aanpast.
        /// </summary>
        [TestMethod]
        public void CommunicatieVormAanpassenTest()
        {
            // ARRANGE

            // testmodelletje

            const int TESTGPID = 1234;      // arbitrair ID van een gelieerde persoon
            const int TESTCVID = 2345;      // en van een communicatievorm
            const int TESTCTID = 3;         // en diens communicatietype

            var testCommunicatieType = new Poco.Model.CommunicatieType { ID = TESTCTID, Validatie = ".*" };
            var testCommunicatieVorm = new CommunicatieVorm
                                           {
                                               ID = TESTCVID,
                                               CommunicatieType = testCommunicatieType,
                                               Nummer = "jos@linux.be"
                                           };

            // Koppel gauw testCommunicatieVorm aan testGelieerdePersoon

            var testGelieerdePersoon = new GelieerdePersoon
                                           {
                                               ID = TESTGPID,
                                               Persoon = new Poco.Model.Persoon(),
                                               Communicatie =
                                                   new EntityCollection<CommunicatieVorm>
                                                       {
                                                           testCommunicatieVorm
                                                       }
                                           };
            testCommunicatieVorm.GelieerdePersoon = testGelieerdePersoon;

            // mocking opzetten

            var repositoryProviderMock = new Mock<IRepositoryProvider>();
            repositoryProviderMock.Setup(src => src.RepositoryGet<CommunicatieVorm>())
                                  .Returns(
                                      new DummyRepo<CommunicatieVorm>(new List<CommunicatieVorm> {testCommunicatieVorm}));

            Factory.InstantieRegistreren(repositoryProviderMock.Object);

            var communicatieSyncMock = new Mock<ICommunicatieSync>();
            Factory.InstantieRegistreren(communicatieSyncMock.Object);


            var target = Factory.Maak<GelieerdePersonenService>();

            var commDetail = new CommunicatieInfo
            {
                CommunicatieTypeID = 3,    // e-mail
                ID = TESTCVID,    // ID van de bestaande communicatievorm
                Nummer = "johan@linux.be"  // arbitrair nieuw e-mailadres
            };

            // Act
            target.CommunicatieVormAanpassen(commDetail);

            // Assert

            Assert.AreEqual(testCommunicatieVorm.Nummer, "johan@linux.be");
        }

        /// <summary>
        /// Bij  het toevoegen van een communicatievorm die voorkeur moet zijn, moeten
        /// bestaande communicatievormen van hetzelfde type hun voorkeur verliezen. 
        /// </summary>
        [TestMethod]
        public void VoorkeursCommunicatieVormToevoegenTest()
        {
            // ARRANGE

            var origineleCommunicatieVorm = new CommunicatieVorm
                                                {
                                                    CommunicatieType = new CommunicatieType {ID = 1, Validatie = ".*"},
                                                    GelieerdePersoon =
                                                        new GelieerdePersoon
                                                            {
                                                                Persoon =
                                                                    new Persoon {AdInAanvraag = true}
                                                            },
                                                    ID = 2,
                                                    Voorkeur = true,
                                                    Nummer = "1234"
                                                };
            origineleCommunicatieVorm.GelieerdePersoon.Communicatie.Add(origineleCommunicatieVorm);

            var communicatieInfo = new CommunicatieInfo
                                       {
                                           CommunicatieTypeID = origineleCommunicatieVorm.CommunicatieType.ID,
                                           Voorkeur = true,
                                           Nummer = "4321"
                                       };

            // dependency injection

            var repositoryProviderMock = new Mock<IRepositoryProvider>();
            repositoryProviderMock.Setup(src => src.RepositoryGet<GelieerdePersoon>())
                                  .Returns(
                                      new DummyRepo<GelieerdePersoon>(new List<GelieerdePersoon>
                                                                          {
                                                                              origineleCommunicatieVorm
                                                                                  .GelieerdePersoon
                                                                          }));
            repositoryProviderMock.Setup(src => src.RepositoryGet<CommunicatieType>())
                                  .Returns(
                                      new DummyRepo<CommunicatieType>(new List<CommunicatieType>
                                                                          {
                                                                              origineleCommunicatieVorm
                                                                                  .CommunicatieType
                                                                          }));
            Factory.InstantieRegistreren(repositoryProviderMock.Object);

            // ACT

            var target = Factory.Maak<GelieerdePersonenService>();
            target.CommunicatieVormToevoegen(origineleCommunicatieVorm.GelieerdePersoon.ID, communicatieInfo);

            // ASSERT

            Assert.IsFalse(origineleCommunicatieVorm.Voorkeur);
        }


        // Tests die nagingen of gewijzigde entiteiten wel bewaard werden, moeten we niet meer doen, want voor
        // change tracking gebruiken we entity framework. We kunnen ervan uitgaan dat dat wel fatsoenlijk
        // werkt.


        /// <summary>
        /// Als een gelieerde persoon een account heeft zonder gebruikersrechten, moet deze informatie
        /// ook opgeleverd worden door AlleDetailsOphalen.
        ///</summary>
        [TestMethod()]
        public void AlleDetailsOphalenAccountTest()
        {
            // arbitraire dingen

            const int someGid = 5;     
            const int someGpid = 3;    
            const string someUsername = "UserName";   
            DateTime someGeboorteDatum = new DateTime(1977,03,08);
            const int someWerkJaar = 2012;

            // arrange

            var gelieerdePersoon = new GelieerdePersoon
                                       {
                                           ID = someGpid,
                                           Persoon =
                                               new Persoon
                                                   {
                                                       GeboorteDatum = someGeboorteDatum,
                                                       Gav =
                                                           new[]
                                                               {
                                                                   new Gav
                                                                       {
                                                                           Login
                                                                               =
                                                                               someUsername
                                                                       }
                                                               }
                                                   },
                                           Groep = new ChiroGroep
                                                       {
                                                           ID = someGid
                                                       }
                                       };

            var groepsWerkJaar = new GroepsWerkJaar {WerkJaar = someWerkJaar, Groep = gelieerdePersoon.Groep};
            gelieerdePersoon.Groep.GroepsWerkJaar.Add(groepsWerkJaar);


            // IOC opzetten

            var repositoryProviderMock = new Mock<IRepositoryProvider>();
            var communicatieVormenManagerMock = new Mock<ICommunicatieVormenManager>();

            repositoryProviderMock.Setup(mck => mck.RepositoryGet<GelieerdePersoon>())
                                  .Returns(new DummyRepo<GelieerdePersoon>(new List<GelieerdePersoon>{gelieerdePersoon}));

            Factory.InstantieRegistreren(repositoryProviderMock.Object);
            Factory.InstantieRegistreren(communicatieVormenManagerMock.Object);


            var target = Factory.Maak<GelieerdePersonenService>();

            // act

            int gelieerdePersoonID = someGpid;
            var actual = target.AlleDetailsOphalen(gelieerdePersoonID);

            // assert

            Assert.AreEqual(someUsername, actual.GebruikersInfo.GavLogin);
        }

        ///<summary>
        /// Controleert of de service verhindert dat een AD-nummer wordt overschreven
        ///</summary>
        [TestMethod()]
        public void BewarenAdNrTest()
        {
            bool gevangen = false;

            // ARRANGE

            // model
            var gelieerdePersoon = new GelieerdePersoon
                                       {
                                           ID = 1,
                                           Persoon = new Persoon
                                                         {
                                                             AdNummer = 2
                                                         }
                                       };

            // mocks
            var repositoryProviderMock = new Mock<IRepositoryProvider>();
            repositoryProviderMock.Setup(src => src.RepositoryGet<GelieerdePersoon>())
                                  .Returns(new DummyRepo<GelieerdePersoon>(new List<GelieerdePersoon> {gelieerdePersoon}));
            Factory.InstantieRegistreren(repositoryProviderMock.Object);

            // ACT

            var target = Factory.Maak<GelieerdePersonenService>();

            try
            {
                target.Bewaren(new PersoonInfo
                                   {
                                       GelieerdePersoonID = gelieerdePersoon.ID,
                                       AdNummer = 39198
                                   }
                    );

            }
            catch (FaultException<FoutNummerFault> ex)
            {
                // ASSERT

                Assert.AreEqual(ex.Detail.FoutNummer, FoutNummer.AlgemeneFout);
                gevangen = true;
            }

            Assert.IsTrue(gevangen);
        }

        /// <summary>
        /// Controleert of een nieuw voorkeursadres van een gekende gelieerde persoon wordt gesynct
        /// met Kipadmin
        /// </summary>
        [TestMethod()]
        public void GelieerdePersonenVerhuizenSyncTest()
        {
            // ARRANGE.

            #region testdata

            var gelieerdePersoon = new GelieerdePersoon
                                       {
                                           ID = 1,
                                           Persoon = new Persoon
                                                         {
                                                             AdInAanvraag = true,
                                                             PersoonsAdres = new List<PersoonsAdres>
                                                                                 {
                                                                                     new PersoonsAdres
                                                                                         {
                                                                                             Adres = new BelgischAdres
                                                                                                         {
                                                                                                             ID = 2,
                                                                                                             StraatNaam
                                                                                                                 =
                                                                                                                 new StraatNaam
                                                                                                                     {
                                                                                                                         ID
                                                                                                                             =
                                                                                                                             3,
                                                                                                                         Naam
                                                                                                                             =
                                                                                                                             "Langstraat",
                                                                                                                         PostNummer
                                                                                                                             =
                                                                                                                             2140
                                                                                                                     }
                                                                                                         }
                                                                                         }
                                                                                 }
                                                         }
                                       };

            gelieerdePersoon.Persoon.GelieerdePersoon.Add(gelieerdePersoon);
            gelieerdePersoon.PersoonsAdres = gelieerdePersoon.Persoon.PersoonsAdres.First();
            gelieerdePersoon.PersoonsAdres.Adres.PersoonsAdres.Add(gelieerdePersoon.PersoonsAdres);
            gelieerdePersoon.PersoonsAdres.Persoon = gelieerdePersoon.Persoon;
            gelieerdePersoon.PersoonsAdres.GelieerdePersoon.Add(gelieerdePersoon);

            var infoNieuwAdres = new PersoonsAdresInfo
                                     {
                                         StraatNaamNaam = "Kipdorp",
                                         HuisNr = 30,
                                         PostNr = 2000,
                                         WoonPlaatsNaam = "Antwerpen",
                                         AdresType = AdresTypeEnum.Thuis
                                     };
            #endregion

            #region Dependency injection

            // mocks voor data access
            var repositoryProviderMock = new Mock<IRepositoryProvider>();
            repositoryProviderMock.Setup(src => src.RepositoryGet<Adres>())
                                  .Returns(new DummyRepo<Adres>(new List<Adres> {gelieerdePersoon.PersoonsAdres.Adres}));
            repositoryProviderMock.Setup(src => src.RepositoryGet<StraatNaam>())
                                  .Returns(
                                      new DummyRepo<StraatNaam>(new List<StraatNaam>
                                                                    {
                                                                        ((BelgischAdres)
                                                                         gelieerdePersoon.PersoonsAdres
                                                                                         .Adres)
                                                                            .StraatNaam,
                                                                        new StraatNaam
                                                                            {
                                                                                PostNummer =
                                                                                    infoNieuwAdres.PostNr,
                                                                                Naam =
                                                                                    infoNieuwAdres
                                                                                    .StraatNaamNaam
                                                                            }
                                                                    }));
            repositoryProviderMock.Setup(src => src.RepositoryGet<WoonPlaats>())
                                  .Returns(
                                      new DummyRepo<WoonPlaats>(new List<WoonPlaats>
                                                                    {
                                                                        new WoonPlaats
                                                                            {
                                                                                PostNummer =
                                                                                    infoNieuwAdres
                                                                                    .PostNr,
                                                                                Naam =
                                                                                    infoNieuwAdres
                                                                                    .WoonPlaatsNaam
                                                                            }
                                                                    }));
            repositoryProviderMock.Setup(src => src.RepositoryGet<Land>())
                                  .Returns(new DummyRepo<Land>(new List<Land>()));


            // mock voor sync
            var adressenSyncMock = new Mock<IAdressenSync>();
            adressenSyncMock.Setup(src => src.StandaardAdressenBewaren(It.IsAny<IEnumerable<PersoonsAdres>>())).Verifiable();

            // mocks registreren bij dependency-injectioncontainer
            Factory.InstantieRegistreren(repositoryProviderMock.Object);
            Factory.InstantieRegistreren(adressenSyncMock.Object);

            #endregion

            // ACT

            var target = Factory.Maak<GelieerdePersonenService>();
            target.GelieerdePersonenVerhuizen(new[] {gelieerdePersoon.ID}, infoNieuwAdres,
                                              gelieerdePersoon.PersoonsAdres.Adres.ID);

            // ASSERT

            adressenSyncMock.Verify(src => src.StandaardAdressenBewaren(It.IsAny<IEnumerable<PersoonsAdres>>()), Times.AtLeastOnce());
        }

        /// <summary>
        /// Controleert of een nieuw voorkeursadres van een gekende gelieerde persoon wordt gesynct.
        ///</summary>
        [TestMethod()]
        public void AdresToevoegenGelieerdePersonenTest()
        {
            // ARRANGE

            // testdata

            var gelieerdePersoon = new GelieerdePersoon {ID = 1, Persoon = new Persoon {AdInAanvraag = true}};
            gelieerdePersoon.Persoon.GelieerdePersoon.Add(gelieerdePersoon);

            var nederland = new Land {Naam = "Nederland"};

            var persoonsAdresInfo = new PersoonsAdresInfo
                                        {
                                            StraatNaamNaam = "Evert van de Beekstraat",
                                            HuisNr = 354,
                                            PostNr = 1118,
                                            PostCode = "CZ",
                                            WoonPlaatsNaam = "Schiphol",
                                            LandNaam = nederland.Naam
                                        };
            // mock voor sync

            var adressenSyncMock = new Mock<IAdressenSync>();
            adressenSyncMock.Setup(src => src.StandaardAdressenBewaren(It.IsAny<IEnumerable<PersoonsAdres>>()))
                            .Verifiable();
            Factory.InstantieRegistreren(adressenSyncMock.Object);

            // dependency injection voor data access

            var repositoryProviderMock = new Mock<IRepositoryProvider>();
            repositoryProviderMock.Setup(src => src.RepositoryGet<GelieerdePersoon>())
                                  .Returns(new DummyRepo<GelieerdePersoon>(new List<GelieerdePersoon> {gelieerdePersoon}));
            repositoryProviderMock.Setup(src => src.RepositoryGet<Adres>())
                                  .Returns(new DummyRepo<Adres>(new List<Adres>()));
            repositoryProviderMock.Setup(src => src.RepositoryGet<StraatNaam>())
                                  .Returns(new DummyRepo<StraatNaam>(new List<StraatNaam>()));
            repositoryProviderMock.Setup(src => src.RepositoryGet<WoonPlaats>())
                                  .Returns(new DummyRepo<WoonPlaats>(new List<WoonPlaats>()));
            repositoryProviderMock.Setup(src => src.RepositoryGet<Land>())
                                  .Returns(new DummyRepo<Land>(new List<Land> {nederland}));
            Factory.InstantieRegistreren(repositoryProviderMock.Object);

            // ACT

            var target = Factory.Maak<GelieerdePersonenService>();
            target.AdresToevoegenGelieerdePersonen(new [] {gelieerdePersoon.ID}, persoonsAdresInfo, true);

            // ASSERT

            adressenSyncMock.Verify(src => src.StandaardAdressenBewaren(It.IsAny<IEnumerable<PersoonsAdres>>()),
                                    Times.AtLeastOnce());
        }

        /// <summary>
        /// Als het voorkeursadres van een persoon verandert na het verwijderen van een adres,
        /// dan moet die wijziging gesynct worden. Een unit test.
        ///</summary>
        // TODO: Ensure that the UrlToTest attribute specifies a URL to an ASP.NET page (for example,
        // http://.../Default.aspx). This is necessary for the unit test to be executed on the web server,
        // whether you are testing a page, web service, or a WCF service.
        [TestMethod()]
        public void AdresVerwijderenVanPersonenTest()
        {
            // ARRANGE

            var voorkeursAdres = new BelgischAdres {ID = 2};
            var anderAdres = new BelgischAdres {ID = 3};

            var gelieerdePersoon = new GelieerdePersoon {Persoon = new Persoon {ID = 1, AdInAanvraag = true}};

            var voorkeursPa = new PersoonsAdres {Persoon = gelieerdePersoon.Persoon, Adres = voorkeursAdres};
            var anderPa = new PersoonsAdres { Persoon = gelieerdePersoon.Persoon, Adres = anderAdres };

            // wat gepruts om alle relaties goed te leggen

            gelieerdePersoon.Persoon.PersoonsAdres.Add(voorkeursPa);
            gelieerdePersoon.Persoon.PersoonsAdres.Add(anderPa);
            gelieerdePersoon.PersoonsAdres = voorkeursPa;   // persoonsadres gelieere persoon bepaalt voorkeursadres
            voorkeursAdres.PersoonsAdres.Add(voorkeursPa);
            anderAdres.PersoonsAdres.Add(anderPa);
            voorkeursPa.GelieerdePersoon.Add(gelieerdePersoon);
            anderPa.GelieerdePersoon.Add(gelieerdePersoon);


            // Dependency injection synchronisatie

            var adressenSyncMock = new Mock<IAdressenSync>();
            adressenSyncMock.Setup(src => src.StandaardAdressenBewaren(It.IsAny<IEnumerable<PersoonsAdres>>()))
                            .Verifiable();
            Factory.InstantieRegistreren(adressenSyncMock.Object);

            // Dependency injection data access

            var repositoryProviderMock = new Mock<IRepositoryProvider>();
            repositoryProviderMock.Setup(src => src.RepositoryGet<Adres>())
                                  .Returns(new DummyRepo<Adres>(new List<Adres> {voorkeursAdres, anderAdres}));
            repositoryProviderMock.Setup(src => src.RepositoryGet<PersoonsAdres>())
                                  .Returns(new DummyRepo<PersoonsAdres>(new List<PersoonsAdres> {voorkeursPa, anderPa}));
            Factory.InstantieRegistreren(repositoryProviderMock.Object);

            // ACT

            var target = Factory.Maak<GelieerdePersonenService>();
            target.AdresVerwijderenVanPersonen(new [] {gelieerdePersoon.Persoon.ID}, voorkeursAdres.ID);

            // ASSERT

            adressenSyncMock.Verify(src => src.StandaardAdressenBewaren(It.IsAny<IEnumerable<PersoonsAdres>>()),
                                    Times.AtLeastOnce());
        }

        /// <summary>
        //  Controleert of een nieuw ingesteld voorkeursadres wel wordt gesynct.
        /// </summary>
        [TestMethod()]
        public void VoorkeursAdresMakenTest()
        {
            // ARRANGE

            var voorkeursAdres = new BelgischAdres { ID = 2 };
            var anderAdres = new BelgischAdres { ID = 3 };

            var gelieerdePersoon = new GelieerdePersoon { Persoon = new Persoon { ID = 1, AdInAanvraag = true } };

            var voorkeursPa = new PersoonsAdres { Persoon = gelieerdePersoon.Persoon, Adres = voorkeursAdres };
            var anderPa = new PersoonsAdres { Persoon = gelieerdePersoon.Persoon, Adres = anderAdres };

            // wat gepruts om alle relaties goed te leggen

            gelieerdePersoon.Persoon.PersoonsAdres.Add(voorkeursPa);
            gelieerdePersoon.Persoon.PersoonsAdres.Add(anderPa);
            gelieerdePersoon.PersoonsAdres = voorkeursPa;   // persoonsadres gelieere persoon bepaalt voorkeursadres
            voorkeursAdres.PersoonsAdres.Add(voorkeursPa);
            anderAdres.PersoonsAdres.Add(anderPa);
            voorkeursPa.GelieerdePersoon.Add(gelieerdePersoon);
            anderPa.GelieerdePersoon.Add(gelieerdePersoon);


            // Dependency injection synchronisatie

            var adressenSyncMock = new Mock<IAdressenSync>();
            adressenSyncMock.Setup(src => src.StandaardAdressenBewaren(It.IsAny<IEnumerable<PersoonsAdres>>()))
                            .Verifiable();
            Factory.InstantieRegistreren(adressenSyncMock.Object);

            // Dependency injection data access

            var repositoryProviderMock = new Mock<IRepositoryProvider>();
            repositoryProviderMock.Setup(src => src.RepositoryGet<GelieerdePersoon>())
                                  .Returns(new DummyRepo<GelieerdePersoon>(new List<GelieerdePersoon> {gelieerdePersoon}));
            Factory.InstantieRegistreren(repositoryProviderMock.Object);

            // ACT

            var target = Factory.Maak<GelieerdePersonenService>();
            target.VoorkeursAdresMaken(anderPa.ID, gelieerdePersoon.ID);

            // ASSERT

            adressenSyncMock.Verify(src => src.StandaardAdressenBewaren(It.IsAny<IEnumerable<PersoonsAdres>>()),
                        Times.AtLeastOnce());
        }

        /// <summary>
        /// Test of CommunicatieVormVerwijderenVanPersoon synct met Kipadmin
        ///</summary>
        [TestMethod()]
        public void CommunicatieVormVerwijderenVanPersoonTest()
        {
            // ARRANGE

            var communicatieVorm = new CommunicatieVorm
                                       {
                                           ID = 1,
                                           GelieerdePersoon =
                                               new GelieerdePersoon
                                                   {
                                                       Persoon =
                                                           new Persoon
                                                               {
                                                                   AdInAanvraag =
                                                                       true
                                                               }
                                                   }
                                       };

            // Dependency injection: synchronisatie

            var communicatieSyncMock = new Mock<ICommunicatieSync>();
            communicatieSyncMock.Setup(src => src.Verwijderen(It.IsAny<CommunicatieVorm>()))
                            .Verifiable();
            Factory.InstantieRegistreren(communicatieSyncMock.Object);

            // Dependency injection: data access

            var repositoryProviderMock = new Mock<IRepositoryProvider>();
            repositoryProviderMock.Setup(src => src.RepositoryGet<CommunicatieVorm>())
                                  .Returns(new DummyRepo<CommunicatieVorm>(new List<CommunicatieVorm> {communicatieVorm}));
            Factory.InstantieRegistreren(repositoryProviderMock.Object);


            // ACT

            var target = Factory.Maak<GelieerdePersonenService>();
            target.CommunicatieVormVerwijderenVanPersoon(communicatieVorm.ID);

            // ASSERT

            communicatieSyncMock.Verify(src => src.Verwijderen(It.IsAny<CommunicatieVorm>()),
                                        Times.AtLeastOnce());
        }

        /// <summary>
        /// Kijkt na of HuisgenotenOphalenZelfdeGroep geen huisgenoten uit een andere groep ophaalt.
        /// </summary>
        [TestMethod()]
        public void HuisGenotenOphalenZelfdeGroepSecurityTest()
        {
            // ARRANGE

            var groep = new ChiroGroep();

            var gelieerdePersoon1 = new GelieerdePersoon { ID = 4, Persoon = new Persoon { ID = 1 }, Groep = groep };
            var gelieerdePersoon2 = new GelieerdePersoon { ID = 5, Persoon = new Persoon { ID = 2 }, Groep = groep };
            var persoon3 = new Persoon {ID = 3};

            gelieerdePersoon1.Persoon.GelieerdePersoon.Add(gelieerdePersoon1);
            gelieerdePersoon2.Persoon.GelieerdePersoon.Add(gelieerdePersoon2);

            var persoonsAdres1 = new PersoonsAdres {Persoon = gelieerdePersoon1.Persoon};
            var persoonsAdres2 = new PersoonsAdres {Persoon = gelieerdePersoon2.Persoon};
            var persoonsAdres3 = new PersoonsAdres {Persoon = persoon3};

            gelieerdePersoon1.Persoon.PersoonsAdres.Add(persoonsAdres1);
            gelieerdePersoon2.Persoon.PersoonsAdres.Add(persoonsAdres2);
            persoon3.PersoonsAdres.Add(persoonsAdres3);

            var adres = new BelgischAdres
                            {
                                PersoonsAdres = new List<PersoonsAdres>
                                                    {
                                                        persoonsAdres1,
                                                        persoonsAdres2,
                                                        persoonsAdres3
                                                    }
                            };
            persoonsAdres1.Adres = adres;
            persoonsAdres2.Adres = adres;
            persoonsAdres3.Adres = adres;

            var repositoryProviderMock = new Mock<IRepositoryProvider>();
            repositoryProviderMock.Setup(src => src.RepositoryGet<GelieerdePersoon>())
                                  .Returns(
                                      new DummyRepo<GelieerdePersoon>(new List<GelieerdePersoon>
                                                                          {
                                                                              gelieerdePersoon1,
                                                                              gelieerdePersoon2
                                                                          }));
            Factory.InstantieRegistreren(repositoryProviderMock.Object);

            // ACT

            var target = Factory.Maak<GelieerdePersonenService>();
            var actual = target.HuisGenotenOphalenZelfdeGroep(gelieerdePersoon1.ID);

            // ASSERT

            Assert.AreEqual(actual.Count, 2);
            Assert.IsFalse(actual.Select(bi => bi.PersoonID).Contains(persoon3.ID));
        }

        /// <summary>
        /// Kijkt na of HuisgenotenOphalenZelfdeGroep geen dubbele resultaten oplevert.
        /// </summary>
        [TestMethod()]
        public void HuisGenotenOphalenZelfdeGroepDistinctTest()
        {
            // ARRANGE

            var groep = new ChiroGroep();

            var gelieerdePersoon1 = new GelieerdePersoon { ID = 4, Persoon = new Persoon { ID = 1 }, Groep = groep };

            gelieerdePersoon1.Persoon.GelieerdePersoon.Add(gelieerdePersoon1);

            var persoonsAdres1 = new PersoonsAdres { Persoon = gelieerdePersoon1.Persoon };
            var persoonsAdres2 = new PersoonsAdres { Persoon = gelieerdePersoon1.Persoon };

            gelieerdePersoon1.Persoon.PersoonsAdres.Add(persoonsAdres1);
            gelieerdePersoon1.Persoon.PersoonsAdres.Add(persoonsAdres2);

            var adres1 = new BelgischAdres
            {
                ID = 5,
                PersoonsAdres = new List<PersoonsAdres>
                                                    {
                                                        persoonsAdres1
                                                    }
            };

            var adres2 = new BelgischAdres
            {
                ID = 6,
                PersoonsAdres = new List<PersoonsAdres>
                                                    {
                                                        persoonsAdres2
                                                    }
            };


            persoonsAdres1.Adres = adres1;
            persoonsAdres2.Adres = adres2;

            var repositoryProviderMock = new Mock<IRepositoryProvider>();
            repositoryProviderMock.Setup(src => src.RepositoryGet<GelieerdePersoon>())
                                  .Returns(
                                      new DummyRepo<GelieerdePersoon>(new List<GelieerdePersoon>
                                                                          {
                                                                              gelieerdePersoon1,
                                                                          }));
            Factory.InstantieRegistreren(repositoryProviderMock.Object);

            // ACT

            var target = Factory.Maak<GelieerdePersonenService>();
            var actual = target.HuisGenotenOphalenZelfdeGroep(gelieerdePersoon1.ID);

            // ASSERT

            Assert.AreEqual(1, actual.Count);
        }
    }
}
