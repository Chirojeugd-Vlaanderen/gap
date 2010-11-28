using System.ServiceModel;
using Chiro.Cdf.Ioc;
using Chiro.Gap.ServiceContracts.FaultContracts;
using Chiro.Gap.ServiceContracts.Mappers;
using Chiro.Gap.TestDbInfo;
using Chiro.Gap.Workers.Exceptions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Chiro.Gap.ServiceContracts.DataContracts;

namespace Chiro.Gap.Services.Test
{
    
    
    /// <summary>
    /// This is a test class for GelieerdePersonenServiceTest and is intended
    /// to contain all GelieerdePersonenServiceTest Unit Tests
    /// </summary>
	[TestClass]
	public class GelieerdePersonenServiceTest
	{


		private TestContext testContextInstance;

		/// <summary>
		///Gets or sets the test context which provides
		///information about and functionality for the current test run.
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
		//[TestInitialize]
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

		[ClassInitialize()]
		public static void MyClassInitialize(TestContext testContext)
		{
			Factory.ContainerInit();
			MappingHelper.MappingsDefinieren();

			// Verwijder alle communicatievormen van GelieerdePersoon1, want straks moeten die terug gemaakt
			// worden.

			var svc = Factory.Maak<GelieerdePersonenService>();

			var gp = svc.AlleDetailsOphalen(TestInfo.GELIEERDEPERSOONID);

			foreach (var cv in gp.CommunicatieInfo)
			{
				svc.CommunicatieVormVerwijderenVanPersoon(cv.ID);
			}
		}

		/// <summary>
		///A test for CommunicatieVormToevoegen
		/// </summary>
		[TestMethod]
		[ExpectedException(typeof(FaultException<FoutNummerFault>))]
		public void CommunicatieVormToevoegenTestOngeldig()
		{
			var target = Factory.Maak<GelieerdePersonenService>();

			var gelieerdePersoonID = TestInfo.GELIEERDEPERSOONID;

			var commInfo = new CommunicatieDetail()
                            	{
                            		CommunicatieTypeID = 1,
                            		Nummer = TestInfo.ONGELDIGTELEFOONNR
                            	};

			target.CommunicatieVormToevoegen(gelieerdePersoonID, commInfo);
			Assert.IsTrue(false);
		}

		///<summary>
		///Toevoegen van een geldig telefoonnr
		/// </summary>
		[TestMethod]
		public void CommunicatieVormToevoegenTestGeldig()
		{
			var target = Factory.Maak<GelieerdePersonenService>();

			var gelieerdePersoonID = TestInfo.GELIEERDEPERSOONID;

			var commInfo = new CommunicatieDetail()
			{
				CommunicatieTypeID = 1,
				Voorkeur = true,
				Nummer = TestInfo.GELDIGTELEFOONNR
			};

			target.CommunicatieVormToevoegen(gelieerdePersoonID, commInfo);
			Assert.IsTrue(true);	// al blij als er geen exception optreedt
		}

		///<summary>
		///Toevoegen van een geldig telefoonnr aan een onbestaande gelieerde persoon.  
		/// Dit moet failen met een GeenGavException
		/// </summary>
		[ExpectedException(typeof(FaultException<FoutNummerFault>))]
		[TestMethod]
		public void CommunicatieVormToevoegenTestOnbestaandePersoon()
		{
			var target = Factory.Maak<GelieerdePersonenService>();

			var gelieerdePersoonID = TestInfo.ONBESTAANDEGELIEERDEPERSOONID;

			var commInfo = new CommunicatieDetail()
			{
				CommunicatieTypeID = 1,
				Voorkeur = true,
				Nummer = TestInfo.GELDIGTELEFOONNR
			};

			target.CommunicatieVormToevoegen(gelieerdePersoonID, commInfo);
			Assert.IsTrue(false);	// hier mogen we niet geraken; hopelijk hebben we een exception gehad.
		}
	}
}
