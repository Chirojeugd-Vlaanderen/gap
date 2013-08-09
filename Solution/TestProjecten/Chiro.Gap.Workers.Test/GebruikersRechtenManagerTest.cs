using System.Collections.Generic;
using Chiro.Gap.Workers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using Chiro.Cdf.Mailer;
using Chiro.Gap.Poco.Model;

namespace Chiro.Gap.Workers.Test
{
    
    
    /// <summary>
    ///This is a test class for GebruikersRechtenManagerTest and is intended
    ///to contain all GebruikersRechtenManagerTest Unit Tests
    ///</summary>
    [TestClass()]
    public class GebruikersRechtenManagerTest
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
        ///A test for ToekennenOfVerlengen
        ///</summary>
        [TestMethod()]
        public void ToekennenOfVerlengenTest()
        {
            // ARRANGE

            var gav = new Gav();
            var gelieerdePersoon = new GelieerdePersoon
            {
                ID = 1,
                Groep = new ChiroGroep { ID = 3 },
                Persoon = new Persoon { ID = 2, Gav = new List<Gav> { gav } }
            };
            gelieerdePersoon.Persoon.GelieerdePersoon.Add(gelieerdePersoon);
            gav.Persoon.Add(gelieerdePersoon.Persoon);
            var gebruikersrecht = new GebruikersRecht
                                  {
                                      Gav = gav,
                                      Groep = gelieerdePersoon.Groep,
                                      VervalDatum = DateTime.Now.AddDays(-1)    // gisteren vervallen
                                  };
            gav.GebruikersRecht.Add(gebruikersrecht);

            // ACT

            var target = new GebruikersRechtenManager();
            var actual = target.ToekennenOfVerlengen(gav, gelieerdePersoon.Groep);

            // ASSERT

            Assert.IsTrue(gebruikersrecht.VervalDatum > DateTime.Now);
        }
    }
}
