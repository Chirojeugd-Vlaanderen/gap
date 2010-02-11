using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

using Chiro.Cdf.Ioc;
using Chiro.Gap.Orm;
using Chiro.Gap.Orm.DataInterfaces;
using Chiro.Gap.ServiceContracts;
using Chiro.Gap.Services;
using Chiro.Gap.TestDbInfo;
using System.Security.Principal;
using System.Threading;
using Chiro.Gap.ServiceContracts.Mappers;

namespace Chiro.Gap.Services.Test
{
	/// <summary>
	/// Summary description for CategorieToevoegen
	/// </summary>
	[TestClass]
	public class CategorieToevoegen
	{
		public CategorieToevoegen()
		{
			//
			// TODO: Add constructor logic here
			//
		}

		[ClassInitialize]
		static public void InitialiseerTests(TestContext tc)
		{
			Factory.ContainerInit();
			MappingHelper.MappingsDefinieren();
		}

		[ClassCleanup]
		static public void AfsluitenTests()
		{
			Factory.Dispose();
		}

		List<int> catlijst = new List<int>(); //lijst van nieuw aangemaakte categorieen, die nog verwijderd moeten worden
		IGroepenService groepenSvc;

		/// <summary>
		/// Zoekt de categorie voor de test 'categorie toevoegen', en verwijdert ze uit de database
		/// als ze bestaat.
		/// </summary>
		private void VerwijderCategorieToevoegTest()
		{
			GroepInfo g = groepenSvc.Ophalen(TestInfo.GROEPID, GroepsExtras.Categorieen);

			int catID = (from cInfo in g.Categorie
				     where String.Compare(cInfo.Code, TestInfo.ONBESTAANDENIEUWECATCODE, true) == 0
				     select cInfo.ID).FirstOrDefault();

			// De 'FirstOrDefault' kiest 0 als er geen gevonden is.

			if (catID != 0)
			{
				groepenSvc.CategorieVerwijderen(catID, true);
			}
		}

		[TestInitialize]
		public void initialiseerTest()
		{
			/// Zorg ervoor dat de PrincipalPermissionAttributes op de service methods
			/// geen excepties genereren, door te doen alsof de service aangeroepen is met de goede
			/// 
			var identity = new GenericIdentity(Properties.Settings.Default.TestUser);
			var roles = new[] { Properties.Settings.Default.TestSecurityGroep };
			var principal = new GenericPrincipal(identity, roles);
			Thread.CurrentPrincipal = principal;

			groepenSvc = Factory.Maak<GroepenService>();

			// CategorieToevoegenNormaal voegt een categorie toe voor:
			//    - Groep: Properties.Settings.Default.GroepID
			//    - CategorieCode: Properties.Settings.Default.CategorieCode
			//
			// Deze moeten we op voorhand verwijderen als die bestaat.

			VerwijderCategorieToevoegTest();
		}

		[TestCleanup]
		public void tearDown()
		{
			VerwijderCategorieToevoegTest();
		}

		[TestMethod]
		public void CategorieToevoegenNormaal()
		{
			int catID = groepenSvc.CategorieToevoegen(TestInfo.GROEPID,
				TestInfo.CATEGORIENAAM,
				TestInfo.ONBESTAANDENIEUWECATCODE);
			catlijst.Add(catID);

			GroepInfo g = groepenSvc.Ophalen(TestInfo.GROEPID, GroepsExtras.Categorieen);

			var query = (from cInfo in g.Categorie
				     where cInfo.ID == catID
				     select cInfo);

			Assert.IsTrue(query.Count() > 0);
		}

		/*        [TestMethod]
			[ExpectedExceptionAttribute(typeof(NotImplementedException))]
			public void CategorieAanmakenLegeNaam()
			{
			    catlijst.Add(gpm.CategorieToevoegen(groepID, "", ""));
			}

			[TestMethod]
			[ExpectedExceptionAttribute(typeof(NotImplementedException))]
			public void CategorieAanmakenGeenNaam()
			{
			    catlijst.Add(gpm.CategorieToevoegen(groepID, null, ""));
			}

			[TestMethod]
			[ExpectedExceptionAttribute(typeof(NotImplementedException))]
			public void CategorieAanmakenGeenCode()
			{
			    catlijst.Add(gpm.CategorieToevoegen(groepID, "kookies", null));
			}

			[TestMethod]
			[ExpectedExceptionAttribute(typeof(NotImplementedException))]
			public void CategorieAanmakenOnbestaandeGroep()
			{
			    catlijst.Add(gpm.CategorieToevoegen(0, "kookies", ""));
			}

			[TestMethod]
			[ExpectedExceptionAttribute(typeof(NotImplementedException))]
			public void CategorieAanmakenBestaandeNaam()
			{
			    catlijst.Add(gpm.CategorieToevoegen(groepID, "Kookies", ""));
			    catlijst.Add(gpm.CategorieToevoegen(groepID, "Kookies", ""));
			}*/
	}
}
