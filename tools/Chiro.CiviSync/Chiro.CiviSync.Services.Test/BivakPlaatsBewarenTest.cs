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
using System.Security;
using AutoMapper;
using Chiro.Cdf.Ioc;
using Chiro.CiviCrm.Api;
using Chiro.CiviCrm.Api.DataContracts;
using Chiro.CiviCrm.Api.DataContracts.Entities;
using Chiro.CiviCrm.Api.DataContracts.Requests;
using Chiro.CiviSync.Helpers;
using Chiro.Kip.ServiceContracts.DataContracts;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Chiro.CiviSync.Services.Test
{
    [TestClass]
    public class BivakPlaatsBewarenTest
    {
        private Mock<ICiviCrmApi> _civiApiMock;
        private Mock<IGapUpdateHelper> _updateHelperMock;

        private readonly DateTime _vandaagZogezegd = new DateTime(2015, 2, 6);
        private const int HuidigWerkJaar = 2014;

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
            TestHelper.IocOpzetten(_vandaagZogezegd, out _civiApiMock, out _updateHelperMock);
        }

        [TestMethod]
        public void BivakPlaatsBewarenNieuwLocBlock()
        {
            // ARRANGE

            // bestaande gegevens
            const int uitstapId = 4;
            var ploeg = new Contact { ExternalIdentifier = "TST/0001", Id = 1, ContactType = ContactType.Organization };
            var bivak = new Event
            {
                Id = 3,
                GapUitstapId = uitstapId,
                ContactResult = Mapper.Map<Contact, ApiResultValues<Contact>>(ploeg),
                LocBlockResult = new ApiResultValues<LocBlock> {Count = 0}
            };
            // nieuwe gegevens
            string plaatsnaam = "D'etalage";
            var adres = new Adres
            {
                Straat = "Kipdorp",
                HuisNr = 24,
                PostNr = 2000,
                WoonPlaats = "Antwerpen"
            };
            // mock
            _civiApiMock.Setup(
                src => src.EventGet(It.IsAny<string>(), It.IsAny<string>(),
                    It.Is<EventRequest>(r => r.GapUitstapId == bivak.GapUitstapId)))
                .Returns(Mapper.Map<Event, ApiResultValues<Event>>(bivak));
            _civiApiMock.Setup(
                src => src.AddressGet(It.IsAny<string>(), It.IsAny<string>(),
                    It.IsAny<AddressRequest>())).Returns(new ApiResultValues<Address> {Count = 0, Values = new Address[0]});
            // controleer of het bewaarde adres het type 'billing' heeft, en of
            // de plaatsnaam mee wordt bewaard.
            _civiApiMock.Setup(
                src => src.LocBlockSave(It.IsAny<string>(), It.IsAny<string>(),
                    It.Is<LocBlockRequest>(r => r.Address.LocationTypeId == 5 && r.Address.Name == plaatsnaam)))
                .Returns(
                    (string k1, string k2, LocBlockRequest r) =>
                        Mapper.Map<LocBlockRequest, ApiResultValues<LocBlock>>(r))
                .Verifiable();

            // ACT

            var service = Factory.Maak<SyncService>();
            service.BivakPlaatsBewaren(uitstapId, plaatsnaam, adres);

            // ASSERT

            _civiApiMock.Verify(
                src => src.LocBlockSave(It.IsAny<string>(), It.IsAny<string>(),
                    It.Is<LocBlockRequest>(r => r.Address.LocationTypeId == 5 && r.Address.Name == plaatsnaam)),
                Times.AtLeastOnce);
        }
    }
}
