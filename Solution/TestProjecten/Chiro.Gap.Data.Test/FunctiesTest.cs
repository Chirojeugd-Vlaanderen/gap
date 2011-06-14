// <copyright company="Chirojeugd-Vlaanderen vzw">
// Copyright (c) 2007-2011
// Mail naar informatica@chiro.be voor alle info over deze broncode
// </copyright>

using Chiro.Gap.Domain;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using Chiro.Cdf.Ioc;
using Chiro.Gap.Orm.DataInterfaces;
using Chiro.Gap.TestDbInfo;

namespace Chiro.Gap.Data.Test
{
	/// <summary>
	/// Summary description for FunctiesTest
	/// </summary>
	[TestClass]
	public class FunctiesTest
	{
		#region Additional test attributes
		//
		// You can use the following additional attributes as you write your tests:
		//
		// Use ClassInitialize to run code before running the first test in the class
		[ClassInitialize]
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
		// [TestInitialize]
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
		public void NationaalBepaaldeFuncties()
		{
			// Arrange
			var dao = Factory.Maak<IFunctiesDao>();

			// Act
			var gg1 = dao.Ophalen(NationaleFunctie.ContactPersoon);
			var gg2 = dao.Ophalen(NationaleFunctie.GroepsLeiding);
			var gv1 = dao.Ophalen(NationaleFunctie.Vb);
			var fi = dao.Ophalen(NationaleFunctie.FinancieelVerantwoordelijke);
			var jr = dao.Ophalen(NationaleFunctie.JeugdRaad);
			var kk = dao.Ophalen(NationaleFunctie.KookPloeg);
			var gp = dao.Ophalen(NationaleFunctie.Proost);

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
			var dao = Factory.Maak<IFunctiesDao>();

			// Act
			var aantal = dao.AantalLeden(TestInfo.GROEPID, NationaleFunctie.ContactPersoon);

			// Assert
			Assert.AreEqual(aantal, 1);
		}

	}
}
