using Cg2.Data.Ef;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Cg2.Orm.DataInterfaces;
using Cg2.Orm;
using Cg2.Workers;

using System;
using System.Linq;
using System.Text;
using System.Data.Objects;
using Cg2.EfWrapper.Entity;
using System.Collections.Generic;
using Cg2.Ioc;

namespace Cg2.Test
{
    
    
    /// <summary>
    ///This is a test class for GroepenEfDaoTest and is intended
    ///to contain all GroepenEfDaoTest Unit Tests
    ///</summary>
    [TestClass()]
    public class GroepenDaoTest
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
            Assert.IsTrue(actual.ID == 310);
        }

        [TestMethod]
        public void ToevoegenEnVerwijderenVanAfdelingen()
        {
            GroepenDao d = new GroepenDao();
            d.ToevoegenAfdeling(310, "De Joskes", "DJ");

            Groep g = d.OphalenMetAfdelingen(310);

            Assert.IsTrue(g.Afdeling.Count == 1);
             
            g.Afdeling.First().TeVerwijderen = true;

            d.BewarenMetAfdelingen(g);

            g = d.OphalenMetAfdelingen(310);

            Assert.IsTrue(g.Afdeling.Count == 0);
        }

        public void OphalenVanGroepMetAfdelingen()
        {
            //TODO eerst nog alle nodige info toevoegen!!!
            GroepenDao d = new GroepenDao();

            Groep g = d.OphalenMetAfdelingen(310);
            Assert.IsTrue(g.Afdeling.Count == 3);
            int count = 0;
            foreach (Afdeling a in g.Afdeling)
            {
                count += a.AfdelingsJaar.Count;
            }
            Assert.IsTrue(count == 2);
        }

        [TestMethod]
        public void OphalenOfficieleAfdelingen()
        {
            GroepenDao d = new GroepenDao();
            IList<OfficieleAfdeling> oas = d.OphalenOfficieleAfdelingen();

            Assert.IsTrue(oas.Count == 6);
            String[] namen = { "Ribbels", "Speelclub", "Rakwi's", "Tito's", "Keti's", "Aspiranten" };
            foreach (String n in namen)
            {
                bool exists = false;
                foreach (OfficieleAfdeling oa in oas)
                {
                    if (oa.Naam.Equals(n)) { exists = true; }
                }
                Assert.IsTrue(exists);
            }
        }

    }
}
