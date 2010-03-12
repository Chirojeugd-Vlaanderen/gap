using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using Moq;

using Chiro.Cdf.Ioc;
using Chiro.Gap.Dummies;
using Chiro.Gap.Workers;
using Chiro.Gap.Orm;
using Chiro.Gap.Orm.DataInterfaces;

namespace Chiro.Gap.Workers.Test.CustomIoc
{
    
    
    /// <summary>
    ///This is a test class for FunctiesManagerTest and is intended
    ///to contain all FunctiesManagerTest Unit Tests
    ///</summary>
	[TestClass()]
	public class FunctiesManagerTest
	{


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
		//You can use the following additional attributes as you write your tests:
		//
		//Use ClassInitialize to run code before running the first test in the class
		[ClassInitialize()]
		public static void MyClassInitialize(TestContext testContext)
		{
			Factory.ContainerInit();
		}
		//
		//Use ClassCleanup to run code after all tests in a class have run
		//[ClassCleanup()]
		//public static void MyClassCleanup()
		//{
		//}
		//
		//Use TestInitialize to run code before running each test
		//[TestInitialize()]
		//public void MyTestInitialize()
		//{
		//}
		//
		//Use TestCleanup to run code after each test has run
		//[TestCleanup()]
		//public void MyTestCleanup()
		//{
		//}
		//
		#endregion


		/// <summary>
		/// Controleert of de nationaal bepaalde functies gecachet worden, door te tellen hoe dikwijls
		/// de DAO opgevraagd wordt.
		/// </summary>
		[TestMethod()]
		public void NationaalBepaaldeFunctiesOphalenTest()
		{
			#region Arrange

			var aantalAanroepen = 0;

			// Deze DAO's nog eens expliciet registreren, om te vermijden dat wijzigingen in
			// andere tests een vertekend beeld opleveren.

			Factory.InstantieRegistreren<IAutorisatieManager>(new AutMgrAltijdGav());
			Factory.InstantieRegistreren<ILedenDao>(new DummyLedenDao());

			// Mock voor IFunctieDao, die een lege lijst geeft als de nationaal bepaalde functies
			// opgevraagd worden.  We willen gewoon tellen hoe dikwijls deze opgevraagd wordt.
			var funDaoMock = new Mock<IFunctiesDao>();
			funDaoMock.Setup(dao => dao.NationaalBepaaldeFunctiesOphalen()).Returns(() => {
				++aantalAanroepen;
				return new List<Functie>();});
			Factory.InstantieRegistreren<IFunctiesDao>(funDaoMock.Object);

			FunctiesManager target = Factory.Maak<FunctiesManager>();

			#endregion

			#region Act
			// tweemaal opvragen
			var resultaat = target.NationaalBepaaldeFunctiesOphalen();
			resultaat = target.NationaalBepaaldeFunctiesOphalen();
			#endregion

			#region Assert
			Assert.AreEqual(aantalAanroepen, 1);
			#endregion
		}
	}
}
