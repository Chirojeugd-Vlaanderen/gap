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
    }
}
