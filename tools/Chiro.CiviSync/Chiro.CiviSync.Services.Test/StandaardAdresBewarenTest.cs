/*
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
using Chiro.Cdf.Ioc.Factory;
using Chiro.CiviCrm.Api;
using Chiro.CiviCrm.Api.DataContracts;
using Chiro.CiviCrm.Api.DataContracts.Entities;
using Chiro.CiviCrm.Api.DataContracts.Requests;
using Chiro.CiviSync.Mapping;
using Chiro.Gap.Log;
using Chiro.Gap.UpdateApi.Client;
using Chiro.Kip.ServiceContracts.DataContracts;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Chiro.CiviSync.Services.Test
{
    [TestClass]
    public class StandaardAdresBewarenTest
    {
        private Mock<ICiviCrmApi> _civiApiMock;
        private Mock<IGapUpdateClient> _updateHelperMock;

        [ClassInitialize]
        public static void InitialilzeTestClass(TestContext c)
        {
            // creer mappings voor de service
            MappingHelper.MappingsDefinieren();
            // creer mappings voor de tests
            TestHelper.MappingsCreeren();
        }

        [TestInitialize]
        public void InitializeTest()
        {
            TestHelper.IocOpzetten(new DateTime(2015, 02, 05), out _civiApiMock, out _updateHelperMock);
        }

        /// <summary>
        /// Adres bewaren van onbekend persoon moet een error loggen. (#3691)
        /// </summary>
        [TestMethod]
        public void StandaardAdresBewarenOnbekend()
        {
            // ARRANGE

            // De API levert niets op:
            _civiApiMock.Setup(
                src => src.ContactGetSingle(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<ContactRequest>()))
                .Returns(new Contact());
            _civiApiMock.Setup(
                src =>
                    src.ContactGet(It.IsAny<string>(), It.IsAny<string>(),
                        It.IsAny<ContactRequest>()))
                .Returns(new ApiResultValues<Contact>());

            var logger = new Mock<IMiniLog>();
            logger.Setup(
                src =>
                    src.Loggen(Niveau.Error, It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int?>(), It.IsAny<int?>()))
                .Verifiable();
            Factory.InstantieRegistreren(logger.Object);

            var service = Factory.Maak<SyncService>();

            // ACT

            service.StandaardAdresBewaren(
                new Adres {Straat = "Kipdorp", HuisNr = 30, PostNr = 2000, WoonPlaats = "Antwerpen"}, new Bewoner[]
                {
                    new Bewoner
                    {
                        AdresType = AdresTypeEnum.Werk,
                        Persoon = new Persoon
                        {
                            AdNummer = null,
                            ID = 1,
                            GeboorteDatum = new DateTime(1977, 03, 08)
                        }
                    }
                });

            // ASSERT

            logger.Verify(
                src =>
                    src.Loggen(Niveau.Error, It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int?>(), It.IsAny<int?>()),
                Times.AtLeastOnce());
        }

        /// <summary>
        /// Adres bewaren van persoon met onbekend AD-nummer moet AD-nummer terugsturen naar GAP. (#3688)
        /// </summary>
        [TestMethod]
        public void StandaardAdresBewarenFoutAd()
        {
            // ARRANGE

            // Zo gezegd ongeldig AD-nummer
            const int adNummer = 1;

            // De API levert niets op:
            _civiApiMock.Setup(
                src => src.ContactGetSingle(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<ContactRequest>()))
                .Returns(new Contact());
            _civiApiMock.Setup(
                src =>
                    src.ContactGet(It.IsAny<string>(), It.IsAny<string>(),
                        It.IsAny<ContactRequest>()))
                .Returns(new ApiResultValues<Contact>());

            // verwacht aanroep van GapUpdateClient
            _updateHelperMock.Setup(src => src.OngeldigAdNaarGap(It.Is<Int32>(ad => ad == adNummer))).Verifiable();


            var service = Factory.Maak<SyncService>();

            // ACT

            service.StandaardAdresBewaren(
                new Adres { Straat = "Kipdorp", HuisNr = 30, PostNr = 2000, WoonPlaats = "Antwerpen" }, new Bewoner[]
                {
                    new Bewoner
                    {
                        AdresType = AdresTypeEnum.Werk,
                        Persoon = new Persoon
                        {
                            AdNummer = adNummer,
                            ID = 1,
                            GeboorteDatum = new DateTime(1977, 03, 08)
                        }
                    }
                });

            // ASSERT

            _updateHelperMock.Verify(src => src.OngeldigAdNaarGap(It.Is<Int32>(ad => ad == adNummer)), Times.AtLeastOnce);
        }
    }
}
