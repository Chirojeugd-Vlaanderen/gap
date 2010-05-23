using System.ServiceModel;
using Chiro.Cdf.Ioc;
using Chiro.Gap.ServiceContracts.FaultContracts;
using Chiro.Gap.ServiceContracts.Mappers;
using Chiro.Gap.Services;
using Chiro.Gap.TestDbInfo;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.VisualStudio.TestTools.UnitTesting.Web;
using Chiro.Gap.Workers;
using Chiro.Gap.ServiceContracts;

namespace Chiro.Gap.Services.Test
{
    
    
    /// <summary>
    ///This is a test class for GelieerdePersonenServiceTest and is intended
    ///to contain all GelieerdePersonenServiceTest Unit Tests
    ///</summary>
	[TestClass()]
	public class GelieerdePersonenServiceTest
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
		//[ClassInitialize()]
		//public static void MyClassInitialize(TestContext testContext)
		//{
		//}
		//
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

		[ClassInitialize()]
		public static void MyClassInitialize(TestContext testContext)
		{
			Factory.ContainerInit();
			MappingHelper.MappingsDefinieren();
		}

		/// <summary>
		///A test for CommunicatieVormToevoegen
		///</summary>
		// TODO: Ensure that the UrlToTest attribute specifies a URL to an ASP.NET page (for example,
		// http://.../Default.aspx). This is necessary for the unit test to be executed on the web server,
		// whether you are testing a page, web service, or a WCF service.
		[TestMethod()]
		[ExpectedException(typeof(FaultException<GapFault>))]
		public void CommunicatieVormToevoegenTest()
		{
			var target = Factory.Maak<GelieerdePersonenService>();

			var gelieerdePersoonID = TestInfo.GELIEERDEPERSOONID;

			var commInfo = new CommunicatieInfo()
                            	{
                            		CommunicatieTypeID = 1,
                            		Nummer = TestInfo.ONGELDIGTELEFOONNR
                            	};

			target.CommunicatieVormToevoegen(gelieerdePersoonID, commInfo);
			Assert.IsTrue(false);
		}
	}
}
