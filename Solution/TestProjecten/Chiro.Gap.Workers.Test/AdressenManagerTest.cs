using Chiro.Cdf.Ioc;
using Chiro.Gap.Workers;
using Chiro.Gap.Workers.Exceptions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using Chiro.Gap.Orm.DataInterfaces;
using Chiro.Gap.WorkerInterfaces;
using Chiro.Gap.Orm.SyncInterfaces;
using Chiro.Gap.Orm;

namespace Chiro.Gap.Workers.Test
{
    
    
    /// <summary>
    ///This is a test class for AdressenManagerTest and is intended
    ///to contain all AdressenManagerTest Unit Tests
    ///</summary>
    [TestClass()]
    public class AdressenManagerTest
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


        /// <summary>
        /// Controleert of AdressenManager.Bewaren de gebruikersrechten wel test
        /// </summary>
        [TestMethod()]
        [ExpectedException(typeof(GeenGavException))]
        public void BewarenTest()
        {
            var target = Factory.Maak<AdressenManager>();

            Adres adr = new BelgischAdres {ID = 5}; // adres met willekeurig bestaand ID. Geen idee of ik er rechten op heb :)

            target.Bewaren(adr);    // probeer te bwearen.

            Assert.Fail();      // Er had een exception moeten zijn.
        }
    }
}
