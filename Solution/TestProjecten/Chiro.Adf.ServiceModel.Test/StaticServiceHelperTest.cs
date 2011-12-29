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
    ///This is a test class for StaticServiceHelperTest and is intended
    ///to contain all StaticServiceHelperTest Unit Tests
    ///</summary>
    [TestClass()]
    public class StaticServiceHelperTest
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

        [ClassInitialize()]
        public static void MyClassInitialize(TestContext testContext)
        {
            Factory.ContainerInit();

            var gelieerdePersonenServiceMock = new Mock<IGelieerdePersonenService>();
            gelieerdePersonenServiceMock.Setup(mock => mock.PersoonIDGet(It.IsAny<int>())).Returns(-1);
            Factory.InstantieRegistreren(gelieerdePersonenServiceMock.Object);
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
        //Use TestInitialize to run code before running each test
        //[TestInitialize()]
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
        /// Aanroep van niet-gemockte service
        /// </summary>
        [TestMethod()]
        public void NietGemockteServiceTest()
        {
            string actual = ServiceHelper.CallService<IGroepenService, string>(svc=>svc.WieBenIk());

            Assert.IsFalse(String.IsNullOrEmpty(actual));
        }

        /// <summary>
        /// Aanroep van gemockte service
        /// </summary>
        [TestMethod()]
        public void GemockteServiceTest()
        {
            int actual = ServiceHelper.CallService<IGelieerdePersonenService, int>(svc => svc.PersoonIDGet(12345));

            Assert.AreEqual(-1, actual);
        }
    }
}
