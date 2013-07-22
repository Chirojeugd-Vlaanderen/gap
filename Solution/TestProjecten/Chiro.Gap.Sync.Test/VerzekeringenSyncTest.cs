using System.Collections.Generic;
using Chiro.Cdf.Ioc;
using Chiro.Gap.Sync;
using Chiro.Kip.ServiceContracts;
using Chiro.Kip.ServiceContracts.DataContracts;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using Chiro.Gap.Poco.Model;
using Moq;
using Persoon = Chiro.Gap.Poco.Model.Persoon;

namespace Chiro.Gap.Sync.Test
{
    
    
    /// <summary>
    ///This is a test class for VerzekeringenSyncTest and is intended
    ///to contain all VerzekeringenSyncTest Unit Tests
    ///</summary>
    [TestClass()]
    public class VerzekeringenSyncTest
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
        [ClassInitialize()]
        public static void MyClassInitialize(TestContext testContext)
        {
            MappingHelper.MappingsDefinieren();
        }
        //
        //Use ClassCleanup to run code after all tests in a class have run
        //[ClassCleanup()]
        //public static void MyClassCleanup()
        //{
        //}
        //
        //Use TestInitialize to run code before running each test
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


        /// <summary>
        /// Kijkt na of verzekeringen loonverlies ook gesynct worden voor personen zonder AD-nummer
        /// (dat AD-nummer is dan waarschijnlijk in aanvraag.)
        /// </summary>
        [TestMethod()]
        public void BewarenTest()
        {
            // ARRANGE

            var groepsWerkJaar = new GroepsWerkJaar {Groep = new ChiroGroep()};
            var gelieerdePersoon = new GelieerdePersoon {Groep = groepsWerkJaar.Groep, Persoon = new Persoon
                                                      {
                                                          AdNummer = null,
                                                      }};
            gelieerdePersoon.Persoon.GelieerdePersoon.Add(gelieerdePersoon);
            groepsWerkJaar.Groep.GelieerdePersoon.Add(gelieerdePersoon);
                                                                                                          
            var persoonsVerzekering = new PersoonsVerzekering
                                          {
                                              Persoon = gelieerdePersoon.Persoon
                                          };

            var kipSyncMock = new Mock<ISyncPersoonService>();
            kipSyncMock.Setup(
                src =>
                src.LoonVerliesVerzekerenAdOnbekend(It.IsAny<PersoonDetails>(), It.IsAny<string>(), It.IsAny<int>())).Verifiable();
            Factory.InstantieRegistreren(kipSyncMock.Object);

            // ACT
            var target = new VerzekeringenSync(); // TODO: Initialize to an appropriate value
            target.Bewaren(persoonsVerzekering, groepsWerkJaar);

            // ASSERT

            kipSyncMock.VerifyAll();
        }
    }
}
