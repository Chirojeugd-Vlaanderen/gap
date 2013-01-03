using Chiro.Adf.ServiceModel;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using Chiro.Gap.ServiceContracts;
using Chiro.Cdf.Ioc;
using System.ServiceModel;
using Chiro.Gap.ServiceContracts.FaultContracts;

namespace Chiro.Adf.ServiceModel.Test
{
    
    
    /// <summary>
    /// Dit is een testclass voor StaticServiceHelperTest,
    ///to contain all StaticServiceHelperTest Unit Tests
    /// </summary>
    [TestClass()]
    public class StaticServiceHelperTest
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

        [ClassInitialize]
        public static void MyClassInitialize(TestContext testContext)
        {
            Factory.ContainerInit();

            var gelieerdePersonenServiceMock = new Mock<IGelieerdePersonenService>();
            gelieerdePersonenServiceMock.Setup(mock => mock.PersoonIDGet(It.IsAny<int>())).Returns(-1);

            var abonnementenServiceMock = new Mock<IAbonnementenService>();
            abonnementenServiceMock.Setup(mock => mock.DubbelPuntBestellen(It.IsAny<int>())).Throws(
                new FaultException());

            Factory.InstantieRegistreren(gelieerdePersonenServiceMock.Object);
            Factory.InstantieRegistreren(abonnementenServiceMock.Object);
        }

        #region Additional test attributes
        // 
        //You can use the following additional attributes as you write your tests:
        //
        //Use ClassInitialize to run code before running the first test in the class
        //[ClassInitialize]
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
        //Use TestInitialize to run code before running each test
        //[TestInitialize]
        //public void MyTestInitialize()
        //{
        //}
        //
        //Use TestCleanup to run code after each test has run
        //[TestCleanup()]
        //public void MyTestCleanup()
        //{
        //}
        //
        #endregion


        /// <summary>
        /// Aanroep van niet-gemockte service, en de unit test 
        /// moet dus naar de service zoeken via de config file.
        /// We gebruiken de URL van de DEV server van Kipdorp. 
        /// Heb je je VPN niet open, geen nood, daar is rekening mee gehouden.
        /// </summary>
        [TestMethod]
        public void NietGemockteServiceTest()
        {
            // OPGELET:
            // Als deze test failt, dan kan dat zijn omdat devsrv1 je niet herkent. (Lokale user?)
            // Open dan in windows explorer \\devsrv1.chiro.wereld, en geef je credentials in.
            // Dat lost dan normaal gezien het probleem op.
            try
            {
                string actual = ServiceHelper.CallService<IGroepenService, string>(svc=>svc.WieBenIk());
                // Ofwel is er een serviceccall omdat de IIS/Casini draait, en dan moet het antwoord correct zijn
                Assert.IsFalse(String.IsNullOrEmpty(actual));
            }
            catch (EndpointNotFoundException endpointNotFound)
            {
                // Ofwel vinden we de service niet, en dat is OK
                Assert.IsNotNull(endpointNotFound);
            }

        }

        /// <summary>
        /// Aanroep van gemockte service
        /// </summary>
        [TestMethod]
        public void GemockteServiceTest()
        {
            int actual = ServiceHelper.CallService<IGelieerdePersonenService, int>(svc => svc.PersoonIDGet(12345));

            Assert.AreEqual(-1, actual);
        }

        /// <summary>
        /// In mijn gemockte service throwt 'DubbelpuntBestellen' een FaultException.  Ik moet die FaultException
        /// kunnen catchen.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(FaultException))]
        public void FaultExceptionGemockteServiceTest()
        {
            ServiceHelper.CallService<IAbonnementenService>(svc => svc.DubbelPuntBestellen(12345));

            // Als we geen exception hebben, is er iets mis gegaan.
            Assert.AreEqual(true, false);
        }
    }
}
