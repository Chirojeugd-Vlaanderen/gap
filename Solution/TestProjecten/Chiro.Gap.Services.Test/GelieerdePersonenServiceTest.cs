using Chiro.Gap.Services;
/*
 * Copyright 2008-2016 the GAP developers. See the NOTICE file at the 
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
using System.Data.Objects.DataClasses;
using System.Diagnostics;
using System.Linq;
using System.ServiceModel;
using Chiro.Cdf.Ioc.Factory;
using Chiro.Cdf.Poco;
using Chiro.Gap.Domain;
using Chiro.Gap.Dummies;
using Chiro.Gap.Poco.Model;
using Chiro.Gap.ServiceContracts.DataContracts;
using Chiro.Gap.ServiceContracts.FaultContracts;
using Chiro.Gap.Services.Dev;
using Chiro.Gap.SyncInterfaces;
using Chiro.Gap.TestAttributes;
using Chiro.Gap.WorkerInterfaces;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using GebruikersRecht = Chiro.Gap.Poco.Model.GebruikersRecht;

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

#pragma warning disable 168
            // Als ik onderstaande niet een keertje instantieer, dan werken mijn tests niet.
            // Geen idee hoe dat komt.

            DevChannelProvider bla;
#pragma warning restore 168
        }

        /// <summary>
        /// Als je een nieuwe persoon inschrijft die de nieuwsbrief wil, dan moet dat ook zo worden bewaard.
        /// </summary>
        [TestMethod]
        public void NieuwsePersoonNieuwsBriefTest()
        {
            // ARRANGE
            var groep = new ChiroGroep {ID = 1};
            var nieuwePersoonDetails = new NieuwePersoonDetails {PersoonInfo = new PersoonInfo {NieuwsBrief = true}};

            // dependency injection
            var repositoryProviderMock = new Mock<IRepositoryProvider>();
            repositoryProviderMock.Setup(src => src.RepositoryGet<Groep>())
                .Returns(new DummyRepo<Groep>(new List<Groep> {groep}));
            repositoryProviderMock.Setup(src => src.RepositoryGet<GelieerdePersoon>())
                .Returns(new DummyRepo<GelieerdePersoon>(new List<GelieerdePersoon>()));
            Factory.InstantieRegistreren(repositoryProviderMock.Object);

            var gelieerdePersonenManagerMock = new Mock<IGelieerdePersonenManager>();
            gelieerdePersonenManagerMock.Setup(
                src => src.Toevoegen(It.Is<Persoon>(p => p.NieuwsBrief), groep, It.IsAny<int>(), It.IsAny<bool>()))
                .Returns(new GelieerdePersoon {Persoon = new Persoon(), Groep = groep})
                .Verifiable();
            Factory.InstantieRegistreren(gelieerdePersonenManagerMock.Object);

            var service = Factory.Maak<GelieerdePersonenService>();

            // ACT
            service.Nieuw(nieuwePersoonDetails, groep.ID, true);

            // ASSERT
            gelieerdePersonenManagerMock.Verify(
                src => src.Toevoegen(It.Is<Persoon>(p => p.NieuwsBrief), groep, It.IsAny<int>(), It.IsAny<bool>()),
                Times.AtLeastOnce);
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
            var gelieerdePersoon = new GelieerdePersoon { ID = 1, Persoon = new Persoon() };
            var communicatieType = new CommunicatieType { ID = 2, Validatie = "^[0-9]*$" };

            // dependency injection
            var repositoryProviderMock = new Mock<IRepositoryProvider>();
            repositoryProviderMock.Setup(src => src.RepositoryGet<GelieerdePersoon>())
                                  .Returns(new DummyRepo<GelieerdePersoon>(new List<GelieerdePersoon> { gelieerdePersoon }));
            repositoryProviderMock.Setup(src => src.RepositoryGet<CommunicatieType>())
                                  .Returns(new DummyRepo<CommunicatieType>(new List<CommunicatieType> { communicatieType }));
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

            var gelieerdePersoon = new GelieerdePersoon { ID = 1, Persoon = new Persoon() };
            var telefoonNr = new CommunicatieType
                                 {
                                     ID = 1,
                                     Omschrijving = "Telefoonnummer",
                                     Validatie =
                                         @"(^0[0-9]{1,2}\-[0-9]{2,3}\s?[0-9]{2}\s?[0-9]{2}$|^04[0-9]{2}\-[0-9]{2,3}\s?[0-9]{2}\s?[0-9]{2}$)|^\+[0-9]*$"
                                 };

            // mocking opzetten

            var repositoryProviderMock = new Mock<IRepositoryProvider>();
            repositoryProviderMock.Setup(src => src.RepositoryGet<GelieerdePersoon>())
                                  .Returns(new DummyRepo<GelieerdePersoon>(new List<GelieerdePersoon> { gelieerdePersoon }));
            repositoryProviderMock.Setup(src => src.RepositoryGet<CommunicatieType>())
                                  .Returns(new DummyRepo<CommunicatieType>(new List<CommunicatieType> { telefoonNr }));

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
        /// Als je een communicatievorm voor heel het gezin toevoegt, maar niet heel het gezin is
        /// 'in sync', dan mag die communicatievorm enkel gesynct worden voor de personen die
        /// in sync zijn.
        /// </summary>
        [TestMethod]
        public void CommunicatieVormToevoegenHeelGezinTest()
        {
            // ARRANGE

            // modelletje

            var gelieerdePersoonInSync = new GelieerdePersoon {ID = 1, Persoon = new Persoon {InSync = true} };
            var gelieerdePersoonNietInSync = new GelieerdePersoon {ID = 2, Persoon = new Persoon()};

            gelieerdePersoonInSync.Persoon.GelieerdePersoon = new List<GelieerdePersoon> {gelieerdePersoonInSync};
            gelieerdePersoonNietInSync.Persoon.GelieerdePersoon = new List<GelieerdePersoon>
            {
                gelieerdePersoonNietInSync
            };

            var adres = new BuitenLandsAdres();

            var pa1 = new PersoonsAdres {Persoon = gelieerdePersoonInSync.Persoon, Adres = adres};
            var pa2 = new PersoonsAdres {Persoon = gelieerdePersoonNietInSync.Persoon, Adres = adres};

            gelieerdePersoonInSync.Persoon.PersoonsAdres = new List<PersoonsAdres> {pa1};
            gelieerdePersoonNietInSync.Persoon.PersoonsAdres = new List<PersoonsAdres> {pa2};
            adres.PersoonsAdres = new List<PersoonsAdres> {pa1, pa2};

            var telefoonNr = new CommunicatieType
            {
                ID = 1,
                Omschrijving = "Telefoonnummer",
                Validatie =
                                         @"(^0[0-9]{1,2}\-[0-9]{2,3}\s?[0-9]{2}\s?[0-9]{2}$|^04[0-9]{2}\-[0-9]{2,3}\s?[0-9]{2}\s?[0-9]{2}$)|^\+[0-9]*$",
            };

            // mocking opzetten

            var repositoryProviderMock = new Mock<IRepositoryProvider>();
            repositoryProviderMock.Setup(src => src.RepositoryGet<GelieerdePersoon>())
                                  .Returns(new DummyRepo<GelieerdePersoon>(new List<GelieerdePersoon> { gelieerdePersoonInSync, gelieerdePersoonNietInSync }));
            repositoryProviderMock.Setup(src => src.RepositoryGet<CommunicatieType>())
                                  .Returns(new DummyRepo<CommunicatieType>(new List<CommunicatieType> { telefoonNr }));
            Factory.InstantieRegistreren(repositoryProviderMock.Object);

            // dependency injection voor synchronisatie:
            // verwacht dat CommunicatieSync.Toevoegen niet wordt aangeroepen voor de persoon niet in sync.
            var communicatieSyncMock = new Mock<ICommunicatieSync>();
            communicatieSyncMock.Setup(snc => snc.Toevoegen(It.Is<CommunicatieVorm>(cv => Equals(cv.GelieerdePersoon, gelieerdePersoonNietInSync)))).Verifiable();
            Factory.InstantieRegistreren(communicatieSyncMock.Object);

            // ACT

            var target = Factory.Maak<GelieerdePersonenService>();

            var commInfo = new CommunicatieDetail()
            {
                CommunicatieTypeID = telefoonNr.ID,
                Voorkeur = true,
                IsGezinsGebonden = true,
                Nummer = "03-484 53 32" // geldig nummer
            };

            target.CommunicatieVormToevoegen(gelieerdePersoonInSync.ID, commInfo);

            // ASSERT

            communicatieSyncMock.Verify(
                snc =>
                    snc.Toevoegen(It.Is<CommunicatieVorm>(cv => Equals(cv.GelieerdePersoon, gelieerdePersoonNietInSync))),
                Times.Never());
        }

        /// <summary>
        /// Kijkt na of CommunicatieVormToevoegen enkel de nieuwe communicatievorm
        /// naar Kipadmin synct, ipv alle communicatie in kipadmin te vervangen  
        /// </summary>
        [TestMethod]
        public void CommunicatieVormToevoegenTest()
        {
            // ARRANGE

            var gelieerdePersoon = new GelieerdePersoon
            {
                ID = 1,
                Persoon = new Persoon {InSync = true},
                Groep = new ChiroGroep()
            };
            var email = new CommunicatieType { ID = 3, Validatie = ".*" };

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
                                  .Returns(new DummyRepo<GelieerdePersoon>(new List<GelieerdePersoon> { gelieerdePersoon }));
            repositoryProviderMock.Setup(src => src.RepositoryGet<CommunicatieType>())
                                  .Returns(new DummyRepo<CommunicatieType>(new List<CommunicatieType> { email }));
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

            var testCommunicatieType = new CommunicatieType { ID = TESTCTID, Validatie = ".*" };
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
                                               Persoon = new Persoon(),
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
                                      new DummyRepo<CommunicatieVorm>(new List<CommunicatieVorm> { testCommunicatieVorm }));

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
                                                    CommunicatieType = new CommunicatieType { ID = 1, Validatie = ".*" },
                                                    GelieerdePersoon =
                                                        new GelieerdePersoon
                                                            {
                                                                Persoon =
                                                                    new Persoon { InSync = true }
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

        /// <summary>
        /// Als er voor een nieuwsbriefabonnement een e-mailadres gekozen wordt
        /// dat al wel bestond, maar geen voorkeursadres was, dan moet dat
        /// e-mailadres het voorkeursadres worden. Zie #3392.
        /// </summary>
        [TestMethod]
        public void NieuwsBriefNieuwVoorkeursAdresTest()
        {
            // ARRANGE

            var gelieerdePersoon = new GelieerdePersoon
            {
                ID = 1,
                Persoon = new Persoon {InSync = true},
                Groep = new ChiroGroep()
            };

            // Voor deze test liggen we niet wakker van het formaat van een e-mailadres:
            var emailType = new CommunicatieType { ID = 3, Validatie = ".*" };

            var oudVoorkeursAdres = new CommunicatieVorm
            {
                GelieerdePersoon = gelieerdePersoon,
                CommunicatieType = emailType,
                ID = 2,
                Nummer = "johan@linux.be",
                Voorkeur = true
            };

            var adresVoorNieuwsBrief = new CommunicatieVorm
            {
                GelieerdePersoon = gelieerdePersoon,
                CommunicatieType = emailType,
                ID = 3,
                Nummer = "commissie.linux@chiro.be",
                Voorkeur = false
            };

            gelieerdePersoon.Communicatie = new List<CommunicatieVorm> { oudVoorkeursAdres, adresVoorNieuwsBrief };

            // dependency injection voor data access

            var repositoryProviderMock = new Mock<IRepositoryProvider>();
            repositoryProviderMock.Setup(src => src.RepositoryGet<GelieerdePersoon>())
                .Returns(new DummyRepo<GelieerdePersoon>(new List<GelieerdePersoon> {gelieerdePersoon}));
            repositoryProviderMock.Setup(src => src.RepositoryGet<CommunicatieType>())
                .Returns(new DummyRepo<CommunicatieType>(new List<CommunicatieType> {emailType}));
            repositoryProviderMock.Setup(src => src.RepositoryGet<CommunicatieVorm>())
                .Returns(
                    new DummyRepo<CommunicatieVorm>(new List<CommunicatieVorm> {oudVoorkeursAdres, adresVoorNieuwsBrief}));
            Factory.InstantieRegistreren(repositoryProviderMock.Object);

            // ACT

            var target = Factory.Maak<GelieerdePersonenService>();
            target.InschrijvenNieuwsBrief(gelieerdePersoon.ID, adresVoorNieuwsBrief.Nummer, true);

            // ASSERT
            Assert.IsTrue(adresVoorNieuwsBrief.Voorkeur);
        }

        /// <summary>
        /// Als er voor een nieuwsbriefabonnement een nieuw e-mailadres wordt meegegeven,
        /// dan moet dat e-mailadres gekoppeld worden aan de gelieerde persoon.  
        /// </summary>
        [TestMethod]
        public void NieuwsBriefNieuwAdresTest()
        {
            // ARRANGE

            const string mailAdres = "johan@linux.be";

            var gelieerdePersoon = new GelieerdePersoon
            {
                ID = 1,
                Persoon = new Persoon { InSync = true },
                Groep = new ChiroGroep()
            };

            // Voor deze test liggen we niet wakker van het formaat van een e-mailadres:
            var emailType = new CommunicatieType { ID = 3, Validatie = ".*" };

            // dependency injection voor data access

            var repositoryProviderMock = new Mock<IRepositoryProvider>();
            repositoryProviderMock.Setup(src => src.RepositoryGet<GelieerdePersoon>())
                                  .Returns(new DummyRepo<GelieerdePersoon>(new List<GelieerdePersoon> { gelieerdePersoon }));
            repositoryProviderMock.Setup(src => src.RepositoryGet<CommunicatieType>())
                                  .Returns(new DummyRepo<CommunicatieType>(new List<CommunicatieType> { emailType }));
            Factory.InstantieRegistreren(repositoryProviderMock.Object);

            // ACT

            var target = Factory.Maak<GelieerdePersonenService>();
            target.InschrijvenNieuwsBrief(gelieerdePersoon.ID, mailAdres, true);

            // ASSERT
            Debug.Assert(gelieerdePersoon.Communicatie.Any(cm => cm.Nummer == mailAdres));
        }


        /// <summary>
        /// Als er een niet-in-sync persoon wordt ingeschreven voor de nieuwsbrief, dan moet die persoon
        /// achteraf in sync zijn.  
        /// </summary>
        [TestMethod]
        public void NieuwsBriefPersoonInSyncTest()
        {
            // ARRANGE

            var gelieerdePersoon = new GelieerdePersoon
            {
                ID = 1,
                Persoon = new Persoon { InSync = false },
                Groep = new ChiroGroep()
            };

            // Voor deze test liggen we niet wakker van het formaat van een e-mailadres:
            var emailType = new CommunicatieType { ID = 3, Validatie = ".*" };

            var email = new CommunicatieVorm
            {
                GelieerdePersoon = gelieerdePersoon,
                CommunicatieType = emailType,
                ID = 2,
                Nummer = "johan@linux.be",
                Voorkeur = true
            };

            gelieerdePersoon.Communicatie = new List<CommunicatieVorm> {email};

            // dependency injection voor synchronisatie:
            // verwacht syc van alle persoonsinfo

            var personenSyncMock = new Mock<IPersonenSync>();
            personenSyncMock.Setup(snc => snc.UpdatenOfMaken(gelieerdePersoon)).Verifiable();
            Factory.InstantieRegistreren(personenSyncMock.Object);

            // dependency injection voor data access
            // voor gelieerde-personenrepo doen we iets speciaals, zodat we bij savechanges
            // kunnen nakijken of de persoon al in sync is.

            var gelieerdePersonenRepoMock = new Mock<IRepository<GelieerdePersoon>>();
            gelieerdePersonenRepoMock.Setup(src => src.ByID(gelieerdePersoon.ID, It.IsAny<string[]>()))
                .Returns(gelieerdePersoon);
            gelieerdePersonenRepoMock.Setup(src => src.SaveChanges()).Callback(() =>
            {
                // Op het moment dat de nieuwsbriefinschrijving wordt geregistreerd,
                // moet de persoon al in sync zijn.
                Debug.Assert(gelieerdePersoon.Persoon.InSync);
            });

            var repositoryProviderMock = new Mock<IRepositoryProvider>();
            repositoryProviderMock.Setup(src => src.RepositoryGet<GelieerdePersoon>())
                                  .Returns(gelieerdePersonenRepoMock.Object);
            repositoryProviderMock.Setup(src => src.RepositoryGet<CommunicatieType>())
                                  .Returns(new DummyRepo<CommunicatieType>(new List<CommunicatieType> { emailType }));
            Factory.InstantieRegistreren(repositoryProviderMock.Object);

            // ACT

            var target = Factory.Maak<GelieerdePersonenService>();
            target.InschrijvenNieuwsBrief(gelieerdePersoon.ID, email.Nummer, true);

            // ASSERT
            Assert.IsTrue(gelieerdePersoon.Persoon.InSync);
            personenSyncMock.Verify(snc => snc.UpdatenOfMaken(gelieerdePersoon), Times.AtLeastOnce);
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
            DateTime someGeboorteDatum = new DateTime(1977, 03, 08);
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

            var groepsWerkJaar = new GroepsWerkJaar { WerkJaar = someWerkJaar, Groep = gelieerdePersoon.Groep };
            gelieerdePersoon.Groep.GroepsWerkJaar.Add(groepsWerkJaar);


            // IOC opzetten

            var repositoryProviderMock = new Mock<IRepositoryProvider>();
            var communicatieVormenManagerMock = new Mock<ICommunicatieVormenManager>();

            repositoryProviderMock.Setup(mck => mck.RepositoryGet<GelieerdePersoon>())
                                  .Returns(new DummyRepo<GelieerdePersoon>(new List<GelieerdePersoon> { gelieerdePersoon }));

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
        /// Controleert of de service verhindert dat een AD-nummer wordt toegekend bij het maken van een nieuwe persoon.
        ///</summary>
        [TestMethod()]
        [ExpectedFoutNummer(typeof(FaultException<FoutNummerFault>), FoutNummer.AlgemeneFout)]
        public void ToekennenAdNrTest()
        {
            // ARRANGE

            // model

            var groep = new ChiroGroep { ID = 1 };

            // mocks
            var repositoryProviderMock = new Mock<IRepositoryProvider>();
            repositoryProviderMock.Setup(src => src.RepositoryGet<Groep>())
                                  .Returns(new DummyRepo<Groep>(new List<Groep> { groep }));
            repositoryProviderMock.Setup(src => src.RepositoryGet<GelieerdePersoon>())
                .Returns(new DummyRepo<GelieerdePersoon>(new List<GelieerdePersoon>()));
            Factory.InstantieRegistreren(repositoryProviderMock.Object);

            // ACT

            var target = Factory.Maak<GelieerdePersonenService>();

            target.Nieuw
            (
                new NieuwePersoonDetails
                {
                    PersoonInfo = new PersoonInfo { AdNummer = 39198 }
                }, groep.ID, false
            );

            Assert.IsTrue(false);
        }

        ///<summary>
        /// Controleert of de service verhindert dat een GelieerdePersoonID wordt toegekend bij het maken van een 
        /// nieuwe persoon.
        ///</summary>
        [TestMethod()]
        [ExpectedFoutNummer(typeof(FaultException<FoutNummerFault>), FoutNummer.AlgemeneFout)]
        public void ToekennenGelieerdePersoonIdTest()
        {
            // ARRANGE

            // model

            var groep = new ChiroGroep { ID = 1 };

            // mocks
            var repositoryProviderMock = new Mock<IRepositoryProvider>();
            repositoryProviderMock.Setup(src => src.RepositoryGet<Groep>())
                                  .Returns(new DummyRepo<Groep>(new List<Groep> { groep }));
            Factory.InstantieRegistreren(repositoryProviderMock.Object);

            // ACT

            var target = Factory.Maak<GelieerdePersonenService>();

            target.Nieuw
            (
                new NieuwePersoonDetails
                {
                    PersoonInfo = new PersoonInfo { GelieerdePersoonID = 39198 }
                }, groep.ID, false
            );

            Assert.IsTrue(false);
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
                Groep = new ChiroGroep {GroepsWerkJaar = new List<GroepsWerkJaar> {new GroepsWerkJaar()}},
                Persoon = new Persoon
                {
                    InSync = true,
                    PersoonsAdres = new List<PersoonsAdres>
                    {
                        new PersoonsAdres
                        {
                            Adres = new BelgischAdres
                            {
                                ID = 2,
                                StraatNaam = new StraatNaam {ID = 3, Naam = "Langstraat", PostNummer = 2140}
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
                                  .Returns(new DummyRepo<Adres>(new List<Adres> { gelieerdePersoon.PersoonsAdres.Adres }));
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
            adressenSyncMock.Setup(src => src.StandaardAdressenBewaren(It.IsAny<IList<PersoonsAdres>>())).Verifiable();

            // mocks registreren bij dependency-injectioncontainer
            Factory.InstantieRegistreren(repositoryProviderMock.Object);
            Factory.InstantieRegistreren(adressenSyncMock.Object);

            #endregion

            // ACT

            var target = Factory.Maak<GelieerdePersonenService>();
            target.GelieerdePersonenVerhuizen(new[] { gelieerdePersoon.ID }, infoNieuwAdres,
                                              gelieerdePersoon.PersoonsAdres.Adres.ID);

            // ASSERT

            adressenSyncMock.Verify(src => src.StandaardAdressenBewaren(It.IsAny<IList<PersoonsAdres>>()), Times.AtLeastOnce());
        }

        /// <summary>
        /// Controleert of een nieuw niet-voorkeursadres van een gekende gelieerde persoon niet wordt gesynct
        /// met Kipadmin
        /// </summary>
        [TestMethod()]
        public void GelieerdePersonenVerhuizenNietVoorkeurSyncTest()
        {
            // ARRANGE.

            #region testdata

            var adres = new BelgischAdres
            {
                ID = 2,
                StraatNaam = new StraatNaam {ID = 3, Naam = "Langstraat", PostNummer = 2140}
            };


            // GelierdePersoon1 heeft het adres niet als voorkeursadres, gelieerdePersoon2 wel.
            var gelieerdePersoon1 = new GelieerdePersoon
            {
                ID = 1,
                Persoon = new Persoon
                {
                    ID = 4,
                    InSync = true,
                },
                PersoonsAdres = new PersoonsAdres {ID = 3, Adres = new BelgischAdres{ID = 8}}
            };
            gelieerdePersoon1.Persoon.PersoonsAdres.Add(new PersoonsAdres { ID = 6, Adres = adres, Persoon = gelieerdePersoon1.Persoon });
            gelieerdePersoon1.Persoon.PersoonsAdres.Add(gelieerdePersoon1.PersoonsAdres);

            var gelieerdePersoon2 = new GelieerdePersoon
            {
                ID = 2,
                Persoon = new Persoon
                {
                    ID = 5,
                    InSync = false,
                    PersoonsAdres = new List<PersoonsAdres>
                    {
                        new PersoonsAdres
                        {
                            ID = 7,
                            Adres = adres
                        }
                    }
                }
            };

            gelieerdePersoon1.Persoon.GelieerdePersoon.Add(gelieerdePersoon1);
            gelieerdePersoon1.PersoonsAdres.Persoon = gelieerdePersoon1.Persoon;

            gelieerdePersoon2.Persoon.GelieerdePersoon.Add(gelieerdePersoon2);
            gelieerdePersoon2.PersoonsAdres = gelieerdePersoon2.Persoon.PersoonsAdres.First();
            gelieerdePersoon2.PersoonsAdres.Persoon = gelieerdePersoon2.Persoon;
            gelieerdePersoon2.PersoonsAdres.GelieerdePersoon.Add(gelieerdePersoon2);
            adres.PersoonsAdres.Add(gelieerdePersoon2.PersoonsAdres);
            adres.PersoonsAdres.Add(gelieerdePersoon1.PersoonsAdres);

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
                                  .Returns(new DummyRepo<Adres>(new List<Adres> { adres }));
            repositoryProviderMock.Setup(src => src.RepositoryGet<StraatNaam>())
                .Returns(
                    new DummyRepo<StraatNaam>(new List<StraatNaam>
                    {
                        ((BelgischAdres) adres).StraatNaam,
                        new StraatNaam {PostNummer = infoNieuwAdres.PostNr, Naam = infoNieuwAdres.StraatNaamNaam}
                    }));
            repositoryProviderMock.Setup(src => src.RepositoryGet<WoonPlaats>())
                .Returns(
                    new DummyRepo<WoonPlaats>(new List<WoonPlaats>
                    {
                        new WoonPlaats {PostNummer = infoNieuwAdres.PostNr, Naam = infoNieuwAdres.WoonPlaatsNaam}
                    }));
            repositoryProviderMock.Setup(src => src.RepositoryGet<Land>())
                                  .Returns(new DummyRepo<Land>(new List<Land>()));


            // mock voor sync
            var adressenSyncMock = new Mock<IAdressenSync>();
            adressenSyncMock.Setup(src => src.StandaardAdressenBewaren(It.Is<IList<PersoonsAdres>>(l => !l.Any()))).Verifiable();

            // mocks registreren bij dependency-injectioncontainer
            Factory.InstantieRegistreren(repositoryProviderMock.Object);
            Factory.InstantieRegistreren(adressenSyncMock.Object);

            #endregion

            // ACT

            var target = Factory.Maak<GelieerdePersonenService>();
            target.GelieerdePersonenVerhuizen(new[] { gelieerdePersoon1.ID }, infoNieuwAdres,
                                              adres.ID);

            // ASSERT

            adressenSyncMock.VerifyAll();
        }

        /// <summary>
        /// Controleert of een nieuw voorkeursadres van een gekende gelieerde persoon wordt gesynct.
        ///</summary>
        [TestMethod()]
        public void AdresToevoegenGelieerdePersonenTest()
        {
            // ARRANGE

            // testdata

            var gelieerdePersoon = new GelieerdePersoon
            {
                ID = 1,
                Persoon = new Persoon {InSync = true},
                Groep = new ChiroGroep()
            };
            gelieerdePersoon.Persoon.GelieerdePersoon.Add(gelieerdePersoon);

            var nederland = new Land { Naam = "Nederland" };

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
            adressenSyncMock.Setup(src => src.StandaardAdressenBewaren(It.IsAny<IList<PersoonsAdres>>()))
                            .Verifiable();
            Factory.InstantieRegistreren(adressenSyncMock.Object);

            // dependency injection voor data access

            var repositoryProviderMock = new Mock<IRepositoryProvider>();
            repositoryProviderMock.Setup(src => src.RepositoryGet<GelieerdePersoon>())
                                  .Returns(new DummyRepo<GelieerdePersoon>(new List<GelieerdePersoon> { gelieerdePersoon }));
            repositoryProviderMock.Setup(src => src.RepositoryGet<Adres>())
                                  .Returns(new DummyRepo<Adres>(new List<Adres>()));
            repositoryProviderMock.Setup(src => src.RepositoryGet<StraatNaam>())
                                  .Returns(new DummyRepo<StraatNaam>(new List<StraatNaam>()));
            repositoryProviderMock.Setup(src => src.RepositoryGet<WoonPlaats>())
                                  .Returns(new DummyRepo<WoonPlaats>(new List<WoonPlaats>()));
            repositoryProviderMock.Setup(src => src.RepositoryGet<Land>())
                                  .Returns(new DummyRepo<Land>(new List<Land> { nederland }));
            Factory.InstantieRegistreren(repositoryProviderMock.Object);

            // ACT

            var target = Factory.Maak<GelieerdePersonenService>();
            target.AdresToevoegenGelieerdePersonen(new[] { gelieerdePersoon.ID }, persoonsAdresInfo, true);

            // ASSERT

            adressenSyncMock.Verify(src => src.StandaardAdressenBewaren(It.IsAny<IList<PersoonsAdres>>()),
                                    Times.AtLeastOnce());
        }

        /// <summary>
        /// Als het voorkeursadres van een persoon verandert na het verwijderen van een adres,
        /// dan moet die wijziging gesynct worden. Een unit test.
        ///</summary>
        [TestMethod()]
        public void AdresVerwijderenVanPersonenNieuweVoorkeurTest()
        {
            // ARRANGE

            var voorkeursAdres = new BelgischAdres { ID = 2 };
            var anderAdres = new BelgischAdres { ID = 3 };

            var gelieerdePersoon = new GelieerdePersoon
            {
                Persoon = new Persoon {ID = 1, InSync = true},
                Groep = new ChiroGroep {GroepsWerkJaar = new List<GroepsWerkJaar> {new GroepsWerkJaar()}}
            };

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
            adressenSyncMock.Setup(src => src.StandaardAdressenBewaren(It.IsAny<IList<PersoonsAdres>>()))
                            .Verifiable();
            Factory.InstantieRegistreren(adressenSyncMock.Object);

            // Dependency injection data access

            var repositoryProviderMock = new Mock<IRepositoryProvider>();
            repositoryProviderMock.Setup(src => src.RepositoryGet<Adres>())
                                  .Returns(new DummyRepo<Adres>(new List<Adres> { voorkeursAdres, anderAdres }));
            repositoryProviderMock.Setup(src => src.RepositoryGet<PersoonsAdres>())
                                  .Returns(new DummyRepo<PersoonsAdres>(new List<PersoonsAdres> { voorkeursPa, anderPa }));
            Factory.InstantieRegistreren(repositoryProviderMock.Object);

            // ACT

            var target = Factory.Maak<GelieerdePersonenService>();
            target.AdresVerwijderenVanPersonen(new[] { gelieerdePersoon.Persoon.ID }, voorkeursAdres.ID);

            // ASSERT

            adressenSyncMock.Verify(src => src.StandaardAdressenBewaren(It.IsAny<IList<PersoonsAdres>>()),
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

            var gelieerdePersoon = new GelieerdePersoon
            {
                Persoon = new Persoon {ID = 1, InSync = true},
                Groep = new ChiroGroep {GroepsWerkJaar = new List<GroepsWerkJaar> {new GroepsWerkJaar()}}
            };

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
            adressenSyncMock.Setup(src => src.StandaardAdressenBewaren(It.IsAny<IList<PersoonsAdres>>()))
                            .Verifiable();
            Factory.InstantieRegistreren(adressenSyncMock.Object);

            // Dependency injection data access

            var repositoryProviderMock = new Mock<IRepositoryProvider>();
            repositoryProviderMock.Setup(src => src.RepositoryGet<GelieerdePersoon>())
                                  .Returns(new DummyRepo<GelieerdePersoon>(new List<GelieerdePersoon> { gelieerdePersoon }));
            Factory.InstantieRegistreren(repositoryProviderMock.Object);

            // ACT

            var target = Factory.Maak<GelieerdePersonenService>();
            target.VoorkeursAdresMaken(anderPa.ID, gelieerdePersoon.ID);

            // ASSERT

            adressenSyncMock.Verify(src => src.StandaardAdressenBewaren(It.IsAny<IList<PersoonsAdres>>()),
                        Times.AtLeastOnce());
        }

        /// <summary>
        /// Test of CommunicatieVormVerwijderen synct met Kipadmin
        ///</summary>
        [TestMethod()]
        public void CommunicatieVormVerwijderenSyncTest()
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
                                InSync =
                                    true
                            },
                        Groep = new ChiroGroep {GroepsWerkJaar = new[] {new GroepsWerkJaar()}}
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
                                  .Returns(new DummyRepo<CommunicatieVorm>(new List<CommunicatieVorm> { communicatieVorm }));
            Factory.InstantieRegistreren(repositoryProviderMock.Object);


            // ACT

            var target = Factory.Maak<GelieerdePersonenService>();
            target.CommunicatieVormVerwijderen(communicatieVorm.ID);

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
            var persoon3 = new Persoon { ID = 3 };

            gelieerdePersoon1.Persoon.GelieerdePersoon.Add(gelieerdePersoon1);
            gelieerdePersoon2.Persoon.GelieerdePersoon.Add(gelieerdePersoon2);

            var persoonsAdres1 = new PersoonsAdres { Persoon = gelieerdePersoon1.Persoon };
            var persoonsAdres2 = new PersoonsAdres { Persoon = gelieerdePersoon2.Persoon };
            var persoonsAdres3 = new PersoonsAdres { Persoon = persoon3 };

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

        /// <summary>
        /// Controleert of een categorie niet twee keer aan dezelfde persoon kan gekoppeld worden.
        /// </summary>
        [TestMethod()]
        public void CategorieKoppelenDubbelTest()
        {
            // ARRANGE

            var gelieerdePersoon = new GelieerdePersoon { ID = 1, Groep = new ChiroGroep() };
            var categorie = new Categorie { ID = 2, Groep = gelieerdePersoon.Groep };
            gelieerdePersoon.Categorie.Add(categorie);
            gelieerdePersoon.Groep.Categorie.Add(categorie);

            var repositoryProviderMock = new Mock<IRepositoryProvider>();
            repositoryProviderMock.Setup(src => src.RepositoryGet<GelieerdePersoon>())
                                  .Returns(new DummyRepo<GelieerdePersoon>(new List<GelieerdePersoon> { gelieerdePersoon }));

            Factory.InstantieRegistreren(repositoryProviderMock.Object);

            // ACT

            var gelieerdePersonenService = Factory.Maak<GelieerdePersonenService>();
            gelieerdePersonenService.CategorieKoppelen(new[] { gelieerdePersoon.ID }, new[] { categorie.ID });

            // ASSERT

            Assert.AreEqual(gelieerdePersoon.Categorie.Count, 1);
        }

        /// <summary>
        /// Kijkt na of de gebruikersrechten meekomen met AlleDetailsOphalen
        /// </summary>
        [TestMethod()]
        public void AlleDetailsOphalenTest()
        {
            // ARRANGE

            var groepsWerkJaar = new GroepsWerkJaar();

            var gelieerdePersoon = new GelieerdePersoon
                                       {
                                           ID = 1,
                                           Groep =
                                               new ChiroGroep
                                                   {
                                                       GroepsWerkJaar =
                                                           new List<GroepsWerkJaar>
                                                               {
                                                                   groepsWerkJaar
                                                               }
                                                   },
                                           Persoon = new Persoon()
                                       };
            gelieerdePersoon.Persoon.GelieerdePersoon.Add(gelieerdePersoon);
            groepsWerkJaar.Groep = gelieerdePersoon.Groep;

            var gebruikersRecht = new GebruikersRecht
                                      {
                                          Groep = gelieerdePersoon.Groep,
                                          VervalDatum =
                                              DateTime.Today.AddMonths(1)
                                      };
            gelieerdePersoon.Groep.GebruikersRecht.Add(gebruikersRecht);
            var gav = new Gav
                          {
                              GebruikersRecht =
                                  new List<GebruikersRecht>
                                      {
                                          gebruikersRecht
                                      }
                          };
            gav.Persoon.Add(gelieerdePersoon.Persoon);
            gelieerdePersoon.Persoon.Gav.Add(gav);
            gebruikersRecht.Gav = gav;

            var repositoryProviderMock = new Mock<IRepositoryProvider>();
            repositoryProviderMock.Setup(src => src.RepositoryGet<GelieerdePersoon>())
                                  .Returns(new DummyRepo<GelieerdePersoon>(new List<GelieerdePersoon> { gelieerdePersoon }));
            Factory.InstantieRegistreren(repositoryProviderMock.Object);

            // ACT

            var target = Factory.Maak<GelieerdePersonenService>();
            var actual = target.AlleDetailsOphalen(gelieerdePersoon.ID);

            // ASSERT

            Assert.AreEqual(actual.GebruikersInfo.VervalDatum, gebruikersRecht.VervalDatum);
        }

        /// <summary>
        /// Als het laatste adres van een persoon wordt verwijderd, dan kan AdressenSync.StandaardAdresBewaren
        /// niet opgeroepen worden, want dat vraagt een PersoonsAdres. Als alle adressen zijn verwijderd, dan
        /// is er geen PersonsAdres meer.
        ///</summary>
        [TestMethod()]
        public void LaatsteAdresVerwijderenVanPersonenTest()
        {
            // ARRANGE

            var voorkeursAdres = new BelgischAdres { ID = 2 };

            var gelieerdePersoon = new GelieerdePersoon
            {
                Persoon = new Persoon {ID = 1, InSync = true},
                Groep = new ChiroGroep {GroepsWerkJaar = new List<GroepsWerkJaar> {new GroepsWerkJaar()}}
            };

            var voorkeursPa = new PersoonsAdres { Persoon = gelieerdePersoon.Persoon, Adres = voorkeursAdres };

            // wat gepruts om alle relaties goed te leggen

            gelieerdePersoon.Persoon.PersoonsAdres.Add(voorkeursPa);
            gelieerdePersoon.PersoonsAdres = voorkeursPa;   // persoonsadres gelieere persoon bepaalt voorkeursadres
            voorkeursAdres.PersoonsAdres.Add(voorkeursPa);
            voorkeursPa.GelieerdePersoon.Add(gelieerdePersoon);


            // Dependency injection synchronisatie

            var adressenSyncMock = new Mock<IAdressenSync>();
            adressenSyncMock.Setup(src => src.StandaardAdressenBewaren(It.IsAny<IList<PersoonsAdres>>()))
                            .Verifiable();
            Factory.InstantieRegistreren(adressenSyncMock.Object);

            // Dependency injection data access

            var repositoryProviderMock = new Mock<IRepositoryProvider>();
            repositoryProviderMock.Setup(src => src.RepositoryGet<Adres>())
                                  .Returns(new DummyRepo<Adres>(new List<Adres> { voorkeursAdres }));
            repositoryProviderMock.Setup(src => src.RepositoryGet<PersoonsAdres>())
                                  .Returns(new DummyRepo<PersoonsAdres>(new List<PersoonsAdres> { voorkeursPa }));
            Factory.InstantieRegistreren(repositoryProviderMock.Object);

            // ACT

            var target = Factory.Maak<GelieerdePersonenService>();
            target.AdresVerwijderenVanPersonen(new[] { gelieerdePersoon.Persoon.ID }, voorkeursAdres.ID);

            // ASSERT

            adressenSyncMock.Verify(src => src.StandaardAdressenBewaren(It.IsAny<IList<PersoonsAdres>>()),
                                    Times.Never());

        }

        /// <summary>
        /// Als je van een adres enkel de deelgemeente moet veranderen, dan moet die wijziging bewaard worden.
        /// Omdat GAP adressen als hetzelfde beschouwt als straat (bepaalt ook postnummer), nummer en bus 
        /// overeenkomen, is dat niet zo vanzelfsprekend als het lijkt.
        /// </summary>
        [TestMethod()]
        public void GelieerdePersonenVerhuizenWoonplaatsTest()
        {
            // ARRANGE.

            #region testdata

            var gelieerdePersoon = new GelieerdePersoon
                                   {
                                       ID = 1,
                                       Persoon = new Persoon
                                                 {
                                                     InSync = true,
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
                                                                                                     "Nieuwstraat",
                                                                                                 PostNummer
                                                                                                     =
                                                                                                     2560
                                                                                             },
                                                                                         HuisNr = 77,
                                                                                         WoonPlaats =
                                                                                             new WoonPlaats
                                                                                             {
                                                                                                 ID = 5,
                                                                                                 Naam =
                                                                                                     "Nijlen",
                                                                                                 PostNummer
                                                                                                     = 2560
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
                StraatNaamNaam = ((BelgischAdres)gelieerdePersoon.PersoonsAdres.Adres).StraatNaam.Naam,
                HuisNr = gelieerdePersoon.PersoonsAdres.Adres.HuisNr,
                PostNr = ((BelgischAdres)gelieerdePersoon.PersoonsAdres.Adres).StraatNaam.PostNummer,
                WoonPlaatsNaam = "Kessel",
                AdresType = AdresTypeEnum.Thuis
            };
            #endregion

            #region Dependency injection

            // mocks voor data access
            var repositoryProviderMock = new Mock<IRepositoryProvider>();
            repositoryProviderMock.Setup(src => src.RepositoryGet<Adres>())
                                  .Returns(new DummyRepo<Adres>(new List<Adres> { gelieerdePersoon.PersoonsAdres.Adres }));
            repositoryProviderMock.Setup(src => src.RepositoryGet<StraatNaam>())
                                  .Returns(
                                      new DummyRepo<StraatNaam>(new List<StraatNaam>
                                                                    {
                                                                        ((BelgischAdres)
                                                                         gelieerdePersoon.PersoonsAdres
                                                                                         .Adres)
                                                                            .StraatNaam
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



            // mocks registreren bij dependency-injectioncontainer
            Factory.InstantieRegistreren(repositoryProviderMock.Object);

            #endregion

            // ACT
            var target = Factory.Maak<GelieerdePersonenService>();
            target.GelieerdePersonenVerhuizen(new[] { gelieerdePersoon.ID }, infoNieuwAdres,
                                              gelieerdePersoon.PersoonsAdres.Adres.ID);

            // ASSERT
            Assert.AreEqual((gelieerdePersoon.PersoonsAdres.Adres as BelgischAdres).WoonPlaats.Naam, infoNieuwAdres.WoonPlaatsNaam);
        }

        /// <summary>
        /// Controleert of bij het ophalen van een gezin waarbij een persoon aangesloten is bij
        /// meerdere groepen, de juiste gelieerde persoon wordt opgehaald.
        /// </summary>
        [TestMethod()]
        public void GezinOphalenTest()
        {
            // ARRANGE

            var persoon = new Persoon { ID = 4 };

            var groep6 = new ChiroGroep { ID = 6 };
            var groep8 = new ChiroGroep { ID = 8 };

            var gelieerdePersoon5 = new GelieerdePersoon 
                        {
                            ID = 5,
                            Persoon = persoon,
                            Groep = groep6
                        };
            var gelieerdePersoon7 = new GelieerdePersoon
                        {
                            ID = 7,
                            Persoon = persoon,
                            Groep = groep8
                        };

            var persoonsAdres = new PersoonsAdres
            {
                ID = 1,
                Adres = new BelgischAdres { ID = 2 },
                Persoon = new Persoon
                {
                    ID = 3,
                    GelieerdePersoon = new List<GelieerdePersoon>
                    {
                        gelieerdePersoon5,
                        gelieerdePersoon7
                    }
                }
            };

            persoonsAdres.Adres.PersoonsAdres.Add(persoonsAdres);

            var repositoryProviderMock = new Mock<IRepositoryProvider>();
            repositoryProviderMock.Setup(src => src.RepositoryGet<Groep>()).Returns(new DummyRepo<Groep>(new List<Groep> { groep6, groep8 }));
            repositoryProviderMock.Setup(src => src.RepositoryGet<Adres>()).Returns(new DummyRepo<Adres>(new List<Adres> { persoonsAdres.Adres }));

            Factory.InstantieRegistreren(repositoryProviderMock.Object);

            // ACT
            
            var target = Factory.Maak<GelieerdePersonenService>();
            var result = target.GezinOphalen(persoonsAdres.Adres.ID, gelieerdePersoon7.Groep.ID);

            // ASSERT

            Assert.IsNotNull(result.Bewoners.Where(bew => bew.GelieerdePersoonID == gelieerdePersoon7.ID).FirstOrDefault());
            Assert.IsNull(result.Bewoners.Where(bew => bew.GelieerdePersoonID == gelieerdePersoon5.ID).FirstOrDefault());

        }

        /// <summary>
        /// Als je van een adres enkel de deelgemeente moet veranderen, dan moet die wijziging bewaard worden.
        /// Test voor buitenlands adres.
        /// </summary>
        [TestMethod()]
        public void GelieerdePersonenVerhuizenWoonplaatsBuitenlandTest()
        {
            // ARRANGE.

            #region testdata

            var gelieerdePersoon = new GelieerdePersoon
            {
                ID = 1,
                Groep = new ChiroGroep {GroepsWerkJaar = new List<GroepsWerkJaar> {new GroepsWerkJaar()}},
                Persoon = new Persoon
                {
                    InSync = true,
                    PersoonsAdres = new List<PersoonsAdres>
                    {
                        new PersoonsAdres
                        {
                            Adres = new BuitenLandsAdres()
                            {
                                ID = 2,
                                Straat = "Rue Nouvelle",
                                PostCode = "12345",
                                HuisNr = 77,
                                WoonPlaats = "Nilin",
                                Land = new Land {Naam = "Frankrijk"}
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
                StraatNaamNaam = ((BuitenLandsAdres)gelieerdePersoon.PersoonsAdres.Adres).Straat,
                HuisNr = gelieerdePersoon.PersoonsAdres.Adres.HuisNr,
                WoonPlaatsNaam = "Kessle",
                LandNaam = ((BuitenLandsAdres)gelieerdePersoon.PersoonsAdres.Adres).Land.Naam,
                AdresType = AdresTypeEnum.Thuis
            };
            #endregion

            #region Dependency injection

            // mocks voor data access
            var repositoryProviderMock = new Mock<IRepositoryProvider>();
            repositoryProviderMock.Setup(src => src.RepositoryGet<Adres>())
                                  .Returns(new DummyRepo<Adres>(new List<Adres> { gelieerdePersoon.PersoonsAdres.Adres }));
            repositoryProviderMock.Setup(src => src.RepositoryGet<Land>())
                .Returns(
                    new DummyRepo<Land>(new List<Land>
                                        {
                                            ((BuitenLandsAdres)
                                                gelieerdePersoon.PersoonsAdres
                                                .Adres)
                                                .Land
                                        }));
            repositoryProviderMock.Setup(src => src.RepositoryGet<StraatNaam>())
                .Returns(new DummyRepo<StraatNaam>(new List<StraatNaam>()));
            repositoryProviderMock.Setup(src => src.RepositoryGet<WoonPlaats>())
                .Returns(new DummyRepo<WoonPlaats>(new List<WoonPlaats>()));

            // mocks registreren bij dependency-injectioncontainer
            Factory.InstantieRegistreren(repositoryProviderMock.Object);

            #endregion

            // ACT
            var target = Factory.Maak<GelieerdePersonenService>();
            target.GelieerdePersonenVerhuizen(new[] { gelieerdePersoon.ID }, infoNieuwAdres,
                                              gelieerdePersoon.PersoonsAdres.Adres.ID);

            // ASSERT
            Assert.AreEqual((gelieerdePersoon.PersoonsAdres.Adres as BuitenLandsAdres).WoonPlaats, infoNieuwAdres.WoonPlaatsNaam);
        }
    }
}
