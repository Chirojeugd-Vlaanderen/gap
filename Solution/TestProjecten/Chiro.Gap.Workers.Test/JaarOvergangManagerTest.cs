using System;
using System.Data.Objects.DataClasses;
using System.Linq.Expressions;
using Chiro.Cdf.Data;
using Chiro.Gap.Domain;
using Chiro.Gap.Orm;
using Chiro.Gap.Orm.DataInterfaces;
using Chiro.Gap.Orm.SyncInterfaces;
using Chiro.Gap.WorkerInterfaces;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Chiro.Gap.ServiceContracts.DataContracts;
using System.Collections.Generic;
using Moq;
using Chiro.Gap.TestDbInfo;

namespace Chiro.Gap.Workers.Test
{
    /// <summary>
    ///This is a test class for JaarOvergangManagerTest and is intended
    ///to contain all JaarOvergangManagerTest Unit Tests
    ///</summary>
	[TestClass()]
	public class JaarOvergangManagerTest
	{

		#region Additional test attributes
		// 
		//You can use the following additional attributes as you write your tests:
		//
		//Use ClassInitialize to run code before running the first test in the class
		//[ClassInitialize()]
		//public static void MyClassInitialize(TestContext testContext)
		//{
		//}
		//
		//Use ClassCleanup to run code after all tests in a class have run
		//[ClassCleanup()]
		//public static void MyClassCleanup()
		//{
		//}
		//
		//Use TestInitialize to run code before running each test
		//[TestInitialize()]
		//public void MyTestInitialize()
		//{
		//}
		//
		//Use TestCleanup to run code after each test has run
		//[TestCleanup()]
		//public void MyTestCleanup()
		//{
		//}
		//
		#endregion

		/// <summary>
		///A test for JaarOvergangUitvoeren
		///</summary>
		[TestMethod()]                    
		public void JaarOvergangUitvoerenTest()
		{
			var ribbeloff = new OfficieleAfdeling {ID = 1, LeefTijdTot = 7, LeefTijdVan = 6, Naam = "Ribbel"};

			var groep = new ChiroGroep {ID = 10};
			var gwj = new GroepsWerkJaar {WerkJaar = 2010, Groep = groep};
			var gwj2 = new GroepsWerkJaar { WerkJaar = 2012, Groep = groep };
			var afdjaar1 = new AfdelingsJaar { ID = 1, GeboorteJaarVan = DateTime.Today.Year - 6, GeboorteJaarTot = DateTime.Today.Year - 7, OfficieleAfdeling = ribbeloff };
			var afd1 = new Afdeling {ID = 2, AfdelingsJaar = new EntityCollection<AfdelingsJaar>{afdjaar1}};
			afdjaar1.Afdeling = afd1;
			groep.Afdeling.Add(afd1);
			gwj.AfdelingsJaar.Add(afdjaar1);

			var newafdjaar = new AfdelingDetail { AfdelingID = afd1.ID, AfdelingsJaarID = afdjaar1.ID, GeboorteJaarVan = DateTime.Today.Year - 8, GeboorteJaarTot = DateTime.Today.Year - 10, OfficieleAfdelingID = ribbeloff.ID, Geslacht = GeslachtsType.Gemengd};

			var groepdao = new Mock<IGroepenDao>(MockBehavior.Strict);
			var geldao = new Mock<IGelieerdePersonenDao>(MockBehavior.Strict);

			var vgdao = new Mock<IVeelGebruikt>(MockBehavior.Strict);
			vgdao.Setup(vg => vg.GroepsWerkJaarOphalen(It.IsAny<int>())).Returns(gwj);

			var autmanag = new Mock<IAutorisatieManager>(MockBehavior.Strict);
			autmanag.Setup(vg => vg.IsSuperGav()).Returns(false);
			autmanag.Setup(vg => vg.IsGavGroep(It.IsAny<int>())).Returns(true);

			var gm = new GroepenManager(groepdao.Object, geldao.Object, vgdao.Object, autmanag.Object);
			var cgmm = new Mock<IChiroGroepenManager>(MockBehavior.Strict);
			cgmm.Setup(e => e.Ophalen(10, ChiroGroepsExtras.AlleAfdelingen | ChiroGroepsExtras.GroepsWerkJaren)).Returns(groep);

			var ajmm = new Mock<IAfdelingsJaarManager>(MockBehavior.Strict);
			ajmm.Setup(e => e.OfficieleAfdelingenOphalen()).Returns(new List<OfficieleAfdeling> {ribbeloff});
			ajmm.Setup(e => e.Aanmaken(afd1, ribbeloff, gwj2, newafdjaar.GeboorteJaarVan, newafdjaar.GeboorteJaarTot, newafdjaar.Geslacht)).Returns(new AfdelingsJaar());

			var gwmm = new Mock<IGroepsWerkJaarManager>(MockBehavior.Strict);
			gwmm.Setup(e => e.RecentsteOphalen(groep.ID)).Returns(gwj);
			gwmm.Setup(e => e.RecentsteOphalen(groep.ID, GroepsWerkJaarExtras.Afdelingen)).Returns(gwj);
			gwmm.Setup(e => e.StartOvergang(It.IsAny<int>())).Returns(DateTime.Today.AddDays(-1));
			gwmm.Setup(e => e.VolgendGroepsWerkJaarMaken(groep)).Returns(gwj2);
			gwmm.Setup(e => e.Bewaren(gwj2, GroepsWerkJaarExtras.Groep | GroepsWerkJaarExtras.Afdelingen)).Returns(gwj2);

			var target = new JaarOvergangManager(gm, cgmm.Object, ajmm.Object, gwmm.Object);
			var teActiveren = new List<AfdelingDetail> {newafdjaar};
			target.JaarOvergangUitvoeren(teActiveren, groep.ID);
			Assert.AreEqual(gwmm.Object.RecentsteOphalen(groep.ID, GroepsWerkJaarExtras.Afdelingen).AfdelingsJaar.Count, 1);
		}
	}
}
