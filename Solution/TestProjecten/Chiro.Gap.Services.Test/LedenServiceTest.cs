using Chiro.Gap.Poco.Model;
using Chiro.Gap.ServiceContracts;
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
using Chiro.Gap.WorkerInterfaces;
using Moq;

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
        [ClassInitialize]
        public static void MyClassInitialize(TestContext testContext)
        {
            Factory.ContainerInit();
            MappingHelper.MappingsDefinieren();
        }

        //Use ClassCleanup to run code after all tests in a class have run
        //[ClassCleanup()]
        //public static void MyClassCleanup()
        //{
        //}

        //Use TestInitialize to run code before running each test
        [TestInitialize]
        public void MyTestInitialize()
        {
            _ledenService = Factory.Maak<LedenService>();
            _personenSvc = Factory.Maak<GelieerdePersonenService>();
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
            using (new TransactionScope())
            {
                #region act

                // Lid3 heeft functies contactpersoon en redactie (eigen functie)
                // Vervang door financieel verantwoordelijke, vb en redactie

                int lidID = TestInfo.LID3_ID;
                IEnumerable<int> functieIDs = new int[]
                                                  {
                                                      (int) NationaleFunctie.FinancieelVerantwoordelijke,
                                                      (int) NationaleFunctie.Vb,
                                                      TestInfo.FUNCTIE_ID
                                                  };
                _ledenService.FunctiesVervangen(lidID, functieIDs);

                #endregion

                #region Assert

                var l = _ledenService.DetailsOphalen(lidID);
                var funIDs = (from f in l.LidInfo.Functies select f.ID);

                Assert.AreEqual(funIDs.Count(), 3);
                Assert.IsTrue(funIDs.Contains((int) NationaleFunctie.FinancieelVerantwoordelijke));
                Assert.IsTrue(funIDs.Contains((int) NationaleFunctie.Vb));
                Assert.IsTrue(funIDs.Contains(TestInfo.FUNCTIE_ID));

                #endregion

            } // Rollback



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
    }
}
