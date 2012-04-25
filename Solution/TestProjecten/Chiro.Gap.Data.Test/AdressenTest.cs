using System.Linq;

using Chiro.Gap.Orm;

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Chiro.Cdf.Ioc;
using Chiro.Gap.Orm.DataInterfaces;
using Chiro.Gap.TestDbInfo;

namespace Chiro.Gap.Data.Test
{
	/// <summary>
	/// Summary description for AdressenTest
	/// </summary>
	[TestClass]
	public class AdressenTest
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

		[ClassInitialize]
		static public void TestInit(TestContext context)
		{
			Factory.ContainerInit();
		}

		/// <summary>
		/// Probeert een straat op te zoeken op basis van een postnummer
		/// </summary>
		[TestMethod]
		public void StraatOpzoeken()
		{
			// Arrange

			var dao = Factory.Maak<IStratenDao>();

			// Act

			var gevonden = dao.MogelijkhedenOphalen(TestInfo.TEZOEKENSTRAAT, TestInfo.POSTNR);

			// Assert

			Assert.IsTrue(gevonden.Count == 1);
		}

		/// <summary>
		/// Test die nakijkt of het ophalen van een adres met bewoners, ook de info bevat
		/// over de straat en gemeente
		/// </summary>
		[TestMethod]
		public void BewonersOphalen()
		{
			// Arrange

			var dao = Factory.Maak<IAdressenDao>();

			// Act

			var gevonden = dao.BewonersOphalen(TestInfo.ADRESID, new[] {TestInfo.GROEPID}, false);

			// Assert

			Assert.IsNotNull(gevonden);
			Assert.IsTrue(gevonden is BelgischAdres);
			Assert.IsNotNull(((BelgischAdres)gevonden).StraatNaam);
		}

		/// <summary>
		/// Test die nakijkt of alle adressen van de opgehaalde
		/// bewoners in orde zijn.
		/// </summary>
		[TestMethod]
		public void AndereAdressenOpgehaaldeBewoners()
		{
			// Arrange

			var dao = Factory.Maak<IAdressenDao>();

			// Act

			var gevonden = dao.BewonersOphalen(TestInfo.ADRESID, new[] { TestInfo.GROEPID }, false);

			// Assert

			foreach (var pa in gevonden.PersoonsAdres.SelectMany(pa => pa.Persoon.PersoonsAdres))
			{
				Assert.IsNotNull(pa.Adres as BelgischAdres);
				Assert.IsNotNull(((BelgischAdres) pa.Adres).StraatNaam);
			}

		}

		/// <summary>
		/// Test die nakijkt of met een belgisch adres de straten mee komen
		/// </summary>
		[TestMethod]
		public void BelgischAdresOphalen()
		{
			// Arrange

			var dao = Factory.Maak<IAdressenDao>();

			// Act

			var gevonden = dao.Ophalen(TestInfo.ADRESID);

			// Assert

			Assert.IsNotNull(((BelgischAdres)gevonden).StraatNaam);
		}
	}
}
