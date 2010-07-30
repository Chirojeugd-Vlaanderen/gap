using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
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
		public AdressenTest()
		{
			//
			// TODO: Add constructor logic here
			//
		}

		private TestContext testContextInstance;

		/// <summary>
		/// Gets or sets the test context which provides
		/// information about and functionality for the current test run.
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
	}
}
