/*
 * Copyright 2008-2014 the GAP developers. See the NOTICE file at the 
 * top-level directory of this distribution, and at
 * https://develop.chiro.be/gap/wiki/copyright
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
using System.Linq;
using Chiro.Cdf.Ioc;
using Chiro.Cdf.Poco;
using Chiro.Gap.Domain;
using Chiro.Gap.Dummies;
using Chiro.Gap.Poco.Model;
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

        /// <summary>
        ///A test for RechtenAfnemen
        ///</summary>
        [TestMethod()]
        public void RechtenAfnemenTest()
        {
            // ARRANGE

            var gav = new Gav();
            var gelieerdePersoon = new GelieerdePersoon
                                   {
                                       ID = 1,
                                       Groep = new ChiroGroep {ID = 3},
                                       Persoon = new Persoon {ID = 2, Gav = new List<Gav> {gav}}
                                   };
            gelieerdePersoon.Persoon.GelieerdePersoon.Add(gelieerdePersoon);
            gav.Persoon.Add(gelieerdePersoon.Persoon);
            var gebruikersrecht = new GebruikersRecht {Gav = gav, Groep = gelieerdePersoon.Groep};
            gav.GebruikersRecht.Add(gebruikersrecht);

            var repositoryProviderMock = new Mock<IRepositoryProvider>();
            repositoryProviderMock.Setup(src => src.RepositoryGet<GelieerdePersoon>())
                .Returns(new DummyRepo<GelieerdePersoon>(new List<GelieerdePersoon> {gelieerdePersoon}));
            repositoryProviderMock.Setup(src => src.RepositoryGet<Gav>())
                .Returns(new DummyRepo<Gav>(new List<Gav> {gav}));
            Factory.InstantieRegistreren(repositoryProviderMock.Object);

            // ACT

            var target = Factory.Maak<GebruikersService>();
            target.RechtenAfnemen(gelieerdePersoon.ID, new[] {gelieerdePersoon.Groep.ID});

            // ASSERT

            Assert.IsTrue(gebruikersrecht.VervalDatum <= DateTime.Now);
            
        }
    }
}
