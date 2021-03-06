﻿/*
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
using Chiro.CiviCrm.Api;
using Chiro.CiviCrm.Api.DataContracts;
using Chiro.CiviCrm.Api.DataContracts.Entities;
using Chiro.CiviCrm.Api.DataContracts.Requests;
using Chiro.Gap.UpdateApi.Client;
using Chiro.Kip.ServiceContracts.DataContracts;
using Moq;
using NUnit.Framework;

namespace Chiro.CiviSync.Services.Test
{
    [TestFixture]
    public class GroepUpdatenTest: SyncTest
    {
        /// <summary>
        /// Als er een nieuw groepsadres doorkomt, dan moet het oude worden vervangen.
        /// </summary>
        [Test]
        public void GroepsAdresVervangen()
        {
            // Dat instantieren van de DI-container in de test zelf, is waarschijnlijk
            // niet de way to go...
            Mock<ICiviCrmApi> civiApiMock;
            Mock<IGapUpdateClient> updateHelperMock;
            using (var factory = TestHelper.IocOpzetten(new DateTime(2015, 02, 05), out civiApiMock,
                out updateHelperMock))
            {

                // Dummy-situatietje in Civi.
                var address = new Address {Id = 2};
                var civiGroep = new Contact
                {
                    Id = 1,
                    ExternalIdentifier = "tst/0000",
                    AddressResult = new ApiResultValues<Address>(address)
                };

                civiApiMock.Setup(
                        src =>
                            src.ContactGet(It.IsAny<string>(), It.IsAny<string>(),
                                It.Is<ContactRequest>(r => r.ExternalIdentifier == civiGroep.ExternalIdentifier)))
                    .Returns(new ApiResultValues<Contact>(civiGroep));

                civiApiMock.Setup(
                        src =>
                            src.ContactSave(It.IsAny<string>(), It.IsAny<string>(),
                                It.IsAny<ContactRequest>()))
                    .Returns(
                        (string key1, string key2, ContactRequest request) =>
                            TestHelper.Map<ContactRequest, ApiResultValues<Contact>>(request));

                // Verwacht dat de bestaande adressen van de groep worden verwijderd.
                civiApiMock.Setup(
                        src =>
                            src.AddressDelete(It.IsAny<string>(), It.IsAny<string>(),
                                It.Is<DeleteRequest>(r => r.Id == address.Id)))
                    .Verifiable();

                // Maak service-mock, en probeer groepsgegevens te vervangen.
                var service = factory.Maak<SyncService>();

                var gapGroep = new Groep
                {
                    Code = civiGroep.ExternalIdentifier,
                    Adres = new Adres {PostNr = "2000"}
                };
                service.GroepUpdaten(gapGroep);

                // Werd het bestaande adres verwijderd?
                civiApiMock.Verify(src =>
                        src.AddressDelete(It.IsAny<string>(), It.IsAny<string>(),
                            It.Is<DeleteRequest>(r => r.Id == address.Id)),
                    Times.AtLeastOnce);
            }
        }
    }
}
