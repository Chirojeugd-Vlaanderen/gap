using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using Chiro.Cdf.Ioc;
using Chiro.Gap.Orm.DataInterfaces;
using Chiro.Gap.Orm;

namespace Chiro.Gap.Data.Test
{
	/// <summary>
	/// Summary description for FunctiesTest
	/// </summary>
	[TestClass]
	public class FunctiesTest
	{
		public FunctiesTest()
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
		[ClassInitialize()]
		public static void MyClassInitialize(TestContext testContext) 
		{
			Factory.ContainerInit();
		}
		//
		// Use ClassCleanup to run code after all tests in a class have run
		// [ClassCleanup()]
		// public static void MyClassCleanup() { }
		//
		// Use TestInitialize to run code before running each test 
		// [TestInitialize()]
		// public void MyTestInitialize() { }
		//
		// Use TestCleanup to run code after each test has run
		// [TestCleanup()]
		// public void MyTestCleanup() { }
		//
		#endregion


		/// <summary>
		/// Kijkt na of de gepredefinieerde functies overeenkomen met hun ID
		/// </summary>
		[TestMethod]
		public void GepredefinieerdeFuncties()
		{
			IFunctiesDao dao = Factory.Maak<IFunctiesDao>();

			Functie gg1 = dao.Ophalen(GepredefinieerdeFunctieType.ContactPersoon);
			Functie gg2 = dao.Ophalen(GepredefinieerdeFunctieType.GroepsLeiding);
			Functie gv1 = dao.Ophalen(GepredefinieerdeFunctieType.Vb);
			Functie fi = dao.Ophalen(GepredefinieerdeFunctieType.FinancieelVerantwoordelijke);
			Functie jr = dao.Ophalen(GepredefinieerdeFunctieType.JeugdRaad);
			Functie kk = dao.Ophalen(GepredefinieerdeFunctieType.KookPloeg);
			Functie gp = dao.Ophalen(GepredefinieerdeFunctieType.Proost);

			Assert.AreEqual(gg1.Code, "GG1", true);
			Assert.AreEqual(gg2.Code, "GG2", true);
			Assert.AreEqual(gv1.Code, "GV1", true);
			Assert.AreEqual(fi.Code, "FI", true);
			Assert.AreEqual(jr.Code, "JR", true);
			Assert.AreEqual(kk.Code, "KK", true);
			Assert.AreEqual(gp.Code, "GP", true);
		}
	}
}
