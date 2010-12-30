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
	/// This is a test class for FunctiesManagerTest and is intended
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

			funDaoMock.Verify(dao => dao.NationaalBepaaldeFunctiesOphalen(), Times.Exactly(1), "Nationale functies waren niet gecachet.");

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
	}
}
