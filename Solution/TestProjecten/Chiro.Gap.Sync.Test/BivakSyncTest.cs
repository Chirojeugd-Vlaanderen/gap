using Chiro.Cdf.Ioc;
using Chiro.Gap.Sync;
using Chiro.Kip.ServiceContracts;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using Chiro.Gap.Poco.Model;
using Moq;

namespace Chiro.Gap.Sync.Test
{
    
    
    /// <summary>
    ///This is a test class for BivakSyncTest and is intended
    ///to contain all BivakSyncTest Unit Tests
    ///</summary>
    [TestClass()]
    public class BivakSyncTest
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
        /// Als ik een bivak met adres bewaar, dan verwacht ik dat dat adres naar Kipadmin wordt gestuurd.
        /// </summary>
        [TestMethod()]
        public void BewarenTest()
        {
            // ARRANGE

            var kipSyncMock = new Mock<ISyncPersoonService>();
            kipSyncMock.Setup(src => src.BivakPlaatsBewaren(It.IsAny<int>(), It.IsAny<string>(), It.IsAny<Kip.ServiceContracts.DataContracts.Adres>())).Verifiable();
            Factory.InstantieRegistreren(kipSyncMock.Object);

            var uitstap = new Uitstap
                              {
                                  IsBivak = true,
                                  Plaats = new Plaats {Adres = new BuitenLandsAdres {Land = new Land()}},
                                  GroepsWerkJaar = new GroepsWerkJaar {Groep = new ChiroGroep()}
                              };
            
            // ACT

            var target = new BivakSync(); // TODO: Initialize to an appropriate value
            target.Bewaren(uitstap);

            // ASSERT 

            kipSyncMock.VerifyAll();
        }
    }
}
