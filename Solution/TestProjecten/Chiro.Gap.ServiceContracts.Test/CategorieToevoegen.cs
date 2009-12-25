using System;
using System.Text;
using System.Collections.Generic;
using System.Diagnostics;
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
    public class CategorieToevoegen
    {
        public CategorieToevoegen()
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

        List<int> catlijst = new List<int>(); //lijst van nieuw aangemaakte categorieen, die nog verwijderd moeten worden
        IGroepenService gpm;

        [TestInitialize]
        public void initialiseerTest()
        {
            Debug.WriteLine("Chiro.Gap.ServiceContracts.Test.CategorieToevoegen: InitialiseerTest - Start");
            gpm = Factory.Maak<GroepenService>();

            // CategorieToevoegenNormaal voegt een categorie toe voor:
            //    - Groep: Properties.Settings.Default.GroepID
            //    - CategorieCode: Properties.Settings.Default.CategorieCode
            // Deze moeten we verwijderen als die bestaat.
            Groep g = gpm.OphalenMetCategorieen(Properties.Settings.Default.GroepID);
            Debug.WriteLine ("Chiro.Gap.ServiceContracts.Test.CategorieToevoegen: InitialiseerTest: heb "
                + g.Categorie.Count.ToString() + " Categorien gevonden voor "
                + " GroepID: " + Properties.Settings.Default.GroepID.ToString());

            foreach (Categorie c in g.Categorie.ToList<Categorie>())
            {  
                if (c.Code.ToString().Equals(Properties.Settings.Default.CategorieCode_Toevoegen))
                {
                    Debug.WriteLine ("Chiro.Gap.ServiceContracts.Test.CategorieToevoegen: InitialiseerTest - Verwijder " 
                        + "CategorieID: " + c.ID.ToString() 
                        + " - GroepID: " + Properties.Settings.Default.GroepID.ToString());
                    gpm.CategorieVerwijderen(c.ID, Properties.Settings.Default.GroepID);
                }
            }
            Debug.WriteLine("Chiro.Gap.ServiceContracts.Test.CategorieToevoegen: InitialiseerTest - Einde");
        }

        [TestCleanup]
        public void tearDown()
        {
            Groep g = gpm.OphalenMetCategorieen(Properties.Settings.Default.GroepID);
            foreach (Categorie c in g.Categorie)
            {
                if(catlijst.Contains(c.ID))
                {
                    gpm.CategorieVerwijderen(c.ID, Properties.Settings.Default.GroepID);
                }
            }
        }

        [TestMethod]
        public void CategorieToevoegenNormaal()
        {
            int catID = gpm.CategorieToevoegen(Properties.Settings.Default.GroepID, 
                    Properties.Settings.Default.CategorieNaam, 
                    Properties.Settings.Default.CategorieCode_Toevoegen);
            catlijst.Add(catID);

            Groep g = gpm.OphalenMetCategorieen(Properties.Settings.Default.GroepID);
            bool found = false;
            foreach (Categorie c in g.Categorie)
            {
                if (c.ID == catID)
                {
                    found = true;
                }
            }
            Assert.IsTrue(found);
        }

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
            catlijst.Add(gpm.CategorieToevoegen(groepID, null, ""));
        }

        [TestMethod]
        [ExpectedExceptionAttribute(typeof(NotImplementedException))]
        public void CategorieAanmakenGeenCode()
        {
            catlijst.Add(gpm.CategorieToevoegen(groepID, "kookies", null));
        }

        [TestMethod]
        [ExpectedExceptionAttribute(typeof(NotImplementedException))]
        public void CategorieAanmakenOnbestaandeGroep()
        {
            catlijst.Add(gpm.CategorieToevoegen(0, "kookies", ""));
        }

        [TestMethod]
        [ExpectedExceptionAttribute(typeof(NotImplementedException))]
        public void CategorieAanmakenBestaandeNaam()
        {
            catlijst.Add(gpm.CategorieToevoegen(groepID, "Kookies", ""));
            catlijst.Add(gpm.CategorieToevoegen(groepID, "Kookies", ""));
        }*/
    }
}
