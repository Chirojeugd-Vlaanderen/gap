using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Chiro.Gap.Orm;
using Chiro.Cdf.Ioc;
using Chiro.Gap.Orm.DataInterfaces;
using System.Data;
using Chiro.Cdf.Data.Entity;
using Chiro.Cdf.Data;

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
		}

		/// <summary>
		/// Deze test kijkt na of een nieuwe entiteit
		/// wel een ID krijgt.
		/// </summary>
		[TestMethod]
		public void IDToekennen()
		{
			// Arrange

			Gav gav = new Gav() { Login = TestInfo.NIEUWEGAV };
			IDao<Gav> gavDao = Factory.Maak<IDao<Gav>>();

			// Act

			gav = gavDao.Bewaren(gav);

			// Assert

			Assert.IsTrue(gav.ID > 0);
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
			int[] gpIDs = {TestInfo.GELIEERDEPERSOONID, TestInfo.GELIEERDEPERSOON2ID};

			// Act

			IEnumerable<GelieerdePersoon> opgehaaldePersonen = dao.Ophalen(gpIDs, gpers => gpers.Groep);

			// Assert

			Assert.IsTrue(opgehaaldePersonen.First().Groep == opgehaaldePersonen.Last().Groep);
		}
	}
}
