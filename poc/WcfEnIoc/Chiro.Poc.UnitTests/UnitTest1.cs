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
﻿using Chiro.Poc.Ioc;
using Chiro.Poc.ServiceGedoe;
using Chiro.Poc.WcfService.ServiceContracts;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Chiro.Poc.UnitTests
{
    /// <summary>
    /// Unit test, die service calls 'wegmockt'.
    /// </summary>
    [TestClass]
    public class UnitTest1
    {
        [TestInitialize]
        public void InitTests()
        {
            Factory.ContainerInit();
        }

        /// <summary>
        /// Deze unit test roept een mock aan in plaats van de echte service.
        /// </summary>
        /// <remarks>
        /// Dit werkt enkel omdat er een andere IServiceClient gebruikt wordt; dit is
        /// gedefinieerd in App.Config
        /// </remarks>
        [TestMethod]
        public void TestMethod1()
        {
            // Arrange: Creeer een mock voor IService1, en configureer de DI-container
            // zodanig dat hij die mock zal gebruiken.
            
            var serviceMock = new Mock<IService1>();
            serviceMock.Setup(m => m.Hallo()).Returns("Antwoord van mock");

            // (Als je hieronder een SynchronisationLockException krijgt bij het debuggen, 
            // doe dan gewoon maar verder.)
            Factory.InstantieRegistreren(serviceMock.Object);

            // Act: laat ServiceHelper een method van IService1 aanroepen

            string result = ServiceHelper.CallService<IService1, string>(svc => svc.Hallo());

            // Assert: We verwachten een antwoord van de mock, niet van de service

            Assert.AreEqual(result, "Antwoord van mock");
        }
    }
}
