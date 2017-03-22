/*
   Copyright 2015,2016 Chirojeugd-Vlaanderen vzw

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
using Chiro.CiviCrm.Api;
using Chiro.CiviCrm.Api.DataContracts;
using Chiro.CiviCrm.Api.DataContracts.Entities;
using Chiro.CiviCrm.Api.DataContracts.Requests;
using Chiro.CiviSync.Mapping;
using Chiro.CiviSync.Test.Mapping;
using Chiro.Gap.UpdateApi.Client;
using Chiro.Kip.ServiceContracts.DataContracts;
using Moq;
using NUnit.Framework;

namespace Chiro.CiviSync.Services.Test
{
    [TestFixture]
    public class BivakPlaatsBewarenTest: SyncTest
    {
        private readonly DateTime _vandaagZogezegd = new DateTime(2015, 2, 6);
        private const int HuidigWerkJaar = 2014;

        [Test]
        public void BivakPlaatsBewarenNieuwLocBlock()
        {
            // ARRANGE

            Mock<ICiviCrmApi> civiApiMock;
            Mock<IGapUpdateClient> updateHelperMock;
            using (var factory = TestHelper.IocOpzetten(_vandaagZogezegd, out civiApiMock, out updateHelperMock))
            {

                // bestaande gegevens
                const int uitstapId = 4;
                var ploeg = new Contact
                {
                    ExternalIdentifier = "TST/0001",
                    Id = 1,
                    ContactType = ContactType.Organization
                };
                var bivak = new Event
                {
                    Id = 3,
                    GapUitstapId = uitstapId,
                    ContactResult = TestHelper.Map<Contact, ApiResultValues<Contact>>(ploeg),
                    LocBlockResult = new ApiResultValues<LocBlock> {Count = 0}
                };
                // nieuwe gegevens
                string plaatsnaam = "D'etalage";
                var adres = new Adres
                {
                    Straat = "Kipdorp",
                    HuisNr = 24,
                    PostNr = "2000",
                    WoonPlaats = "Antwerpen"
                };
                // mock
                civiApiMock.Setup(
                        src => src.EventGet(It.IsAny<string>(), It.IsAny<string>(),
                            It.Is<EventRequest>(r => r.GapUitstapId == bivak.GapUitstapId)))
                    .Returns(TestHelper.Map<Event, ApiResultValues<Event>>(bivak));
                civiApiMock.Setup(
                        src => src.AddressGet(It.IsAny<string>(), It.IsAny<string>(),
                            It.IsAny<AddressRequest>()))
                    .Returns(new ApiResultValues<Address> {Count = 0, Values = new Address[0]});
                // controleer of het bewaarde adres het type 'billing' heeft, en of
                // de plaatsnaam mee wordt bewaard.
                civiApiMock.Setup(
                        src => src.LocBlockSave(It.IsAny<string>(), It.IsAny<string>(),
                            It.Is<LocBlockRequest>(r => r.Address.LocationTypeId == 5 && r.Address.Name == plaatsnaam)))
                    .Returns(
                        (string k1, string k2, LocBlockRequest r) =>
                            TestHelper.Map<LocBlockRequest, ApiResultValues<LocBlock>>(r))
                    .Verifiable();

                // ACT

                var service = factory.Maak<SyncService>();
                service.BivakPlaatsBewaren(uitstapId, plaatsnaam, adres);

                // ASSERT

                civiApiMock.Verify(
                    src => src.LocBlockSave(It.IsAny<string>(), It.IsAny<string>(),
                        It.Is<LocBlockRequest>(r => r.Address.LocationTypeId == 5 && r.Address.Name == plaatsnaam)),
                    Times.AtLeastOnce);
            }
        }
    }
}
