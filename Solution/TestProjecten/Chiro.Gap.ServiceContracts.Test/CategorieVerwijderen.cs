using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Chiro.Gap.Orm;
using Chiro.Gap.Workers;
using Chiro.Cdf.Ioc;
using Chiro.Gap.Orm.DataInterfaces;
using System.IO;
using System.Runtime.Serialization;
using Chiro.Gap.Services;

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
				Properties.Settings.Default.GroepID,
				Properties.Settings.Default.CategorieCode_Verwijderen);

			if (catID == 0)
			{
				catID = groepenSvc.CategorieToevoegen(
					Properties.Settings.Default.GroepID,
					Properties.Settings.Default.CategorieNaam,
					Properties.Settings.Default.CategorieCode_Verwijderen);
			}

			catlijst.Add(catID);
		}

		[TestCleanup]
		public void tearDown()
		{
			IGroepenService gpm = Factory.Maak<GroepenService>();
			Groep g = gpm.OphalenMetCategorieen(Properties.Settings.Default.GroepID);
			foreach (Categorie c in g.Categorie)
			{
				if (catlijst.Contains(c.ID))
				{
					
					gpm.CategorieVerwijderen(c.ID);
				}
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
				Properties.Settings.Default.GroepID,
				Properties.Settings.Default.CategorieCode_Verwijderen);

			// Act: verwijder de categorie met gegeven ID, en probeer categorie
			// opnieuw op te halen

			groepenSvc.CategorieVerwijderen(catID);
			catID = groepenSvc.CategorieIDOphalen(
				Properties.Settings.Default.GroepID,
				Properties.Settings.Default.CategorieCode_Verwijderen);

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
