using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Chiro.Gap.Orm;
using Chiro.Gap.Workers;
using Chiro.Cdf.Ioc;
using Chiro.Gap.Orm.DataInterfaces;
using System.IO;
using System.Runtime.Serialization;

namespace Chiro.Gap.Workers.Test
{
    /// <summary>
    /// Summary description for CategorieToevoegen
    /// </summary>
    [TestClass]
    public class CategorieToevoegen
    {
        int groepID = 317;
        String categorienaam = "TestKookies";
        GroepenManager gm;

        public CategorieToevoegen()
        {
            //
            // TODO: Add constructor logic here
            //
        }

        [ClassInitialize]
        static public void InitTests(TestContext context)
        {
            Factory.ContainerInit();
        }

        [TestInitialize]
        public void setUp(){
            gm = Factory.Maak<GroepenManager>();
        }

        [TestCleanup]
        public void breakDown()
        {
            Groep g = gm.Ophalen(groepID, e => e.Categorie);
            foreach (Categorie c in g.Categorie)
            {
                if (c.Naam.Equals(categorienaam))
                {
                    gm.CategorieVerwijderen(g, c);
                }
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
        public void TestCategorieToevoegen()
        {
            Groep g = gm.Ophalen(groepID, e => e.Categorie);
            foreach(Categorie c in g.Categorie)
            {
                Assert.IsFalse(c.Naam.Equals(categorienaam));
            }

            gm.CategorieToevoegen(g, categorienaam, "");

            g = gm.Ophalen(groepID, e => e.Categorie);
            bool found = false;
            foreach (Categorie c in g.Categorie)
            {
                if(c.Naam.Equals(categorienaam)){
                    found = true;
                }
            }
            Assert.IsTrue(found);
        }
    }
}
