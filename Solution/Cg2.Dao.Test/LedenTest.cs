using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Cg2.Data.Ef;
using Cg2.Orm;

namespace Cg2.Dao.Test
{
    /// <summary>
    /// Tests voor LedenDao
    /// </summary>
    [TestClass]
    public class LedenTest
    {
        public LedenTest()
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
        public void KindVerwijderen()
        {
            // Arrange

            // Haalt gewoon een willekeurig kind op.
            // TODO: deze test zal sowieso failen als er
            // geen leden zijn!

            LedenDao ldao = new LedenDao();
            Lid l = ldao.TestKindGet();
            int id = l.ID;

            // Act

            l.TeVerwijderen = true;
            ldao.Bewaren(l);

            Lid l2 = ldao.Ophalen(id);

            // Assert

            Assert.IsNull(l2);

        }
    }
}
