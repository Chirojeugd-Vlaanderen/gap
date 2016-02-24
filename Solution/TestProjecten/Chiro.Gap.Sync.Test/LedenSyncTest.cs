/*
 * Copyright 2010-2016, Chirojeugd-Vlaanderen vzw.
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
using Chiro.Cdf.Ioc.Factory;
using Chiro.Gap.Domain;
using Chiro.Gap.Poco.Model;
using Chiro.Kip.ServiceContracts;
using Chiro.Kip.ServiceContracts.DataContracts;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Persoon = Chiro.Kip.ServiceContracts.DataContracts.Persoon;

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
            kipSyncMock.Setup(src => src.AfdelingenUpdaten(It.IsAny<Persoon>(), It.IsAny<string>(), It.IsAny<int>(), It.IsAny<AfdelingEnum[]>())).Verifiable();
            Factory.InstantieRegistreren(kipSyncMock.Object);

            var lid = new Kind
            {
                AfdelingsJaar = new AfdelingsJaar {OfficieleAfdeling = new OfficieleAfdeling {ID = 1}},
                GroepsWerkJaar = new GroepsWerkJaar {Groep = new ChiroGroep()},
                GelieerdePersoon = new GelieerdePersoon {Persoon = new Poco.Model.Persoon {InSync = true}}
            };

            // ACT

            var target = Factory.Maak<LedenSync>();
            target.AfdelingenUpdaten(lid);

            // ASSERT

            kipSyncMock.VerifyAll();
        }

        /// <summary>
        /// Bewaren moet bewaren, en niet verwijderen. Als het lid inactief is, moet de stopdatum naar Civi.
        ///</summary>
        [TestMethod()]
        public void BewarenNietGebruikenOmTeVerwijderenTest()
        {
            // ARRANGE

            DateTime uitschrijfDatum = DateTime.Now;
            var kipSyncMock = new Mock<ISyncPersoonService>();
            kipSyncMock.Setup(src => src.LidVerwijderen(It.IsAny<int>(), It.IsAny<string>(), It.IsAny<int>(), It.IsAny<DateTime>())).Verifiable();
            // Die assert staat hieronder op een gekke plaats.
            kipSyncMock.Setup(
                src => src.LidBewaren(It.IsAny<int>(), It.Is<LidGedoe>(lg => lg.UitschrijfDatum == uitschrijfDatum)))
                .Verifiable();
            Factory.InstantieRegistreren(kipSyncMock.Object);

            var lid = new Kind
            {
                AfdelingsJaar = new AfdelingsJaar { OfficieleAfdeling = new OfficieleAfdeling { ID = 1 } },
                GroepsWerkJaar = new GroepsWerkJaar { Groep = new ChiroGroep() },
                GelieerdePersoon = new GelieerdePersoon { Persoon = new Poco.Model.Persoon {AdNummer = 2} },
                NonActief = true,
                UitschrijfDatum = uitschrijfDatum,
                EindeInstapPeriode = DateTime.Now.AddMonths(-6)
            };

            // ACT

            var target = Factory.Maak<LedenSync>();
            target.Bewaren(lid);

            // ASSERT

            kipSyncMock.Verify(
                src => src.LidVerwijderen(It.IsAny<int>(), It.IsAny<string>(), It.IsAny<int>(), It.IsAny<DateTime>()),
                Times.Never());
            kipSyncMock.Verify(
                src => src.LidBewaren(It.IsAny<int>(), It.Is<LidGedoe>(lg => lg.UitschrijfDatum == uitschrijfDatum)),
                Times.AtLeastOnce());
        }

        /// <summary>
        /// Test voor bewaren persoon met geslacht x.
        ///</summary>
        [TestMethod()]
        public void BewarenPersoonGeslachtXTest()
        {
            // ARRANGE

            var kipSyncMock = new Mock<ISyncPersoonService>();
            kipSyncMock.Setup(src => src.NieuwLidBewaren(It.IsAny<PersoonDetails>(), It.IsAny<LidGedoe>())).Verifiable();
            Factory.InstantieRegistreren(kipSyncMock.Object);

            var lid = new Kind
            {
                AfdelingsJaar = new AfdelingsJaar {OfficieleAfdeling = new OfficieleAfdeling {ID = 1}},
                GroepsWerkJaar = new GroepsWerkJaar {Groep = new ChiroGroep()},
                GelieerdePersoon =
                    new GelieerdePersoon
                    {
                        Persoon = new Poco.Model.Persoon {ID = 2, Geslacht = GeslachtsType.X, InSync = true}
                    },
                NonActief = false,
                UitschrijfDatum = null,
                EindeInstapPeriode = DateTime.Now.AddMonths(-6)
            };

            // ACT

            var target = Factory.Maak<LedenSync>();
            target.Bewaren(lid);

            // ASSERT

            kipSyncMock.Verify(src => src.NieuwLidBewaren(It.IsAny<PersoonDetails>(), It.IsAny<LidGedoe>()),
                Times.AtLeastOnce);
        }
    }
}
