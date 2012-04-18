using Chiro.Cdf.Ioc;
using Chiro.Gap.Domain;
using Chiro.Gap.Workers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using Chiro.Gap.Orm.DataInterfaces;
using Chiro.Gap.Orm.SyncInterfaces;
using Chiro.Gap.Orm;

using Moq;

namespace Chiro.Gap.Workers.Test.CustomIoc
{
    
    
    /// <summary>
    /// Dit is een testclass voor LedenManagerTest,
    ///to contain all LedenManagerTest Unit Tests
    /// </summary>
    [TestClass()]
    public class LedenManagerTest
    {


        private TestContext testContextInstance;

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
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

        #region Additional test attributes
        // 
        //You can use the following additional attributes as you write your tests:
        //
        //Use ClassInitialize to run code before running the first test in the class
        //[ClassInitialize()]
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
        /// Controleert of uitschrijvingen van kadermedewerkers (waarvan de probeerperiode per definitie
        /// voorbij is, want ze hebben geen probeerperiode) toch gesynct worden.
        /// </summary>
        [TestMethod]
        public void KaderUitschrijvenTest()
        {
            // arrange

            var ledenSyncMock = new Mock<ILedenSync>();
            ledenSyncMock.Setup(snc => snc.Verwijderen(It.IsAny<Lid>()));   // verwacht dat ledensync een lid moet bewaren

            var leidingDaoMock = new Mock<ILeidingDao>();
            leidingDaoMock.Setup(dao => dao.Bewaren(It.IsAny<Leiding>(), It.IsAny<LidExtras>())).Returns(
                (Leiding x, LidExtras y) => x);  // bewaren doet niets behalven het originele leiding terug opleveren

            Factory.InstantieRegistreren(ledenSyncMock.Object);
            Factory.InstantieRegistreren(leidingDaoMock.Object);

            var target = Factory.Maak<LedenManager>();

            // construeer gauw een uitgeschreven kadermedewerker

            Lid lid = new Leiding
                          {
                              EindeInstapPeriode = DateTime.Today,  // probeerperiode kadermedewerker is irrelevant
                              NonActief = true,
                              GroepsWerkJaar = new GroepsWerkJaar {Groep = new KaderGroep {NiveauInt = (int)Niveau.Gewest}}
                          };

            // act

            target.Bewaren(lid, LidExtras.Geen, true);

            // assert: controleer of de ledensync is aangeroepen

            ledenSyncMock.Verify(snc => snc.Verwijderen(It.IsAny<Lid>()));
            Assert.IsTrue(true);
        }

        ///<summary>
        /// Controleert of uitschrijvingen van leiding waarvan de probeerperiode voorbij is,
        /// niet naar kipadmin gesynct worden.
        /// </summary>
        [TestMethod]
        public void LeidingUitschrijvenTest()
        {
            // arrange

            var ledenSyncMock = new Mock<ILedenSync>();
            ledenSyncMock.Setup(snc => snc.Verwijderen(It.IsAny<Lid>()));   // deze mag niet aangeroepen worden

            var leidingDaoMock = new Mock<ILeidingDao>();
            leidingDaoMock.Setup(dao => dao.Bewaren(It.IsAny<Leiding>(), It.IsAny<LidExtras>())).Returns(
                (Leiding x, LidExtras y) => x);  // bewaren doet niets behalven het originele leiding terug opleveren

            Factory.InstantieRegistreren(ledenSyncMock.Object);
            Factory.InstantieRegistreren(leidingDaoMock.Object);

            var target = Factory.Maak<LedenManager>();

            // construeer gauw een uitgeschreven leid(st)er

            Lid lid = new Leiding
            {
                EindeInstapPeriode = DateTime.Today,  // probeerperiode kadermedewerker is irrelevant
                NonActief = true,
                GroepsWerkJaar = new GroepsWerkJaar { Groep = new ChiroGroep() }
            };

            // act

            target.Bewaren(lid, LidExtras.Geen, true);

            // assert: controleer of de ledensync is aangeroepen

            ledenSyncMock.Verify(snc => snc.Verwijderen(It.IsAny<Lid>()), Times.Never());
            Assert.IsTrue(true);
        }


        ///<summary>
        ///Kijkt na of we een leid(st)er kunnen inschrijven zonder afdelingen
        ///</summary>
        [TestMethod()]
        public void InschrijvenTest()
        {
            // LedenManager_Accessor, zodat we ook private members kunnen testen.
            var target = Factory.Maak<LedenManager_Accessor>();

            // Creeer een aantal dummygegevens om op te testen.

            var groep = new ChiroGroep {ID = 493};
            var gp = new GelieerdePersoon
                         {
                             Persoon = new Persoon
                                           {
                                               Geslacht = GeslachtsType.Man,
                                               GeboorteDatum = new DateTime(1992, 3, 7)
                                           },
                             Groep = groep
                         };
            var gwj = new GroepsWerkJaar
                          {
                              Groep = groep,
                              WerkJaar = 2011
                          };
            var voorstellid = new LidVoorstel
                                  {
                                      AfdelingsJaarIDs = new int[0],
                                      AfdelingsJarenIrrelevant = false,
                                      LeidingMaken = true
                                  };

            // act

            var actual = target.Inschrijven(gp, gwj, false, voorstellid) as Leiding;

            // assert

            Assert.IsNotNull(actual);
            Assert.AreEqual(0, actual.AfdelingsJaar.Count);
        }
    }
}
