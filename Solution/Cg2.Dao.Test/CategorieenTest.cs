using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Cg2.Orm.DataInterfaces;
using Cg2.Orm;
using Cg2.Workers;
using Cg2.Dummies;
using Cg2.EfWrapper.Entity;
using Cg2.Ioc;

namespace Cg2.Dao.Test
{
    /// <summary>
    /// Tests mbt de categorieen die een gelieerde persoon kan hebben.
    /// </summary>
    [TestClass]
    public class CategorieenTest
    {
        public CategorieenTest()
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

        [ClassInitialize]
        static public void TestInit(TestContext context)
        {
            // Zorg ervoor dat GelieerdePersoon 2 geen categorieen heeft.

            int gp2ID = Properties.Settings.Default.TestGelieerdePersoon2ID;

            Cg2.Ioc.Factory.InitContainer();

            IGelieerdePersonenDao dao = Factory.Maak<IGelieerdePersonenDao>();
            GelieerdePersoon gp = dao.Ophalen(gp2ID, foo => foo.Categorie);

            foreach (Categorie c in gp.Categorie)
            {
                c.TeVerwijderen = true;
            }

            dao.Bewaren(gp, foo => foo.Categorie);
            // De 'TeVerwijderen' zorgt er bij 1->veel enkel voor
            // dat de link tussen GelieerdePersoon en Categorie
            // verwijderd wordt, en de Categorie zelf niet.
            // Om dat laatste te bewerkstellingen, had 
            // GelieerdePersoon gedecoreerd moeten zijn met
            // [AssociationEndBehavior("PersoonsCategorie", Owned=true)]
        }

        /// <summary>
        /// Test of testgelieerdepersoon wel degelijk gevonden wordt
        /// in testcategorie.
        /// (Voornamelijk een test op many->many)
        /// </summary>
        [TestMethod]
        public void PersonenOphalenUitCategorie()
        {
            // arrange

            int catID = Properties.Settings.Default.TestCategorieID;
            int gpID = Properties.Settings.Default.TestGelieerdePersoonID;

            IGelieerdePersonenDao dao = Factory.Maak<IGelieerdePersonenDao>();

            // act

            IList<GelieerdePersoon> lijst = dao.OphalenUitCategorie(catID);

            // assert

            var query = (from gp in lijst
                         where gp.ID == gpID
                         select gp);

            Assert.IsTrue(query.Count() == 1);
        }

        /// <summary>
        /// Test op toevoegen van een gelieerde persoon aan een categorie
        /// (many->many via PersoonsCategorie)
        /// </summary>
        [TestMethod]
        public void PersoonToevoegenAanCategorie()
        {
            #region arrange

            int catID = Properties.Settings.Default.TestCategorieID;
            int gpID = Properties.Settings.Default.TestGelieerdePersoon2ID;

            IGelieerdePersonenDao gpDao = Factory.Maak<IGelieerdePersonenDao>();
            ICategorieenDao catDao = Factory.Maak<ICategorieenDao>();

            GelieerdePersoon gp = gpDao.Ophalen(gpID);

            // Opgelet: onderstaande haalt standaard alle personen gekoppeld
            // aan een categorie mee op.  Voor een test is dat niet erg, maar
            // als je in business een persoon aan een categorie wil koppelen,
            // is dat niet gewenst.

            Categorie cat = catDao.Ophalen(catID);
            #endregion

            #region act

            // koppel categorie gauw handmatig aan groep
            gp.Categorie.Add(cat);
            cat.GelieerdePersoon.Add(gp);

            // bewaar via dao (we zijn de dao aan het testen)

            catDao.Bewaren(cat);
            #endregion

            #region assert
            IList<GelieerdePersoon> lijst = gpDao.OphalenUitCategorie(catID);

            var query = (from item in lijst
                         where item.ID == gpID
                         select item);

            Assert.IsTrue(query.Count() == 1);
            #endregion

        }
    }
}
