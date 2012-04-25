using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Chiro.Gap.ServiceContracts;
using Chiro.Cdf.Ioc;
using Chiro.Gap.Orm.DataInterfaces;
using Moq;
using Chiro.Gap.Dummies;
using Chiro.Gap.Orm;
using Chiro.Gap.ServiceContracts.Mappers;
using Chiro.Gap.TestDbInfo;
using System.Security.Principal;
using System.Threading;
using Chiro.Gap.ServiceContracts.DataContracts;

namespace Chiro.Gap.Services.Test
{
	/// <summary>
	/// Test op groepenservice
	/// </summary>
	/// <remarks>Blijkbaar heeft iemand mijn mocks voor de DAO weggehaald.  Dan testen we maar
	/// heel de flow.</remarks>
	[TestClass]
	public class GroepenServiceTest
	{
		public GroepenServiceTest()
		{
		}


		#region Additional test attributes
		//
		// You can use the following additional attributes as you write your tests:
		//
		// Use ClassInitialize to run code before running the first test in the class
		// [ClassInitialize]
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
		static public void InitialiseerTests(TestContext tc)
		{
			Factory.ContainerInit();
			MappingHelper.MappingsDefinieren();
		}

		[ClassCleanup]
		static public void AfsluitenTests()
		{
		}


		/// <summary>
		/// Deze functie zorgt ervoor dat de PrincipalPermissionAttributes op de service methods
		/// geen excepties genereren, door te doen alsof de service aangeroepen is met de goede
		/// security
		/// </summary>
		[TestInitialize]
		public void VoorElkeTest()
		{
			var identity = new GenericIdentity(Properties.Settings.Default.TestUser);
			var roles = new[] { Properties.Settings.Default.TestSecurityGroep };
			var principal = new GenericPrincipal(identity, roles);
			Thread.CurrentPrincipal = principal;
		}


		/// <summary>
		/// Ophalen groepsinfo (zonder categorieën of afdelingen)
		/// </summary>
		[TestMethod]
		public void GroepOphalen()
		{
			#region Arrange
			IGroepenService svc = Factory.Maak<GroepenService>();
			#endregion

			#region Act
			GroepInfo g = svc.InfoOphalen(TestInfo.GROEPID);
			#endregion

			#region Assert
			Assert.IsTrue(g.ID == TestInfo.GROEPID);
			#endregion
		}

		/// <summary>
		/// Ophalen groepsinfo met categorieën en afdelingen
		/// </summary>
		[TestMethod]
		public void GroepDetailOphalen()
		{
			#region Arrange
			IGroepenService svc = Factory.Maak<GroepenService>();
			#endregion

			#region Act
			GroepDetail g = svc.DetailOphalen(TestInfo.GROEPID);

			CategorieInfo categorie = (from cat in g.Categorieen
					           where cat.ID == TestInfo.CATEGORIEID
					           select cat).FirstOrDefault();

			AfdelingsJaarDetail afdeling = (from afd in g.Afdelingen
						 where afd.AfdelingID == TestInfo.AFDELING1ID
						 select afd).FirstOrDefault();
			#endregion

			#region Assert
			Assert.IsTrue(g.ID == TestInfo.GROEPID);
			Assert.IsTrue(categorie != null);
			Assert.IsTrue(afdeling != null);
			#endregion
		}
	}
}
