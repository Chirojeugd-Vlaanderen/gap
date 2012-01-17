using Chiro.Cdf.Ioc;
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
    ///This is a test class for LedenManagerTest and is intended
    ///to contain all LedenManagerTest Unit Tests
    ///</summary>
    [TestClass()]
    public class LedenManagerTest
    {


        private TestContext testContextInstance;

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
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
        /// run code before running each test
        /// </summary>
        [TestInitialize()]
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
        ///</summary>
        [TestMethod()]
        public void BewarenTest()
        {
            // arrange

            var ledenSyncMock = new Mock<ILedenSync>();
            ledenSyncMock.Setup(snc => snc.Bewaren(It.IsAny<Lid>()));   // verwacht dat ledensync een lid moet bewaren

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
                              GroepsWerkJaar = new GroepsWerkJaar {Groep = new KaderGroep()}
                          };

            // act

            target.Bewaren(lid, LidExtras.Geen, true);

            // assert: controleer of de ledensync is aangeroepen

            ledenSyncMock.Verify(snc => snc.Bewaren(It.IsAny<Lid>()));
            Assert.IsTrue(true);
        }
    }
}
