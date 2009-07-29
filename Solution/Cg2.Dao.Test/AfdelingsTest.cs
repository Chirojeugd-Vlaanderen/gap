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
    /// Summary description for UnitTest1
    /// </summary>
    [TestClass]
    public class AfdelingsTest
    {
        public AfdelingsTest()
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
        public void AfdelingsJaarCreeren()
        {
            // TODO: test fixen

            // Als deze test failt, dan is een vorige cleanup 
            // waarschijnlijk niet goed gelukt.  Verwijder dan
            // manueel afdeling 'TE' voor groep 310 en eventueel
            // gekoppeld afdelingsjaar.

            // Arrange

            GroepenDao gdao = new GroepenDao();
            Dao<OfficieleAfdeling> oadao = new Dao<OfficieleAfdeling>();            

            Groep g = gdao.Ophalen(310);
            Afdeling a = gdao.AfdelingCreeren(310, "Testers", "TE");
            OfficieleAfdeling oa = oadao.Ophalen(6);    // aspiranten

            // Act

            AfdelingsJaar aj = gdao.AfdelingsJaarCreeren(g, a, oa, 1988, 1989);

            // Assert

            Assert.IsTrue(aj.ID > 0);

            // Cleanup

            Dao<AfdelingsJaar> ajdao = new Dao<AfdelingsJaar>();
            Dao<Afdeling> adao = new Dao<Afdeling>();
            ajdao.Verwijderen(aj);
            adao.Verwijderen(a);

        }
    }
}
