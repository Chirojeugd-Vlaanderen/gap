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
    /// Summary description
    /// </summary>
    [TestClass]
    public class CategorieVerwijderen
    {
        int groepID = 317;
        String categorienaam = "TestKookies";
        GroepenManager gm;

        public CategorieVerwijderen()
        {
        }

        [ClassInitialize]
        static public void InitTests(TestContext context)
        {
            Factory.ContainerInit();
        }

        [TestInitialize]
        public void setUp(){
            gm = Factory.Maak<GroepenManager>();

            Groep g = gm.Ophalen(groepID, e => e.Categorie);
            gm.CategorieToevoegen(g, categorienaam, "");
        }

        [TestCleanup]
        public void breakDown()
        {
        }

        [TestMethod]
        public void TestCategorieVerwijderen()
        {
            /*Groep g = gm.Ophalen(groepID, e => e.Categorie);

            bool found = false;
            int cID = 0;
            foreach(Categorie c in g.Categorie)
            {
                if (c.Naam.Equals(categorienaam))
                {
                    found = true;
                    cID = c.ID;
                }
            }
            Assert.IsTrue(found);

            gm.CategorieVerwijderen(g, c);

            g = gm.Ophalen(groepID, e => e.Categorie);
            found = false;
            foreach (Categorie c in g.Categorie)
            {
                if(c.Naam.Equals(categorienaam)){
                    found = true;
                }
            }
            Assert.IsFalse(found);*/
        }
    }
}
