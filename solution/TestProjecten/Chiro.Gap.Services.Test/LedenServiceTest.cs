﻿using Chiro.Gap.Dummies;
using Chiro.Gap.Poco.Model;
using Chiro.Gap.ServiceContracts;
using Chiro.Gap.SyncInterfaces;
using Chiro.Gap.WorkerInterfaces;
using Chiro.Gap.Workers;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Transactions;

using Chiro.Cdf.Ioc;
using Chiro.Gap.Domain;
using Chiro.Gap.ServiceContracts.Mappers;
using Chiro.Gap.TestDbInfo;
using Chiro.Gap.ServiceContracts.DataContracts;
using Chiro.Cdf.Poco;
using Moq;
using GebruikersRecht = Chiro.Gap.Poco.Model.GebruikersRecht;
using Chiro.Gap.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting.Web;

namespace Chiro.Gap.Services.Test
{

    /// <summary>
    /// Dit is een testclass voor Unit Tests van LedenServiceTest,
    /// to contain all LedenServiceTest Unit Tests
    /// </summary>
    [TestClass]
    public class LedenServiceTest
    {
        private ILedenService _ledenService = null;
        private IGelieerdePersonenService _personenSvc = null;

        private TestContext testContextInstance;

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        /// </summary>
        public TestContext TestContext
        {
            get { return testContextInstance; }
            set { testContextInstance = value; }
        }

        #region Additional test attributes

        // Use ClassInitialize to run static code before running the first test in the class
        //[ClassInitialize]
        //public static void MyClassInitialize(TestContext testContext)
        //{
        //}

        //Use ClassCleanup to run code after all tests in a class have run
        //[ClassCleanup()]
        //public static void MyClassCleanup()
        //{
        //}

        //Use TestInitialize to run code before running each test
        [TestInitialize]
        public void MyTestInitialize()
        {
            // Reset de IOC-container voor iedere test.
            Factory.ContainerInit();
            MappingHelper.MappingsDefinieren();
        }

        //Use TestCleanup to run code after each test has run
        //[TestCleanup()]
        //public void MyTestCleanup()
        //{
        //}
        //

        #endregion


        /// <summary>
        ///Kijkt na of opgehaalde functies goed gemapt worden.
        /// </summary>
        [TestMethod]
        public void OphalenTest()
        {
            // Act
            const int lidID = TestInfo.LID3_ID;

            var actual = _ledenService.DetailsOphalen(lidID);

            // Assert
            var ids = (from f in actual.LidInfo.Functies select f.ID);
            Assert.IsTrue(ids.Contains((int) NationaleFunctie.ContactPersoon));
            Assert.IsTrue(ids.Contains(TestInfo.FUNCTIE_ID));
        }

        ///<summary>
        /// A test for FunctiesVervangen
        /// </summary>
        [TestMethod]
        public void FunctiesVervangenTest()
        {
            #region arrange

            // testdata genereren

            var gwj = new GroepsWerkJaar();
            var groep = new ChiroGroep
                            {
                                GroepsWerkJaar = new List<GroepsWerkJaar> {gwj}
                            };
            gwj.Groep = groep;

            var contactPersoon = new Functie
                                     {
                                         ID = 1,
                                         IsNationaal = true,
                                         Niveau = Niveau.Alles,
                                         Naam = "Contactpersoon",
                                         Type = LidType.Leiding
                                     };
            var finVer = new Functie
                             {
                                 ID = 2,
                                 IsNationaal = true,
                                 Niveau = Niveau.Alles,
                                 Naam = "FinancieelVerantwoordelijke",
                                 Type = LidType.Leiding
                             };
            var vb = new Functie
                         {
                             ID = 3,
                             IsNationaal = true,
                             Niveau = Niveau.Alles,
                             Naam = "VB",
                             Type = LidType.Leiding
                         };
            var redactie = new Functie
                               {
                                   ID = 4,
                                   IsNationaal = false,
                                   Niveau = Niveau.Groep,
                                   Naam = "RED",
                                   Type = LidType.Leiding,
                                   Groep = groep
                               };
            var leiding = new Leiding
                              {
                                  ID = 100,
                                  GroepsWerkJaar = gwj,
                                  Functie = new List<Functie> {contactPersoon, redactie},
                                  GelieerdePersoon = new GelieerdePersoon {Groep = groep}
                              };

            // repositories maken die de testdata opleveren

            var ledenRepo = new DummyRepo<Lid>(new[] {leiding});
            var functieRepo = new DummyRepo<Functie>(new[] {contactPersoon, finVer, vb, redactie});

            // repositoryprovider opzetten
            var repoProviderMock = new Mock<IRepositoryProvider>();
            repoProviderMock.Setup(src => src.RepositoryGet<Lid>()).Returns(ledenRepo);
            repoProviderMock.Setup(src => src.RepositoryGet<Functie>()).Returns(functieRepo);
            Factory.InstantieRegistreren(repoProviderMock.Object);

            // om #1413 te detecteren, moeten we de echte authorisatiemanager
            // gebruiken. Dat wil dan weer zeggen dat we met de authenticatiemanager
            // moeten foefelen, en gebruikersrechten moeten simuleren.

            var authenticatieMgrMock = new Mock<IAuthenticatieManager>();
            authenticatieMgrMock.Setup(src => src.GebruikersNaamGet()).Returns("testGebruiker");
            groep.GebruikersRecht.Add(new GebruikersRecht
                                          {
                                              Groep = groep,
                                              Gav = new Gav
                                                        {
                                                            Login = "testGebruiker"
                                                        }
                                          });

            Factory.InstantieRegistreren<IAutorisatieManager>(new AutorisatieManager(authenticatieMgrMock.Object));

            // Hier maken we uiteindelijk de ledenservice

            var ledenService = Factory.Maak<LedenService>();
            #endregion

            #region act

            var leidingsFuncties = leiding.Functie;

            IEnumerable<int> functieIDs = new int[]
                                              {
                                                  finVer.ID,
                                                  vb.ID,
                                                  redactie.ID
                                              };

            ledenService.FunctiesVervangen(leiding.ID, functieIDs);

            #endregion

            #region Assert

            Assert.AreEqual(leiding.Functie.Count(), 3);
            Assert.IsTrue(leiding.Functie.Contains(finVer));
            Assert.IsTrue(leiding.Functie.Contains(vb));
            Assert.IsTrue(leiding.Functie.Contains(redactie));

            // om problemen te vermijden met entity framework, mag je bestaande collecties niet zomaar vervangen;
            // je moet entiteiten toevoegen aan/verwijderen uit bestaande collecties.
            Assert.AreEqual(leiding.Functie, leidingsFuncties);

            #endregion


        }

        [TestMethod]
        public void TestLidMakenBuitenVoorstel()
        {
            using (var ts = new TransactionScope())
            {

                #region Arrange

                string fouten;

                // Maak een nieuwe persoon
                var gp = _personenSvc.AanmakenForceer(
                    new PersoonInfo
                        {
                            AdNummer = null,
                            ChiroLeefTijd = 0,
                            GeboorteDatum = new System.DateTime(2003, 5, 8),
                            Geslacht = GeslachtsType.Vrouw,
                            Naam = "TestLidMakenBuitenVoorstel",
                            VoorNaam = "Sabine",
                        },
                    groepID: TestInfo.GROEP_ID,
                    forceer: true);

                // GP2 zit niet in een afdeling, we vragen zijn voorgestelde afdeling en steken hem/haar dan in de andere
                var gelieerdePersoonIDs = new List<int> {gp.GelieerdePersoonID};

                #endregion

                var voorstel = _ledenService.VoorstelTotInschrijvenGenereren(gelieerdePersoonIDs, out fouten).First();
                int gekozenafdelingsjaarID = voorstel.AfdelingsJaarIDs.Contains(TestInfo.AFDELINGS_JAAR2_ID)
                                                 ? TestInfo.AFDELINGS_JAAR1_ID
                                                 : TestInfo.AFDELINGS_JAAR2_ID;
                voorstel.AfdelingsJaarIDs = new[] {gekozenafdelingsjaarID};
                voorstel.AfdelingsJaarIrrelevant = false;
                var defvoorstel = new InTeSchrijvenLid[] {voorstel};

                #region Act

                int lidID = _ledenService.Inschrijven(defvoorstel, out fouten).First();

                #endregion

                #region Assert

                var l = _ledenService.DetailsOphalen(lidID);
                Assert.IsTrue(l.LidInfo.AfdelingIdLijst.Contains(TestInfo.AFDELING2_ID));

                #endregion

                // We committen de transactie niet, zodat we het lid achteraf niet
                // opnieuw moeten uitschrijven.

            } // Rollback
        }

        /// <summary>
        /// Controleert of de return value van een inschrijvingsvoorstel de leden sorteert op 
        /// geboortedatum met Chiroleeftijd.  Dat is op dit moment belangrijk omdat de ledenlijst
        /// bij de jaarovergang anders door elkaar staat; we kunnen die aan de UI-kant nog niet
        /// helemaal sorteren.  Zie ticket #1391.  (Als er iets is aangepast waardoor deze test
        /// wel moet failen, moet ticket #1391 mogelijk opnieuw geopend worden.)
        /// </summary>
        [TestMethod()]
        public void VoorstelTotInschrijvenGenererenTest()
        {
            throw new NotImplementedException(NIEUWEBACKEND.Info);
            //// Ik ga hier toch niet met data uit de database werken.
            //// Om problemen te vermijden escape ik hier dan maar de IOC-container.

            //// Gauw wat handgemaakte dummydata:

            //var g = new ChiroGroep
            //              {
            //                  ID = 1,
            //                  GroepsWerkJaar = {new GroepsWerkJaar()},
            //                  GelieerdePersoon =
            //                      {
            //                          new GelieerdePersoon
            //                              {ID = 1, Persoon = new Persoon {GeboorteDatum = new DateTime(1987, 3, 8)}},
            //                          new GelieerdePersoon
            //                              {ID = 2, Persoon = new Persoon {GeboorteDatum = new DateTime(1989, 3, 2)}}
            //                      }
            //              };

            //var gwj = g.GroepsWerkJaar.FirstOrDefault();

            //// We mocken een en ander:

            //var gpmMock = new Mock<IGelieerdePersonenManager>();

            //var gwjmMock = new Mock<IGroepsWerkJaarManager>();

            //var lmMock = new Mock<ILedenManager>();

            //// Simulatie: 'het lid bestaat nog niet'
            //lmMock.Setup(src => src.OphalenViaPersoon(It.IsAny<int>(), It.IsAny<int>())).Returns((Lid)null);
            //// Simulatie: 'we maken de persoon leid(st)er'
            //lmMock.Setup(
            //    src => src.InschrijvingVoorstellen(It.IsAny<GelieerdePersoon>(), It.IsAny<GroepsWerkJaar>(), true)).
            //    Returns(
            //        new LidVoorstel {AfdelingsJaarIDs = null, AfdelingsJarenIrrelevant = true, LeidingMaken = true});

            //IGelieerdePersonenManager gpm = gpmMock.Object;
            //IGroepsWerkJaarManager gwjm = gwjmMock.Object;
            //ILedenManager lm = lmMock.Object;

            //// Dingen die we niet gebruiken, mogen null blijven.

            //FunctiesManager fm = null;
            //AfdelingsJaarManager ajm = null; 
            //VerzekeringenManager vrzm = null;
            //LedenService target = new LedenService(gpm, lm, fm, ajm, gwjm, vrzm);

            //IEnumerable<int> gelieerdePersoonIDs = new[] {1, 2};
            //string foutBerichten = string.Empty;
            //IEnumerable<InTeSchrijvenLid> actual;
            //actual = target.VoorstelTotInschrijvenGenereren(gelieerdePersoonIDs, out foutBerichten);

            //// We verwachten nu dat de personen opgeleverd worden van jong naar oud.  Dus
            //// eerst persoon 2, dan persoon 1.

            //Assert.IsTrue(actual.First().GelieerdePersoonID == 2);
            //Assert.IsTrue(actual.Last().GelieerdePersoonID == 1);
        }


        ///<summary>
        ///Controleert of het vervangen van functies gesynct wordt naar Kipadmin.
        ///</summary>
        [TestMethod()]
        public void FunctiesVervangenSyncTest()
        {
            #region arrange

            // testdata genereren

            var gwj = new GroepsWerkJaar();
            var groep = new ChiroGroep
            {
                GroepsWerkJaar = new List<GroepsWerkJaar> { gwj }
            };
            gwj.Groep = groep;

            var contactPersoon = new Functie
            {
                ID = 1,
                IsNationaal = true,
                Niveau = Niveau.Alles,
                Naam = "Contactpersoon",
                Type = LidType.Leiding
            };

            var leiding = new Leiding
            {
                ID = 100,
                GroepsWerkJaar = gwj,
                Functie = new List<Functie>(),
                GelieerdePersoon = new GelieerdePersoon { Groep = groep }
            };

            // repositories maken die de testdata opleveren
            var ledenRepo = new DummyRepo<Lid>(new[] { leiding });
            var functieRepo = new DummyRepo<Functie>(new[] { contactPersoon });

            // repositoryprovider opzetten
            var repoProviderMock = new Mock<IRepositoryProvider>();
            repoProviderMock.Setup(src => src.RepositoryGet<Lid>()).Returns(ledenRepo);
            repoProviderMock.Setup(src => src.RepositoryGet<Functie>()).Returns(functieRepo);
            Factory.InstantieRegistreren(repoProviderMock.Object);

            // sync mocken.
            var ledenSyncMock = new Mock<ILedenSync>();
            ledenSyncMock.Setup(src => src.FunctiesUpdaten(leiding)).Verifiable();
            Factory.InstantieRegistreren(ledenSyncMock.Object);

            // Hier maken we uiteindelijk de ledenservice

            var ledenService = Factory.Maak<LedenService>();
            #endregion

            #region act
            ledenService.FunctiesVervangen(leiding.ID, new [] { contactPersoon.ID });
            #endregion

            #region Assert
            ledenSyncMock.Verify();
            #endregion
        }

        ///<summary>
        /// Controleert of de uitschrijving van kadermedewerkers wordt gesynct naar kipadmin.
        /// (Voor kadermedewerkers is er geen probeerperiode, want kaderinschrijvingen zijn gratis)
        ///</summary>
        [TestMethod()]
        public void UitschrijvenKaderSyncTest()
        {
            // arrange

            // testsituatie opbouwen
            var groepsWerkJaar = new GroepsWerkJaar {Groep = new KaderGroep {NiveauInt = (int) Niveau.Gewest}};
            groepsWerkJaar.Groep.GroepsWerkJaar = new List<GroepsWerkJaar> { groepsWerkJaar };

            var gelieerdePersoon = new GelieerdePersoon {ID = 1, Groep = groepsWerkJaar.Groep};
            groepsWerkJaar.Groep.GelieerdePersoon = new List<GelieerdePersoon> { gelieerdePersoon };          

            var medewerker = new Leiding
                                 {
                                     EindeInstapPeriode = DateTime.Today,
                                     // probeerperiode kadermedewerker is irrelevant
                                     GroepsWerkJaar = groepsWerkJaar,
                                     GelieerdePersoon = gelieerdePersoon
                                 };
            gelieerdePersoon.Lid = new List<Lid> {medewerker};
            


            // data access opzetten
            var dummyLeidingRepo = new DummyRepo<Leiding>(new List<Leiding>{medewerker});
            var dummyGpRepo = new DummyRepo<GelieerdePersoon>(new List<GelieerdePersoon> {medewerker.GelieerdePersoon});
            var repoProviderMock = new Mock<IRepositoryProvider>();
            repoProviderMock.Setup(src => src.RepositoryGet<Leiding>()).Returns(dummyLeidingRepo);
            repoProviderMock.Setup(src => src.RepositoryGet<GelieerdePersoon>()).Returns(dummyGpRepo);
            Factory.InstantieRegistreren(repoProviderMock.Object);

            // synchronisatie mocken
            var ledenSyncMock = new Mock<ILedenSync>();
            ledenSyncMock.Setup(snc => snc.Verwijderen(It.IsAny<Lid>())).Verifiable();   // verwacht dat ledensync een lid moet bewaren
            Factory.InstantieRegistreren(ledenSyncMock.Object);

            var target = Factory.Maak<LedenService>();

            // ACT

            string foutbericht;
            target.Uitschrijven(new[] {medewerker.GelieerdePersoon.ID}, out foutbericht);

            // ASSERT: controleer of de ledensync is aangeroepen

            ledenSyncMock.VerifyAll();
        }

        /// <summary>
        ///A test for Uitschrijven
        ///</summary>
        [TestMethod()]
        public void UitschrijvenTest()
        {
            // ARRANGE

            // testsituatie opbouwen
            var groepsWerkJaar = new GroepsWerkJaar { Groep = new KaderGroep { NiveauInt = (int)Niveau.Gewest } };
            groepsWerkJaar.Groep.GroepsWerkJaar = new List<GroepsWerkJaar> { groepsWerkJaar };

            var gelieerdePersoon1 = new GelieerdePersoon { ID = 1, Groep = groepsWerkJaar.Groep };
            var gelieerdePersoon2 = new GelieerdePersoon { ID = 2, Groep = groepsWerkJaar.Groep };
            groepsWerkJaar.Groep.GelieerdePersoon = new List<GelieerdePersoon> { gelieerdePersoon1, gelieerdePersoon2 };

            var medewerker1 = new Leiding
            {
                EindeInstapPeriode = DateTime.Today,
                // probeerperiode kadermedewerker is irrelevant
                GroepsWerkJaar = groepsWerkJaar,
                GelieerdePersoon = gelieerdePersoon1
            };
            var medewerker2 = new Leiding
            {
                EindeInstapPeriode = DateTime.Today,
                // probeerperiode kadermedewerker is irrelevant
                GroepsWerkJaar = groepsWerkJaar,
                GelieerdePersoon = gelieerdePersoon2
            };
            gelieerdePersoon1.Lid = new List<Lid> { medewerker1 };
            gelieerdePersoon2.Lid = new List<Lid> { medewerker2 };

            // data access opzetten
            var dummyLeidingRepo = new DummyRepo<Leiding>(new List<Leiding> { medewerker1, medewerker2 });
            var dummyGpRepo = new DummyRepo<GelieerdePersoon>(groepsWerkJaar.Groep.GelieerdePersoon.ToList());
            var repoProviderMock = new Mock<IRepositoryProvider>();
            repoProviderMock.Setup(src => src.RepositoryGet<Leiding>()).Returns(dummyLeidingRepo);
            repoProviderMock.Setup(src => src.RepositoryGet<GelieerdePersoon>()).Returns(dummyGpRepo);
            Factory.InstantieRegistreren(repoProviderMock.Object);

            var target = Factory.Maak<LedenService>();

            // ACT

            string foutBerichten = string.Empty; 
            target.Uitschrijven(new [] {gelieerdePersoon1.ID}, out foutBerichten);

            // ASSERT

            Assert.IsNotNull(medewerker1.UitschrijfDatum);  // medewerker 1 uitgeschreven
            Assert.IsNull(medewerker2.UitschrijfDatum);     // medewerker 2 niet uitgeschreven
        }

        ///<summary>
        /// Test of leiding waarvan de probeerperiode voor bij is, daadwerkelijk NIET gesynct
        /// wordt met Kipadmin.
        ///</summary>
        [TestMethod()]
        public void UitschrijvenLeidingSyncTest()
        {
            // arrange

            // testsituatie opbouwen
            var groepsWerkJaar = new GroepsWerkJaar { Groep = new ChiroGroep () };
            groepsWerkJaar.Groep.GroepsWerkJaar = new List<GroepsWerkJaar> { groepsWerkJaar };

            var gelieerdePersoon = new GelieerdePersoon { ID = 1, Groep = groepsWerkJaar.Groep };
            groepsWerkJaar.Groep.GelieerdePersoon = new List<GelieerdePersoon> { gelieerdePersoon };

            var leiding = new Leiding
            {
                EindeInstapPeriode = DateTime.Today, // (defaults naar 0:00 uur; instapperiode voorbij)
                GroepsWerkJaar = groepsWerkJaar,
                GelieerdePersoon = gelieerdePersoon
            };
            gelieerdePersoon.Lid = new List<Lid> { leiding };



            // data access opzetten
            var dummyLeidingRepo = new DummyRepo<Leiding>(new List<Leiding> { leiding });
            var dummyGpRepo = new DummyRepo<GelieerdePersoon>(new List<GelieerdePersoon> { leiding.GelieerdePersoon });
            var repoProviderMock = new Mock<IRepositoryProvider>();
            repoProviderMock.Setup(src => src.RepositoryGet<Leiding>()).Returns(dummyLeidingRepo);
            repoProviderMock.Setup(src => src.RepositoryGet<GelieerdePersoon>()).Returns(dummyGpRepo);
            Factory.InstantieRegistreren(repoProviderMock.Object);

            // synchronisatie mocken
            var ledenSyncMock = new Mock<ILedenSync>();
            ledenSyncMock.Setup(snc => snc.Verwijderen(It.IsAny<Lid>())).Verifiable();   // verwacht dat ledensync een lid moet bewaren
            Factory.InstantieRegistreren(ledenSyncMock.Object);

            var target = Factory.Maak<LedenService>();

            // ACT

            string foutbericht;
            target.Uitschrijven(new[] { leiding.GelieerdePersoon.ID }, out foutbericht);

            // ASSERT: controleer dat de ledensync NIET werd aangeroepen

            ledenSyncMock.Verify(src=>src.Verwijderen(leiding), Times.Never());
        }

        ///<summary>
        ///Controleert of zoeken op leden geen uitgeschreven leden oplevert.
        ///</summary>
        [TestMethod()]
        public void ZoekenTest()
        {
            // ARRANGE

            // testdata: een werkjaar met 1 lid, dat vandaag werd uitgeschreven.
            var kind = new Kind
                           {
                               UitschrijfDatum = DateTime.Today,
                               GelieerdePersoon = new GelieerdePersoon {Persoon = new Persoon()},
                               AfdelingsJaar = new AfdelingsJaar {Afdeling = new Afdeling()}
                           };
            var gwj = new GroepsWerkJaar {ID = 1, Lid = new[] {kind}};
            kind.GroepsWerkJaar = gwj;

            // dependency injection opzetten
            var repositoryProviderMock = new Mock<IRepositoryProvider>();
            repositoryProviderMock.Setup(src => src.RepositoryGet<Lid>()).Returns(new DummyRepo<Lid>(gwj.Lid.ToList()));
            Factory.InstantieRegistreren(repositoryProviderMock.Object);

            // service construeren
            var target = Factory.Maak<LedenService>();

            // ACT

            var filter = new LidFilter { GroepsWerkJaarID = gwj.ID };
            var actual = target.Zoeken(filter, false);

            // ASSERT

            Assert.IsNull(actual.FirstOrDefault());
        }

        /// <summary>
        ///Controleert of een uitgeschreven lid opnieuw ingeschreven kan worden.
        ///</summary>
        [TestMethod()]
        public void InschrijvenTest()
        {
            // ARRANGE

            // testdata
            var groepsWerkJaar = new GroepsWerkJaar {Groep = new ChiroGroep()};
            groepsWerkJaar.Groep.GroepsWerkJaar.Add(groepsWerkJaar);

            var leider = new Leiding {UitschrijfDatum = DateTime.Today, GroepsWerkJaar = groepsWerkJaar};
            var gp = new GelieerdePersoon  // gelieerde persoon, uitgeschreven als leiding
                         {
                             ID = 1,
                             Persoon = new Persoon {Naam = "Bosmans", VoorNaam = "Jos"},
                             Lid = new List<Lid> {leider},
                             Groep = groepsWerkJaar.Groep
                         };

            // dependency injection
            var repositoryProviderMock = new Mock<IRepositoryProvider>();
            repositoryProviderMock.Setup(src => src.RepositoryGet<GelieerdePersoon>())
                                  .Returns(new DummyRepo<GelieerdePersoon>(new List<GelieerdePersoon> {gp}));
            Factory.InstantieRegistreren(repositoryProviderMock.Object);

            var target = Factory.Maak<LedenService>();

            // ACT
            var lidInformatie = new[]
                                    {
                                        new InTeSchrijvenLid
                                            {
                                                AfdelingsJaarIrrelevant = true,
                                                GelieerdePersoonID = gp.ID,
                                                LeidingMaken = true,
                                                VolledigeNaam = gp.Persoon.VolledigeNaam
                                            }
                                    };
            string foutBerichten;
            target.Inschrijven(lidInformatie, out foutBerichten);

            // ASSERT
            Assert.IsNull(leider.UitschrijfDatum);
        }
    }
}