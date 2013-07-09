using System.Collections.Generic;
using Chiro.Cdf.Ioc;
using Chiro.Gap.Dummies;
using Chiro.Gap.Poco.Model;
using Chiro.Gap.ServiceContracts.Mappers;
using Chiro.Gap.SyncInterfaces;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Chiro.Cdf.Poco;
using Chiro.Gap.ServiceContracts.DataContracts;
using Moq;
using Microsoft.VisualStudio.TestTools.UnitTesting.Web;
using Chiro.Gap.WorkerInterfaces;
using Chiro.Gap.Domain;

namespace Chiro.Gap.Services.Test
{


    /// <summary>
    ///This is a test class for UitstappenServiceTest and is intended
    ///to contain all UitstappenServiceTest Unit Tests
    ///</summary>
    [TestClass()]
    public class UitstappenServiceTest
    {


        private TestContext testContextInstance;

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext
        {
            get { return testContextInstance; }
            set { testContextInstance = value; }
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
            MappingHelper.MappingsDefinieren();
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
        /// Controleert of een bewaard bivak naar Kipadmin wordt gesynct
        /// </summary>
        [TestMethod()]
        public void BewarenTest()
        {
            // ARRANGE 
            var groepsWerkJaar = new GroepsWerkJaar {Groep = new ChiroGroep {ID = 1}};
            var uitstapInfo = new UitstapInfo {IsBivak = true};

            // mock synchronisatie voor kipadmin
            var bivakSyncMock = new Mock<IBivakSync>();
            bivakSyncMock.Setup(src => src.Bewaren(It.IsAny<Uitstap>())).Verifiable();
            Factory.InstantieRegistreren(bivakSyncMock.Object);

            // mock data acces
            var repositoryProviderMock = new Mock<IRepositoryProvider>();
            repositoryProviderMock.Setup(src => src.RepositoryGet<GroepsWerkJaar>())
                                  .Returns(new DummyRepo<GroepsWerkJaar>(new List<GroepsWerkJaar> { groepsWerkJaar }));
            Factory.InstantieRegistreren(repositoryProviderMock.Object);

            // ACT

            var target = Factory.Maak<UitstappenService>();

            target.Bewaren(groepsWerkJaar.Groep.ID, uitstapInfo);

            // ASSERT

            bivakSyncMock.Verify(src => src.Bewaren(It.IsAny<Uitstap>()), Times.AtLeastOnce());
        }

        /// <summary>
        /// Als een voormalig bivak bewaard wordt, maar de IsBivak-vlag is nu gecleard, dan
        /// moet het bivak opnieuw verwijderd worden uit Kipadmin.
        /// </summary>
        [TestMethod()]
        public void BivakVerwijderenAlsGeenBivakMeerTest()
        {
            // ARRANGE 
            var groepsWerkJaar = new GroepsWerkJaar
                                     {
                                         Groep = new ChiroGroep {ID = 1},
                                     };
            var bivak = new Uitstap {ID = 2, IsBivak = true, GroepsWerkJaar = groepsWerkJaar};
            groepsWerkJaar.Uitstap.Add(bivak);

            var uitstapInfo = new UitstapInfo{ID = bivak.ID, IsBivak = false};

            // mock synchronisatie voor kipadmin
            var bivakSyncMock = new Mock<IBivakSync>();
            bivakSyncMock.Setup(src => src.Verwijderen(bivak.ID)).Verifiable();
            Factory.InstantieRegistreren(bivakSyncMock.Object);

            // mock data acces
            var repositoryProviderMock = new Mock<IRepositoryProvider>();
            repositoryProviderMock.Setup(src => src.RepositoryGet<GroepsWerkJaar>())
                                  .Returns(new DummyRepo<GroepsWerkJaar>(new List<GroepsWerkJaar> { groepsWerkJaar }));
            Factory.InstantieRegistreren(repositoryProviderMock.Object);

            // ACT

            var target = Factory.Maak<UitstappenService>();

            target.Bewaren(groepsWerkJaar.Groep.ID, uitstapInfo);

            // ASSERT

            bivakSyncMock.Verify(src => src.Verwijderen(bivak.ID), Times.AtLeastOnce());
        }

        /// <summary>
        /// Controleert of een gesavede bivakplaats ook gesynct wordt naar Kipadmin
        /// </summary>
        [TestMethod()]
        public void PlaatsBewarenTest()
        {
            // ARRANGE 
            var groepsWerkJaar = new GroepsWerkJaar
            {
                Groep = new ChiroGroep { ID = 1 },
            };
            var bivak = new Uitstap { ID = 2, IsBivak = true, GroepsWerkJaar = groepsWerkJaar };
            groepsWerkJaar.Uitstap.Add(bivak);
            var land = new Land() {Naam = "Nederland"};

            // mock synchronisatie kipadmin
            var bivakSyncMock = new Mock<IBivakSync>();
            bivakSyncMock.Setup(src => src.Bewaren(It.IsAny<Uitstap>())).Verifiable();
            Factory.InstantieRegistreren(bivakSyncMock.Object);

            // mock data acces
            var repositoryProviderMock = new Mock<IRepositoryProvider>();
            repositoryProviderMock.Setup(src => src.RepositoryGet<Uitstap>())
                                  .Returns(new DummyRepo<Uitstap>(new List<Uitstap> { bivak }));
            repositoryProviderMock.Setup(src => src.RepositoryGet<Adres>())
                                   .Returns(new DummyRepo<Adres>(new List<Adres>()));
            repositoryProviderMock.Setup(src => src.RepositoryGet<Land>())
                                   .Returns(new DummyRepo<Land>(new List<Land> { land }));
            repositoryProviderMock.Setup(src => src.RepositoryGet<WoonPlaats>())
                                   .Returns(new DummyRepo<WoonPlaats>(new List<WoonPlaats>()));
            repositoryProviderMock.Setup(src => src.RepositoryGet<StraatNaam>())
                                   .Returns(new DummyRepo<StraatNaam>(new List<StraatNaam>()));
            Factory.InstantieRegistreren(repositoryProviderMock.Object);

            // ACT
            var target = Factory.Maak<UitstappenService>();
            target.PlaatsBewaren(bivak.ID, "Onze kampplaats", new AdresInfo {LandNaam = land.Naam});

            // ASSERT
            bivakSyncMock.Verify(src => src.Bewaren(It.IsAny<Uitstap>()), Times.AtLeastOnce());
        }

        /// <summary>
        /// Test: gekende bivakplaats toevoegen als eerste bivakplaats aan een uitstap
        /// </summary>
        [TestMethod()]
        public void GekendePlaatsBewarenTest()
        {
            // ARRANGE

            var groep = new ChiroGroep {ID = 3};
            var bivak = new Uitstap {ID = 1, GroepsWerkJaar = new GroepsWerkJaar{Groep = groep}};
            var adres = new BuitenLandsAdres
                            {
                                Straat = "Vorststraat",
                                HuisNr = 3,
                                PostNummer = 1234,
                                PostCode = "BR",
                                WoonPlaats = "Killegem",
                                Land = new Land{Naam = "Nederland"}
                            };
            var plaats = new Plaats { ID = 2, Naam = "Coole bivakplaats", Adres = adres, Groep = groep};
            adres.BivakPlaats.Add(plaats);

            // dependency injection voor data access

            var dummyRepositoryProvider = new Mock<IRepositoryProvider>();
            dummyRepositoryProvider.Setup(src => src.RepositoryGet<Uitstap>())
                                   .Returns(new DummyRepo<Uitstap>(new List<Uitstap> { bivak }));
            dummyRepositoryProvider.Setup(src => src.RepositoryGet<Plaats>())
                                   .Returns(new DummyRepo<Plaats>(new List<Plaats> { plaats }));
            dummyRepositoryProvider.Setup(src => src.RepositoryGet<Adres>())
                                   .Returns(new DummyRepo<Adres>(new List<Adres> { adres }));
            dummyRepositoryProvider.Setup(src => src.RepositoryGet<Land>())
                                   .Returns(new DummyRepo<Land>(new List<Land> { adres.Land }));
            dummyRepositoryProvider.Setup(src => src.RepositoryGet<WoonPlaats>())
                                   .Returns(new DummyRepo<WoonPlaats>(new List<WoonPlaats>()));
            dummyRepositoryProvider.Setup(src => src.RepositoryGet<StraatNaam>())
                                   .Returns(new DummyRepo<StraatNaam>(new List<StraatNaam>()));
            Factory.InstantieRegistreren(dummyRepositoryProvider.Object);

            // ACT

            var target = Factory.Maak<UitstappenService>();
            target.PlaatsBewaren(bivak.ID, plaats.Naam,
                                 new AdresInfo
                                     {
                                         StraatNaamNaam = adres.Straat,
                                         HuisNr = adres.HuisNr,
                                         PostNr = adres.PostNummer ?? 0,
                                         PostCode = adres.PostCode,
                                         WoonPlaatsNaam = adres.WoonPlaats,
                                         LandNaam = adres.Land.Naam
                                     });

            // ASSERT

            Assert.AreEqual(bivak.Plaats.ID, plaats.ID);
        }

        /// <summary>
        /// Test: onbekende bivakplaats met bekend adres toevoegen als eerste bivakplaats aan een uitstap
        /// </summary>
        [TestMethod()]
        public void PlaatsOpGekendAdresBewarenTest()
        {
            // ARRANGE

            var groep = new ChiroGroep { ID = 3 };
            var bivak = new Uitstap { ID = 1, GroepsWerkJaar = new GroepsWerkJaar { Groep = groep } };
            var adres = new BuitenLandsAdres
            {
                Straat = "Vorststraat",
                HuisNr = 3,
                PostNummer = 1234,
                PostCode = "BR",
                WoonPlaats = "Killegem",
                Land = new Land { Naam = "Nederland" }
            };

            // dependency injection voor data access

            var dummyRepositoryProvider = new Mock<IRepositoryProvider>();
            dummyRepositoryProvider.Setup(src => src.RepositoryGet<Uitstap>())
                                   .Returns(new DummyRepo<Uitstap>(new List<Uitstap> { bivak }));
            dummyRepositoryProvider.Setup(src => src.RepositoryGet<Plaats>())
                                   .Returns(new DummyRepo<Plaats>(new List<Plaats>()));
            dummyRepositoryProvider.Setup(src => src.RepositoryGet<Adres>())
                                   .Returns(new DummyRepo<Adres>(new List<Adres> { adres }));
            dummyRepositoryProvider.Setup(src => src.RepositoryGet<Land>())
                                   .Returns(new DummyRepo<Land>(new List<Land> { adres.Land }));
            dummyRepositoryProvider.Setup(src => src.RepositoryGet<WoonPlaats>())
                                   .Returns(new DummyRepo<WoonPlaats>(new List<WoonPlaats>()));
            dummyRepositoryProvider.Setup(src => src.RepositoryGet<StraatNaam>())
                                   .Returns(new DummyRepo<StraatNaam>(new List<StraatNaam>()));
            Factory.InstantieRegistreren(dummyRepositoryProvider.Object);

            // ACT

            var target = Factory.Maak<UitstappenService>();
            target.PlaatsBewaren(bivak.ID, "Warme Bivakplaats",
                                 new AdresInfo
                                 {
                                     StraatNaamNaam = adres.Straat,
                                     HuisNr = adres.HuisNr,
                                     PostNr = adres.PostNummer ?? 0,
                                     PostCode = adres.PostCode,
                                     WoonPlaatsNaam = adres.WoonPlaats,
                                     LandNaam = adres.Land.Naam
                                 });

            // ASSERT

            Assert.AreEqual(bivak.Plaats.Adres.ID, adres.ID);
        }

        /// <summary>
        /// Test: onbekende bivakplaats op onbekend adres toevoegen als eerste bivakplaats aan een uitstap
        /// </summary>
        [TestMethod()]
        public void PlaatsOpOnbekendAdresBewarenTest()
        {
            // ARRANGE

            var groep = new ChiroGroep { ID = 3 };
            var bivak = new Uitstap { ID = 1, GroepsWerkJaar = new GroepsWerkJaar { Groep = groep } };
            var land = new Land { Naam = "Nederland" };

            // dependency injection voor data access

            var dummyRepositoryProvider = new Mock<IRepositoryProvider>();
            dummyRepositoryProvider.Setup(src => src.RepositoryGet<Uitstap>())
                                   .Returns(new DummyRepo<Uitstap>(new List<Uitstap> { bivak }));
            dummyRepositoryProvider.Setup(src => src.RepositoryGet<Plaats>())
                                   .Returns(new DummyRepo<Plaats>(new List<Plaats>()));
            dummyRepositoryProvider.Setup(src => src.RepositoryGet<Adres>())
                                   .Returns(new DummyRepo<Adres>(new List<Adres>()));
            dummyRepositoryProvider.Setup(src => src.RepositoryGet<Land>())
                                   .Returns(new DummyRepo<Land>(new List<Land> { land }));
            dummyRepositoryProvider.Setup(src => src.RepositoryGet<WoonPlaats>())
                                   .Returns(new DummyRepo<WoonPlaats>(new List<WoonPlaats>()));
            dummyRepositoryProvider.Setup(src => src.RepositoryGet<StraatNaam>())
                                   .Returns(new DummyRepo<StraatNaam>(new List<StraatNaam>()));
            Factory.InstantieRegistreren(dummyRepositoryProvider.Object);

            // ACT

            var target = Factory.Maak<UitstappenService>();
            target.PlaatsBewaren(bivak.ID, "Warme Bivakplaats",
                                 new AdresInfo
                                 {
                                     StraatNaamNaam = "Zonnestraat",
                                     HuisNr = 8,
                                     PostNr = 9876,
                                     PostCode = "PF",
                                     WoonPlaatsNaam = "Warmegem",
                                     LandNaam = "Nederland"
                                 });

            // ASSERT

            Assert.IsInstanceOfType(bivak.Plaats.Adres, typeof(BuitenLandsAdres));
        }

        /// <summary>
        /// De bivakplaats van een bivak vervangen
        /// </summary>
        [TestMethod()]
        public void PlaatsVervangenTest()
        {
            // ARRANGE

            var groep = new ChiroGroep { ID = 3 };
            var bivak = new Uitstap { ID = 1, GroepsWerkJaar = new GroepsWerkJaar { Groep = groep } };
            var adres = new BuitenLandsAdres
            {
                Straat = "Vorststraat",
                HuisNr = 3,
                PostNummer = 1234,
                PostCode = "BR",
                WoonPlaats = "Killegem",
                Land = new Land { Naam = "Nederland" }
            };
            var plaats = new Plaats { ID = 2, Naam = "Coole bivakplaats", Adres = adres, Groep = groep };
            bivak.Plaats = plaats; // Bivak heeft al een plaats

            // dependency injection voor data access

            var dummyRepositoryProvider = new Mock<IRepositoryProvider>();
            dummyRepositoryProvider.Setup(src => src.RepositoryGet<Uitstap>())
                                   .Returns(new DummyRepo<Uitstap>(new List<Uitstap> { bivak }));
            dummyRepositoryProvider.Setup(src => src.RepositoryGet<Plaats>())
                                   .Returns(new DummyRepo<Plaats>(new List<Plaats> { plaats }));
            dummyRepositoryProvider.Setup(src => src.RepositoryGet<Adres>())
                                   .Returns(new DummyRepo<Adres>(new List<Adres> { adres }));
            dummyRepositoryProvider.Setup(src => src.RepositoryGet<Land>())
                                   .Returns(new DummyRepo<Land>(new List<Land> { adres.Land }));
            dummyRepositoryProvider.Setup(src => src.RepositoryGet<WoonPlaats>())
                                   .Returns(new DummyRepo<WoonPlaats>(new List<WoonPlaats>()));
            dummyRepositoryProvider.Setup(src => src.RepositoryGet<StraatNaam>())
                                   .Returns(new DummyRepo<StraatNaam>(new List<StraatNaam>()));

            Factory.InstantieRegistreren(dummyRepositoryProvider.Object);

            // ACT

            var target = Factory.Maak<UitstappenService>();
            target.PlaatsBewaren(bivak.ID, "Warme Bivakplaats",
                                 new AdresInfo
                                 {
                                     StraatNaamNaam = "Zonnestraat",
                                     HuisNr = 8,
                                     PostNr = 9876,
                                     PostCode = "PF",
                                     WoonPlaatsNaam = "Warmegem",
                                     LandNaam = "Nederland"
                                 });

            // ASSERT

            Assert.AreNotEqual(bivak.Plaats.ID, plaats.ID); // is de plaats wel veranderd?
        }



        /// <summary>
        /// Test om te kijken of verwijderen van bivak wel synct
        /// </summary>
        [TestMethod()]
        public void UitstapVerwijderenTest()
        {
            // ARRANGE 
            var groepsWerkJaar = new GroepsWerkJaar
            {
                Groep = new ChiroGroep { ID = 1 },
            };
            var bivak = new Uitstap { ID = 2, IsBivak = true, GroepsWerkJaar = groepsWerkJaar };
            groepsWerkJaar.Uitstap.Add(bivak);

            // mock synchronisatie voor kipadmin
            var bivakSyncMock = new Mock<IBivakSync>();
            bivakSyncMock.Setup(src => src.Verwijderen(bivak.ID)).Verifiable();
            Factory.InstantieRegistreren(bivakSyncMock.Object);

            // mock data acces
            var repositoryProviderMock = new Mock<IRepositoryProvider>();
            repositoryProviderMock.Setup(src => src.RepositoryGet<Uitstap>())
                                   .Returns(new DummyRepo<Uitstap>(new List<Uitstap> { bivak }));
            Factory.InstantieRegistreren(repositoryProviderMock.Object);

            // ACT

            var target = Factory.Maak<UitstappenService>();

            target.UitstapVerwijderen(bivak.ID);

            // ASSERT

            bivakSyncMock.Verify(src => src.Verwijderen(bivak.ID), Times.AtLeastOnce());
        }
    }
}
