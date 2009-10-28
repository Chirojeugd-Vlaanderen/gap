using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Chiro.Gap.Orm.DataInterfaces;
using Chiro.Gap.Orm;
using Chiro.Gap.Fouten.Exceptions;
using Cg2.Dummies;
using Chiro.Cdf.Ioc;

namespace Chiro.Gap.Workers.Test
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
            Factory.ContainerInit();
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

            Factory.InstantieRegistreren<ILedenDao>(ledenDaoMock.Object);
            Factory.InstantieRegistreren<IAutorisatieManager>(autorisatieMgrMock.Object);

            LedenDaoCollectie daos = Factory.Maak<LedenDaoCollectie>();
            LedenManager lm = Factory.Maak<LedenManager>();

            // Act

            lm.PaginaOphalen(Properties.Settings.Default.TestGroepsWerkJaarID);

            // Verwacht exception
        }

        /// <summary>
        /// Als een niet-GAV probeert een communicatievorm te verwijderen
        /// die niet aan een gelieerde persoon gekoppeld is, moet die een
        /// GeenGavException krijgen, en niks anders :)
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(GeenGavException))]
        public void FouteCommVormVerwijderenGeenGav()
        {
            #region arrange

            // fake gelieerde persoon zonder communicatiemiddelen.

            GelieerdePersoon communicatielozePersoon = new GelieerdePersoon();
            communicatielozePersoon.Communicatie = new System.Data.Objects.DataClasses.EntityCollection<CommunicatieVorm>();

            // GelieerdePersonenDao mocken, zodat die steeds de fake persoon oproept
            
            var gpDaoMock = new Mock<IGelieerdePersonenDao>();
            gpDaoMock.Setup(foo => foo.Ophalen(It.IsAny<int>()
                , It.IsAny<System.Linq.Expressions.Expression<Func<GelieerdePersoon,object>>[]>() )).Returns(communicatielozePersoon);

            // GroepenDao mocken.

            var groepenDaoMock = new Mock<IGroepenDao>();

            // GelieerdePersonenManager aanmaken, waarbij autorisatieManager steeds 'false'
            // antwoordt.

            Factory.InstantieRegistreren<IGelieerdePersonenDao>(gpDaoMock.Object);
            Factory.InstantieRegistreren<IGroepenDao>(groepenDaoMock.Object);
            Factory.InstantieRegistreren<IAutorisatieManager>(new AutMgrNooitGav());

            CommVormManager gpMgr = Factory.Maak<CommVormManager>();
            #endregion

            #region act
            // Probeer nu een fictieve communicatievorm te verwijderen.
            // We verwachten 'GeenGavException'

            gpMgr.CommVormVerwijderen(1, 1); 
            // CommunicatieVormID en GelieerdePersoonID zijn irrelevant owv de mocking
            #endregion

            #region assert
            // Als we dit tegenkomen, is het sowieso mislukt
            Assert.IsTrue(false);
            #endregion
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

            Factory.InstantieRegistreren<ILedenDao>(ledenDaoMock.Object);
            Factory.InstantieRegistreren<IAutorisatieManager>(autorisatieMgrMock.Object);

            LedenManager lm = Factory.Maak<LedenManager>();

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

            // Test een beetje opgekuist.  Er wordt heel wat gemockt, dus is er
            // minder voorbereiding nodig.

            // Stel een situatie op waarbij er 3 personen op hetzelfde
            // adres wonen, en waarbij 2 van die 3 personen gelieerd
            // zijn aan jouw groep.

            Persoon p1 = new Persoon { ID = 1 };
            Persoon p2 = new Persoon { ID = 2 };
            Persoon p3 = new Persoon { ID = 3 };

            // Creeer nu PersonenDaoMock, dia alle huisgenoten van p1 ophaalt.

            var pDaoMock = new Mock<IPersonenDao>();
            pDaoMock.Setup(foo => foo.HuisGenotenOphalen(1)).Returns(new List<Persoon> { p1, p2, p3 });

            // en een AutorisatieManagerMock, die zorgt dat gebruiker alvast toegang heeft tot p1.

            var auManMock = new Mock<IAutorisatieManager>();

            auManMock.Setup(foo => foo.IsGavGelieerdePersoon(1)).Returns(true);
            auManMock.Setup(foo => foo.EnkelMijnPersonen(It.IsAny<IList<int>>())).Returns(new List<int>{1, 3});

            // Tenslotte de personenManager die we willen
            // testen.

            Factory.InstantieRegistreren<IPersonenDao>(pDaoMock.Object);
            Factory.InstantieRegistreren<IAutorisatieManager>(auManMock.Object);

            PersonenManager pm = Factory.Maak<PersonenManager>();

            #endregion

            #region Act
            IList<Persoon> huisgenoten = pm.HuisGenotenOphalen(p1.ID);
            #endregion

            #region Assert
            // We verwachten enkel p1 en p3.

            var idQuery = (from p in huisgenoten select p.ID);

            Assert.IsTrue(idQuery.Contains(1));
            Assert.IsTrue(idQuery.Contains(3));
            Assert.IsFalse(idQuery.Contains(2));

            // Ik vermoed dat onderstaande nakijkt of alle 'gesetupte' methods wel
            // aangerpen werden.
            auManMock.VerifyAll();

            #endregion
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void AdNummerWijzigen()
        {
            #region Arrange
            
            // GelieerdePersonenDao mocken.  Van een Dao verwachten
            // we dat die gewoon doet wat we vragen; er is daar geen
            // businesslogica geimplementeerd

            var gpDaoMock = new Mock<IGelieerdePersonenDao>();

            gpDaoMock.Setup(
                foo => foo.Ophalen(Properties.Settings.Default.TestGelieerdePersoonID, 
                It.IsAny<System.Linq.Expressions.Expression<System.Func<GelieerdePersoon, Object>>>())).Returns(
                    () => MaakTestGelieerdePersoon());

            // Het stuk It.IsAny<System.Linq.Expressions.Expression<System.Func<GelieerdePersoon, Object>>>()
            // zorgt ervoor dat de Mock de linq-expressies in 'Ophalen' negeert.
            //
            // De constructie in 'Returns' zorgt ervoor dat MaakTestGelieerdePersoon iedere
            // keer uitgevoerd wordt bij aanroep van 'Ophalen'.  (En niet eenmalig bij het
            // opzetten van de mock.)

            // Maak nu de GelieerdePersoonenManager aan die we willen testen.

            Factory.InstantieRegistreren<IGelieerdePersonenDao>(gpDaoMock.Object);
            Factory.InstantieRegistreren<IAutorisatieManager>(new AutMgrAltijdGav());

            GelieerdePersonenManager gpm = Factory.Maak<GelieerdePersonenManager>();

            #endregion

            #region Act
            // Haal gelieerde persoon met TestGelieerdePersoonID op
            GelieerdePersoon gp = gpm.Ophalen(Properties.Settings.Default.TestGelieerdePersoonID); 

            // Pruts met AD-nummer
            ++gp.Persoon.AdNummer;
  
            // Probeer te bewaren
            gpm.Bewaren(gp);
            #endregion

            #region Assert
            // Als we hier geraken, is het zeker niet gelukt.
            Assert.IsTrue(false);            
            #endregion
        }

        /// <summary>
        /// Test die nakijkt of ik een persoon wel kan bewaren
        /// als ik het ad-nummer niet wijzig.
        /// </summary>
        [TestMethod]
        public void AdNummerNietWijzigen()
        {
            #region Arrange

            // GelieerdePersonenDao mocken.  Van een Dao verwachten
            // we dat die gewoon doet wat we vragen; er is daar geen
            // businesslogica geimplementeerd

            var gpDaoMock = new Mock<IGelieerdePersonenDao>();

            gpDaoMock.Setup(foo => foo.Ophalen(Properties.Settings.Default.TestGelieerdePersoonID
                , It.IsAny<System.Linq.Expressions.Expression<System.Func<GelieerdePersoon, Object>>>())).Returns(() => MaakTestGelieerdePersoon());

            // Het stuk It.IsAny<System.Linq.Expressions.Expression<System.Func<GelieerdePersoon, Object>>>()
            // zorgt ervoor dat de Mock de linq-expressies in 'Ophalen' negeert.
            //
            // De constructie in 'Returns' zorgt ervoor dat MaakTestGelieerdePersoon iedere
            // keer uitgevoerd wordt bij aanroep van 'Ophalen'.  (En niet eenmalig bij het
            // opzetten van de mock.)

            // Maak nu de GelieerdePersoonenManager aan die we willen testen.

            Factory.InstantieRegistreren<IGelieerdePersonenDao>(gpDaoMock.Object);
            Factory.InstantieRegistreren<IAutorisatieManager>(new AutMgrAltijdGav());

            GelieerdePersonenManager gpm = Factory.Maak<GelieerdePersonenManager>();

            #endregion

            #region Act
            // Haal gelieerde persoon met TestGelieerdePersoonID op
            GelieerdePersoon gp = gpm.Ophalen(Properties.Settings.Default.TestGelieerdePersoonID);

            // Probeer te bewaren
            gpm.Bewaren(gp);
            #endregion

            #region Assert
            // Als we hier geraken, is het ok.
            Assert.IsTrue(true);
            gpDaoMock.VerifyAll();  // nakijken of de mock van GelieerdePersonenDao inderdaad aangeroepen werd.
            #endregion
        }


        /// <summary>
        /// Maakt een (nep) gelieerde persoon om te testen:
        ///   GelieerdePersoonID: TestGelieerdePersoonID
        ///   Ad-nummer: TestAdNummer
        /// </summary>
        /// <returns>Een nieuw gelieerdepersoonsobject</returns>
        private GelieerdePersoon MaakTestGelieerdePersoon()
        {
            // gewenste situatie opbouwen van een persoon die
            // gekoppeld is aan een groep.

            Factory.InstantieRegistreren<IAutorisatieManager>(new AutMgrAltijdGav());

            GelieerdePersonenManager gpm = Factory.Maak<GelieerdePersonenManager>();

            Groep g = new Groep();
            Persoon p = new Persoon { AdNummer = 1 };
            GelieerdePersoon gp = gpm.PersoonAanGroepKoppelen(p, g, 0);
            gp.ID = Properties.Settings.Default.TestGelieerdePersoonID;

            return gp;
        }
    }
}
