using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Security.Principal;
using System.Text;
using System.Threading;

using Chiro.Cdf.Ioc;
using Chiro.Gap.Orm;
using Chiro.Gap.Orm.DataInterfaces;
using Chiro.Gap.ServiceContracts.Mappers;
using Chiro.Gap.ServiceContracts;
using Chiro.Gap.ServiceContracts.FaultContracts;
using Chiro.Gap.Services;
using Chiro.Gap.TestDbInfo;
using System.ServiceModel;

namespace Chiro.Gap.Services.Test
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

		IGroepenService _groepenSvc;
		IGelieerdePersonenService _personenSvc;
		
		[TestInitialize]
		public void setUp()
		{
			/// Zorg ervoor dat de PrincipalPermissionAttributes op de service methods
			/// geen excepties genereren, door te doen alsof de service aangeroepen is met de goede
			/// 
			var identity = new GenericIdentity(Properties.Settings.Default.TestUser);
			var roles = new[] { Properties.Settings.Default.TestSecurityGroep };
			var principal = new GenericPrincipal(identity, roles);
			Thread.CurrentPrincipal = principal;

			_groepenSvc = Factory.Maak<GroepenService>();
			_personenSvc = Factory.Maak<GelieerdePersonenService>();

			// Maak de categorieën voor de tests aan, als ze niet bestaan

			foreach (string code in TestInfo.ONBESTAANDECATEGORIECODES)
			{
				int catID = _groepenSvc.CategorieIDOphalen(TestInfo.GROEPID, code);

				if (catID == 0)
				{
					catID = _groepenSvc.CategorieToevoegen(
						TestInfo.GROEPID,
						code,
						code);
				}
			}
		}

		/// <summary>
		/// Verwijder eventuele overblijvende categorieën
		/// </summary>
		[TestCleanup]
		public void tearDown()
		{
			foreach (string code in TestInfo.ONBESTAANDECATEGORIECODES)
			{
				int catID = _groepenSvc.CategorieIDOphalen(TestInfo.GROEPID, code);

				if (catID != 0)
				{
					_groepenSvc.CategorieVerwijderen(catID, true);
				}
			}
		}



		/// <summary>
		/// Verwijderen van een lege categorie
		/// </summary>
		[TestMethod]
		public void CategorieVerwijderenNormaal()
		{
			// Arrange: Categorie-ID bepalen van te verwijderen categorie.

			int catID = _groepenSvc.CategorieIDOphalen(
				TestInfo.GROEPID,
				TestInfo.ONBESTAANDECATEGORIECODES[0]);

			// Act: verwijder de categorie met gegeven ID, en probeer categorie
			// opnieuw op te halen

			_groepenSvc.CategorieVerwijderen(catID, false);
			catID = _groepenSvc.CategorieIDOphalen(
				TestInfo.GROEPID,
				TestInfo.ONBESTAANDECATEGORIECODES[0]);

			// Assert: categorie niet meer gevonden.

			Assert.IsTrue(catID == 0);
		}

		/// <summary>
		/// Probeert een categorie te verwijderen waaraan een persoon gekoppeld is.  Er wordt een
		/// exception verwacht.
		/// </summary>
		[TestMethod]
		[ExpectedException(typeof(FaultException<GekoppeldeObjectenFault<PersoonInfo>>))]
		public void CategorieVerwijderenMetPersoon()
		{
			// Arrange: categorie opzoeken, en persoon toevoegen.

			int catID = _groepenSvc.CategorieIDOphalen(
				TestInfo.GROEPID,
				TestInfo.ONBESTAANDECATEGORIECODES[1]);

			_personenSvc.CategorieKoppelen(
				new List<int> { TestInfo.GELIEERDEPERSOONID },
				new List<int> { catID });

			// Act

			// Verwijder categorie zonder te forceren
			_groepenSvc.CategorieVerwijderen(catID, false);

			// Assert

			// Als we hier geraken, liep er iets mis.
			Assert.IsTrue(false);
		}

		/// <summary>
		/// Geforceerd een categorie met personen verwijderen
		/// </summary>
		[TestMethod]
		public void CategorieVerwijderenMetPersoonForceer()
		{
			// Arrange: categorie opzoeken, en persoon toevoegen.

			int catID = _groepenSvc.CategorieIDOphalen(
				TestInfo.GROEPID,
				TestInfo.ONBESTAANDECATEGORIECODES[2]);

			_personenSvc.CategorieKoppelen(
				new List<int> { TestInfo.GELIEERDEPERSOONID },
				new List<int> { catID });

			// Act

			// Verwijder categorie met forceren
			_groepenSvc.CategorieVerwijderen(catID, true);

			// Assert

			// Probeer categorie terug op te halen.  Dat moet failen.
			catID = _groepenSvc.CategorieIDOphalen(
				TestInfo.GROEPID, 
				TestInfo.ONBESTAANDECATEGORIECODES[2]);
			Assert.IsTrue(catID == 0);

			// Controleer ook of de gelieerde persoon niet per ongeluk mee is verwijderd
			var gp = _personenSvc.DetailsOphalen(TestInfo.GELIEERDEPERSOONID);
			Assert.IsTrue(gp != null);
		}
	}
}
