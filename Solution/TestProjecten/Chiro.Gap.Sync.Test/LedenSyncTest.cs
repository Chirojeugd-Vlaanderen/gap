/*
 * Copyright 2008-2013 the GAP developers. See the NOTICE file at the 
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

using System.Collections.Generic;
using Chiro.Cdf.Ioc;
using Chiro.Gap.Dummies;
using Chiro.Kip.ServiceContracts;
using Chiro.Kip.ServiceContracts.DataContracts;

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Persoon = Chiro.Kip.ServiceContracts.DataContracts.Persoon;

namespace Chiro.Gap.Sync.Test
{
    /// <summary>
    /// Tests voor de ledensync
    /// </summary>
    [TestClass]
    public class LedenSyncTest
    {
        /// <summary>
        /// Gets or sets the test context which provides
        /// information about and functionality for the current test run.
        /// </summary>
        public TestContext TestContext { get; set; }

        [ClassInitialize]
        static public void InitialiseerTests(TestContext tc)
        {
            Factory.ContainerInit();
            MappingHelper.MappingsDefinieren();
        }

        [ClassCleanup]
        static public void AfsluitenTests()
        {
            
        }

        /// <summary>
        /// Test voor syncen van geüpdatete kadertests
        /// </summary>
        [TestMethod]
        public void FunctiesUpdatenKaderTest()
        {
            var data = new DummyData();

            // Verwacht dat de syncservice wordt aangeroepen met functie-id's
            var svcMock = new Mock<ISyncPersoonService>();
            svcMock.Setup(
                svc =>
                svc.FunctiesUpdaten(It.IsAny<Persoon>(),
                                    data.DummyGewest.Code,
                                    data.GwjGewest.WerkJaar,
                                    It.IsAny<List<FunctieEnum>>())).Verifiable();
            Factory.InstantieRegistreren(svcMock.Object);

            // Maak ledensync

            var target = Factory.Maak<LedenSync>();

            target.FunctiesUpdaten(data.KaderJos);

            svcMock.VerifyAll();
        }
    }
}
