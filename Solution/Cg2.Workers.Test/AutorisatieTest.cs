using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Cg2.Orm.DataInterfaces;
using Cg2.Orm;
using Cg2.Fouten.Exceptions;
using Cg2.Dummies;

namespace Cg2.Workers.Test
{
    /// <summary>
    /// Tests op de security van de workers.
    /// </summary>
    [TestClass]
    public class AutorisatieTest
    {
        public AutorisatieTest()
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
        static public void InitialiseerTests(TestContext context)
        {
            // we doen even niets.
        }

        /// <summary>
        /// Probeert ledenlijst op te halen voor niet-GAV.
        /// Verwacht een exception.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(GeenGavException))]
        public void LijstLedenGeenGav()
        {
            // Arrange

            var ledenDaoMock = new Mock<ILedenDao>();
            var autorisatieMgrMock = new Mock<IAutorisatieManager>();

            ledenDaoMock.Setup(foo => foo.PaginaOphalen(Properties.Settings.Default.TestGroepsWerkJaarID)).Returns(new List<Lid>());
            autorisatieMgrMock.Setup(foo => foo.IsGavGroepsWerkJaar(Properties.Settings.Default.TestGroepsWerkJaarID)).Returns(false);

            LedenDaoCollectie daos = new LedenDaoCollectie(ledenDaoMock.Object, null, null, null, null, null);

            LedenManager lm = new LedenManager(daos, autorisatieMgrMock.Object);

            // Act

            lm.PaginaOphalen(Properties.Settings.Default.TestGroepsWerkJaarID);

            // Verwacht exception
        }

        /// <summary>
        /// Probeert ledenlijst op te halen als wel GAV
        /// Verwacht geen exception.
        /// </summary>
        [TestMethod]
        public void LijstLedenGav()
        {
            // Arrange

            var ledenDaoMock = new Mock<ILedenDao>();
            var autorisatieMgrMock = new Mock<IAutorisatieManager>();

            ledenDaoMock.Setup(foo => foo.PaginaOphalen(Properties.Settings.Default.TestGroepsWerkJaarID)).Returns(new List<Lid>());
            autorisatieMgrMock.Setup(foo => foo.IsGavGroepsWerkJaar(Properties.Settings.Default.TestGroepsWerkJaarID)).Returns(true);

            LedenDaoCollectie daos = new LedenDaoCollectie(ledenDaoMock.Object, null, null, null, null, null);

            LedenManager lm = new LedenManager(daos, autorisatieMgrMock.Object);

            // Act

            IList<Lid> lijst = lm.PaginaOphalen(Properties.Settings.Default.TestGroepsWerkJaarID);

            // Assert

            Assert.IsTrue(lijst != null);
        }

        /// <summary>
        /// Kijk na of adresgenoten van groepen waarvan je geen GAV
        /// bent toch niet worden opgehaald.
        /// </summary>
        [TestMethod]
        public void AdresGenotenEnkelEigenGroep()
        {
            #region Arrange

            PersonenManager pm = new PersonenManager(null); // geen DAO nodig voor test
            GelieerdePersonenManager gpm = new GelieerdePersonenManager(null, null
                , new AutMgrAltijdGav());

            // Stel een situatie op waarbij er 3 personen op hetzelfde
            // adres wonen, en waarbij 2 van die 3 personen gelieerd
            // zijn aan jouw groep.

            Groep g1 = new Groep { ID = 1 };
            Groep g2 = new Groep { ID = 2 };
            Persoon p1 = new Persoon { ID = 1 };
            Persoon p2 = new Persoon { ID = 2 };
            Persoon p3 = new Persoon { ID = 3 };
            Adres a = new Adres { ID = 1 };

            // koppel de 3 personen aan hetzelfde adres

            pm.AdresToevoegen(p1, a);
            pm.AdresToevoegen(p2, a);
            pm.AdresToevoegen(p3, a);

            // p1 en p3 koppelen aan g1, p2 aan g2, en
            // meteen fake GelieerdePersoonID's geven.

            gpm.PersoonAanGroepKoppelen(p1, g1, 0).ID = 1;
            gpm.PersoonAanGroepKoppelen(p3, g1, 0).ID = 3;
            gpm.PersoonAanGroepKoppelen(p2, g2, 0).ID = 2;


            // Creeer nu de GelieerdePersonenDaoMock, dia alle huisgenoten van p1 ophaalt.

            var gpDaoMock = new Mock<IGelieerdePersonenDao>();
            gpDaoMock.Setup(foo => foo.HuisGenotenOphalen(1)).Returns(new List<Persoon> { p1, p2, p3 });

            // en een AutorisatieManagerMock, die zorgt dat gebruiker alvast toegang heeft tot p1.

            var auManMock = new Mock<IAutorisatieManager>();
            auManMock.Setup(foo => foo.IsGavGelieerdePersoon(1)).Returns(true);

            // Tenslotte een nieuwe gelieerdePersonenManager die we willen
            // testen.

            GelieerdePersonenManager gpm2 = new GelieerdePersonenManager(
                gpDaoMock.Object, null, auManMock.Object);


            #endregion

            #region Act
            IList<Persoon> huisgenoten = gpm2.HuisGenotenOphalen(p1.ID);
            #endregion

            #region Assert
            // We verwachten enkel p1 en p3.

            var idQuery = (from p in huisgenoten select p.ID);

            Assert.IsTrue(idQuery.Contains(1));
            Assert.IsTrue(idQuery.Contains(3));
            Assert.IsFalse(idQuery.Contains(2));
            #endregion
        }
    }
}
