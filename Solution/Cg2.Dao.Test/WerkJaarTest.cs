using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Cg2.Data;
using Cg2.Orm;

namespace Cg2.Dao.Test
{
    /// <summary>
    /// Summary description for WerkJaarTest
    /// </summary>
    [TestClass]
    public class WerkJaarTest
    {
        public WerkJaarTest()
        {
            //
            // TODO: Add constructor logic here
            //
        }

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
        // You can use the following additional attributes as you write your tests:
        //
        // Use ClassInitialize to run code before running the first test in the class
        // [ClassInitialize()]
        // public static void MyClassInitialize(TestContext testContext) { }
        //
        // Use ClassCleanup to run code after all tests in a class have run
        // [ClassCleanup()]
        // public static void MyClassCleanup() { }
        //
        // Use TestInitialize to run code before running each test 
        // [TestInitialize()]
        // public void MyTestInitialize() { }
        //
        // Use TestCleanup to run code after each test has run
        // [TestCleanup()]
        // public void MyTestCleanup() { }
        //
        #endregion

        [TestMethod]
        public void GroepsWerkJaarZoeken()
        {
            #region Arrange
            GroepsWerkJaarDao gwjDao = new GroepsWerkJaarDao();
            int gwjID = Properties.Settings.Default.TestGroepsWerkJaarID;
            int testGroepID = Properties.Settings.Default.TestGroepID;
            #endregion

            #region Act
            GroepsWerkJaar gwj = gwjDao.Ophalen(gwjID);
            #endregion

            #region Assert
            Assert.IsTrue(gwj.Groep != null);
            Assert.IsTrue(gwj.Groep.ID == testGroepID);
            #endregion
        }
    }
}
