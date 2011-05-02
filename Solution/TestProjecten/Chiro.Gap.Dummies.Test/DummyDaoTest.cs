using Chiro.Cdf.Ioc;
using Chiro.Gap.Orm;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Chiro.Gap.Dummies.Test
{
    /// <summary>
	/// This is a test class for DummyDaoTest and is intended
	/// to contain all DummyDaoTest Unit Tests
	/// </summary>
	[TestClass]
	public class DummyDaoTest
	{
		#region Additional test attributes

		// You can use the following additional attributes as you write your tests:

		// Use ClassInitialize to run code before running the first test in the class
		// [ClassInitialize()]
		// public static void MyClassInitialize(TestContext testContext)
		// {
		// }

		// Use ClassCleanup to run code after all tests in a class have run
		// [ClassCleanup()]
		// public static void MyClassCleanup()
		// {
		// }

		// Use TestInitialize to run code before running each test
		// [TestInitialize]
		// public void MyTestInitialize()
		// {
		// }

		// Use TestCleanup to run code after each test has run
		// [TestCleanup()]
		// public void MyTestCleanup()
		// {
		// }
		#endregion

		[ClassInitialize]
		static public void InitialiseerTests(TestContext tc)
		{
			Factory.ContainerInit();
		}

		[ClassCleanup]
		static public void AfsluitenTests()
		{
			Factory.Dispose();
		}

		/// <summary>
		/// Test of bij het bewaren van een gelieerde persoon via de DummyDao, de terugkeerwaarde
		/// niet null is.
		/// </summary>
		[TestMethod]
		public void BewarenTest()
		{
			#region Arrange
			var testData = new DummyData();
			var dao = new DummyGelieerdePersonenDao();

			GelieerdePersoon gp = testData.GelieerdeJos;
			#endregion

			#region Act
			var resultaat = dao.Bewaren(gp);
			#endregion

			#region Assert
			Assert.IsNotNull(resultaat);
			#endregion
		}
	}
}
