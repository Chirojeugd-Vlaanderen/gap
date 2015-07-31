/*
 * Copyright 2015 Chirojeugd-Vlaanderen vzw. See the NOTICE file at the 
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
using Chiro.Cdf.Ioc;
using Chiro.Cdf.Ioc.Factory;
using Chiro.Cdf.Poco;
using Chiro.Gap.Dummies;
using Chiro.Gap.Poco.Model;
using Chiro.Gap.SyncInterfaces;
using Chiro.Gap.WorkerInterfaces;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Chiro.Gap.Maintenance.Test
{
    [TestClass]
    public class MembershipMaintenanceTest
    {
        [TestInitialize]
        public void MyTestInitialize()
        {
            // Hercreeeer de IOC-container voor iedere test.
            // Dan hebben we geen mocks meer van vorige tests die mogelijk
            // rare dingen doen.
            Factory.ContainerInit();
        }

        /// <summary>
        /// Als iemand in zijn probeerperiode zit, mag de membershipmaintenance
        /// hem nog niet naar de Civi sturen.
        /// </summary>
        [TestMethod]
        public void LedenInProbeerPeriodeNietSyncen()
        {
            // ARRANGE

            const int huidigWerkjaar = 2014;
            DateTime vandaagZoGezegd = new DateTime(2015, 02, 23);

            // We hebben 1 leidster, die nog in haar probeerperiode zit.
            var leidster = new Leiding
            {
                ID = 1,
                EindeInstapPeriode = vandaagZoGezegd.AddDays(7),
                GelieerdePersoon = new GelieerdePersoon
                {
                    ID = 2,
                    Persoon = new Persoon
                    {
                        ID = 3,
                        VoorNaam = "Kelly",
                        Naam = "Pfaff"
                    }
                },
                GroepsWerkJaar = new GroepsWerkJaar
                {
                    ID = 4,
                    WerkJaar = huidigWerkjaar
                }
            };

            // De repository bevat enkel deze leidster.
            var ledenRepo = new DummyRepo<Lid>(new List<Lid> {leidster});

            // Repositoryprovidermock opzetten
            var repoProviderMock = new Mock<IRepositoryProvider>();
            repoProviderMock.Setup(src => src.RepositoryGet<Lid>()).Returns(ledenRepo);

            // Mock voor personenSync
            var personenSyncMock = new Mock<IPersonenSync>();
            personenSyncMock.Setup(
                src =>
                    src.MembershipRegistreren(It.Is<Lid>(l => l.ID == leidster.ID))).Verifiable();

            // Meer mocks.
            var groepsWerkJaarManagerMock = new Mock<IGroepsWerkJarenManager>();
            groepsWerkJaarManagerMock.Setup(src => src.HuidigWerkJaarNationaal()).Returns(huidigWerkjaar);
            groepsWerkJaarManagerMock.Setup(src => src.Vandaag()).Returns(vandaagZoGezegd);

            // Mocks registeren
            Factory.InstantieRegistreren(repoProviderMock.Object);
            Factory.InstantieRegistreren(personenSyncMock.Object);
            Factory.InstantieRegistreren(groepsWerkJaarManagerMock.Object);

            // ACT

            var target = Factory.Maak<MembershipMaintenance>();
            target.MembershipsMaken();

            // ASSERT

            personenSyncMock.Verify(
                src => src.MembershipRegistreren(It.Is<Lid>(l => l.ID == leidster.ID)), Times.Never);
        }

        /// <summary>
        /// Als de property 'LaatsteMembership' van een persoon <c>null</c> is, mag dat niet
        /// verhinderen dat er memberships worden gemaakt.
        /// </summary>
        [TestMethod]
        public void NooitGesyncteLedenSyncen()
        {
            // ARRANGE

            const int huidigWerkjaar = 2014;
            DateTime vandaagZoGezegd = new DateTime(2015, 02, 23);

            // We hebben 1 leidster, die nog in haar probeerperiode zit.
            var leidster = new Leiding
            {
                ID = 1,
                EindeInstapPeriode = vandaagZoGezegd.AddDays(-7),
                GelieerdePersoon = new GelieerdePersoon
                {
                    ID = 2,
                    Persoon = new Persoon
                    {
                        ID = 3,
                        VoorNaam = "Kelly",
                        Naam = "Pfaff"
                    }
                },
                GroepsWerkJaar = new GroepsWerkJaar
                {
                    ID = 4,
                    WerkJaar = huidigWerkjaar
                }
            };

            // De repository bevat enkel deze leidster.
            var ledenRepo = new DummyRepo<Lid>(new List<Lid> { leidster });

            // Repositoryprovidermock opzetten
            var repoProviderMock = new Mock<IRepositoryProvider>();
            repoProviderMock.Setup(src => src.RepositoryGet<Lid>()).Returns(ledenRepo);

            // Mock voor personenSync
            var personenSyncMock = new Mock<IPersonenSync>();
            personenSyncMock.Setup(
                src =>
                    src.MembershipRegistreren(It.Is<Lid>(l => l.ID == leidster.ID))).Verifiable();

            // Meer mocks.
            var groepsWerkJaarManagerMock = new Mock<IGroepsWerkJarenManager>();
            groepsWerkJaarManagerMock.Setup(src => src.HuidigWerkJaarNationaal()).Returns(huidigWerkjaar);
            groepsWerkJaarManagerMock.Setup(src => src.Vandaag()).Returns(vandaagZoGezegd);

            // Mocks registeren
            Factory.InstantieRegistreren(repoProviderMock.Object);
            Factory.InstantieRegistreren(personenSyncMock.Object);
            Factory.InstantieRegistreren(groepsWerkJaarManagerMock.Object);

            // ACT

            var target = Factory.Maak<MembershipMaintenance>();
            target.MembershipsMaken();

            // ASSERT

            personenSyncMock.Verify(
                src => src.MembershipRegistreren(It.Is<Lid>(l => l.ID == leidster.ID)), Times.AtLeastOnce);
        }

    }
}
