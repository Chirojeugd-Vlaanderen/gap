using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Cg2.Orm.DataInterfaces;
using Cg2.Data.Ef;
using Cg2.Orm;

namespace Cg2.Test
{
    /// <summary>
    /// Summary description for PersonenDaoTest
    /// </summary>
    [TestClass]
    public class PersonenDaoTest
    {
        public PersonenDaoTest()
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

        /// <summary>
        /// Personen ophalen
        /// </summary>
        [TestMethod]
        public void OphalenTest()
        {
            IPersonenDao dao = new PersonenDao();
            int id = 1893;

            Persoon actual;
            actual = dao.Ophalen(id);
            Assert.IsTrue(actual.ID > 0);
            Assert.IsTrue(actual.Communicatie.Count == 0);
        }

        [TestMethod]
        public void OphalenMetCommunicatieTest()
        {
            IPersonenDao dao = new PersonenDao();
            int id = 1893;

            Persoon actual;
            actual = dao.OphalenMetCommunicatie(id);
            Assert.IsTrue(actual.ID > 0);
            Assert.IsTrue(actual.Communicatie.Count > 0);
        }
    }
}
