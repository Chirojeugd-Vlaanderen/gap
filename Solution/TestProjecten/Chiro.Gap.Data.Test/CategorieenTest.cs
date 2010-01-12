using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Chiro.Gap.Orm.DataInterfaces;
using Chiro.Gap.Orm;
using Chiro.Gap.Workers;
using Chiro.Gap.Dummies;
using Chiro.Cdf.Data.Entity;
using Chiro.Cdf.Ioc;
using Chiro.Gap.TestDbInfo;

namespace Chiro.Gap.Data.Test
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

		public CategorieenTest() { }

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
			_catID = TestInfo.CATEGORIEID;
			_gpID = TestInfo.GELIEERDEPERSOONID;
			_gp2ID = TestInfo.GELIEERDEPERSOON2ID;
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
			Categorie c = _catdao.Ophalen(_catID, foo => foo.GelieerdePersoon);

			// assert
			Assert.IsTrue(c.ID == _catID);
		}

		/// <summary>
		/// Ophalen van een categorie op basis van groepid en code
		/// </summary>
		[TestMethod]
		public void CategorieOphalenGroepCode()
		{
			// act
			Categorie c = _catdao.Ophalen(TestInfo.GROEPID, TestInfo.CATEGORIECODE);

			// assert
			Assert.IsTrue(c != null);
		}

		/// <summary>
		/// Controleert of bij het ophalen van een pagina uit een categorie, de waarde van
		/// de output parameter aantalTotaal klopt.
		/// </summary>
		[TestMethod]
		public void PaginaOphalenUitCategorieAantal()
		{
			// arrange
			int aantalTotaal;

			// act
			var pagina = _gpdao.PaginaOphalenMetLidInfoVolgensCategorie(
				TestInfo.CATEGORIEID, 
				1, 10, 
				out aantalTotaal);

			// assert
			Assert.IsTrue(aantalTotaal == TestInfo.AANTALINCATEGORIE);
		}

		/// <summary>
		/// Controleert of PaginaOphalenMetLidInfo de lidinfo wel juist ophaalt.
		/// </summary>
		[TestMethod]
		public void PaginaOphalenMetLidInfo()
		{
			// arrange
			int aantalTotaal;

			// act
			var pagina = _gpdao.PaginaOphalenMetLidInfoVolgensCategorie(
				TestInfo.CATEGORIE2ID,
				1, 10,
				out aantalTotaal);

			// assert
			GelieerdePersoon lid = (from gp in pagina
						where gp.ID == TestInfo.GELIEERDEPERSOON3ID
						select gp).FirstOrDefault();
			GelieerdePersoon geenLid = (from gp in pagina
						    where gp.ID == TestInfo.GELIEERDEPERSOON2ID
						    select gp).FirstOrDefault();

			Assert.IsTrue(lid.Lid.First().ID > 0);
			Assert.IsTrue(geenLid.Lid.Count() == 0);
		}


		/// <summary>
		/// Test op toevoegen van een gelieerde persoon aan een categorie
		/// (many->many via PersoonsCategorie)
		/// </summary>
		[TestMethod]
		public void PersoonToevoegenAanCategorie()
		{
			//arrange

			// Onderstaande test uitvoeren met GelieerdePersoon2, want
			// achteraf wordt de categorie opnieuw verwijderd.

			// GelieerdePersoon1 wordt in andere tests geacht een
			// categorie te hebben.

			GelieerdePersoon gp = _gpdao.Ophalen(_gp2ID);
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

			cat.GelieerdePersoon.Where(koppeling => koppeling.ID == _gp2ID).First().TeVerwijderen = true;
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
