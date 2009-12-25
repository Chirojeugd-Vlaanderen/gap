using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Chiro.Gap.Orm;
using Chiro.Gap.Workers;
using Chiro.Cdf.Ioc;
using Chiro.Gap.Orm.DataInterfaces;
using System.IO;
using System.Runtime.Serialization;
using Chiro.Gap.Dummies;

namespace Chiro.Gap.Workers.Test
{
	/// <summary>
	/// Summary description for CategorieToevoegen
	/// </summary>
	[TestClass]
	public class CategorieToevoegen
	{
		int groepID = Properties.Settings.Default.GroepID;
		String categorienaam = Properties.Settings.Default.CategorieNaam;
		GroepenManager gm;

		public CategorieToevoegen()
		{
			//
			// TODO: Add constructor logic here
			//
		}


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

		[TestInitialize]
		public void setUp()
		{
			gm = Factory.Maak<GroepenManager>();
		}

		[TestCleanup]
		public void breakDown()
		{
			// De bedoeling is dat de businesslaag getest wordt, en niet de data access.
			// De toegevoegde categorie moet dus enkel uit het geheugen opnieuw
			// verwijderd worden.

			//Groep g = gm.Ophalen(groepID, e => e.Categorie);
			Groep g = DummyData.DummyGroep;

			//foreach (Categorie c in g.Categorie)
			//{
			//        if (c.Naam.Equals(categorienaam))
			//        {
			//                gm.CategorieVerwijderen(g, c);
			//        }
			//}
			foreach (Categorie c in (from ctg in g.Categorie where ctg.Naam.Equals(categorienaam) select ctg).ToList())
			{
				// Eigenlijk is de parameter 'g' overbodig, aangezien een categorie
				// telkens aan slechts 1 groep gebonden is.

				gm.CategorieVerwijderen(g, c);
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

		[TestMethod]
		public void TestCategorieToevoegen()
		{
			// Het is niet de bedoeling dat deze test de database 
			// raadpleegt.  Refs #129.

			// Groep g = gm.Ophalen(groepID, e => e.Categorie);
			Groep g = DummyData.DummyGroep;

			//foreach (Categorie c in g.Categorie)
			//{
			//        Assert.IsFalse(c.Naam.Equals(categorienaam));
			//}
			Assert.IsTrue((from c in g.Categorie where c.Naam.Equals(categorienaam) select c).FirstOrDefault() == null);

			gm.CategorieToevoegen(g, categorienaam, "");

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
