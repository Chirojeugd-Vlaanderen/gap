using System.Collections.Generic;
using Chiro.Cdf.Ioc;
using Chiro.Gap.Dummies;
using Chiro.Gap.Poco.Model;
using Chiro.Gap.UpdateSvc.Service;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using Microsoft.VisualStudio.TestTools.UnitTesting.Web;
using Chiro.Gap.WorkerInterfaces;
using Chiro.Gap.SyncInterfaces;
using Chiro.Cdf.Poco;
using Moq;

namespace Chiro.Gap.UpdateSvc.Test
{
    
    
    /// <summary>
    ///This is a test class for UpdateServiceTest and is intended
    ///to contain all UpdateServiceTest Unit Tests
    ///</summary>
    [TestClass()]
    public class UpdateServiceTest
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
        
        //Use TestCleanup to run code after each test has run
        //[TestCleanup()]
        //public void MyTestCleanup()
        //{
        //}
        //
        #endregion


        /// <summary>
        ///A test for GroepDesactiveren
        ///</summary>
        [TestMethod()]
        public void GroepDesactiverenTest()
        {
            // ARRANGE

            var groep = new ChiroGroep {Code = "TST/0001"};
            var repositoryProviderMock = new Mock<IRepositoryProvider>();
            repositoryProviderMock.Setup(src => src.RepositoryGet<Groep>())
                                  .Returns(new DummyRepo<Groep>(new List<Groep> {groep}));
            Factory.InstantieRegistreren(repositoryProviderMock.Object);

            // ACT

            var target = Factory.Maak<UpdateService>();
            target.GroepDesactiveren(groep.Code, DateTime.Now);

            // ASSERT

            Assert.IsNotNull(groep.StopDatum);
        }

        /// <summary>
        ///A test for DubbelVerwijderen
        ///</summary>
        [TestMethod()]
        public void DubbelVerwijderenTest()
        {
            // ARRANGE

            var origineel = new Persoon();
            var dubbel = new Persoon();

            var allePersonen = new List<Persoon> {origineel, dubbel};

            var repositoryProviderMock = new Mock<IRepositoryProvider>();
            repositoryProviderMock.Setup(src => src.RepositoryGet<Persoon>())
                                  .Returns(new DummyRepo<Persoon>(allePersonen));

            Factory.InstantieRegistreren(repositoryProviderMock.Object);

            // ACT
            var target = Factory.Maak<UpdateService_Accessor>();
            target.DubbelVerwijderen(origineel, dubbel);

            // ASSERT
            Assert.IsTrue(allePersonen.Contains(origineel));
            Assert.IsFalse(allePersonen.Contains(dubbel));
        }

        /// <summary>
        /// Test op dubbel verwijderen waar de 2 personen een adres delen.
        /// </summary>
        [TestMethod()]
        public void DubbelVerwijderenGedeeldAdresTest()
        {
            // ARRANGE

            var origineel = new Persoon();
            var dubbel = new Persoon();

            var gelieerdeOrigineel = new GelieerdePersoon {Persoon = origineel, Groep = new ChiroGroep()};
            var gelieerdeDubbel = new GelieerdePersoon {Persoon = dubbel, Groep = new ChiroGroep()};

            origineel.GelieerdePersoon.Add(gelieerdeOrigineel);
            dubbel.GelieerdePersoon.Add(gelieerdeDubbel);

            var adres = new BelgischAdres();

            var origineelPa = new PersoonsAdres {Persoon = origineel, Adres = adres};
            var dubbelPa = new PersoonsAdres {Persoon = dubbel, Adres = adres};

            adres.PersoonsAdres.Add(origineelPa);
            adres.PersoonsAdres.Add(dubbelPa);

            origineel.PersoonsAdres.Add(origineelPa);
            dubbel.PersoonsAdres.Add(dubbelPa);

            var allePersonen = new List<Persoon> { origineel, dubbel };
            var allePersoonsAdressen = new List<PersoonsAdres> {origineelPa, dubbelPa};

            var repositoryProviderMock = new Mock<IRepositoryProvider>();
            repositoryProviderMock.Setup(src => src.RepositoryGet<Persoon>())
                .Returns(new DummyRepo<Persoon>(allePersonen));
            repositoryProviderMock.Setup(src => src.RepositoryGet<PersoonsAdres>())
                .Returns(new DummyRepo<PersoonsAdres>(allePersoonsAdressen));

            Factory.InstantieRegistreren(repositoryProviderMock.Object);

            // ACT
            var target = Factory.Maak<UpdateService_Accessor>();
            target.DubbelVerwijderen(origineel, dubbel);

            // ASSERT
            Assert.AreEqual(1, allePersoonsAdressen.Count);
        }
    }
}
