using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using Cg2.EfWrapper.Entity;

namespace Cg2.EfWrapper.Test
{
    /// <summary>
    /// Summary description for VerwijderTest
    /// </summary>
    [TestClass]
    public class VerwijderTest
    {
        public VerwijderTest()
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
        /// Verwijdert een adres van een persoon
        /// </summary>
        [TestMethod]
        public void AdresVerwijderen()
        {
            #region Arrange
            int pid = TestHelpers.PersoonMetTweeAdressenMaken();
            GelieerdePersoon p = TestHelpers.PersoonOphalen(pid);

            int adrid1 = p.PersoonsAdres.First().Adres.ID;
            #endregion

            #region Act
            p.PersoonsAdres.Last().TeVerwijderen = true;

            using (Entities db = new Entities())
            {
                db.AttachObjectGraph(p, bla => bla.PersoonsAdres, bla => bla.PersoonsAdres.First().Adres);
                db.SaveChanges();
            }
            #endregion

            #region Assert
            GelieerdePersoon q = TestHelpers.PersoonOphalen(pid);
            Assert.AreEqual(q.PersoonsAdres.Count(), 1);
            Assert.AreEqual(q.PersoonsAdres.First().Adres.ID, adrid1);
            #endregion
        }
    }
}
