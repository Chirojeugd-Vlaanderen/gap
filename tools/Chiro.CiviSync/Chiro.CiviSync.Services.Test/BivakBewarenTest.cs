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
using System.Security.Cryptography;
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
    public class BivakBewarenTest
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
        public void EventTypeBivak()
        {
            // ARRANGE

            var ploeg = new Contact { ExternalIdentifier = "TST/0001", Id = 1, ContactType = ContactType.Organization };

            _civiApiMock.Setup(
                src =>
                    src.ContactGetSingle(It.IsAny<string>(), It.IsAny<string>(),
                        It.Is<ContactRequest>(r => r.ExternalIdentifier == ploeg.ExternalIdentifier))).Returns(ploeg);

            _civiApiMock.Setup(
                src =>
                    src.EventSave(It.IsAny<string>(), It.IsAny<string>(),
                        It.Is<EventRequest>(r => r.EventTypeId == (int) EvenementType.Bivak)))
                .Returns((string key1, string key2, EventRequest r) => Mapper.Map<EventRequest, ApiResultValues<Event>>(r))
                .Verifiable();

            // ACT

            var service = Factory.Maak<SyncService>();
            service.BivakBewaren(new Bivak
            {
                DatumVan = new DateTime(2015, 07, 01),
                DatumTot = new DateTime(2015, 07, 10),
                Naam = "Trappistenkamp Achel",
                Opmerkingen = "Voor zolang er hier nog trappist is.",
                StamNummer = ploeg.ExternalIdentifier,
                UitstapID = 2,
                WerkJaar = HuidigWerkJaar
            });

            // ASSERT

            _civiApiMock.Verify(
                src =>
                    src.EventSave(It.IsAny<string>(), It.IsAny<string>(),
                        It.Is<EventRequest>(r => r.EventTypeId == (int) EvenementType.Bivak)), Times.AtLeastOnce);
        }
    }
}
