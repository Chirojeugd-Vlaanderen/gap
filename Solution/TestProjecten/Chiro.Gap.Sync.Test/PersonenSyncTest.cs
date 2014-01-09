using System.Collections.Generic;
using Chiro.Cdf.Ioc;
using Chiro.Gap.Sync;
using Chiro.Kip.ServiceContracts;
using Chiro.Kip.ServiceContracts.DataContracts;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using Chiro.Gap.Poco.Model;
using Moq;
using Persoon = Chiro.Kip.ServiceContracts.DataContracts.Persoon;

namespace Chiro.Gap.Sync.Test
{
    
    
    /// <summary>
    ///This is a test class for PersonenSyncTest and is intended
    ///to contain all PersonenSyncTest Unit Tests
    ///</summary>
    [TestClass()]
    public class PersonenSyncTest
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
        ///A test for Bewaren
        ///</summary>
        [TestMethod()]
        public void BewarenMetCommunicatieTest()
        {
            // ARRANGE

            var kipSyncMock = new Mock<ISyncPersoonService>();
            kipSyncMock.Setup(src => src.AlleCommunicatieBewaren(It.IsAny<Persoon>(), It.IsAny<IEnumerable<CommunicatieMiddel>>())).Verifiable();
            Factory.InstantieRegistreren(kipSyncMock.Object);

            var gelieerdePersoon = new GelieerdePersoon
                                       {
                                           Persoon = new Gap.Poco.Model.Persoon { InSync = true },
                                           Communicatie =
                                               new List<CommunicatieVorm> 
                                               {
                                                   new CommunicatieVorm 
                                                   { 
                                                       CommunicatieType = new Chiro.Gap.Poco.Model.CommunicatieType { ID = 3 } 
                                                   }
                                               }
                                       };
            gelieerdePersoon.Persoon.GelieerdePersoon.Add(gelieerdePersoon);

            // ACT

            var target = new PersonenSync();
            target.Bewaren(gelieerdePersoon, false, true);

            // ASSERT

            kipSyncMock.VerifyAll();
        }
    }
}
