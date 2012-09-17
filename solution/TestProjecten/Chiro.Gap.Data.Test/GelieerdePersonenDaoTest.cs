using Chiro.Gap.Data.Ef;
using Chiro.Gap.TestDbInfo;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using Chiro.Gap.Orm;

namespace Chiro.Gap.Data.Test
{
    
    
    /// <summary>
    ///This is a test class for GelieerdePersonenDaoTest and is intended
    ///to contain all GelieerdePersonenDaoTest Unit Tests
    ///</summary>
    [TestClass()]
    public class GelieerdePersonenDaoTest
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


        ///<summary>
        ///Controleer of DetailsOphalen ook accounts ophaalt zonder gebruikersrechten.
        ///</summary>
        [TestMethod()]
        public void DetailsOphalenTest()
        {
            var target = new GelieerdePersonenDao();
            int gelieerdePersoonID = TestInfo.GELIEERDE_PERSOON5_ID;

            GelieerdePersoon actual = target.DetailsOphalen(gelieerdePersoonID);

            Assert.AreEqual(actual.Persoon.Gav.Count, 1);
        }
    }
}
