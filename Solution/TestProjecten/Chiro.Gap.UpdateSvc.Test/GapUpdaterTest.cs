/*
 * Copyright 2014, 2016 Chirojeugd-Vlaanderen vzw. See the NOTICE file at the 
 * top-level directory of this distribution, and at
 * https://gapwiki.chiro.be/copyright
 * 
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 * 
 *     http://www.apache.org/licenses/LICENSE-2.0
 * 
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

using System;
using System.Collections.Generic;
using Chiro.Cdf.Ioc.Factory;
using Chiro.Cdf.Poco;
using Chiro.Gap.Dummies;
using Chiro.Gap.Poco.Model;
using Chiro.Gap.UpdateApi.Workers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Chiro.Gap.Poco.Model.Exceptions;
using Chiro.Gap.UpdateApi.Models;

namespace Chiro.Gap.UpdateSvc.Test
{


    /// <summary>
    ///This is a test class for UpdateServiceTest and is intended
    ///to contain all UpdateServiceTest Unit Tests
    ///</summary>
    [TestClass()]
    public class GapUpdaterTest
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
        /// AansluitingBijwerken mag niet crashen als het lid niet wordt gevonden. #4526.
        /// </summary>
        [TestMethod()]
        public void AansluitingBijwerkenOnbekendLid()
        {
            // In principe kun je discussiëren over het geval dat de groep en de persoon wel
            // bestaan, maar het lid van het membership niet. Maar deze situatie doet zich
            // normaal gezien niet voor.
            //
            // De aansluitingsinformatie is voor het GAP enkel relevant om te weten of het
            // een membership moet aanmaken voor een lid. Voor onbestaande leden is het dus
            // niet erg dat die informatie verloren gaat. Te meer omdat CiviSync vermijdt dat
            // iemand dubbel aangesloten wordt, als het toch ergens mis gaat.

            // ARRANGE

            var model = new AansluitingModel
            {
                AdNummer = 1,
                StamNummer = "BLA/0000",
                RecentsteWerkJaar = 2015
            };

            // Dummy ledenrepository maken, die zeker geen leden vindt.
            var repositoryProviderMock = new Mock<IRepositoryProvider>();
            repositoryProviderMock.Setup(src => src.RepositoryGet<Lid>())
                                  .Returns(new DummyRepo<Lid>(new List<Lid>()));
            Factory.InstantieRegistreren(repositoryProviderMock.Object);

            // ACT

            var target = Factory.Maak<GapUpdater>();
            target.Bijwerken(model);

            // ASSERT

            // We zijn blij dat we hier raken zonder exceptions.
            Assert.IsTrue(true);
        }

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

            var target = Factory.Maak<GapUpdater>();
            target.GroepDesactiveren(groep.Code, DateTime.Now);

            // ASSERT

            Assert.IsNotNull(groep.StopDatum);
        }

        /// <summary>
        /// A test for GroepDesactiveren.
        /// 
        /// Verwacht een FoutNummerException als de groep niet wordt gevonden.
        /// </summary>
        [TestMethod()]
        [ExpectedException(typeof(FoutNummerException))]
        public void OnbestaandeGroepDesactiverenTest()
        {
            // ARRANGE

            var repositoryProviderMock = new Mock<IRepositoryProvider>();
            repositoryProviderMock.Setup(src => src.RepositoryGet<Groep>())
                                  .Returns(new DummyRepo<Groep>(new List<Groep>()));
            Factory.InstantieRegistreren(repositoryProviderMock.Object);

            // ACT

            var target = Factory.Maak<GapUpdater>();
            target.GroepDesactiveren("EENDERWAT", DateTime.Now);
        }

        /// <summary>
        ///A test for DubbelVerwijderen. Is de dubbele inderdaad weg?
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
            var target = Factory.Maak<GapUpdater>();
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
            var target = Factory.Maak<GapUpdater>();
            target.DubbelVerwijderen(origineel, dubbel);

            // ASSERT
            Assert.AreEqual(1, allePersoonsAdressen.Count);
        }

        /// <summary>
        /// A test for DubbelVerwijderen. Concrete situatie dat dubbel en origineel lid zijn, maar origineel lid
        /// is inactief.
        ///</summary>
        [TestMethod()]
        public void DubbelVerwijderenTestOrigineelLidInactief()
        {
            // ARRANGE

            var groep = new ChiroGroep();
            var groepsWerkJaar = new GroepsWerkJaar {Groep = groep};

            var origineel = new Persoon();
            var dubbel = new Persoon();

            var origineleGp = new GelieerdePersoon {Persoon = origineel, Groep = groep};
            var dubbeleGp = new GelieerdePersoon {Persoon = dubbel, Groep = groep};
            origineel.GelieerdePersoon.Add(origineleGp);
            dubbel.GelieerdePersoon.Add(dubbeleGp);
            groep.GelieerdePersoon.Add(origineleGp);
            groep.GelieerdePersoon.Add(dubbeleGp);

            var origineelLid = new Leiding
                               {
                                   GelieerdePersoon = origineleGp,
                                   GroepsWerkJaar = groepsWerkJaar,
                                   UitschrijfDatum = DateTime.Today.AddDays(-1),
                                   NonActief = true,
                                   ID = 1
                               };
            var dubbelLid = new Leiding
                            {
                                GelieerdePersoon = dubbeleGp,
                                GroepsWerkJaar = groepsWerkJaar,
                                UitschrijfDatum = null,
                                NonActief = false,
                                ID = 2
                            };
            origineleGp.Lid.Add(origineelLid);
            dubbeleGp.Lid.Add(dubbelLid);
            groepsWerkJaar.Lid.Add(origineelLid);
            groepsWerkJaar.Lid.Add(dubbelLid);

            var allePersonen = new List<Persoon> {origineel, dubbel};

            var repositoryProviderMock = new Mock<IRepositoryProvider>();
            repositoryProviderMock.Setup(src => src.RepositoryGet<Persoon>())
                .Returns(new DummyRepo<Persoon>(allePersonen));
            repositoryProviderMock.Setup(src => src.RepositoryGet<Lid>())
                .Returns(new DummyRepo<Lid>(new List<Lid> {origineelLid, dubbelLid}));
            repositoryProviderMock.Setup(src => src.RepositoryGet<GelieerdePersoon>())
                .Returns(new DummyRepo<GelieerdePersoon>(new List<GelieerdePersoon> {origineleGp, dubbeleGp}));

            Factory.InstantieRegistreren(repositoryProviderMock.Object);

            // ACT
            var target = Factory.Maak<GapUpdater>();
            target.DubbelVerwijderen(origineel, dubbel);

            // ASSERT
            Assert.IsTrue(origineleGp.Lid.Contains(dubbelLid));
            Assert.IsFalse(groepsWerkJaar.Lid.Contains(origineelLid));
        }
    }
}
