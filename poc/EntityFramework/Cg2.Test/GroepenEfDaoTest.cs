using Cg2.Data.Ef;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Cg2.Core.Domain;
using Cg2.Core.DataInterfaces;

namespace Cg2.Test
{
    
    
    /// <summary>
    ///This is a test class for GroepenEfDaoTest and is intended
    ///to contain all GroepenEfDaoTest Unit Tests
    ///</summary>
    [TestClass()]
    public class GroepenEfDaoTest
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
        ///A test for Ophalen
        ///</summary>
        [TestMethod()]
        public void OphalenTest()
        {
            IDao<Groep> target = new Dao<Groep>(); 
            int id = 310; 
            Groep actual;
            actual = target.Ophalen(id);
            Assert.IsTrue(actual.ID > 0);
        }

    }
}
