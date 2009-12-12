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
using Chiro.Gap.Services;

namespace Chiro.Gap.ServiceContracts.Test
{
    /// <summary>
    /// Summary description for CategorieToevoegen
    /// </summary>
    [TestClass]
    public class CategorieVerwijderen
    {
        public CategorieVerwijderen()
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

        IGroepenService gpm;
        List<int> catlijst = new List<int>(); //lijst van nieuw aangemaakte categorieen, die nog verwijderd moeten worden
        int groepID = 317;
        String catnaam = "Kookies";

        [TestInitialize]
        public void setUp()
        {
            gpm = Factory.Maak<GroepenService>();
            int catID = gpm.CategorieToevoegen(groepID, catnaam, "hallo");
            catlijst.Add(catID);
        }

        [TestCleanup]
        public void tearDown()
        {
            IGroepenService gpm = Factory.Maak<GroepenService>();
            Groep g = gpm.OphalenMetCategorieen(groepID);
            foreach (Categorie c in g.Categorie)
            {
                if(catlijst.Contains(c.ID))
                {
                    gpm.CategorieVerwijderen(c.ID, groepID);
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
        public void CategorieVerwijderenNormaal()
        {
            Groep g = gpm.OphalenMetCategorieen(groepID);
            foreach(Categorie c in g.Categorie)
            {
                if (c.Naam.Equals(catnaam))
                {
                    gpm.CategorieVerwijderen(c.ID, groepID);
                }
            }

            g = gpm.OphalenMetCategorieen(groepID);
            bool found = false;
            foreach (Categorie c in g.Categorie)
            {
                if (c.Naam.Equals(catnaam))
                {
                    found = true;
                }
            }
            Assert.IsFalse(found);
        }

        //TODO

        [TestMethod]
        [ExpectedExceptionAttribute(typeof(NotImplementedException))]
        public void CategorieAanmakenLegeNaam()
        {
            
            catlijst.Add(gpm.CategorieToevoegen(groepID, "", ""));
        }

        [TestMethod]
        [ExpectedExceptionAttribute(typeof(NotImplementedException))]
        public void CategorieAanmakenGeenNaam()
        {
            IGroepenService gpm = Factory.Maak<GroepenService>();
            catlijst.Add(gpm.CategorieToevoegen(groepID, null, ""));
        }

        [TestMethod]
        [ExpectedExceptionAttribute(typeof(NotImplementedException))]
        public void CategorieAanmakenGeenCode()
        {
            IGroepenService gpm = Factory.Maak<GroepenService>();
            catlijst.Add(gpm.CategorieToevoegen(groepID, "kookies", null));
        }

        [TestMethod]
        [ExpectedExceptionAttribute(typeof(NotImplementedException))]
        public void CategorieAanmakenOnbestaandeGroep()
        {
            IGroepenService gpm = Factory.Maak<GroepenService>();
            catlijst.Add(gpm.CategorieToevoegen(0, "kookies", ""));
        }

        [TestMethod]
        [ExpectedExceptionAttribute(typeof(NotImplementedException))]
        public void CategorieAanmakenBestaandeNaam()
        {
            IGroepenService gpm = Factory.Maak<GroepenService>();
            catlijst.Add(gpm.CategorieToevoegen(groepID, "Kookies", ""));
            catlijst.Add(gpm.CategorieToevoegen(groepID, "Kookies", ""));
        }
    }
}
