﻿/*
   Copyright 2016 Chirojeugd-Vlaanderen vzw

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
    /// <summary>
    /// Tests voor RelationshipLogic.
    /// </summary>
    /// <remarks>
    /// Die dependency injection heb ik hier precies op een erg louche manier
    /// gedaan. Enfin, ik heb het gewoon gekopieerd van de MembershipLogicTest.
    /// </remarks>
    [TestClass]
    public class RelationshipLogicTest
    {
        private static readonly DateTime VandaagZogezegd = new DateTime(2015, 04, 20);
        private static readonly int DummyGroepId = 2;
        private static readonly int DummyPersoonId = 1;

        private void MockDatum(IDiContainer container)
        {
            var datumProviderMock = new Mock<IDatumProvider>();
            datumProviderMock.Setup(src => src.Vandaag()).Returns(VandaagZogezegd);
            container.InstantieRegistreren(datumProviderMock.Object);
        }

        /// <summary>
        /// Test of RequestMaken een null-datum vervangt door <c>DateTime.MinValue</c>
        /// </summary>
        [TestMethod]
        public void EindDatumResetten()
        {
            var factory = new UnityDiContainer();
            MockDatum(factory);

            var target = factory.Maak<RelationshipLogic>();

            var result = target.RequestMaken(RelatieType.LidVan, DummyPersoonId, DummyGroepId, null);
            Assert.AreEqual(DateTime.MinValue, result.EndDate);
        }
    }
}