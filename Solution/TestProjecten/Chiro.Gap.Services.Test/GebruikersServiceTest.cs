using System;
using System.Collections.Generic;
using Chiro.Ad.ServiceContracts;
using Chiro.Cdf.Ioc;
using Chiro.Cdf.Poco;
using Chiro.Cdf.ServiceHelper;
using Chiro.Gap.Domain;
using Chiro.Gap.Dummies;
using Chiro.Gap.Poco.Model;
using Chiro.Gap.ServiceContracts.DataContracts;
using Chiro.Gap.WorkerInterfaces;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

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


        ///<summary>
        ///Test voor gebruiker zonder rechten maken.
        ///</summary>
        [TestMethod()]
        public void GebruikerMakenTest()
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
            repositoryProviderMock.Setup(src => src.RepositoryGet<GebruikersRechtV2>())
                .Returns(new DummyRepo<GebruikersRechtV2>(new List<GebruikersRechtV2>()));
            repositoryProviderMock.Setup(src => src.RepositoryGet<Groep>())
                .Returns(new DummyRepo<Groep>(new List<Groep> {gelieerdePersoon.Groep}));
            Factory.InstantieRegistreren(repositoryProviderMock.Object);

            // Mock AD-service
            var adServiceMock = new Mock<IAdService>();
            adServiceMock.Setup(src => src.GapLoginAanvragen(It.IsAny<int>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).Verifiable();

            var channelProviderMock = new Mock<IChannelProvider>();
            channelProviderMock.Setup(src => src.GetChannel<IAdService>()).Returns(adServiceMock.Object);

            Factory.InstantieRegistreren(channelProviderMock.Object);

            // ACT

            var target = Factory.Maak<GebruikersService>();
            // maak een gebruiker zonder rechten
            target.RechtenToekennen(gelieerdePersoon.ID, null);

            // ASSERT

            adServiceMock.Verify(src => src.GapLoginAanvragen(It.IsAny<int>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()), Times.Once());
        }

        /// <summary>
        ///A test for RechtenAfnemen
        ///</summary>
        [TestMethod()]
        public void RechtenAfnemenTest()
        {
            // ARRANGE

            var gr = new GebruikersRechtV2();
            var gelieerdePersoon = new GelieerdePersoon
                                   {
                                       ID = 1,
                                       Groep = new ChiroGroep {ID = 3},
                                       Persoon = new Persoon {ID = 2, GebruikersRechtV2 = new List<GebruikersRechtV2> {gr}}
                                   };
            gelieerdePersoon.Persoon.GelieerdePersoon.Add(gelieerdePersoon);

            gr.Groep = gelieerdePersoon.Groep;
            gr.Persoon = gelieerdePersoon.Persoon;
            gelieerdePersoon.Persoon.GebruikersRechtV2.Add(gr);

            var repositoryProviderMock = new Mock<IRepositoryProvider>();
            repositoryProviderMock.Setup(src => src.RepositoryGet<GelieerdePersoon>())
                .Returns(new DummyRepo<GelieerdePersoon>(new List<GelieerdePersoon> {gelieerdePersoon}));
            repositoryProviderMock.Setup(src => src.RepositoryGet<GebruikersRechtV2>())
                .Returns(new DummyRepo<GebruikersRechtV2>(new List<GebruikersRechtV2> { gr }));
            Factory.InstantieRegistreren(repositoryProviderMock.Object);

            // ACT

            var target = Factory.Maak<GebruikersService>();
            target.RechtenAfnemen(gelieerdePersoon.ID, new[] {gelieerdePersoon.Groep.ID});

            // ASSERT

            Assert.IsTrue(gr.VervalDatum <= DateTime.Now);
        }

        [TestMethod()]
        public void GebruikerOphalenTest()
        {
            // ARRANGE

            const int mijnAdNr = 1;
            const string mijnLogin = "joe";

            var gebruikersRecht = new GebruikersRechtV2
            {
                Persoon = new Persoon { ID = 2, AdNummer = mijnAdNr },
                Groep = new ChiroGroep { ID = 3 },
                PersoonsPermissies = Permissies.Lezen,
                VervalDatum = DateTime.Now.AddDays(1)
            };
            gebruikersRecht.Persoon.GebruikersRechtV2.Add(gebruikersRecht);
            gebruikersRecht.Groep.GebruikersRechtV2.Add(gebruikersRecht);

            var authenticatieManagerMock = new Mock<IAuthenticatieManager>();
            var autorisatieMangerMock = new Mock<IAutorisatieManager>();

            authenticatieManagerMock.Setup(src => src.AdNummerGet()).Returns(mijnAdNr);
            autorisatieMangerMock.Setup(svc => svc.MagLezen(gebruikersRecht.Persoon, gebruikersRecht.Persoon)).Verifiable();

            var repositoryProviderMock = new Mock<IRepositoryProvider>();
            repositoryProviderMock.Setup(src => src.RepositoryGet<Persoon>()).Returns(new DummyRepo<Persoon>(new List<Persoon> {gebruikersRecht.Persoon}));

            Factory.InstantieRegistreren(authenticatieManagerMock.Object);
            Factory.InstantieRegistreren(autorisatieMangerMock.Object);
            Factory.InstantieRegistreren(repositoryProviderMock.Object);

            // ACT

            var target = Factory.Maak<GebruikersService>();
            target.GebruikerOphalen(mijnLogin);

            // ASSERT

            autorisatieMangerMock.Verify(src => src.MagLezen(gebruikersRecht.Persoon, gebruikersRecht.Persoon), Times.AtLeastOnce());
        }
    }
}
