using System.Linq;
using System.ServiceModel;
using Chiro.Gap.Dummies;
using Chiro.Gap.Poco.Model;
using Chiro.Gap.ServiceContracts.FaultContracts;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Chiro.Gap.ServiceContracts;
using Chiro.Cdf.Ioc;
using Chiro.Gap.ServiceContracts.Mappers;
using Chiro.Gap.TestDbInfo;
using System.Security.Principal;
using System.Threading;
using Chiro.Gap.ServiceContracts.DataContracts;
using System;
using Chiro.Cdf.Poco;
using Chiro.Gap.Domain;
using Moq;
using Chiro.Gap.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting.Web;
using Chiro.Gap.WorkerInterfaces;
using System.Collections.Generic;

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
			GroepInfo g = svc.InfoOphalen(TestInfo.GROEP_ID);
			#endregion

			#region Assert
			Assert.IsTrue(g.ID == TestInfo.GROEP_ID);
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
			GroepDetail g = svc.DetailOphalen(TestInfo.GROEP_ID);

			CategorieInfo categorie = (from cat in g.Categorieen
					           where cat.ID == TestInfo.CATEGORIE_ID
					           select cat).FirstOrDefault();

			AfdelingsJaarDetail afdeling = (from afd in g.Afdelingen
						 where afd.AfdelingID == TestInfo.AFDELING1_ID
						 select afd).FirstOrDefault();
			#endregion

			#region Assert
			Assert.IsTrue(g.ID == TestInfo.GROEP_ID);
			Assert.IsTrue(categorie != null);
			Assert.IsTrue(afdeling != null);
			#endregion
		}

        ///<summary>
        /// Kijkt na of er een foutmelding komt als een gebruiker probeert een functie bij te maken met een code
        /// die al bestaat voor een nationale functie (in dit geval 'CP')
        ///</summary>
        [TestMethod()]
        [ExpectedException(typeof(FaultException<BestaatAlFault<FunctieInfo>>))]
        public void FunctieToevoegenNationaleCode()
        {
            // Arrange.

            // De groep:
            int groepID = 123;          // arbitraire groepID
            var groep = new ChiroGroep {ID = groepID};

            // De bestaande nationale functie:

            var bestaandeFunctie = new Functie
                                       {
                                           Code = "CP",
                                           IsNationaal = true
                                       };


            // Mocking opzetten. Maak een groepenrepository en een functiesrepository die enkel
            // opleveren wat we hierboven creeerden.

            var groepenRepositoryMock = new Mock<IRepository<Groep>>();
            groepenRepositoryMock.Setup(src => src.ByID(It.IsAny<int>())).Returns(groep);
            var functieRepositoryMock = new Mock<IRepository<Functie>>();
            functieRepositoryMock.Setup(src => src.Select()).Returns((new[] {bestaandeFunctie}).AsQueryable());

            // Vervolgens configureren we een repositoryprovider, die de gemockte repository oplevert.

            var repositoryProviderMock = new Mock<IRepositoryProvider>();
            repositoryProviderMock.Setup(src => src.RepositoryGet<Groep>()).Returns(groepenRepositoryMock.Object);
            repositoryProviderMock.Setup(src => src.RepositoryGet<Functie>()).Returns(functieRepositoryMock.Object);

            // Tenslotte configureren we de IOC-container om de gemockte repository provider 
            // te gebruiken.

            Factory.InstantieRegistreren(repositoryProviderMock.Object);

            // Nu kunnen we de service die we willen testen creeren:

            var target = Factory.Maak<GroepenService>();

            // Act.

            // gegevens van de nieuwe functie. Op de code na arbitrair
            string naam = "Championplukker";    // naam van nieuwe functie
            string code = bestaandeFunctie.Code;    // code die toevallig gelijk is aan die van de bestaande functie
            Nullable<int> maxAantal = null;     // geen max. aantal voor de functie
            int minAantal = 0;                  // ook geen min. aantal
            LidType lidType = LidType.Alles;    // lidtype irrelevant voor deze functie
            Nullable<int> werkJaarVan = 2012;   // in gebruik vanaf 2012-2013
            
            target.FunctieToevoegen(groepID, naam, code, maxAantal, minAantal, lidType, werkJaarVan);

            // Assert

            Assert.Fail();  // De bedoeling is dat we hier niet komen, maar dat een exception werd gethrowd.
        }


        /// <summary>
        ///Kijkt na of 'functiesOphalen' al minstens de nationale functies ophaalt.
        ///</summary>
        [TestMethod]
        public void FunctiesOphalenTest()
        {
            // groepswerkjaar
            var groepsWerkJaar = new GroepsWerkJaar
                                     {
                                         ID = 1,
                                         Groep = new ChiroGroep(),
                                         WerkJaar = 2012
                                     };

            // nationale functie
            var nationaleFunctie = new Functie
                                       {
                                           ID = 2,
                                           IsNationaal = true,
                                           Type = LidType.Alles,
                                           Niveau = Niveau.Alles
                                       };

            // zet repositoryprovider zodanig op dat ons testgroepswerkjaar wordt opgeleverd
            // door de GroepsWerkJarenRepository, en onze testnationalefunctie door de FunctiesRepository

            var gwjRepo = new DummyRepo<GroepsWerkJaar>(new[] {groepsWerkJaar});
            var funRepo = new DummyRepo<Functie>(new[] {nationaleFunctie});


            var repoProviderMock = new Mock<IRepositoryProvider>();
            repoProviderMock.Setup(src => src.RepositoryGet<GroepsWerkJaar>()).Returns(gwjRepo);
            repoProviderMock.Setup(src => src.RepositoryGet<Functie>()).Returns(funRepo);

            Factory.InstantieRegistreren(repoProviderMock.Object);

            var target = Factory.Maak<GroepenService>();

            // Haal alle functies op relevant voor het groepswerkjaar met ID 1.
            var actual = target.FunctiesOphalen(1, LidType.Alles);

            Assert.AreEqual(actual.First().ID, 2);
        }
	}
}
