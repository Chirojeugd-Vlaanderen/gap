using Chiro.Gap.Workers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Chiro.Gap.Orm.DataInterfaces;
using Chiro.Gap.Orm;
using System.Collections.Generic;
using Chiro.Gap.Dummies;
using Chiro.Cdf.Ioc;

namespace Chiro.Gap.Workers.Test
{


	/// <summary>
	///This is a test class for GelieerdePersonenManagerTest and is intended
	///to contain all GelieerdePersonenManagerTest Unit Tests
	///</summary>
	[TestClass()]
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
            Factory.Dispose();
        }

		#region Additional test attributes
		// 
		//You can use the following additional attributes as you write your tests:
		//
		//Use ClassInitialize to run code before running the first test in the class
		//[ClassInitialize()]
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

			var gpMgr = Factory.Maak<GelieerdePersonenManager>();
			// (kijk even na of Jos wel in de categorie zit)
			Assert.IsTrue(DummyData.GelieerdeJos.Categorie.Contains(DummyData.Vervelend));
			int aantalCategorieen = DummyData.GelieerdeJos.Categorie.Count;
		
			// Act

			gpMgr.CategorieKoppelen(
				new GelieerdePersoon[] { DummyData.GelieerdeJos }, 
				DummyData.Vervelend, 
				true);
			
			// Assert		

			Assert.IsTrue(DummyData.GelieerdeJos.Categorie.Count == aantalCategorieen);
		}

		[TestMethod]
		public void LoskoppelenCategorie()
		{
			// Arrange

			var gpMgr = Factory.Maak<GelieerdePersonenManager>();
			// Voeg Irene voor het gemak toe aan de vervelende personen, zodat ze verwijderd kan worden.
			gpMgr.CategorieKoppelen(
				new GelieerdePersoon[] { DummyData.GelieerdeIrene },
				DummyData.Vervelend,
				true);

			// Act

			// opnieuw loskoppelen
			gpMgr.CategorieKoppelen(
				new GelieerdePersoon[] { DummyData.GelieerdeIrene },
				DummyData.Vervelend,
				false);

			// Assert

			// De koppeling categorie - gelieerde persoon moet nu gemarkeerd zijn als 'te verwijderen', ze
			// blijft dus wel bestaan.

			Assert.IsTrue(DummyData.Vervelend.GelieerdePersoon.Contains(DummyData.GelieerdeIrene));

			// In praktijk wil dat zeggen dat Irene als te verwijderen gemarkeerd is.
			// Eigenlijk is dat gevaarlijk, want als nu de persoon gepersisteerd wordt ipv de categorie,
			// dan valt de persoon weg.  Wat niet de bedoeling is uiteraard.  Maar ik vrees dat
			// het moeilijk anders kan zolang er geen beter multi-tier support is voor EF.

			Assert.IsTrue(DummyData.GelieerdeIrene.TeVerwijderen);

		}

	}
}
