using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Chiro.Gap.Orm;
using Chiro.Gap.Workers;
using Chiro.Cdf.Ioc;
using Chiro.Cdf.Data.Entity;
using Chiro.Gap.Orm.DataInterfaces;
using Chiro.Cdf.Data;
using Chiro.Gap.TestDbInfo;

namespace Chiro.Gap.Data.Test
{
	/// <summary>
	/// Tests voor LedenDao
	/// </summary>
	[TestClass]
	public class LedenTest
	{
		public LedenTest()
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
		// [TestInitialize()]
		// public void MyTestInitialize() { }
		//
		// Use TestCleanup to run code after each test has run
		// [TestCleanup()]
		// public void MyTestCleanup() { }
		//
		#endregion

		/// <summary>
		/// Voorbereidend werk voor deze tests
		/// </summary>
		/// <param name="context"></param>
		[ClassInitialize]
		static public void TestInitialiseren(TestContext context)
		{
			Factory.ContainerInit();

			int afdelingsJaarID = TestInfo.AFDELINGSJAARID;

			IDao<AfdelingsJaar> ajdao = Factory.Maak<IDao<AfdelingsJaar>>();
			AfdelingsJaar aj = ajdao.Ophalen(afdelingsJaarID, lmb => lmb.GroepsWerkJaar.Groep);

			// Voeg kind toe (GelieerdePersoonID2 in AfdelingsJaarID) om in test te kunnen verwijderen

			int gelieerdePersoon2ID = TestInfo.GELIEERDEPERSOON2ID;

			IGelieerdePersonenDao gpdao = Factory.Maak<IGelieerdePersonenDao>();
			IDao<Kind> kdao = Factory.Maak<IDao<Kind>>();
			ILedenDao ldao = Factory.Maak<ILedenDao>();

			Lid l2 = ldao.Ophalen(gelieerdePersoon2ID, aj.GroepsWerkJaar.ID);

			if (l2 == null)
			{
				// enkel toevoegen als nog niet bestaat

				LedenManager lm = Factory.Maak<LedenManager>();

				GelieerdePersoon gp = gpdao.Ophalen(gelieerdePersoon2ID, lmb => lmb.Groep);

				// GelieerdePersoon2 moet Kind gemaakt worden, want in de test KindVerwijderen
				// zal geprobeerd worden op GelieerdePersoon2 te 'ontkinden'.  Zie #184.

				Kind k = lm.KindMaken(gp);
				kdao.Bewaren(k
				    , lmb => lmb.GelieerdePersoon.WithoutUpdate()
				    , lmb => lmb.AfdelingsJaar.GroepsWerkJaar.WithoutUpdate()
				    , lmb => lmb.GroepsWerkJaar.WithoutUpdate());
			}
		}

		/// <summary>
		/// Opkuis na de tests; verwijdert voornamelijk bijgemaakt lid.
		/// </summary>
		[ClassCleanup]
		public static void TestOpkuisen()
		{
			// Verwijder lid (GelieerdePersoonID in AfdelingsJaarID) om achteraf opnieuw toe te voegen

			int gelieerdePersoonID = TestInfo.GELIEERDEPERSOONID;
			int afdelingsJaarID = TestInfo.AFDELINGSJAARID;

			IDao<AfdelingsJaar> ajdao = Factory.Maak<IDao<AfdelingsJaar>>();
			AfdelingsJaar aj = ajdao.Ophalen(afdelingsJaarID, lmb => lmb.GroepsWerkJaar.Groep);

			ILedenDao ldao = Factory.Maak<ILedenDao>();
			Lid l = ldao.Ophalen(gelieerdePersoonID, aj.GroepsWerkJaar.ID);

			if (l != null)
			{
				l.TeVerwijderen = true;
				ldao.Bewaren(l);
			}

		}



		/// <summary>
		/// Maakt kind aan door gelieerde persoon bepaald door TestGelieerdePersoonID 
		/// (zie settings) te koppelen aan afdelingsjaar bepaald door
		/// TestAfdelingsJaarID (zie ook settings)
		/// </summary>
		[TestMethod]
		public void NieuwKind()
		{
			#region Arrange
			int gelieerdePersoonID = TestInfo.GELIEERDEPERSOONID;
			int afdelingsJaarID = TestInfo.AFDELINGSJAARID;

			LedenManager lm = Factory.Maak<LedenManager>();

			IGelieerdePersonenDao gpdao = Factory.Maak<IGelieerdePersonenDao>();
			IDao<AfdelingsJaar> ajdao = Factory.Maak<IDao<AfdelingsJaar>>();
			IDao<Kind> kdao = Factory.Maak<IDao<Kind>>();

			GelieerdePersoon gp = gpdao.Ophalen(gelieerdePersoonID, lmb => lmb.Groep);
			AfdelingsJaar aj = ajdao.Ophalen(afdelingsJaarID, lmb => lmb.GroepsWerkJaar.Groep);

			/* TODO deze methode zou niet met afdelingsjaar moeten werken
				    Kind k = lm.KindMaken(gp, aj);
				    #endregion

				    #region Act
				    k = kdao.Bewaren(k
					, lmb => lmb.GelieerdePersoon.WithoutUpdate()
					, lmb => lmb.AfdelingsJaar.GroepsWerkJaar.WithoutUpdate()
					, lmb => lmb.GroepsWerkJaar.WithoutUpdate());
				    #endregion

				    #region Assert

				    ILedenDao ldao = Factory.Maak<ILedenDao>();
				    Lid l = ldao.Ophalen(gelieerdePersoonID, aj.GroepsWerkJaar.ID);

				    Assert.IsTrue(l != null && l is Kind);
			*/
			#endregion
		}

		/// <summary>
		/// Verwijdert kind (GelieerdePersoon2ID, AfdelingsJaarID)
		/// </summary>
		[TestMethod]
		public void KindVerwijderen()
		{
			// Arrange

			int gelieerdePersoonID = TestInfo.GELIEERDEPERSOON2ID;
			int afdelingsJaarID = TestInfo.AFDELINGSJAARID;

			IDao<AfdelingsJaar> ajdao = Factory.Maak<IDao<AfdelingsJaar>>();
			AfdelingsJaar aj = ajdao.Ophalen(afdelingsJaarID, lmb => lmb.GroepsWerkJaar.Groep);

			ILedenDao ldao = Factory.Maak<ILedenDao>();
			Lid l = ldao.Ophalen(gelieerdePersoonID, aj.GroepsWerkJaar.ID);

			// Act

			l.TeVerwijderen = true;
			ldao.Bewaren(l);

			// Assert

			Lid l2 = ldao.Ophalen(gelieerdePersoonID, aj.GroepsWerkJaar.Groep.ID);
			Assert.IsNull(l2);
		}

		/// <summary>
		/// Leidingwijzigen
		/// </summary>
		[TestMethod]
		public void LeidingWijzigen()
		{
			// Arrange

			int lid3ID = TestInfo.LID3ID;

			ILeidingDao ldao = Factory.Maak<ILeidingDao>();
			Leiding l = ldao.Ophalen(lid3ID);

			// Act

			ldao.Bewaren(l);

			// Assert

			Assert.IsTrue(true); // eerst al eens kijken of we er zonder crash komen.
		}

		/// <summary>
		/// Test ophalen leden op basis van functie en groepswerkjaar
		/// </summary>
		[TestMethod]
		public void OphalenUitFunctie()
		{
			// Arrange

			ILedenDao dao = Factory.Maak<ILedenDao>();

			// Act

			var result = dao.OphalenUitFunctie(
				NationaleFunctie.ContactPersoon,
				TestInfo.GROEPSWERKJAARID,
				ld => ld.GroepsWerkJaar.Groep,
				ld => ld.Functie,
				ld => ld.GelieerdePersoon.Persoon);

			// Assert

			Assert.AreEqual(result.Count(), 1);
			Assert.AreEqual(result.First().GelieerdePersoon.ID, TestInfo.GELIEERDEPERSOON3ID);
			Assert.AreEqual(result.First().GroepsWerkJaar.Groep.ID, TestInfo.GROEPID);
			Assert.AreEqual(result.First().Functie.First().ID, (int)NationaleFunctie.ContactPersoon);
		}

		/// <summary>
		/// Test ophalen leden op basis van functie en groepswerkjaar, waarbij nagekeken wordt of
		/// de koppeling naar functie bij elk opgehaald lid naar dezelfde functie wijst.
		/// </summary>
		[TestMethod]
		public void OphalenUitFunctieDdd()
		{
			// Arrange

			ILedenDao dao = Factory.Maak<ILedenDao>();

			// Act

			var result = dao.OphalenUitFunctie(
				TestInfo.FUNCTIEID,
				TestInfo.GROEPSWERKJAARID,
				ld => ld.GroepsWerkJaar.Groep,
				ld => ld.Functie,
				ld => ld.GelieerdePersoon.Persoon);

			var opgehaaldeFunctie = (from fun in result.First().Functie
						 where fun.ID == TestInfo.FUNCTIEID
						 select fun).First();
			// query nodig om - in geval van meerdere gekoppelde functies -
			// zeker de juiste te heben

			// Assert

			Assert.IsTrue(result.Count() > 1);	// check of db ok
			Assert.IsTrue(opgehaaldeFunctie.Lid.Count() > 1);	// eigenlijke check
		}

	}

}
