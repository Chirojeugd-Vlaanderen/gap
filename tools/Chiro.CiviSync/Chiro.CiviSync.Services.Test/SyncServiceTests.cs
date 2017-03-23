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
using Chiro.Cdf.Ioc;
using Chiro.CiviCrm.Api;
using Chiro.CiviCrm.Api.DataContracts;
using Chiro.CiviCrm.Api.DataContracts.Entities;
using Chiro.CiviCrm.Api.DataContracts.Requests;
using Chiro.CiviSync.Services.Test;
using Chiro.Gap.UpdateApi.Client;
using Chiro.Kip.ServiceContracts.DataContracts;
using Moq;
using NUnit.Framework;

namespace Chiro.CiviSync.Services.Tests
{
    [TestFixture]
    public class SyncServiceTests: SyncTest
    {
        [Test]
        public void CommunicatieToevoegenVoorkeurTest()
        {
            //ARRANGE
            Mock<ICiviCrmApi> civiApiMock;
            Mock<IGapUpdateClient> updateHelperMock;
            using (var factory = TestHelper.IocOpzetten(new DateTime(2015, 02, 05), out civiApiMock,
                out updateHelperMock))
            {

                var persoon = new Persoon {AdNummer = 2};
                var contact = new Contact {ExternalIdentifier = persoon.AdNummer.ToString(), Id = 3};
                var communicatieMiddel = new CommunicatieMiddel {IsBulk = true, Type = CommunicatieType.Email};

                // ContactGet levert altijd hetzelfde contact op, for testing purposes.
                civiApiMock.Setup(
                        src =>
                            src.ContactGet(It.IsAny<string>(), It.IsAny<string>(),
                                It.IsAny<ContactRequest>()))
                    .Returns(new ApiResultValues<Contact>(contact));
                // Communicatie opvragen doet eigeniljk niets, for testing purposes.
                civiApiMock.Setup(
                        src =>
                            src.GenericCall(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CiviEntity>(),
                                ApiAction.Get,
                                It.IsAny<BaseRequest>()))
                    .Returns(new ApiResult());
                // Verwacht dat er een e-mailadres wordt bewaard met 'IsBulkMail'.
                civiApiMock.Setup(
                        src =>
                            src.GenericCall(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CiviEntity>(),
                                ApiAction.Create,
                                It.Is<BaseRequest>(r => r is EmailRequest && ((EmailRequest) r).IsBulkMail.Value)))
                    .Returns(new ApiResult())
                    .Verifiable();


                var service = factory.Maak<SyncService>();

                // ACT
                service.CommunicatieToevoegen(persoon, communicatieMiddel);

                // ASSERT
                civiApiMock.Verify(src =>
                        src.GenericCall(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CiviEntity>(),
                            ApiAction.Create,
                            It.Is<BaseRequest>(r => r is EmailRequest && ((EmailRequest) r).IsBulkMail.Value)),
                    Times.AtLeastOnce);
            }
        }
    }
}