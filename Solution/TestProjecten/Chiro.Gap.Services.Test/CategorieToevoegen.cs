using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;

using Chiro.Cdf.Ioc;
using Chiro.Gap.ServiceContracts;
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
		[ClassInitialize]
		static public void InitialiseerTests(TestContext tc)
		{
			Factory.ContainerInit();
			MappingHelper.MappingsDefinieren();
		}

		[ClassCleanup]
		static public void AfsluitenTests()
		{
		}

		List<int> catlijst = new List<int>(); //lijst van nieuw aangemaakte categorieën, die nog verwijderd moeten worden
		IGroepenService _groepenSvc;

		/// <summary>
		/// Zoekt de categorie voor de test 'categorie toevoegen', en verwijdert ze uit de database
		/// als ze bestaat.
		/// </summary>
		private void VerwijderCategorieToevoegTest()
		{
			var categorieen = _groepenSvc.CategorieenOphalen(TestInfo.GROEPID);

			int catID = (from cInfo in categorieen
				     where String.Compare(cInfo.Code, TestInfo.ONBESTAANDENIEUWECATCODE, true) == 0
				     select cInfo.ID).FirstOrDefault();

			// De 'FirstOrDefault' kiest 0 als er geen gevonden is.

			if (catID != 0)
			{
				_groepenSvc.CategorieVerwijderen(catID, true);
			}
		}

		[TestInitialize]
		public void InitialiseerTest()
		{
			// Zorg ervoor dat de PrincipalPermissionAttributes op de service methods
			// geen excepties genereren, door te doen alsof de service aangeroepen is met de goede
			 
			var identity = new GenericIdentity(Properties.Settings.Default.TestUser);
			var roles = new[] { Properties.Settings.Default.TestSecurityGroep };
			var principal = new GenericPrincipal(identity, roles);
			Thread.CurrentPrincipal = principal;

			_groepenSvc = Factory.Maak<GroepenService>();

			// CategorieToevoegenNormaal voegt een categorie toe voor:
			//    - Groep: Properties.Settings.Default.GroepID
			//    - CategorieCode: Properties.Settings.Default.CategorieCode
			//
			// Deze moeten we op voorhand verwijderen als die bestaat.

			VerwijderCategorieToevoegTest();
		}

		[TestCleanup]
		public void TearDown()
		{
			VerwijderCategorieToevoegTest();
		}

		[TestMethod]
		public void CategorieToevoegenNormaal()
		{
			int catID = _groepenSvc.CategorieToevoegen(TestInfo.GROEPID,
				TestInfo.CATEGORIENAAM,
				TestInfo.ONBESTAANDENIEUWECATCODE);
			catlijst.Add(catID);

			var categorieen = _groepenSvc.CategorieenOphalen(TestInfo.GROEPID);

			var query = (from cInfo in categorieen
				     where cInfo.ID == catID
				     select cInfo);

			Assert.IsTrue(query.Count() > 0);
		}
	}
}
