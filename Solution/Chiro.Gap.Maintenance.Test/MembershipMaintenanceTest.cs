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
using Chiro.Cdf.Poco;
using Chiro.Gap.Dummies;
using Chiro.Gap.Poco.Model;
using Chiro.Gap.SyncInterfaces;
using Chiro.Gap.WorkerInterfaces;
using Chiro.Gap.Workers;
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

            // Gebruik de echte groepswerkjarenmanager en ledenmanager.
            var gwjm = new GroepsWerkJarenManager(new VeelGebruikt());
            Factory.InstantieRegistreren<IGroepsWerkJarenManager>(gwjm);
            var lm = new LedenManager();
            Factory.InstantieRegistreren<ILedenManager>(lm);


            // We hebben 1 leidster, die nog in haar probeerperiode zit.
            var leidster = new Leiding
            {
                ID = 1,
                EindeInstapPeriode = DateTime.Now.AddDays(7),
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
                    WerkJaar = gwjm.HuidigWerkJaarNationaal()
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
                    src.MembershipRegistreren(It.Is<Persoon>(p => p.ID == leidster.GelieerdePersoon.Persoon.ID),
                        It.IsAny<int>())).Verifiable();

            // Mocks registeren
            Factory.InstantieRegistreren(repoProviderMock.Object);
            Factory.InstantieRegistreren(personenSyncMock.Object);

            // ACT

            var target = Factory.Maak<MembershipMaintenance>();
            target.MembershipsMaken();

            // ASSERT

            personenSyncMock.Verify(
                src => src.MembershipRegistreren(It.Is<Persoon>(p => p.ID == leidster.GelieerdePersoon.Persoon.ID),
                    It.IsAny<int>()), Times.Never);
        }
    }
}
