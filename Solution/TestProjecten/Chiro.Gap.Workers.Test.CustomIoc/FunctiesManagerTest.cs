using System.Diagnostics;

using Chiro.Gap.Workers.Exceptions;

using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using Moq;

using Chiro.Cdf.Ioc;
using Chiro.Gap.Dummies;
using Chiro.Gap.Orm;
using Chiro.Gap.Orm.DataInterfaces;

namespace Chiro.Gap.Workers.Test.CustomIoc
{
	/// <summary>
	/// Dit is een testclass voor Unit Tests van FunctiesManagerTest,
	/// to contain all FunctiesManagerTest Unit Tests
	/// </summary>
	[TestClass]
	public class FunctiesManagerTest
	{
		#region Additional test attributes
		// 
		//You can use the following additional attributes as you write your tests:
		//
		//Use ClassInitialize to run code before running the first test in the class
		[ClassInitialize]
		public static void MyClassInitialize(TestContext testContext)
		{
			Factory.ContainerInit();
		}
		//
		// Use ClassCleanup to run code after all tests in a class have run
		// [ClassCleanup()]
		// public static void MyClassCleanup()
		// { 
		// }
		//
		// Use TestInitialize to run code before running each test
		// [TestInitialize]
		// public void MyTestInitialize()
		// {
		// }
		//
		// Use TestCleanup to run code after each test has run
		// [TestCleanup()]
		// public void MyTestCleanup()
		// {
		// }
		//
		#endregion

		/// <summary>
		/// Controleert of de nationaal bepaalde functies gecachet worden, door te tellen hoe dikwijls
		/// de DAO opgevraagd wordt.
		/// </summary>
		[TestMethod]
		public void NationaalBepaaldeFunctiesOphalenTest()
		{
			#region Arrange

			// Deze DAO's nog eens expliciet registreren, om te vermijden dat wijzigingen in
			// andere tests een vertekend beeld opleveren.

			Factory.InstantieRegistreren<IAutorisatieManager>(new AutMgrAltijdGav());
			Factory.InstantieRegistreren<ILedenDao>(new DummyLedenDao());

			// Mock voor IFunctieDao, die een lege lijst geeft als de nationaal bepaalde functies
			// opgevraagd worden.  We willen gewoon tellen hoe dikwijls deze opgevraagd wordt.
			var funDaoMock = new Mock<IFunctiesDao>();
			funDaoMock.Setup(dao => dao.NationaalBepaaldeFunctiesOphalen()).Returns(new List<Functie>());
			Factory.InstantieRegistreren(funDaoMock.Object);

			var target = Factory.Maak<FunctiesManager>();

			#endregion

			#region Act
			// tweemaal opvragen
			var resultaat = target.NationaalBepaaldeFunctiesOphalen();
			resultaat = target.NationaalBepaaldeFunctiesOphalen();
			#endregion

			#region Assert

			funDaoMock.Verify(dao => dao.NationaalBepaaldeFunctiesOphalen(), Times.AtMost(1), "Nationale functies waren niet gecachet.");

			#endregion
		}

		/// <summary>
		/// Test voor ticket #890 
		/// </summary>
		[TestMethod]
		public void FunctieOphalenTest()
		{
			// Arrange

			var auMgrMock = new Mock<IAutorisatieManager>();
			var funDaoMock = new Mock<IFunctiesDao>();

			auMgrMock.Setup(mgr => mgr.IsGavCategorie(It.IsAny<int>())).Returns(false);
			auMgrMock.Setup(mgr => mgr.IsGavFunctie(It.IsAny<int>())).Returns(true);

			funDaoMock.Setup(mgr => mgr.Ophalen(It.IsAny<int>())).Returns(new Functie());

			Factory.InstantieRegistreren<IAutorisatieManager>(auMgrMock.Object);
			Factory.InstantieRegistreren<IFunctiesDao>(funDaoMock.Object);

			var funMgr = Factory.Maak<FunctiesManager>();

			// act

			var resultaat = funMgr.Ophalen(100, false); // haal functie op zonder iets extra

			// assert

			// Aangezien ik de autorisatiemanager gemockt heb, zodat je rechten krijgt op iedere
			// functie, moet er een functie opgehaald zijn.

			Assert.IsNotNull(resultaat);
		}

		/// <summary>
		/// Controleert of een ongebruikte functie probleemloos verwijderd kan worden
		/// </summary>
		[TestMethod]
		public void OngebruikteFunctieVerwijderenTest()
		{
			// arrange

			var testData = new DummyData();

			var veelGebruiktMock = new Mock<IVeelGebruikt>();
			veelGebruiktMock.Setup(vgb => vgb.GroepsWerkJaarOphalen(testData.DummyGroep.ID)).Returns(testData.HuidigGwj);
			Factory.InstantieRegistreren<IVeelGebruikt>(veelGebruiktMock.Object);

			var mgr = Factory.Maak<FunctiesManager>();

			// act

			var result = mgr.Verwijderen(testData.OngebruikteFunctie, false);

			// assert

			Assert.IsNull(result);
		}

		/// <summary>
		/// probeert een functie die dit jaar in gebruik is te verwijderen.  We verwachten een exception.
		/// </summary>
		[TestMethod]
		[ExpectedException(typeof(BlokkerendeObjectenException<Lid>))]
		public void FunctieDitJaarInGebruikVerwijderenTest()
		{
			// arrange

			var testData = new DummyData();

			var veelGebruiktMock = new Mock<IVeelGebruikt>();
			veelGebruiktMock.Setup(vgb => vgb.GroepsWerkJaarOphalen(testData.DummyGroep.ID)).Returns(testData.HuidigGwj);
			Factory.InstantieRegistreren<IVeelGebruikt>(veelGebruiktMock.Object);

			var mgr = Factory.Maak<FunctiesManager>();

			// act

			var result = mgr.Verwijderen(testData.UniekeFunctie, false);

			// assert

			// Als we hier toekomen zonder exception, dan ging er iets mis.
			Assert.IsTrue(false);
		}

		/// <summary>
		/// probeert een functie die enkel dit jaar in gebruik is, geforceerd te verwijderen. 
		/// We verwachten dat ze definitief weg is.
		/// </summary>
		[TestMethod]
		public void FunctieEnkelDitJaarInGebruikGeforceerdVerwijderenTest()
		{
			// arrange

			var testData = new DummyData();

			var veelGebruiktMock = new Mock<IVeelGebruikt>();
			veelGebruiktMock.Setup(vgb => vgb.GroepsWerkJaarOphalen(testData.DummyGroep.ID)).Returns(testData.HuidigGwj);
			Factory.InstantieRegistreren<IVeelGebruikt>(veelGebruiktMock.Object);

			var mgr = Factory.Maak<FunctiesManager>();

			// act

			var result = mgr.Verwijderen(testData.UniekeFunctie, true);

			// assert

			Assert.IsNull(result);
		}

		/// <summary>
		/// probeert een functie die zowel dit jaar als vorig jaar gebruikt is, 
		/// geforceerd te verwijderen.  We verwachten dat het 'werkjaar tot'  wordt
		/// ingevuld.
		/// </summary>
		[TestMethod]
		public void FunctieLangerInGebruikGeforceerdVerwijderenTest()
		{
			// arrange

			var testData = new DummyData();

			Debug.Assert(testData.HuidigGwj != null);

			var veelGebruiktMock = new Mock<IVeelGebruikt>();
			veelGebruiktMock.Setup(vgb => vgb.GroepsWerkJaarOphalen(testData.DummyGroep.ID)).Returns(testData.HuidigGwj);
			Factory.InstantieRegistreren<IVeelGebruikt>(veelGebruiktMock.Object);

			var mgr = Factory.Maak<FunctiesManager>();

			// act

			var result = mgr.Verwijderen(testData.TraditieFunctie, true);

			// assert

			// functie niet meer geldig
			Assert.IsNotNull(result);
			Assert.AreEqual(result.WerkJaarTot, testData.HuidigGwj.WerkJaar - 1);

			// enkel het lid van dit werkjaar blijft over
			Assert.AreEqual(result.Lid.Count, 1);
		}
	}
}
