using Microsoft.VisualStudio.TestTools.UnitTesting;
using Chiro.Pdox.Data;

namespace Chiro.Gap.InitieleImport.Test
{
    
    
    /// <summary>
    ///This is a test class for ImportHelperTest and is intended
    ///to contain all ImportHelperTest Unit Tests
    ///</summary>
	[TestClass()]
	public class ImportHelperTest
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
		///A test for SpitsStraatNr
		///</summary>
		[TestMethod()]
		public void SpitsStraatNrTest()
		{
			var target = new ImportHelper();
			const string straatNr = "Grote Steenweg 98b";

			string straat;
			const string straatExpected = "Grote Steenweg";

			int? nr;
			const int nrExpected = 98;

			string bus;
			const string busExpected = "b";

			target.SpitsStraatNr(straatNr, out straat, out nr, out bus);
			Assert.AreEqual(straatExpected, straat);
			Assert.AreEqual(nrExpected, nr);
			Assert.AreEqual(busExpected, bus);
		}

		[TestMethod()]
		public void SpitsStraatNrZonderBusTest()
		{
			var target = new ImportHelper();
			const string straatNr = "Grote Steenweg 98";

			string straat;
			const string straatExpected = "Grote Steenweg";

			int? nr;
			const int nrExpected = 98;

			string bus;
			const string busExpected = null;

			target.SpitsStraatNr(straatNr, out straat, out nr, out bus);
			Assert.AreEqual(straatExpected, straat);
			Assert.AreEqual(nrExpected, nr);
			Assert.AreEqual(busExpected, bus);
		}

		[TestMethod()]
		public void SpitsStraatNrZonderNummerTest()
		{
			var target = new ImportHelper();
			const string straatNr = "Grote Steenweg";

			string straat;
			const string straatExpected = "Grote Steenweg";

			int? nr;
			int? nrExpected = null;

			string bus;
			const string busExpected = null;

			target.SpitsStraatNr(straatNr, out straat, out nr, out bus);
			Assert.AreEqual(straatExpected, straat);
			Assert.AreEqual(nrExpected, nr);
			Assert.AreEqual(busExpected, bus);
		}
	}
}
