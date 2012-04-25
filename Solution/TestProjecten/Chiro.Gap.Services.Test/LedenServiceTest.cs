using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.VisualStudio.TestTools.UnitTesting.Web;

using System.Collections.Generic;
using System.Linq;

using Chiro.Cdf.Ioc;
using Chiro.Gap.Domain;
using Chiro.Gap.Orm;
using Chiro.Gap.Workers;
using Chiro.Gap.ServiceContracts;
using Chiro.Gap.ServiceContracts.Mappers;
using Chiro.Gap.Services;
using Chiro.Gap.TestDbInfo;
using Chiro.Gap.ServiceContracts.DataContracts;

namespace Chiro.Gap.Services.Test
{
    
    
    /// <summary>
    /// Dit is een testclass voor Unit Tests van LedenServiceTest,
    /// to contain all LedenServiceTest Unit Tests
    /// </summary>
	[TestClass]
	public class LedenServiceTest
	{


		private TestContext testContextInstance;

		/// <summary>
		///Gets or sets the test context which provides
		///information about and functionality for the current test run.
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
		//You can use the following additional attributes as you write your tests:
		//
		//Use ClassInitialize to run code before running the first test in the class
		[ClassInitialize]
		public static void MyClassInitialize(TestContext testContext)
		{
			Factory.ContainerInit();
			MappingHelper.MappingsDefinieren();
		}
		
		//Use ClassCleanup to run code after all tests in a class have run
		//[ClassCleanup()]
		//public static void MyClassCleanup()
		//{
		//}
		//
		//Use TestInitialize to run code before running each test
		//[TestInitialize]
		//public void MyTestInitialize()
		//{
		//}
		//
		//Use TestCleanup to run code after each test has run
		//[TestCleanup()]
		//public void MyTestCleanup()
		//{
		//}
		//
		#endregion


		/// <summary>
		///Kijkt na of opgehaalde functies goed gemapt worden.
		/// </summary>
		[TestMethod]
		public void OphalenTest()
		{
			// Arrange
			var target = Factory.Maak<LedenService>();

			// Act
			const int lidID = TestInfo.LID3ID;

			var actual = target.DetailsOphalen(lidID);

			// Assert
			var ids = (from f in actual.LidInfo.Functies select f.ID);
			Assert.IsTrue(ids.Contains((int)NationaleFunctie.ContactPersoon));
			Assert.IsTrue(ids.Contains(TestInfo.FUNCTIEID));
		}

		///<summary>
		///A test for FunctiesVervangen
		/// </summary>
		[TestMethod]
		public void FunctiesVervangenTest()
		{
			#region arrange
			LedenService target = Factory.Maak<LedenService>();
			#endregion

			#region act
			// Lid3 heeft functies contactpersoon en redactie (eigen functie)
			// Vervang door financieel verantwoordelijke, vb en redactie

			int lidID = TestInfo.LID3ID;
			IEnumerable<int> functieIDs = new int[] {
				(int)NationaleFunctie.FinancieelVerantwoordelijke,
				(int)NationaleFunctie.Vb,
				TestInfo.FUNCTIEID};
			target.FunctiesVervangen(lidID, functieIDs);
			#endregion

			#region Assert
			var l = target.DetailsOphalen(lidID);
			var funIDs = (from f in l.LidInfo.Functies select f.ID);

			Assert.AreEqual(funIDs.Count(), 3);
			Assert.IsTrue(funIDs.Contains((int)NationaleFunctie.FinancieelVerantwoordelijke));
			Assert.IsTrue(funIDs.Contains((int)NationaleFunctie.Vb));
			Assert.IsTrue(funIDs.Contains(TestInfo.FUNCTIEID));
			#endregion

			#region Cleanup
			target.FunctiesVervangen(lidID, new int[]{
				(int)NationaleFunctie.ContactPersoon,
				TestInfo.FUNCTIEID});
			#endregion
		}

		[TestMethod]
		public void TestLidMakenBuitenVoorstel()
		{
			#region arrange
			LedenService target = Factory.Maak<LedenService>();
			#endregion

			#region act
			// GP4 zit niet in een afdeling, we vragen zijn voorgestelde afdeling en steken hem/haar dan in de andere
			List<int> gps = new List<int>();
			gps.Add(TestInfo.GELIEERDEPERSOON2ID);
			string fouten;
			var voorstel = target.VoorstelTotInschrijvenGenereren(gps, out fouten).First();
			int gekozenafdelingsjaarID = voorstel.AfdelingsJaarIDs.Contains(TestInfo.AFDELINGSJAAR2ID) ? TestInfo.AFDELINGSJAAR1ID : TestInfo.AFDELINGSJAAR2ID;
			voorstel.AfdelingsJaarIDs = new [] {gekozenafdelingsjaarID};
			voorstel.AfdelingsJaarIrrelevant = false;
			List<InTeSchrijvenLid> defvoorstel = new List<InTeSchrijvenLid>();
			defvoorstel.Add(voorstel);
			int lidID = target.Inschrijven(defvoorstel, out fouten).First();
			#endregion

			#region Assert
			var l = target.DetailsOphalen(lidID);
			Assert.IsTrue(l.LidInfo.AfdelingIdLijst.Contains(TestInfo.AFDELING2ID));
			#endregion

			#region Cleanup
			target.Uitschrijven(gps, out fouten);
			#endregion
		}
	}
}
