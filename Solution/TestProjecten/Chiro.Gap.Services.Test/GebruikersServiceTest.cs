using System.Collections.Generic;
using System.Linq;
using Chiro.Cdf.Ioc;
using Chiro.Gap.Domain;
using Chiro.Gap.Dummies;
using Chiro.Gap.Poco.Model;
using Chiro.Gap.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using Microsoft.VisualStudio.TestTools.UnitTesting.Web;
using Chiro.Gap.WorkerInterfaces;
using Chiro.Cdf.Poco;
using Moq;
using GebruikersRecht = Chiro.Gap.ServiceContracts.DataContracts.GebruikersRecht;

namespace Chiro.Gap.Services.Test
{
    
    
    /// <summary>
    ///This is a test class for GebruikersServiceTest and is intended
    ///to contain all GebruikersServiceTest Unit Tests
    ///</summary>
    [TestClass()]
    public class GebruikersServiceTest
    {


        private TestContext testContextInstance;

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
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
        [TestInitialize()]
        public void MyTestInitialize()
        {
            Factory.ContainerInit();
        }
        //
        //Use TestCleanup to run code after each test has run
        //[TestCleanup()]
        //public void MyTestCleanup()
        //{
        //}
        //
        #endregion


        /// <summary>
        ///A test for RechtenToekennen
        ///</summary>
        [TestMethod()]
        public void RechtenToekennenTest()
        {
            // ARRANGE

            var gelieerdePersoon = new GelieerdePersoon
                                       {
                                           Persoon =
                                               new Persoon
                                                   {
                                                       AdNummer = 12345,
                                                       Naam = "Magdalena",
                                                       VoorNaam = "Maria"
                                                   },
                                           Groep = new ChiroGroep {ID = 1},
                                           Communicatie = new List<CommunicatieVorm>
                                                              {
                                                                  new CommunicatieVorm
                                                                      {
                                                                          CommunicatieType =
                                                                              new CommunicatieType
                                                                                  {
                                                                                      ID =
                                                                                          (int)
                                                                                          CommunicatieTypeEnum
                                                                                              .Email
                                                                                  },
                                                                          Nummer = "piep@boe.be"
                                                                      }
                                                              }
                                       };

            var repositoryProviderMock = new Mock<IRepositoryProvider>();
            repositoryProviderMock.Setup(src => src.RepositoryGet<GelieerdePersoon>())
                                  .Returns(new DummyRepo<GelieerdePersoon>(new List<GelieerdePersoon> {gelieerdePersoon}));
            repositoryProviderMock.Setup(src => src.RepositoryGet<Gav>()).Returns(new DummyRepo<Gav>(new List<Gav>()));
            Factory.InstantieRegistreren(repositoryProviderMock.Object);

            // ACT

            var target = Factory.Maak<GebruikersService>();
            // maak een gebruiker zonder rechten
            target.RechtenToekennen(gelieerdePersoon.ID, null);

            // ASSERT

            Assert.IsTrue(gelieerdePersoon.Persoon.Gav.Any());
        }
    }
}
