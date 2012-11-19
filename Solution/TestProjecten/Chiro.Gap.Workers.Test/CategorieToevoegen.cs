using System;
using System.Linq;
using Chiro.Gap.Dummies;
using Chiro.Gap.Poco.Model;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Chiro.Cdf.Ioc;

namespace Chiro.Gap.Workers.Test
{
	/// <summary>
	/// Testclass voor CategorieToevoegen
	/// </summary>
	[TestClass]
	public class CategorieToevoegen
	{
		int groepID = Properties.Settings.Default.GroepID;
		String categorienaam = Properties.Settings.Default.CategorieNaam;
		GroepenManager gm;

		#region initialiseren en afronden

        [ClassInitialize]
		static public void TestsInitialiseren(TestContext tc)
        {
            Factory.ContainerInit();
        }

        [ClassCleanup]
		static public void TestsAfsluiten()
        {
            
        }

		[TestInitialize]
		public void SetUp()
		{
			gm = Factory.Maak<GroepenManager>();
		}

		#region Additional test attributes
		//
		// You can use the following additional attributes as you write your tests:
		//
		// Use ClassInitialize to run code before running the first test in the class
		// [ClassInitialize]
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
		
		#endregion

		[TestMethod]
		public void TestCategorieToevoegen()
		{
			var testData = new DummyData();

			// Het is niet de bedoeling dat deze test de database 
			// raadpleegt.  Refs #129.

			// Groep g = gm.Ophalen(groepID, e => e.Categorie);
			Groep g = testData.DummyGroep;

			//foreach (Categorie c in g.Categorie)
			//{
			//        Assert.IsFalse(c.Naam.Equals(categorienaam));
			//}
			Assert.IsTrue((from c in g.Categorie where c.Naam.Equals(categorienaam) select c).FirstOrDefault() == null);

			gm.CategorieToevoegen(g, categorienaam, String.Empty);

			//g = gm.Ophalen(groepID, e => e.Categorie);
			//bool found = false;
			//foreach (Categorie c in g.Categorie)
			//{
			//        if (c.Naam.Equals(categorienaam))
			//        {
			//                found = true;
			//        }
			//}
			//Assert.IsTrue(found);

			Assert.IsTrue((from c in g.Categorie where c.Naam.Equals(categorienaam) select c).FirstOrDefault() != null);

			// Testen of groepen met een categorie goed bewaard worden, moet gebeuren in Chiro.Gap.Data.Test.
		}
	}
}
