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

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Chiro.CiviSync.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Chiro.Cdf.Ioc;
using Chiro.CiviCrm.Api;
using Chiro.CiviCrm.Api.DataContracts;
using Chiro.CiviCrm.Api.DataContracts.Entities;
using Chiro.CiviCrm.Api.DataContracts.Requests;
using Chiro.CiviSync.Mapping;
using Chiro.CiviSync.Services.Test;
using Chiro.Gap.Log;
using Chiro.Gap.UpdateApi.Client;
using Chiro.Kip.ServiceContracts.DataContracts;
using Moq;

namespace Chiro.CiviSync.Services.Tests
{
    [TestClass()]
    public class SyncServiceTests
    {
        [ClassInitialize]
        public static void InitialilzeTestClass(TestContext c)
        {
            // creer mappings voor de service
            MappingHelper.MappingsDefinieren();
            // creer mappings voor de tests
            TestHelper.MappingsCreeren();
        }

        [TestMethod()]
        public void CommunicatieToevoegenVoorkeurTest()
        {
            //ARRANGE
            Mock<ICiviCrmApi> civiApiMock;
            Mock<IGapUpdateClient> updateHelperMock;
            IDiContainer factory;
            TestHelper.IocOpzetten(new DateTime(2015, 02, 05), out factory, out civiApiMock, out updateHelperMock);

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
                    src.GenericCall(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CiviEntity>(), ApiAction.Get,
                        It.IsAny<BaseRequest>()))
                .Returns(new ApiResult());
            // Verwacht dat er een e-mailadres wordt bewaard met 'IsBulkMail'.
            civiApiMock.Setup(
                src =>
                    src.GenericCall(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CiviEntity>(), ApiAction.Create,
                        It.Is<BaseRequest>(r => r is EmailRequest && ((EmailRequest) r).IsBulkMail.Value)))
                .Returns(new ApiResult()).Verifiable();


            var service = factory.Maak<SyncService>();

            // ACT
            service.CommunicatieToevoegen(persoon, communicatieMiddel);

            // ASSERT
            civiApiMock.Verify(src =>
                src.GenericCall(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CiviEntity>(), ApiAction.Create,
                    It.Is<BaseRequest>(r => r is EmailRequest && ((EmailRequest) r).IsBulkMail.Value)),
                Times.AtLeastOnce);
        }
    }
}