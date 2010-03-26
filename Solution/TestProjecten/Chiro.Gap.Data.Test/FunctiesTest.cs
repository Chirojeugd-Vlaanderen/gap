using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using Chiro.Cdf.Ioc;
using Chiro.Gap.Orm.DataInterfaces;
using Chiro.Gap.Orm;
using Chiro.Gap.TestDbInfo;

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
			// Arrange
			IFunctiesDao dao = Factory.Maak<IFunctiesDao>();

			// Act
			Functie gg1 = dao.Ophalen(NationaleFunctie.ContactPersoon);
			Functie gg2 = dao.Ophalen(NationaleFunctie.GroepsLeiding);
			Functie gv1 = dao.Ophalen(NationaleFunctie.Vb);
			Functie fi = dao.Ophalen(NationaleFunctie.FinancieelVerantwoordelijke);
			Functie jr = dao.Ophalen(NationaleFunctie.JeugdRaad);
			Functie kk = dao.Ophalen(NationaleFunctie.KookPloeg);
			Functie gp = dao.Ophalen(NationaleFunctie.Proost);

			// Assert
			Assert.AreEqual(gg1.Code, "CP", true);
			Assert.AreEqual(gg2.Code, "GL", true);
			Assert.AreEqual(gv1.Code, "VB", true);
			Assert.AreEqual(fi.Code, "FV", true);
			Assert.AreEqual(jr.Code, "JR", true);
			Assert.AreEqual(kk.Code, "CK", true);
			Assert.AreEqual(gp.Code, "PR", true);
		}


		/// <summary>
		/// Tel het aantal contactpersonen van de testgroep; we verwachten 1.
		/// </summary>
		[TestMethod]
		public void EenContactpersoon()
		{
			// Arrange
			IFunctiesDao dao = Factory.Maak<IFunctiesDao>();

			// Act
			int aantal = dao.AantalLeden(TestInfo.GROEPID, NationaleFunctie.ContactPersoon);

			// Assert
			Assert.AreEqual(aantal, 1);

		}

	}
}
