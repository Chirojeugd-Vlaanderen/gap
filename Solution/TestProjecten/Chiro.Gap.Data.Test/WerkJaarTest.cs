// <copyright company="Chirojeugd-Vlaanderen vzw">
// Copyright (c) 2007-2011
// Mail naar informatica@chiro.be voor alle info over deze broncode
// </copyright>
 
using Chiro.Cdf.Ioc;
using Chiro.Gap.Orm;
using Chiro.Gap.Orm.DataInterfaces;
using Chiro.Gap.TestDbInfo;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Chiro.Gap.Data.Test
{
	/// <summary>
	/// Summary description for WerkJaarTest
	/// </summary>
	[TestClass]
	public class WerkJaarTest
	{
		/// <summary>
		///Gets or sets the test context which provides
		///information about and functionality for the current test run.
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
		public static void InitialiseerTests(TestContext ctx)
		{
			Factory.ContainerInit();
		}

		[TestMethod]
		public void GroepsWerkJaarZoeken()
		{
			#region Arrange
			var gwjDao = Factory.Maak<IGroepsWerkJaarDao>();
			int gwjID = TestInfo.GROEPSWERKJAARID;
			int testGroepID = TestInfo.GROEPID;
			#endregion

			#region Act
			GroepsWerkJaar gwj = gwjDao.Ophalen(gwjID, grwj=>grwj.Groep);
			#endregion

			#region Assert
			Assert.IsTrue(gwj.Groep != null);
			Assert.IsTrue(gwj.Groep.ID == testGroepID);
			#endregion
		}

		/// <summary>
		/// Controleert of de afdelingsjaren meekomen met het recentste groepswerkjaar.
		/// </summary>
		[TestMethod]
		public void RecentsteGroepsWerkJaarMetAfdelingsJaren()
		{
			// arrange
			var dao = Factory.Maak<IGroepsWerkJaarDao>();

			// act
			GroepsWerkJaar gwj = dao.RecentsteOphalen(TestInfo.GROEPID, grwj=>grwj.AfdelingsJaar);

			//assert
			Assert.IsTrue(gwj.AfdelingsJaar.Count >= 1);

		}
	}
}
