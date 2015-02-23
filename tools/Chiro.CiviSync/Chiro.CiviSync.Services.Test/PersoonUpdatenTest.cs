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
    public class PersoonUpdatenTest
    {
        private Mock<ICiviCrmApi> _civiApiMock;
        private Mock<IGapUpdateHelper> _updateHelperMock;

        [ClassInitialize]
        public static void InitializeTestClass(TestContext c)
        {
            // creer mappings voor de service
            MappingHelper.MappingsDefinieren();
            // creer mappings voor de tests
            TestHelper.MappingsCreeren();
        }

        [TestInitialize]
        public void InitializeTest()
        {
            TestHelper.IocOpzetten(DateTime.Now, out _civiApiMock, out _updateHelperMock);
        }

        [TestMethod]
        public void PersoonUpdaten()
        {
            // ARRANGE

            const int adNummer = 2;
            var persoon = new Contact
            {
                ExternalIdentifier = adNummer.ToString(),
                FirstName = "Kees",
                LastName = "Flodder",
                GapId = 3
            };

            _civiApiMock.Setup(
                src => src.ContactGetSingle(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<ContactRequest>()))
                .Returns(persoon);
            _civiApiMock.Setup(
                src =>
                    src.ContactSave(It.IsAny<string>(), It.IsAny<string>(),
                        It.Is<ContactRequest>(r => r.ExternalIdentifier == persoon.ExternalIdentifier)))
                .Returns(
                    (string key1, string key2, ContactRequest request) =>
                        Mapper.Map<ContactRequest, ApiResultValues<Contact>>(request))
                .Verifiable();

            var service = Factory.Maak<SyncService>();

            // ACT

            service.PersoonUpdaten(new Persoon
            {
                AdNummer = adNummer,
                GeboorteDatum = new DateTime(1977, 03, 08)
            });

            // ASSERT

            _civiApiMock.Verify(
                src =>
                    src.ContactSave(It.IsAny<string>(), It.IsAny<string>(),
                        It.Is<ContactRequest>(r => r.ExternalIdentifier == persoon.ExternalIdentifier)),
                Times.AtLeastOnce);
        }
    }
}
