using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.VisualStudio.TestTools.UnitTesting.Web;

using System.Linq;

using Chiro.Cdf.Ioc;
using Chiro.Gap.Orm;
using Chiro.Gap.Workers;
using Chiro.Gap.ServiceContracts;
using Chiro.Gap.ServiceContracts.Mappers;
using Chiro.Gap.Services;
using Chiro.Gap.TestDbInfo;

namespace Chiro.Gap.Services.Test
{
    
    
    /// <summary>
    ///This is a test class for LedenServiceTest and is intended
    ///to contain all LedenServiceTest Unit Tests
    ///</summary>
	[TestClass()]
	public class LedenServiceTest
	{


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
		//You can use the following additional attributes as you write your tests:
		//
		//Use ClassInitialize to run code before running the first test in the class
		[ClassInitialize()]
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
		//[TestInitialize()]
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
		///</summary>
		[TestMethod()]
		public void OphalenTest()
		{
			// Arrange
			LedenService target = Factory.Maak<LedenService>();

			// Act
			int lidID = TestInfo.LID3ID;	// is contactpersoon en redactielid.
			LidExtras extras = LidExtras.Functies;
			var actual = target.Ophalen(lidID, extras);

			// Assert
			var ids = (from f in actual.Functies select f.ID);
			Assert.IsTrue(ids.Contains((int)GepredefinieerdeFunctieType.ContactPersoon));
			Assert.IsTrue(ids.Contains(TestInfo.FUNCTIEID));
		}
	}
}
