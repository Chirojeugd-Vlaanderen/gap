using Microsoft.VisualStudio.TestTools.UnitTesting;

using Chiro.Gap.Orm.DataInterfaces;
using Chiro.Cdf.Ioc;
using Chiro.Gap.Data.Ef;
using Chiro.Gap.TestDbInfo;

namespace Chiro.Gap.Data.Test
{
	/// <summary>
	/// Summary description for Authorisatietest
	/// </summary>
	[TestClass]
	public class AutorisatieTest
	{
		/// <summary>
		/// Gets or sets the test context which provides
		/// information about and functionality for the current test run.
		/// </summary>
		public TestContext TestContext { get; set; }

		#region Additional test attributes

		// You can use the following additional attributes as you write your tests:
		//
		// Use ClassInitialize to run code before running the first test in the class
		// [ClassInitialize]
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

		#endregion

		/// <summary>
		/// Uit te voeren voor de 1ste test:
		/// </summary>
		/// <param name="context"></param>
		[ClassInitialize]
		static public void TestInitialiseren(TestContext context)
		{
			Factory.ContainerInit();
		}

		/// <summary>
		/// Check of gebruiker die GAV is van groepswerkjaar gedetecteerd
		/// wordt door IsGavGroepsWerkJaar.
		/// </summary>
		[TestMethod]
		public void GroepsWerkJaarWelGav()
		{
			// Arrange

			// Ik gebruik hier geen IOC om de Authorisatiedao aan te maken,
			// aangezien er in de Web.Config een dummy gekoppeld wordt aan
			// IAuthorisatieDao.

			IAutorisatieDao dao = new AutorisatieDao();

			// Act

			bool resultaat = dao.IsGavGroepsWerkJaar(TestInfo.GAV1, TestInfo.GROEPS_WERKJAAR_ID);

			// Assert

			Assert.IsTrue(resultaat);
		}

		/// <summary>
		/// Check of gebruiker die geen GAV is van groepswerkjaar gedetecteerd
		/// wordt door IsGavGroepsWerkJaar.
		/// </summary>
		[TestMethod]
		public void GroepsWerkJaarGeenGav()
		{
			// Arrange

			// Ik gebruik hier geen IOC om de Authorisatiedao aan te maken,
			// aangezien er in de Web.Config een dummy gekoppeld wordt aan
			// IAuthorisatieDao.

			IAutorisatieDao dao = new AutorisatieDao();

			// Act

			bool resultaat = dao.IsGavGroepsWerkJaar(TestInfo.GAV2, TestInfo.GROEPS_WERKJAAR_ID);

			// Assert

			Assert.IsFalse(resultaat);
		}

		/// <summary>
		/// Als je je afvraagt of je GAV bent van een onbestaand persoon, dan moet het antwoord nee zijn.
		/// We willen vermijden dat een kwaadwillig iemand het verschil tussen een ID van een onbestaand
		/// persoon en het ID van een persoon waarop je geen rechten hebt te weten kan komen.
		/// </summary>
		[TestMethod]
		public void OnbestaandeGelieerdePersoonID()
		{
			// Arrange

			// Ik gebruik hier geen IOC om de Authorisatiedao aan te maken,
			// aangezien er in de Web.Config een dummy gekoppeld wordt aan
			// IAuthorisatieDao.

			IAutorisatieDao dao = new AutorisatieDao();

			// Act

			bool resultaat = dao.IsGavGelieerdePersoon(TestInfo.GAV2, TestInfo.ONBESTAANDE_GELIEERDE_PERSOON_ID);

			// Assert

			Assert.IsFalse(resultaat);
		}
	}
}
