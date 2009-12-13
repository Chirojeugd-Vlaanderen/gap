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
        static public void InitialiseerTests(TestContext tc)
        {
            Factory.ContainerInit();
        }

        [ClassCleanup]
        static public void AfsluitenTests()
        {
            Factory.Dispose();
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

/*        [TestMethod]
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
        }*/
    }
}
