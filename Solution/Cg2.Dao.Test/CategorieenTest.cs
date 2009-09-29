using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Cg2.Orm.DataInterfaces;
using Cg2.Orm;
using Cg2.Data.Ef;
using Cg2.Workers;
using Cg2.Dummies;
using Cg2.EfWrapper.Entity;

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

            GelieerdePersonenDao dao = new GelieerdePersonenDao();
            GelieerdePersoon gp = dao.Ophalen(gp2ID, foo => foo.Categorie);

            foreach (Categorie c in gp.Categorie)
            {
                c.TeVerwijderen = true;
            }

            dao.Bewaren(gp, foo => foo.Categorie.First().WithoutUpdate());
            // De First() is nodig voor 1->veel of veel->veel.  Hopelijk zorgt
            // WithoutUpdate() ervoor dat de categorie zelf niet weggegooid
            // wordt.

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

            IGelieerdePersonenDao dao = new GelieerdePersonenDao();

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

            IGelieerdePersonenDao gpDao = new GelieerdePersonenDao();
            IDao<Categorie> catDao = new Dao<Categorie>();

            GelieerdePersoon gp = gpDao.Ophalen(gpID, foo => foo.Categorie, foo => foo.Groep);
            Categorie cat = catDao.Ophalen(catID, foo => foo.Groep);


            GelieerdePersonenManager gpm = new GelieerdePersonenManager(null, null, null, new AutMgrAltijdGav());
            // Deze GelieerdePersonenManager zal enkel gebruikt worden om
            // de gelieerde persoon aan een categorie te koppelen.  Hij mag
            // geen data-access doen (vandaar de null-dao's), en niet moeilijk
            // doen over GAV-rechten.

            #endregion

            #region act
            // koppel categorie aan groep
            gpm.CategorieKoppelen(gp, cat);

            // bewaar via dao (we zijn de dao aan het testen)
            gpDao.Bewaren(gp, foo => foo.Categorie);
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
