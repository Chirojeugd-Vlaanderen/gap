using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

using Chiro.Gap.Domain;

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

using Chiro.Cdf.Ioc;
using Chiro.Gap.Dummies;
using Chiro.Gap.Orm.DataInterfaces;
using Chiro.Gap.Orm;
using Chiro.Gap.Workers.Exceptions;

namespace Chiro.Gap.Workers.Test
{
	/// <summary>
	/// Tests op de security van de workers.
	/// </summary>
	[TestClass]
	public class AutorisatieTest
	{
		private static DummyData _testData;

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
		// [TestInitialize]
		// public void MyTestInitialize() { }
		//
		// Use TestCleanup to run code after each test has run
		// [TestCleanup()]
		// public void MyTestCleanup() { }
		//
		#endregion

		[ClassInitialize]
		static public void InitialiseerTests(TestContext tc)
		{
			Factory.ContainerInit();
			_testData = new DummyData();

		}

		[ClassCleanup]
		static public void AfsluitenTests()
		{
			Factory.Dispose();
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

			var testData = new DummyData();
			var ledenDaoMock = new Mock<ILedenDao>();
			var autorisatieMgrMock = new Mock<IAutorisatieManager>();

			ledenDaoMock.Setup(foo => foo.AllesOphalen(testData.HuidigGwj.ID, LidEigenschap.Naam)).Returns(new List<Lid>());
			autorisatieMgrMock.Setup(foo => foo.IsGavGroepsWerkJaar(testData.HuidigGwj.ID)).Returns(false);

			Factory.InstantieRegistreren(ledenDaoMock.Object);
			Factory.InstantieRegistreren(autorisatieMgrMock.Object);

			var daos = Factory.Maak<LedenDaoCollectie>();
			var lm = Factory.Maak<LedenManager>();

			// Act

			lm.Zoeken(new LidFilter{GroepsWerkJaarID = testData.HuidigGwj.ID}, LidExtras.Geen);

			// Verwacht exception
		}

		/// <summary>
		/// Probeert aantal groepswerkjaren van een groep op te halen waarvoor
		/// men geen GAV is, via LedenManager.PaginaOphalen.
		/// Verwacht een exception.
		/// </summary>
		[TestMethod]
		[ExpectedException(typeof(GeenGavException))]
		public void LijstLedenGeenGavAantalGwj()
		{
			// Arrange

			var testData = new DummyData();

			var ledenDaoMock = new Mock<ILedenDao>();
			var autorisatieMgrMock = new Mock<IAutorisatieManager>();
			var groepenDaoMock = new Mock<IGroepenDao>();
			var groepsWerkJaarDaoMock = new Mock<IGroepsWerkJaarDao>();

			ledenDaoMock.Setup(foo => foo.AllesOphalen(testData.HuidigGwj.ID, LidEigenschap.Naam)).Returns(new List<Lid>());
			autorisatieMgrMock.Setup(foo => foo.IsGavGroepsWerkJaar(testData.HuidigGwj.ID)).Returns(false);
			groepsWerkJaarDaoMock.Setup(foo => foo.Ophalen(testData.HuidigGwj.ID)).Returns(testData.HuidigGwj);
			groepenDaoMock.Setup(foo => foo.Ophalen(testData.DummyGroep.ID)).Returns(testData.DummyGroep);

			Factory.InstantieRegistreren(ledenDaoMock.Object);
			Factory.InstantieRegistreren(groepenDaoMock.Object);
			Factory.InstantieRegistreren(autorisatieMgrMock.Object);
			Factory.InstantieRegistreren(groepsWerkJaarDaoMock.Object);

			Factory.Maak<LedenDaoCollectie>();
			var lm = Factory.Maak<LedenManager>();

			// Act
			lm.Zoeken(new LidFilter{GroepsWerkJaarID  =  testData.HuidigGwj.ID}, LidExtras.Geen);

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

			var communicatielozePersoon = new GelieerdePersoon();
			communicatielozePersoon.Communicatie = new System.Data.Objects.DataClasses.EntityCollection<CommunicatieVorm>();

			// GelieerdePersonenDao mocken, zodat die steeds de fake persoon oproept

			var gpDaoMock = new Mock<IGelieerdePersonenDao>();
			gpDaoMock.Setup(foo => foo.Ophalen(It.IsAny<int>()
			    , It.IsAny<Expression<Func<GelieerdePersoon, object>>[]>())).Returns(communicatielozePersoon);

			// GroepenDao mocken.

			var groepenDaoMock = new Mock<IGroepenDao>();

			// GelieerdePersonenManager aanmaken, waarbij autorisatieManager steeds 'false'
			// antwoordt.

			Factory.InstantieRegistreren(gpDaoMock.Object);
			Factory.InstantieRegistreren(groepenDaoMock.Object);
			Factory.InstantieRegistreren<IAutorisatieManager>(new AutMgrNooitGav());

			var gpMgr = Factory.Maak<CommVormManager>();
			#endregion

			#region act
			// Probeer nu een fictieve communicatievorm te verwijderen.
			// We verwachten 'GeenGavException'

			// CommunicatieVormID en GelieerdePersoonID zijn irrelevant owv de mocking,
			// maar als je hier null meegeeft voor parameter 'origineel' 
			// throwt gpMgr een NullRefereneException omdat hij origineel.ID opvraagt
			var commVorm = new CommunicatieVorm();
			commVorm.ID = 0;
			commVorm.GelieerdePersoon = new GelieerdePersoon();
			commVorm.GelieerdePersoon.ID = 0;
			gpMgr.CommunicatieVormVerwijderen(commVorm);

			#endregion

			#region assert
			// Als we dit tegenkomen, is het sowieso mislukt
			Assert.IsTrue(false);
			#endregion
		}

		/// <summary>
		/// Probeert lijst actieve leden op te halen als wel GAV
		/// Verwacht geen exception.
		/// </summary>
		[TestMethod]
		public void LijstActieveLedenGav()
		{
			// Arrange

			// Gebruik static member variable _testData, zodat die gemaakt wordt vooraleer er ergens wordt
			// gemockt.  (=> Fouten vermijden bij het maken van testdata)
			var testData = _testData;

			var ledenDaoMock = new Mock<ILedenDao>();
			var groepenDaoMock = new Mock<IGroepenDao>();
			var autorisatieMgrMock = new Mock<IAutorisatieManager>();
			var groepsWerkJaarDaoMock = new Mock<IGroepsWerkJaarDao>();
			var leidingDaoMock = new Mock<ILeidingDao>();

			// We mocken de dao's dat het zoeken naar leden/leiding een lege lijst oplevert

			ledenDaoMock.Setup(foo => foo.ActieveLedenOphalen(testData.HuidigGwj.ID, LidEigenschap.Naam)).Returns(new List<Lid>());
			leidingDaoMock.Setup(foo => foo.Zoeken(It.IsAny<LidFilter>(), It.IsAny<Expression<Func<Leiding, object>>[]>())).
				Returns(new List<Leiding>());
			
			autorisatieMgrMock.Setup(foo => foo.IsGavGroepsWerkJaar(testData.HuidigGwj.ID)).Returns(true);
			autorisatieMgrMock.Setup(foo => foo.IsGavGroep(testData.DummyGroep.ID)).Returns(true);

			groepsWerkJaarDaoMock.Setup(foo => foo.Ophalen(testData.HuidigGwj.ID, It.IsAny<Expression<Func<GroepsWerkJaar, object>>>())).Returns(testData.HuidigGwj);
			groepsWerkJaarDaoMock.Setup(foo => foo.IsRecentste(testData.HuidigGwj.ID)).Returns(true);
			groepenDaoMock.Setup(foo => foo.Ophalen(testData.DummyGroep.ID, It.IsAny<Expression<Func<Groep, object>>>())).Returns(testData.DummyGroep);
			
			Factory.InstantieRegistreren(ledenDaoMock.Object);
			Factory.InstantieRegistreren(leidingDaoMock.Object);
			Factory.InstantieRegistreren(autorisatieMgrMock.Object);
			Factory.InstantieRegistreren(groepenDaoMock.Object);
			Factory.InstantieRegistreren(groepsWerkJaarDaoMock.Object);

			var lm = Factory.Maak<LedenManager>();

			// Act
			var lijst = lm.Zoeken(
				new LidFilter{GroepsWerkJaarID  = testData.HuidigGwj.ID}, 
				LidExtras.Geen);

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

			var p1 = new Persoon { ID = 1 };
			var p2 = new Persoon { ID = 2 };
			var p3 = new Persoon { ID = 3 };

			// Creeer nu PersonenDaoMock, dia alle huisgenoten van p1 ophaalt.

			var pDaoMock = new Mock<IPersonenDao>();
			pDaoMock.Setup(foo => foo.HuisGenotenOphalen(1)).Returns(new List<Persoon> { p1, p2, p3 });

			// en een AutorisatieManagerMock, die zorgt dat gebruiker alvast toegang heeft tot p1.

			var auManMock = new Mock<IAutorisatieManager>();

			auManMock.Setup(foo => foo.IsGavGelieerdePersoon(1)).Returns(true);
			auManMock.Setup(foo => foo.EnkelMijnPersonen(It.IsAny<IList<int>>())).Returns(new List<int> { 1, 3 });

			// Tenslotte de personenManager die we willen
			// testen.

			Factory.InstantieRegistreren(pDaoMock.Object);
			Factory.InstantieRegistreren(auManMock.Object);

			var pm = Factory.Maak<PersonenManager>();

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

		/// <summary>
		/// Probeer een afdelingsjaar van een eigen afdeling te maken in een groep waar je
		/// geen GAV van bent
		/// </summary>
		[TestMethod]
		[ExpectedException(typeof(GeenGavException))]
		public void AfdelingsJaarMakenAndereGroep()
		{
			var gwj = new GroepsWerkJaar {ID = 1};
			var afd = new Afdeling();
			var oa = new OfficieleAfdeling();

			// Creer een AutorisatieManagerMock die zegt dat de gebruiker geen GAV is
			// van het gegeven groepswerkjaar.

			var auMgrMock = new Mock<IAutorisatieManager>();
			auMgrMock.Setup(mgr => mgr.IsGavGroepsWerkJaar(1)).Returns(false);

			Factory.InstantieRegistreren(auMgrMock.Object);

			var ajMgr = Factory.Maak<AfdelingsJaarManager>();

			ajMgr.Aanmaken(afd, oa, gwj, 200, 2001, GeslachtsType.Gemengd);

			// Als we hier nog komen, heeft de verwachte exception zich niet voorgedaan.

			Assert.IsTrue(false);
		}

		/// <summary>
		/// Verwacht een exception wanneer geprobeerd wordt het AD-nummer aan te passen.
		/// </summary>
		[TestMethod]
		[ExpectedException(typeof(InvalidOperationException))]
		public void AdNummerWijzigen()
		{
			#region Arrange

			// testData aanmaken.  Voor de zekerheid eerst de AutMgrAltijdGav registreren als
			// IAutorisatieManager, want het kan goed zijn dat een vorige test dat wijzigde.

			Factory.InstantieRegistreren<IAutorisatieManager>(new AutMgrAltijdGav());
			var testData = new DummyData();

			// GelieerdePersonenDao mocken.  Van een Dao verwachten
			// we dat die gewoon doet wat we vragen; er is daar geen
			// businesslogica geimplementeerd

			var gpDaoMock = new Mock<IGelieerdePersonenDao>();

			gpDaoMock.Setup(
			    foo => foo.Ophalen(testData.GelieerdeJos.ID,
			    It.IsAny<Expression<Func<GelieerdePersoon, Object>>>())).Returns(testData.KloonJos);

			// Het stuk It.IsAny<System.Linq.Expressions.Expression<System.Func<GelieerdePersoon, Object>>>()
			// zorgt ervoor dat de Mock de linq-expressies in 'Ophalen' negeert.
			//
			// De constructie in 'Returns' zorgt ervoor dat MaakTestGelieerdePersoon iedere
			// keer uitgevoerd wordt bij aanroep van 'Ophalen'.  (En niet eenmalig bij het
			// opzetten van de mock.)

			// Maak nu de GelieerdePersoonenManager aan die we willen testen.

			Factory.InstantieRegistreren(gpDaoMock.Object);
			Factory.InstantieRegistreren<IAutorisatieManager>(new AutMgrAltijdGav());

			var gpm = Factory.Maak<GelieerdePersonenManager>();

			#endregion

			#region Act
			// Haal gelieerde persoon met TestGelieerdePersoonID op
			GelieerdePersoon gp = gpm.Ophalen(testData.GelieerdeJos.ID);

			// Pruts met AD-nummer
			++gp.Persoon.AdNummer;

			// Probeer te bewaren
			gpm.Bewaren(gp, PersoonsExtras.Geen);
			#endregion

			#region Assert
			// Als we hier geraken, is het zeker niet gelukt.
			Assert.IsTrue(false);
			#endregion
		}

		/// <summary>
		/// Test die nakijkt of ik een persoon kan bewaren
		/// als ik het ad-nummer niet wijzig.  (Verwacht = ja)
		/// </summary>
		[TestMethod]
		public void AdNummerNietWijzigen()
		{
			#region Arrange

			// testData aanmaken.  Voor de zekerheid eerst de AutMgrAltijdGav registreren als
			// IAutorisatieManager, want het kan goed zijn dat een vorige test dat wijzigde.

			var autMgr = new AutMgrAltijdGav();

			Factory.InstantieRegistreren<IAutorisatieManager>(autMgr);
			var testData = new DummyData();

			// GelieerdePersonenDao mocken.  Van een Dao verwachten
			// we dat die gewoon doet wat we vragen; er is daar geen
			// businesslogica geimplementeerd

			var gpDaoMock = new Mock<IGelieerdePersonenDao>();

			// Ophalen geeft gewoon 'GelieerdeJos', en bewaren een kopie daarvan.
			gpDaoMock.Setup(foo => foo.Ophalen(testData.GelieerdeJos.ID
			    , It.IsAny<Expression<Func<GelieerdePersoon, Object>>>())).Returns(() => testData.GelieerdeJos);

			gpDaoMock.Setup(foo => foo.Bewaren(
				It.IsAny<GelieerdePersoon>(), 
				It.IsAny<Expression<Func<GelieerdePersoon, object>>[]>())).Returns((
					GelieerdePersoon foo, 
					Expression<Func<GelieerdePersoon, object>>[] bar) => foo);

			// Het stuk It.IsAny<Expression<Func<GelieerdePersoon, Object>>>()
			// zorgt ervoor dat de Mock de linq-expressies in 'Ophalen' negeert.
			//
			// De constructie in 'Returns' zorgt ervoor dat MaakTestGelieerdePersoon iedere
			// keer uitgevoerd wordt bij aanroep van 'Ophalen'.  (En niet eenmalig bij het
			// opzetten van de mock.)

			// Maak nu de GelieerdePersoonenManager aan die we willen testen.

			Factory.InstantieRegistreren(gpDaoMock.Object);

			var gpm = Factory.Maak<GelieerdePersonenManager>();

			#endregion

			#region Act
			// Haal gelieerde persoon met TestGelieerdePersoonID op
			GelieerdePersoon gp = gpm.Ophalen(testData.GelieerdeJos.ID);

			// Probeer te bewaren
			gpm.Bewaren(gp,	PersoonsExtras.Geen);
			#endregion

			#region Assert
			// Als we hier geraken, is het ok.
			Assert.IsTrue(true);
			gpDaoMock.VerifyAll();  // nakijken of de mock van GelieerdePersonenDao inderdaad aangeroepen werd.
			#endregion
		}
	}
}
