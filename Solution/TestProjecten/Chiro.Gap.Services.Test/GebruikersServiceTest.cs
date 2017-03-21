/*
 * Copyright 2015 Chirojeugd-Vlaanderen vzw. See the NOTICE file at the 
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
using Chiro.Ad.ServiceContracts;
using Chiro.Cdf.Poco;
using Chiro.Cdf.ServiceHelper;
using Chiro.Gap.Domain;
using Chiro.Gap.Dummies;
using Chiro.Gap.Poco.Model;
using Chiro.Gap.Test;
using Chiro.Gap.WorkerInterfaces;
using NUnit.Framework;
using Moq;

namespace Chiro.Gap.Services.Test
{
    
    
    /// <summary>
    ///This is a test class for GebruikersServiceTest and is intended
    ///to contain all GebruikersServiceTest Unit Tests
    ///</summary>
    [TestFixture]
    public class GebruikersServiceTest: ChiroTest
    {
        public GebruikersServiceTest(): base()
        {
        }

        [SetUp]
        public void Setup()
        {
            PermissionHelper.FixPermissions();
            Factory.TypeRegistreren<IAutorisatieManager>(typeof(AutMgrAltijdGav));
        }

        ///<summary>
        ///Test voor gebruiker zonder rechten maken.
        ///</summary>
        [Test]
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
        [Test]
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
            repositoryProviderMock.Setup(src => src.RepositoryGet<Persoon>())
                .Returns(new DummyRepo<Persoon>(new List<Persoon> { gelieerdePersoon.Persoon }));
            repositoryProviderMock.Setup(src => src.RepositoryGet<GebruikersRechtV2>())
                .Returns(new DummyRepo<GebruikersRechtV2>(new List<GebruikersRechtV2> { gr }));
            Factory.InstantieRegistreren(repositoryProviderMock.Object);

            // ACT

            var target = Factory.Maak<GebruikersService>();
            target.RechtenAfnemen(gelieerdePersoon.Persoon.ID, new[] {gelieerdePersoon.Groep.ID});

            // ASSERT

            Assert.IsTrue(gr.VervalDatum <= DateTime.Now);
        }

        /// <summary>
        ///A test for RechtenAfnemen
        ///</summary>
        [Test]
        public void RechtenAfnemenPermissiesTest()
        {
            // ARRANGE

            var gr = new GebruikersRechtV2();
            var gelieerdePersoon = new GelieerdePersoon
            {
                ID = 1,
                Groep = new ChiroGroep { ID = 3 },
                Persoon = new Persoon { ID = 2, GebruikersRechtV2 = new List<GebruikersRechtV2> { gr } }
            };
            gelieerdePersoon.Persoon.GelieerdePersoon.Add(gelieerdePersoon);

            gr.Groep = gelieerdePersoon.Groep;
            gr.Persoon = gelieerdePersoon.Persoon;
            gelieerdePersoon.Persoon.GebruikersRechtV2.Add(gr);

            var repositoryProviderMock = new Mock<IRepositoryProvider>();
            repositoryProviderMock.Setup(src => src.RepositoryGet<Persoon>())
                .Returns(new DummyRepo<Persoon>(new List<Persoon> { gelieerdePersoon.Persoon }));
            repositoryProviderMock.Setup(src => src.RepositoryGet<GebruikersRechtV2>())
                .Returns(new DummyRepo<GebruikersRechtV2>(new List<GebruikersRechtV2> { gr }));

            Factory.InstantieRegistreren(repositoryProviderMock.Object);

            // ACT

            var target = Factory.Maak<GebruikersService>();
            target.RechtenAfnemen(gelieerdePersoon.Persoon.ID, new[] { gelieerdePersoon.Groep.ID });

            // ASSERT

            Assert.IsTrue(gr.VervalDatum <= DateTime.Now);
        }
    }
}
