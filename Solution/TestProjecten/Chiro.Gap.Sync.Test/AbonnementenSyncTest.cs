/*
 * Copyright 2013-2015, Chirojeugd-Vlaanderen vzw.
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
using Chiro.Gap.Poco.Model;
using Chiro.Kip.ServiceContracts;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Chiro.Gap.Sync.Test
{
    [TestClass]
    public class AbonnementenSyncTest
    {
        [ClassInitialize()]
        public static void MyClassInitialize(TestContext testContext)
        {
            MappingHelper.MappingsDefinieren();
        }

        [TestInitialize()]
        public void MyTestInitialize()
        {
            Factory.ContainerInit();
        }

        /// <summary>
        /// Test op verwijderen van alle abonnementen als de gelieerde persoon
        /// geen e-mailadres heeft.
        /// </summary>
        [TestMethod]
        public void AlleAbonnementenVerwijderenGeenEmailTest()
        {
            // ARRANGE

            var gelieerdePersoon = new GelieerdePersoon {Communicatie = new List<CommunicatieVorm>()};

            // Verwacht dat er een (dummy) e-mailadres meegegeven wordt.
            var kipSyncMock = new Mock<ISyncPersoonService>();
            kipSyncMock.Setup(src => src.AbonnementVerwijderen(It.Is<string>(ml => !string.IsNullOrEmpty(ml))))
                .Verifiable();
            Factory.InstantieRegistreren(kipSyncMock.Object);

            // ACT

            var target = Factory.Maak<AbonnementenSync>();
            target.AlleAbonnementenVerwijderen(gelieerdePersoon);

            // ASSERT

            kipSyncMock.VerifyAll();
        }
    }
}
