using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Cg2.Orm.DataInterfaces;
using Cg2.Orm;
using Cg2.Workers;
using Cg2.Dummies;
using Cg2.EfWrapper.Entity;
using Cg2.Ioc;

namespace Cg2.Dao.Test
{
	 /// <summary>
	 /// Tests mbt de categorieen die een gelieerde persoon kan hebben.
	 /// </summary>
	 [TestClass]
	 public class CategorieenTest
	 {
		  // De 'TeVerwijderen' zorgt er bij 1->veel enkel voor
		  // dat de link tussen GelieerdePersoon en Categorie
		  // verwijderd wordt, en de Categorie zelf niet.
		  // Om dat laatste te bewerkstellingen, had 
		  // GelieerdePersoon gedecoreerd moeten zijn met
		  // [AssociationEndBehavior("PersoonsCategorie", Owned=true)]
		  // dus TODO apart verwijderen van categorie toelaten!

		  ICategorieenDao catdao = null;
		  IGelieerdePersonenDao gpdao = null;
		  int catID, gpID, gp2ID;

		  public CategorieenTest(){ }

		  /// <summary>
		  ///Gets or sets the test context which provides
		  ///information about and functionality for the current test run.
		  ///</summary>
		  public TestContext TestContext { get; set; }

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
		  [ClassInitialize]
		  static public void TestInit(TestContext context)
		  {
				Cg2.Ioc.Factory.InitContainer();
		  }

		  [TestInitialize()]
		  public void MyTestInitialize()
		  {
				catdao = Factory.Maak<ICategorieenDao>();
				gpdao = Factory.Maak<IGelieerdePersonenDao>();
				catID = Properties.Settings.Default.TestCategorieID;
				gpID = Properties.Settings.Default.TestGelieerdePersoonID;
				gp2ID = Properties.Settings.Default.TestGelieerdePersoon2ID;
		  }

		  [TestCleanup()]
		  public void MyTestCleanup()
		  {
		  }

		  #endregion

         /// <summary>
         /// Test op het ophalen van een categorie
         /// </summary>
          [TestMethod]
          public void CategorieOphalen()
          {
              // act
              Categorie c = catdao.Ophalen(catID, foo=>foo.GelieerdePersoon);

              // assert
              Assert.IsTrue(c.ID == catID);
          }
		  

		  /// <summary>
		  /// Test op toevoegen van een gelieerde persoon aan een categorie
		  /// (many->many via PersoonsCategorie)
		  /// </summary>
		  [TestMethod]
		  public void PersoonToevoegenAanCategorie()
		  {
				//arrange
				GelieerdePersoon gp = gpdao.Ophalen(gpID);
				Categorie cat = catdao.Ophalen(catID);

				//add
				// koppel categorie aan groep
				cat.GelieerdePersoon.Add(gp);
				gp.Categorie.Add(cat);

				// bewaar via dao (we zijn de dao aan het testen)
				catdao.Bewaren(cat);

				//assert
				IList<GelieerdePersoon> lijst = catdao.Ophalen(catID).GelieerdePersoon.ToList();

				var query = (from item in lijst
								 where item.ID == gpID
								 select item);

				Assert.IsTrue(query.Count() == 1);

				//cleanup
				cat.GelieerdePersoon.First().TeVerwijderen = true;
				catdao.Bewaren(cat);
		  }
	 }
}
