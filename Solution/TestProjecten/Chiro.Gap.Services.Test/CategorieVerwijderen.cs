using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

using Chiro.Cdf.Ioc;
using Chiro.Gap.Orm;
using Chiro.Gap.Orm.DataInterfaces;
using Chiro.Gap.ServiceContracts.Mappers;
using Chiro.Gap.Services;
using Chiro.Gap.TestDbInfo;

namespace Chiro.Gap.ServiceContracts.Test
{
	/// <summary>
	/// Summary description for CategorieToevoegen
	/// </summary>
	[TestClass]
	public class CategorieVerwijderen
	{
		public CategorieVerwijderen()
		{
			//
			// TODO: Add constructor logic here
			//
		}

		[ClassInitialize]
		static public void InitialiseerTests(TestContext tc)
		{
			// Dit gebeurt normaalgesproken bij het starten van de service,
			// maar blijkbaar is het moeilijk de service te herstarten bij het testen.
			// Vandaar op deze manier:

			MappingHelper.MappingsDefinieren();
			Factory.ContainerInit();
		}

		[ClassCleanup]
		static public void AfsluitenTests()
		{
			Factory.Dispose();
		}

		IGroepenService groepenSvc;
		
		List<int> catlijst = new List<int>(); //lijst van nieuw aangemaakte categorieen, die nog verwijderd moeten worden

		[TestInitialize]
		public void setUp()
		{
			groepenSvc = Factory.Maak<GroepenService>();

			// Kijken of Broes' testcategorie al bestaat.  Zo niet: creeren.

			int catID = groepenSvc.CategorieIDOphalen(
				TestInfo.GROEPID,
				TestInfo.ONBESTAANDECATEGORIECODE2);

			if (catID == 0)
			{
				catID = groepenSvc.CategorieToevoegen(
					TestInfo.GROEPID,
					TestInfo.CATEGORIENAAM,
					TestInfo.ONBESTAANDECATEGORIECODE2);
			}

			catlijst.Add(catID);
		}

		[TestCleanup]
		public void tearDown()
		{
			/// Controleert of de toegevoegde categorie nog bestaat, en verwijdert ze
			/// als dat het geval is.
			/// 
			GroepInfo g = groepenSvc.Ophalen(TestInfo.GROEPID, GroepsExtras.Categorieen);

			int catID = (from catInfo in g.Categorie
				     where String.Compare(catInfo.Code, TestInfo.ONBESTAANDECATEGORIECODE2, true) == 0
				     select catInfo.ID).FirstOrDefault();

			if (catID != 0)
			{
				groepenSvc.CategorieVerwijderen(catID);
			}

		}

		/// <summary>
		/// Verwijderen van een categorie
		/// </summary>
		[TestMethod]
		public void CategorieVerwijderenNormaal()
		{
			// Arrange: Categorie-ID bepalen van te verwijderen categorie.

			int catID = groepenSvc.CategorieIDOphalen(
				TestInfo.GROEPID,
				TestInfo.ONBESTAANDECATEGORIECODE2);

			// Act: verwijder de categorie met gegeven ID, en probeer categorie
			// opnieuw op te halen

			groepenSvc.CategorieVerwijderen(catID);
			catID = groepenSvc.CategorieIDOphalen(
				TestInfo.GROEPID,
				TestInfo.ONBESTAANDECATEGORIECODE2);

			// Assert: categorie niet meer gevonden.

			Assert.IsTrue(catID == 0);
		}

		//TODO

		/*        [TestMethod]
			[ExpectedExceptionAttribute(typeof(NotImplementedException))]
			public void CategorieAanmakenLegeNaam()
			{
            
			    catlijst.Add(gpm.CategorieToevoegen(Properties.Settings.Default.GroepID, "", ""));
			}

			[TestMethod]
			[ExpectedExceptionAttribute(typeof(NotImplementedException))]
			public void CategorieAanmakenGeenNaam()
			{
			    IGroepenService gpm = Factory.Maak<GroepenService>();
			    catlijst.Add(gpm.CategorieToevoegen(Properties.Settings.Default.GroepID, null, ""));
			}

			[TestMethod]
			[ExpectedExceptionAttribute(typeof(NotImplementedException))]
			public void CategorieAanmakenGeenCode()
			{
			    IGroepenService gpm = Factory.Maak<GroepenService>();
			    catlijst.Add(gpm.CategorieToevoegen(Properties.Settings.Default.GroepID, "kookies", null));
			}

			[TestMethod]
			[ExpectedExceptionAttribute(typeof(NotImplementedException))]
			public void CategorieAanmakenOnbestaandeGroep()
			{
			    IGroepenService gpm = Factory.Maak<GroepenService>();
			    catlijst.Add(gpm.CategorieToevoegen(0, "kookies", ""));
			}

			[TestMethod]
			[ExpectedExceptionAttribute(typeof(NotImplementedException))]
			public void CategorieAanmakenBestaandeNaam()
			{
			    IGroepenService gpm = Factory.Maak<GroepenService>();
			    catlijst.Add(gpm.CategorieToevoegen(Properties.Settings.Default.GroepID, "Kookies", ""));
			    catlijst.Add(gpm.CategorieToevoegen(Properties.Settings.Default.GroepID, "Kookies", ""));
			}*/
	}
}
