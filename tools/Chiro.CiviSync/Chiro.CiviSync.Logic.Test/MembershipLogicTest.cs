﻿/*
   Copyright 2015 Chirojeugd-Vlaanderen vzw

   Licensed under the Apache License, Version 2.0 (the "License");
   you may not use this file except in compliance with the License.
   You may obtain a copy of the License at

       http://www.apache.org/licenses/LICENSE-2.0

   Unless required by applicable law or agreed to in writing, software
   distributed under the License is distributed on an "AS IS" BASIS,
   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
   See the License for the specific language governing permissions and
   limitations under the License.
 */
using System;
using Chiro.Cdf.Ioc;
using Chiro.CiviCrm.Api.DataContracts;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Chiro.CiviSync.Logic.Test
{
    [TestClass]
    public class MembershipLogicTest
    {
        private static readonly DateTime VandaagZogezegd = new DateTime(2015, 04, 20);

        [TestInitialize]
        public void InitializeTest()
        {
            Factory.ContainerInit();
            var datumProviderMock = new Mock<IDatumProvider>();
            datumProviderMock.Setup(src => src.Vandaag()).Returns(VandaagZogezegd);
            Factory.InstantieRegistreren(datumProviderMock.Object);
        }

        /// <summary>
        /// Controleert de begindatum van een membership. (Zie #3417)
        /// </summary>
        [TestMethod]
        public void MembershipVanWerkjaarBeginDatum()
        {
            var target = Factory.Maak<MembershipLogic>();
            var result = target.VanWerkjaar(MembershipType.Aansluiting, 4, 2014);
            Assert.AreEqual(result.StartDate, VandaagZogezegd);
        }

        /// <summary>
        /// Controleert de begindatum van een membership van een werkjaar dat al voorbij is. (Zie #3417)
        /// </summary>
        [TestMethod]
        public void MembershipVanVorigWerkjaarBeginDatum()
        {
            var target = Factory.Maak<MembershipLogic>();
            var result = target.VanWerkjaar(MembershipType.Aansluiting, 4, 2013);
            Assert.AreEqual(result.StartDate, result.EndDate);
        }
    }
}