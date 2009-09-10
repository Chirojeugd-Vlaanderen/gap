using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Cg2.Orm;
using Cg2.Workers;
using Cg2.Ioc;
using Cg2.EfWrapper.Entity;
using Cg2.Orm.DataInterfaces;

namespace Cg2.Dao.Test
{
    /// <summary>
    /// Tests voor LedenDao
    /// </summary>
    [TestClass]
    public class LedenTest
    {
        public LedenTest()
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

        /// <summary>
        /// Voorbereidend werk voor deze tests
        /// </summary>
        /// <param name="context"></param>
        [ClassInitialize]
        static public void TestInitialiseren(TestContext context)
        {
            Factory.InitContainer();

            // Verwijder lid (GelieerdePersoonID in AfdelingsJaarID) om achteraf opnieuw toe te voegen

            int gelieerdePersoonID = Properties.Settings.Default.TestGelieerdePersoonID;
            int afdelingsJaarID = Properties.Settings.Default.TestAfdelingsJaarID;

            IDao<AfdelingsJaar> ajdao = Factory.Maak<IDao<AfdelingsJaar>>();
            AfdelingsJaar aj = ajdao.Ophalen(afdelingsJaarID, lmb => lmb.GroepsWerkJaar.Groep);

            ILedenDao ldao = Factory.Maak<ILedenDao>();
            Lid l = ldao.Ophalen(gelieerdePersoonID, aj.GroepsWerkJaar.ID);

            if (l != null)
            {
                l.TeVerwijderen = true;
                ldao.Bewaren(l);
            }

            // Voeg kind toe (GelieerdePersoonID2 in AfdelingsJaarID) om in test te kunnen verwijderen

            int gelieerdePersoon2ID = Properties.Settings.Default.TestGelieerdePersoon2ID;

            IGelieerdePersonenDao gpdao = Factory.Maak<IGelieerdePersonenDao>();
            IDao<Kind> kdao = Factory.Maak<IDao<Kind>>();

            Lid l2 = ldao.Ophalen(gelieerdePersoon2ID, aj.GroepsWerkJaar.ID);

            if (l2 == null)
            {
                // enkel toevoegen als nog niet bestaat

                LedenManager lm = Factory.Maak<LedenManager>();

                GelieerdePersoon gp = gpdao.Ophalen(gelieerdePersoon2ID, lmb => lmb.Groep);

                Kind k = lm.KindMaken(gp, aj);
                kdao.Bewaren(k
                    , lmb => lmb.GelieerdePersoon.WithoutUpdate()
                    , lmb => lmb.AfdelingsJaar.GroepsWerkJaar.WithoutUpdate()
                    , lmb => lmb.GroepsWerkJaar.WithoutUpdate());
            }
        }

        [TestMethod]
        public void NieuwKind()
        {
            #region Arrange
            int gelieerdePersoonID = Properties.Settings.Default.TestGelieerdePersoonID;
            int afdelingsJaarID = Properties.Settings.Default.TestAfdelingsJaarID;

            LedenManager lm = Factory.Maak<LedenManager>();

            IGelieerdePersonenDao gpdao = Factory.Maak<IGelieerdePersonenDao>();
            IDao<AfdelingsJaar> ajdao = Factory.Maak<IDao<AfdelingsJaar>>();
            IDao<Kind> kdao = Factory.Maak<IDao<Kind>>();

            GelieerdePersoon gp = gpdao.Ophalen(gelieerdePersoonID, lmb => lmb.Groep);
            AfdelingsJaar aj = ajdao.Ophalen(afdelingsJaarID, lmb => lmb.GroepsWerkJaar.Groep);

            Kind k = lm.KindMaken(gp, aj);
            #endregion

            #region Act
            kdao.Bewaren(k
                , lmb => lmb.GelieerdePersoon.WithoutUpdate()
                , lmb => lmb.AfdelingsJaar.GroepsWerkJaar.WithoutUpdate()
                , lmb => lmb.GroepsWerkJaar.WithoutUpdate());
            #endregion

            #region Assert

            ILedenDao ldao = Factory.Maak<ILedenDao>();
            Lid l = ldao.Ophalen(gelieerdePersoonID, aj.GroepsWerkJaar.ID);

            Assert.IsTrue(l != null && l is Kind);

            #endregion
        }

        /// <summary>
        /// Verwijdert kind (GelieerdePersoon2ID, AfdelingsJaarID)
        /// </summary>
        [TestMethod]
        public void KindVerwijderen()
        {
            // Arrange

            int gelieerdePersoonID = Properties.Settings.Default.TestGelieerdePersoon2ID;
            int afdelingsJaarID = Properties.Settings.Default.TestAfdelingsJaarID;

            IDao<AfdelingsJaar> ajdao = Factory.Maak<IDao<AfdelingsJaar>>();
            AfdelingsJaar aj = ajdao.Ophalen(afdelingsJaarID, lmb => lmb.GroepsWerkJaar.Groep);

            ILedenDao ldao = Factory.Maak<ILedenDao>();
            Lid l = ldao.Ophalen(gelieerdePersoonID, aj.GroepsWerkJaar.ID);
 
            // Act

            l.TeVerwijderen = true;
            ldao.Bewaren(l);

            // Assert

            Lid l2 = ldao.Ophalen(gelieerdePersoonID, aj.GroepsWerkJaar.Groep.ID);
            Assert.IsNull(l2);
        }
    }
}
