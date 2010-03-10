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

			if (!DummyData.GelieerdeJos.Categorie.Contains(DummyData.Vervelend))
			{
				// Zeker zijn dat Jos al een keer in de categorie zit.
				gpMgr.CategorieKoppelen(new GelieerdePersoon[] { DummyData.GelieerdeJos }, DummyData.Vervelend);
			}

			int aantalCategorieen = DummyData.GelieerdeJos.Categorie.Count;
		
			// Act

			gpMgr.CategorieKoppelen(new GelieerdePersoon[] { DummyData.GelieerdeJos }, DummyData.Vervelend);
			
			// Assert		

			Assert.IsTrue(DummyData.GelieerdeJos.Categorie.Count == aantalCategorieen);
		}

		[TestMethod]
		public void LoskoppelenCategorie()
		{
			// Arrange

			var gpMgr = Factory.Maak<GelieerdePersonenManager>();
			// Voeg Irene voor het gemak toe aan de vervelende personen, zodat ze verwijderd kan worden.
			gpMgr.CategorieKoppelen(new GelieerdePersoon[] { DummyData.GelieerdeIrene },DummyData.Vervelend);

			// Act

			// opnieuw loskoppelen
			gpMgr.CategorieLoskoppelen(new int[] { DummyData.GelieerdeIrene.ID },DummyData.Vervelend);

			// Assert

			// LosKoppelen is een 'delete', dus moet er direct bewaard worden.

			Assert.IsFalse(DummyData.Vervelend.GelieerdePersoon.Contains(DummyData.GelieerdeIrene));
		}

	}
}
