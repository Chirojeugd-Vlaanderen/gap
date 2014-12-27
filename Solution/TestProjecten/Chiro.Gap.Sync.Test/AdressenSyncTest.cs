using System.Diagnostics;
using System.Linq;
using Chiro.Cdf.Ioc;
using Chiro.Gap.Sync;
using Chiro.Kip.ServiceContracts;
using Chiro.Kip.ServiceContracts.DataContracts;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using Chiro.Gap.Poco.Model;
using System.Collections.Generic;
using Moq;
using Adres = Chiro.Gap.Poco.Model.Adres;
using Persoon = Chiro.Gap.Poco.Model.Persoon;

namespace Chiro.Gap.Sync.Test
{
    
    
    /// <summary>
    ///This is a test class for AdressenSyncTest and is intended
    ///to contain all AdressenSyncTest Unit Tests
    ///</summary>
    [TestClass()]
    public class AdressenSyncTest
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
        ///A test for StandaardAdressenBewaren
        ///</summary>
        [TestMethod()]
        public void StandaardAdressenBewarenTest()
        {
            // ARRANGE

            var adres = new BuitenLandsAdres {Land = new Land()};

            var persoonsAdres1 = new PersoonsAdres {Adres = adres, Persoon = new Persoon()};
            var persoonsAdres2 = new PersoonsAdres {Adres = adres, Persoon = new Persoon()};

            adres.PersoonsAdres.Add(persoonsAdres1);
            adres.PersoonsAdres.Add(persoonsAdres2);

            IEnumerable<Bewoner> teSyncenBewoners = null;

            // we mocken kipsync; registreer te syncen bewoners
            var kipSyncMock = new Mock<ISyncPersoonService>();
            kipSyncMock.Setup(
                src =>
                    src.StandaardAdresBewaren(It.IsAny<Kip.ServiceContracts.DataContracts.Adres>(),
                        It.IsAny<IEnumerable<Bewoner>>()))
                .Callback<Kip.ServiceContracts.DataContracts.Adres, IEnumerable<Bewoner>>(
                    (a, b) => teSyncenBewoners = b);

            Factory.InstantieRegistreren(kipSyncMock.Object);
            
            // ACT

            var target = Factory.Maak<AdressenSync>(); 
            target.StandaardAdressenBewaren(new List<PersoonsAdres>{persoonsAdres1});

            // ASSERT
            
            Assert.IsNotNull(teSyncenBewoners);
            Assert.AreEqual(1, teSyncenBewoners.Count());
        }
    }
}
