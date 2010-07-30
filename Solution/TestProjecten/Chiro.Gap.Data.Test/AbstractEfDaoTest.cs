using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

using Chiro.Cdf.Data;
using Chiro.Cdf.Data.Entity;
using Chiro.Cdf.Ioc;

using Chiro.Gap.Data.Ef;
using Chiro.Gap.Orm;
using Chiro.Gap.Orm.DataInterfaces;
using Chiro.Gap.TestDbInfo;
using Chiro.Gap.Workers;

namespace Chiro.Gap.Data.Test
{
	/// <summary>
	/// Summary description for DaoTest
	/// </summary>
	[TestClass]
	public class AbstractEfDaoTest
	{
		public AbstractEfDaoTest()
		{
			//
			// TODO: Add constructor logic here
			//
		}

		private TestContext testContextInstance;

		/// <summary>
		/// Gets or sets the test context which provides
		/// information about and functionality for the current test run.
		/// </summary>
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
		// [TestInitialize]
		// public void MyTestInitialize() { }
		//
		// Use TestCleanup to run code after each test has run
		// [TestCleanup()]
		// public void MyTestCleanup() { }
		//
		#endregion

		[ClassInitialize]
		public static void TestInitialiseren(TestContext context)
		{
			Factory.ContainerInit();

			// Verwijder GAV die we straks zullen aanmaken.
			IGavDao gavDao = Factory.Maak<IGavDao>();

			Gav gav = gavDao.Ophalen(TestInfo.NIEUWEGAV);
			if (gav != null)
			{
				gav.TeVerwijderen = true;
				gavDao.Bewaren(gav);
			}

			// Verwijder onbestaande categorie, zodat ze zeker onbestaand is :-)
			var catDao = Factory.Maak<ICategorieenDao>();

			Categorie cat = catDao.Ophalen(TestInfo.GROEPID, TestInfo.ONBESTAANDENIEUWECATCODE);
			if (cat != null)
			{
				cat.TeVerwijderen = true;
				catDao.Bewaren(cat);
			}


		}

		/// <summary>
		/// Deze test kijkt na of een nieuwe entiteit
		/// wel een ID krijgt.
		/// </summary>
		[TestMethod]
		public void IDToekennen()
		{
			// Arrange

			Gav gav = new Gav()
			{
				Login = TestInfo.NIEUWEGAV
			};
			IDao<Gav> gavDao = Factory.Maak<IDao<Gav>>();

			// Act

			gav = gavDao.Bewaren(gav);

			// Assert

			Assert.IsTrue(gav.ID > 0);
		}

		/// <summary>
		/// Kijkt na of nieuwe entiteiten ergens in de expression tree van de lambda-expressies
		/// een ID krijgen.
		/// </summary>
		[TestMethod]
		public void IDToekennenIndirect()
		{
			#region Arrange

			var gDao = Factory.Maak<IGroepenDao>();
			var cDao = Factory.Maak<ICategorieenDao>();
			var mgr = Factory.Maak<GroepenManager>();

			Groep g = gDao.Ophalen(TestInfo.GROEPID, grp => grp.Categorie);

			#endregion

			#region Act

			Categorie c = mgr.CategorieToevoegen(
				g,
				TestInfo.ONBESTAANDENIEUWECATNAAM,
				TestInfo.ONBESTAANDENIEUWECATCODE);

			g = gDao.Bewaren(g, grp => grp.Categorie);

			#endregion

			#region Assert

			c = (from cat in g.Categorie
				 where String.Compare(cat.Code, TestInfo.ONBESTAANDENIEUWECATCODE) == 0
				 select cat).First();

			Assert.IsTrue(c.ID > 0);

			#endregion

			#region Cleanup

			c.TeVerwijderen = true;
			g = gDao.Bewaren(g, grp => grp.Categorie);

			#endregion
		}


		/// <summary>
		/// Controleert of de Dao.AllesOphalen wel gedetachte entity's
		/// oplevert.
		/// </summary>
		[TestMethod]
		public void AllesOphalenIsDetacht()
		{
			// Arrange
			IDao<Groep> dao = Factory.Maak<IDao<Groep>>();

			// Act
			IList<Groep> g = dao.AllesOphalen();

			// Assert
			Assert.IsTrue(g[0].EntityState == EntityState.Detached);
		}

		/// <summary>
		/// Controleert of Dao.bewaren een gedetachte entity
		/// oplevert
		/// </summary>
		[TestMethod]
		public void GedetachtNaBewaren()
		{
			// Arrange
			IDao<Groep> dao = Factory.Maak<IDao<Groep>>();

			// Act            
			Groep g = dao.Ophalen(TestInfo.GROEPID);
			g = dao.Bewaren(g);

			// Assert
			Assert.IsTrue(g.EntityState == EntityState.Detached);
		}

		/// <summary>
		/// Als een entiteit wordt verwijderd uit de database via 'te verwijderen', willen we dat 
		/// de terugkeerwaarde null is
		/// </summary>
		[TestMethod]
		public void NullNaVerwijderen()
		{
			#region Arrange

			var gDao = Factory.Maak<IGroepenDao>();
			var cDao = Factory.Maak<ICategorieenDao>();
			var mgr = Factory.Maak<GroepenManager>();

			Groep g = gDao.Ophalen(TestInfo.GROEPID, grp => grp.Categorie);

			// categorie maken en toevoegen, zodat we kunnen zien of we ze goed kunnen verwijderen

			Categorie c = mgr.CategorieToevoegen(
				g,
				TestInfo.ONBESTAANDENIEUWECATNAAM,
				TestInfo.ONBESTAANDENIEUWECATCODE);

			c = cDao.Bewaren(c);

			#endregion

			#region Act

			c.TeVerwijderen = true;
			c = cDao.Bewaren(c);

			#endregion

			#region Assert

			Assert.IsNull(c);

			#endregion

		}

		/// <summary>
		/// Als een entiteit wordt verwijderd uit de database via 'te verwijderen', en die entiteit is 
		/// niet de root, dan mag die entiteit ook niet meer gekoppeld zijn aan het teruggegeven object.
		/// </summary>
		[TestMethod]
		public void TeVerwijderenObjectDaadwerkelijkVerdwenen()
		{
			#region Arrange

			var gDao = Factory.Maak<IGroepenDao>();
			var mgr = Factory.Maak<GroepenManager>();

			Groep g = gDao.Ophalen(TestInfo.GROEPID, grp => grp.Categorie);

			// categorie maken en toevoegen, zodat we kunnen zien of we ze goed kunnen verwijderen

			Categorie c = mgr.CategorieToevoegen(
				g,
				TestInfo.ONBESTAANDENIEUWECATNAAM,
				TestInfo.ONBESTAANDENIEUWECATCODE);

			g = gDao.Bewaren(g, grp => grp.Categorie);

			#endregion

			#region Act

			c = (from cat in g.Categorie
				 where String.Compare(cat.Code, TestInfo.ONBESTAANDENIEUWECATCODE) == 0
				 select cat).First();

			c.TeVerwijderen = true;
			g = gDao.Bewaren(g, grp => grp.Categorie);

			#endregion

			#region Assert

			var gevonden = (from cat in g.Categorie
							where String.Compare(cat.Code, TestInfo.ONBESTAANDENIEUWECATCODE) == 0
							select cat).FirstOrDefault();

			Assert.IsNull(gevonden);	// verwacht dat categorie niet meer aan groep hangt

			#endregion

		}

		/// <summary>
		/// Als een lijst opgehaald wordt met gekoppelde entiteiten, en een aantal van die gekoppelde entiteiten
		/// zijn dezelfde, dan moet dat ook na het ophalen zo zijn.
		/// In deze test worden 2 gelieerde personen van dezelfde groep opgehaald, inclusief de groep.  Dan moet
		/// de groep van de eerste gelieerde persoon ook gelijk zijn aan de groep van de 2de gelieerde persoon.
		/// </summary>
		[TestMethod]
		public void GeenDubbelsBijLijstOphalen()
		{
			// Arrange

			IDao<GelieerdePersoon> dao = Factory.Maak<IDao<GelieerdePersoon>>();
			int[] gpIDs = { TestInfo.GELIEERDEPERSOONID, TestInfo.GELIEERDEPERSOON2ID };

			// Act

			IEnumerable<GelieerdePersoon> opgehaaldePersonen = dao.Ophalen(gpIDs, gpers => gpers.Groep);

			// Assert

			Assert.IsTrue(opgehaaldePersonen.First().Groep == opgehaaldePersonen.Last().Groep);
		}

		/// <summary>
		/// Kijkt na of de eerste pagina voldoende entity's bevat.
		/// </summary>
		[TestMethod]
		public void PaginaOphalen()
		{
			// Arrange

			int totaal;
			IDao<GelieerdePersoon> dao = Factory.Maak<IDao<GelieerdePersoon>>();

			// Act

			IEnumerable<GelieerdePersoon> opgehaaldePersonen = dao.PaginaOphalen(TestInfo.GROEPID, gprs => gprs.Groep.ID, 1, 10, out totaal);

			// Assert

			Assert.IsTrue(opgehaaldePersonen.Count() >= TestInfo.MINAANTALGELPERS);
		}
	}
}
