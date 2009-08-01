using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Cg2.Data.Ef;
using Cg2.Orm;
using Cg2.Ioc;

namespace Cg2.Workers.Test
{
    /// <summary>
    /// Summary description for UnitTest1
    /// </summary>
    [TestClass]
    public class ToevoegTest
    {
        public ToevoegTest()
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
        public void TestMethod1()
        {
            GroepenManager gm = Factory.Maak<GroepenManager>();
            Groep g = gm.Ophalen(310);

            Persoon p = Persoon.CreatePersoon("Broes", 0, 300);
            GelieerdePersoon gp = GelieerdePersoon.CreateGelieerdePersoon(10, 300);
            gp.Persoon = p;
            p.GelieerdePersoon.Add(gp);
            gp.Groep = g;

            GelieerdePersonenManager gpm = Factory.Maak<GelieerdePersonenManager>();
            gpm.Bewaren(gp);

            gp.TeVerwijderen = true;

            gpm.Bewaren(gp);
        }
    }
}
