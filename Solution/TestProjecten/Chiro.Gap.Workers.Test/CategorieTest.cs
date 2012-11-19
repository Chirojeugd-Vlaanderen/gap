using System;
using Chiro.Gap.Dummies;
using Chiro.Gap.Poco.Model;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Chiro.Cdf.Ioc;

namespace Chiro.Gap.Workers.Test
{


	/// <summary>
	/// Dit is een testclass voor Unit Tests van GelieerdePersonenManagerTest,
	/// to contain all GelieerdePersonenManagerTest Unit Tests
	/// </summary>
	[TestClass]
	public class CategorieTest
	{



        [ClassInitialize]
        static public void InitialiseerTests(TestContext tc)
        {
            Factory.ContainerInit();
        }

        [ClassCleanup]
        static public void AfsluitenTests()
        {
            
        }

		#region Additional test attributes
		// 
		//You can use the following additional attributes as you write your tests:
		//
		//Use ClassInitialize to run code before running the first test in the class
		//[ClassInitialize]
		//public static void MyClassInitialize(TestContext testContext)
		//{
		//}
		//
		//Use ClassCleanup to run code after all tests in a class have run
		//[ClassCleanup()]
		//public static void MyClassCleanup()
		//{
		//}
		//
		//Use TestInitialize to run code before running each test
		//[TestInitialize]
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

		// Tests die niets deden meteen weggegooid.


		/// <summary>
		/// Controleert of een categorie niet per ongeluk dubbel gekoppeld kan worden aan eenzelfde persoon.
		/// GelieerdeJos zit in de categorie Vervelend, we proberen hem daar nog eens aan toe te voegen, en
		/// hopen dat het mislukt.
		/// </summary>
		[TestMethod]
		public void DubbeleKoppelingCategorie()
		{
			// Arrange

			var testData = new DummyData();

			var gpMgr = Factory.Maak<GelieerdePersonenManager>();

			if (!testData.GelieerdeJos.Categorie.Contains(testData.Vervelend))
			{
				// Zeker zijn dat Jos al een keer in de categorie zit.
				gpMgr.CategorieKoppelen(
					new GelieerdePersoon[] { testData.GelieerdeJos }, 
					testData.Vervelend);
			}

			int aantalCategorieen = testData.GelieerdeJos.Categorie.Count;
		
			// Act

			gpMgr.CategorieKoppelen(new GelieerdePersoon[] { testData.GelieerdeJos }, testData.Vervelend);
			
			// Assert		

			Assert.IsTrue(testData.GelieerdeJos.Categorie.Count == aantalCategorieen);
		}

		[TestMethod]
		public void LoskoppelenCategorie()
		{
            //// Arrange

            //var testData = new DummyData();
            //var gpMgr = Factory.Maak<GelieerdePersonenManager>();
            //// Voeg Irene voor het gemak toe aan de vervelende personen, zodat ze verwijderd kan worden.
            //gpMgr.CategorieKoppelen(
            //    new GelieerdePersoon[] { testData.GelieerdeIrene }, 
            //    testData.Vervelend);

            //// Act

            //// opnieuw loskoppelen
            //gpMgr.CategorieLoskoppelen(new int[] { testData.GelieerdeIrene.ID }, testData.Vervelend);

            //// Assert

            //// LosKoppelen is een 'delete', dus moet er direct bewaard worden.

            //Assert.IsFalse(testData.Vervelend.GelieerdePersoon.Contains(testData.GelieerdeIrene));
            throw new NotImplementedException();
		}

	}
}
