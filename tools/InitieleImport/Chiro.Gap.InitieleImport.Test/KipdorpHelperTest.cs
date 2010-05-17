using Chiro.Pdox.Data;
using Microsoft.VisualStudio.TestTools.UnitTesting;
namespace Chiro.Gap.InitieleImport.Test
{
    
    
    /// <summary>
    ///This is a test class for KipdorpHelperTest and is intended
    ///to contain all KipdorpHelperTest Unit Tests
    ///</summary>
	[TestClass()]
	public class KipdorpHelperTest
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


		/// <summary>
		///A test for StamNrNaarBestand
		///</summary>
		[TestMethod()]
		public void StamNrNaarBestandTest()
		{
			KipdorpHelper target = new KipdorpHelper();
			string stamnr = "mg /0113";
			string expected = "mg_0113";
			string actual;

			actual = target.StamNrNaarBestand(stamnr);
			Assert.AreEqual(expected, actual);
		}

		/// <summary>
		///A test for RecentsteAansluitingsBestand
		///</summary>
		[TestMethod()]
		public void RecentsteAansluitingsBestandTest()
		{
			KipdorpHelper target = new KipdorpHelper();
			string stamnr = "mg /0113";
			string expected = "//kip-fls05/ADMINISTRATIE-Data/Administratief/Aansluitingen/Email/2009/mg_0113/mg_0113.a3";
			string actual;
			actual = target.RecentsteAansluitingsBestand(stamnr);
			Assert.AreEqual(expected, actual, true);
		}
	}
}
