using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Chiro.Gap.Orm;
using Chiro.Gap.Workers;
using Chiro.Cdf.Ioc;
using Chiro.Gap.Orm.DataInterfaces;
using Chiro.Cdf.Data;
using Chiro.Gap.TestDbInfo;
using Chiro.Gap.Domain;

namespace Chiro.Gap.Data.Test
{
	/// <summary>
	/// Data-Access tests voor afdelingen en alles wat
	/// daarrond hangt.
	/// </summary>
	/// <remarks>Deze tests werken op een testgroep in
	/// de database.  Het script om de testgroep te maken,
	/// zit in SVN: 
	/// https://develop.chiro.be/subversion/cg2/trunk/database/TestData/TestData/TestGroepMaken.sql
	/// 
	/// De IDs nodig voor de test zitten in de settings van dit project.
	/// </remarks>
	[TestClass]
	public class AfdelingsTest
	{
		/// <summary>
		/// Gets or sets the test context which provides
		/// information about and functionality for the current test run.
		/// </summary>
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
		// [TestInitialize]
		// public void MyTestInitialize() { }
		//
		// Use TestCleanup to run code after each test has run
		// [TestCleanup()]
		// public void MyTestCleanup() { }
		//
		#endregion

		[ClassInitialize]
		static public void TestsInitialiseren(TestContext context)
		{
			// Initialiseer IoC-container.  Niet zeker van of dit
			// een goeie plaats is...
			Factory.ContainerInit();

			const int GWJ_ID = TestInfo.GROEPSWERKJAARID;
			const int AFD3_ID = TestInfo.AFDELING3ID;

			// Verwijder mogelijk afdelingsjaren/leden voor afdeling3
			// REKENING HOUDEN MET het feit dat afdeling3 normaal gezien geen afdelingsjaar
			// heeft; check op null is wel degelijk nodig.

			var ajDao = Factory.Maak<IAfdelingsJarenDao>();
			var aj = ajDao.Ophalen(GWJ_ID, AFD3_ID, afdj => afdj.Leiding, afdj => afdj.Kind);

			if (aj != null)
			{
				foreach (var ld in aj.Kind)
				{
					ld.TeVerwijderen = true;
				}

				foreach (var ld in aj.Leiding)
				{
					ld.TeVerwijderen = true;
				}

				aj.TeVerwijderen = true;
				ajDao.Bewaren(aj, afdj => afdj.Kind, afdj => afdj.Leiding);
			}
		}

		/// <summary>
		/// Opzoeken van een testafdelingsjaar
		/// </summary>
		[TestMethod]
		public void AfdelingsJaarOpzoeken()
		{
			#region Arrange
			int gwjID = TestInfo.GROEPSWERKJAARID;
			int afdID = TestInfo.AFDELING1ID;

			var dao = Factory.Maak<IAfdelingsJarenDao>();
			#endregion

			#region Act
			AfdelingsJaar aj = dao.Ophalen(gwjID, afdID);
			#endregion

			#region Assert
			Assert.IsTrue(aj != null);
			Assert.IsTrue(aj.GroepsWerkJaar.ID == gwjID);
			Assert.IsTrue(aj.Afdeling.ID == afdID);
			#endregion
		}

		/// <summary>
		/// Haalt de afdeling op bepaald door TestAfdelingID,
		/// en controleert of die gekoppeld is aan groep
		/// bepaald door TestGroepID
		/// (zie property's)
		/// </summary>
		[TestMethod]
		public void AfdelingOphalen()
		{
			#region Arrange
			int afdID = TestInfo.AFDELING1ID;
			int groepID = TestInfo.GROEPID;

			var dao = Factory.Maak<IAfdelingenDao>();
			#endregion

			#region Act
			Afdeling afd = dao.Ophalen(afdID);
			#endregion

			#region Assert
			Assert.IsTrue(afd != null);
			Assert.IsTrue(afd.Groep.ID == groepID);
			#endregion
		}

		/// <summary>
		/// Creëert een AfdelingsWerkJaar voor de afdeling
		/// bepaald door TestAfdeling3ID in het groepswerkjaar
		/// bepaald door TestGroepsWerkJaarID (zie settings)
		/// </summary>
		[TestMethod]
		public void AfdelingsJaarCreeren()
		{
			// Arrange

			var gwDao = Factory.Maak<IGroepsWerkJaarDao>();
			var aDao = Factory.Maak<IAfdelingenDao>();
			var ajDao = Factory.Maak<IAfdelingsJarenDao>();
			var oaDao = Factory.Maak<IDao<OfficieleAfdeling>>();

			var afdMgr = Factory.Maak<AfdelingsJaarManager>();

			const int GWJ_ID = TestInfo.GROEPSWERKJAARID;
			const int AFD3_ID = TestInfo.AFDELING3ID;
			const int OA_ID = TestInfo.OFFICIELEAFDELINGID;

			const int VAN = TestInfo.AFDELING3VAN;
			const int TOT = TestInfo.AFDELING3TOT;

			// Voor het gemak haal ik groepswerkjaar en afdeling via
			// de DAO's op ipv via de workers.

			GroepsWerkJaar gw = gwDao.Ophalen(GWJ_ID, gwj => gwj.Groep);
			Afdeling afd = aDao.Ophalen(AFD3_ID);
			OfficieleAfdeling oa = oaDao.Ophalen(OA_ID);

			// Het afdelingsjaar wordt gemaakt door een worker.

			AfdelingsJaar aj = afdMgr.Aanmaken(afd, oa, gw, VAN, TOT, GeslachtsType.Gemengd, false);

			// Act

			// Bewaren *MOET* gebeuren via de DAO, want het is de
			// dao die we testen; we willen niet dat een fout in de
			// worker de test doet failen.

			ajDao.Bewaren(aj);

			// Nu opnieuw ophalen.

			AfdelingsJaar aj2 = ajDao.Ophalen(GWJ_ID, AFD3_ID);

			// Assert

			Assert.IsTrue(aj2.ID > 0);
			Assert.IsTrue(aj2.GroepsWerkJaar.ID == GWJ_ID);
			Assert.IsTrue(aj2.Afdeling.ID == AFD3_ID);
			Assert.IsTrue(aj2.OfficieleAfdeling.ID == OA_ID);

			// Cleanup is niet meer nodig, want het nieuw gemaakte
			// afdelingsjaar wordt verwijderd in TestsInitialiseren
		}

		/// <summary>
		/// Controleert of OphalenMetAfdelingen uit GroepenDao
		/// slechts 1 afdelingsjaar oplevert voor de testafdeling.
		/// </summary>
		[TestMethod]
		public void OphalenMetAfdelingenAfdelingsJaar()
		{
			// Arrange

			var dao = Factory.Maak<IGroepenDao>();

			// Act

			Groep g = dao.OphalenMetAfdelingen(TestInfo.GROEPSWERKJAARID);

			// Assert

			var ajQuery = (
				from afd in g.Afdeling
				where afd.ID == TestInfo.AFDELING1ID
				select afd.AfdelingsJaar);

			Assert.IsTrue(ajQuery.Count() == 1);
		}

		/// <summary>
		/// Controleert de koppelingen Groep->Afdeling->AfdelingsJaar->GroepsWerkJaar->Groep
		/// na GroepenDao.OphalenMetAfdelingen.
		/// </summary>
		[TestMethod]
		public void OphalenMetAfdelingenGroepsWerkJaar()
		{
			// Arrange

			var dao = Factory.Maak<IGroepenDao>();

			// Act

			Groep g = dao.OphalenMetAfdelingen(TestInfo.GROEPSWERKJAARID);

			// Assert

			foreach (Afdeling a in g.Afdeling)
			{
				foreach (AfdelingsJaar aj in a.AfdelingsJaar)
				{
					Assert.IsTrue(aj.GroepsWerkJaar.Groep.ID == g.ID);
				}
			}
		}
	}
}
