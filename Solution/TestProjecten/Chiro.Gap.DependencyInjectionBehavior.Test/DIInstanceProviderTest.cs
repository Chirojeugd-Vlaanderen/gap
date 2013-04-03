using Chiro.Cdf.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.ServiceModel;
using Moq;

namespace Chiro.Gap.DependencyInjectionBehavior.Test
{
    
    
    /// <summary>
    ///This is a test class for DIInstanceProviderTest and is intended
    ///to contain all DIInstanceProviderTest Unit Tests
    ///</summary>
    [TestClass()]
    public class DIInstanceProviderTest
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

        [ServiceContract]
        public interface IDisposableService: IDisposable
        {
            [OperationContract]
            string Hello();
        }

        /// <summary>
        /// Controleert of ReleaseInstance van onze DIInstanceProvider disposable
        /// services wel degelijk disposet.
        /// </summary>
        [TestMethod()]
        public void ReleaseInstanceTest()
        {
            // Gauw een mock van IDisposableService-instantie
            var serviceMock = new Mock<IDisposableService>();
            serviceMock.Setup(src => src.Dispose()).Verifiable();
            object instance = serviceMock.Object;

            // Creeer DIInstanceProvider
            Type serviceType = typeof (IDisposableService);
            var target = new DIInstanceProvider(serviceType, null); 

            // Laat de provider nu de instantie vrijgeven
            target.ReleaseInstance(null, instance);
            
            // Check of Dispose is aangeroepen.
            serviceMock.VerifyAll();
        }
    }
}
