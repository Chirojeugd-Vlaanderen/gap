using System.Collections.Generic;
using Chiro.Cdf.Ioc;
using Chiro.Gap.Domain;
using Chiro.Gap.Poco.Model.Exceptions;
using Chiro.Gap.Sync;
using Chiro.Gap.TestAttributes;
using Chiro.Kip.ServiceContracts;
using Chiro.Kip.ServiceContracts.DataContracts;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using Chiro.Gap.Poco.Model;
using Moq;
using Persoon = Chiro.Gap.Poco.Model.Persoon;

namespace Chiro.Gap.Sync.Test
{
    
    
    /// <summary>
    ///This is a test class for LedenSyncTest and is intended
    ///to contain all LedenSyncTest Unit Tests
    ///</summary>
    [TestClass()]
    public class LedenSyncTest
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
        [ClassInitialize()]
        public static void MyClassInitialize(TestContext testContext)
        {
            MappingHelper.MappingsDefinieren();
        }
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
        ///A test for AfdelingenUpdaten
        ///</summary>
        [TestMethod()]
        public void AfdelingenUpdatenTest()
        {
            // ARRANGE

            var kipSyncMock = new Mock<ISyncPersoonService>();
            kipSyncMock.Setup(src => src.AfdelingenUpdaten(It.IsAny<Kip.ServiceContracts.DataContracts.Persoon>(), It.IsAny<string>(), It.IsAny<int>(), It.IsAny<IEnumerable<AfdelingEnum>>())).Verifiable();
            Factory.InstantieRegistreren(kipSyncMock.Object);

            var lid = new Kind
                          {
                              AfdelingsJaar = new AfdelingsJaar {OfficieleAfdeling = new OfficieleAfdeling {ID = 1}},
                              GroepsWerkJaar = new GroepsWerkJaar {Groep = new ChiroGroep()},
                              GelieerdePersoon = new GelieerdePersoon {Persoon = new Persoon()}
                          };

            // ACT

            var target = Factory.Maak<LedenSync>();
            target.AfdelingenUpdaten(lid);

            // ASSERT

            kipSyncMock.VerifyAll();
        }

        /// <summary>
        /// Bewaren moet bewaren, en niet verwijderen. Om een inactief lid te verwijderen, bestaat de method 'Verwijderen'.
        ///</summary>
        [TestMethod()]
        [ExpectedFoutNummer(typeof(FoutNummerException), FoutNummer.LidUitgeschreven)]
        public void BewarenTest()
        {
            // ARRANGE

            var kipSyncMock = new Mock<ISyncPersoonService>();
            kipSyncMock.Setup(src => src.LidVerwijderen(It.IsAny<int>(), It.IsAny<string>(), It.IsAny<int>(), It.IsAny<DateTime>())).Verifiable();
            Factory.InstantieRegistreren(kipSyncMock.Object);

            var lid = new Kind
            {
                AfdelingsJaar = new AfdelingsJaar { OfficieleAfdeling = new OfficieleAfdeling { ID = 1 } },
                GroepsWerkJaar = new GroepsWerkJaar { Groep = new ChiroGroep() },
                GelieerdePersoon = new GelieerdePersoon { Persoon = new Persoon {AdNummer = 2} },
                NonActief = true,
                UitschrijfDatum = DateTime.Now,
                EindeInstapPeriode = DateTime.Now.AddMonths(-6)
            };

            // ACT

            var target = Factory.Maak<LedenSync>();
            target.Bewaren(lid);

            // ASSERT

            kipSyncMock.Verify(
                src => src.LidVerwijderen(It.IsAny<int>(), It.IsAny<string>(), It.IsAny<int>(), It.IsAny<DateTime>()),
                Times.Never());
        }
    }
}
