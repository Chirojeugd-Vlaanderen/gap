using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Cg2.Data.Ef;
using Cg2.Orm;
using Cg2.Workers;
using Cg2.Ioc;
using Cg2.EfWrapper.Entity;

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

            // Verwijder lid om achteraf opnieuw toe te voegen

            int gelieerdePersoonID = Properties.Settings.Default.TestGelieerdePersoonID;
            int afdelingsJaarID = Properties.Settings.Default.TestAfdelingsJaarID;

            Dao<AfdelingsJaar> ajdao = new Dao<AfdelingsJaar>();
            AfdelingsJaar aj = ajdao.Ophalen(afdelingsJaarID, lmb => lmb.GroepsWerkJaar.Groep);

            LedenDao ldao = new LedenDao();
            Lid l = ldao.Ophalen(gelieerdePersoonID, aj.GroepsWerkJaar.ID);

            if (l != null)
            {
                l.TeVerwijderen = true;
                ldao.Bewaren(l);
            }

            Assert.IsTrue(l != null && l is Kind);

        }

        [TestMethod]
        public void NieuwKind()
        {
            #region Arrange
            int gelieerdePersoonID = Properties.Settings.Default.TestGelieerdePersoonID;
            int afdelingsJaarID = Properties.Settings.Default.TestAfdelingsJaarID;

            LedenManager lm = Factory.Maak<LedenManager>();

            GelieerdePersonenDao gpdao = new GelieerdePersonenDao();
            Dao<AfdelingsJaar> ajdao = new Dao<AfdelingsJaar>();
            Dao<Kind> kdao = new Dao<Kind>();

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

            LedenDao ldao = new LedenDao();
            Lid l = ldao.Ophalen(gelieerdePersoonID, aj.GroepsWerkJaar.ID);

            Assert.IsTrue(l != null && l is Kind);

            #endregion
        }

        [TestMethod]
        public void KindVerwijderen()
        {
            // Arrange

            // Haalt gewoon een willekeurig kind op.
            // TODO: deze test zal sowieso failen als er
            // geen leden zijn!

            LedenDao ldao = new LedenDao();
            Lid l = ldao.TestKindGet();
            int id = l.ID;

            // Act

            l.TeVerwijderen = true;
            ldao.Bewaren(l);

            Lid l2 = ldao.Ophalen(id);

            // Assert

            Assert.IsNull(l2);

        }
    }
}
