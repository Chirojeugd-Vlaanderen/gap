using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Cg2.Orm.DataInterfaces;
using Cg2.Orm;
using Cg2.Workers;
using Cg2.Dummies;
using Chiro.Cdf.EfWrapper.Entity;
using Chiro.Cdf.Ioc;

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

		  ICategorieenDao _catdao = null;
		  IGelieerdePersonenDao _gpdao = null;
		  int _catID, _gpID, _gp2ID;

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
				Chiro.Cdf.Ioc.Factory.ContainerInit();
		  }

		  [TestInitialize()]
		  public void MyTestInitialize()
		  {
				_catdao = Factory.Maak<ICategorieenDao>();
				_gpdao = Factory.Maak<IGelieerdePersonenDao>();
				_catID = Properties.Settings.Default.TestCategorieID;
				_gpID = Properties.Settings.Default.TestGelieerdePersoonID;
				_gp2ID = Properties.Settings.Default.TestGelieerdePersoon2ID;
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
              Categorie c = _catdao.Ophalen(_catID, foo=>foo.GelieerdePersoon);

              // assert
              Assert.IsTrue(c.ID == _catID);
          }
		  

		  /// <summary>
		  /// Test op toevoegen van een gelieerde persoon aan een categorie
		  /// (many->many via PersoonsCategorie)
		  /// </summary>
		  [TestMethod]
		  public void PersoonToevoegenAanCategorie()
		  {
				//arrange
				GelieerdePersoon gp = _gpdao.Ophalen(_gpID);
				Categorie cat = _catdao.Ophalen(_catID);

				//add
				// koppel categorie aan groep
				cat.GelieerdePersoon.Add(gp);
				gp.Categorie.Add(cat);

				// bewaar via dao (we zijn de dao aan het testen)
				_catdao.Bewaren(cat);

				//assert
				IList<GelieerdePersoon> lijst = _catdao.Ophalen(_catID).GelieerdePersoon.ToList();

				var query = (from item in lijst
								 where item.ID == _gpID
								 select item);

				Assert.IsTrue(query.Count() == 1);

				//cleanup
				cat.GelieerdePersoon.First().TeVerwijderen = true;
				_catdao.Bewaren(cat);
		  }

          [TestMethod]
          public void TestHashCodes()
          {
              GelieerdePersoon p1 = new GelieerdePersoon();
              p1.ID = 10;
              GelieerdePersoon p2 = new GelieerdePersoon();
              p2.ID = 10;
              Categorie c1 = new Categorie();
              c1.ID = 100;
              Categorie c2 = new Categorie();
              c2.ID = 10;
              Assert.AreEqual(p1.GetHashCode(), p2.GetHashCode());
              Assert.AreEqual(c1.GetHashCode(), c2.GetHashCode());
              Assert.AreNotEqual(c1.GetHashCode(), p1.GetHashCode());
          }
	 }
}
