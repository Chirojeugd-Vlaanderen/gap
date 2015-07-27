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
using Chiro.CiviSync.Mapping;
using Chiro.Gap.UpdateApi.Client;
using Chiro.Kip.ServiceContracts.DataContracts;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Chiro.CiviSync.Services.Test
{
    [TestClass]
    public class BivakBewarenTest
    {
        private Mock<ICiviCrmApi> _civiApiMock;
        private Mock<IGapUpdateClient> _updateHelperMock;

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

        /// <summary>
        /// Test of event type en organiserende ploeg goed gesynct worden.
        /// </summary>
        [TestMethod]
        public void DetailsBivak()
        {
            // ARRANGE

            var ploeg = new Contact { ExternalIdentifier = "TST/0001", Id = 1, ContactType = ContactType.Organization };
            var teBewarenBivak = new Bivak
            {
                DatumVan = new DateTime(2015, 07, 01),
                DatumTot = new DateTime(2015, 07, 10),
                Naam = "Trappistenkamp Achel",
                Opmerkingen = "Voor zolang er hier nog trappist is.",
                StamNummer = ploeg.ExternalIdentifier,
                UitstapID = 2,
                WerkJaar = HuidigWerkJaar
            };

            _civiApiMock.Setup(
                src =>
                    src.ContactGet(It.IsAny<string>(), It.IsAny<string>(),
                        It.Is<ContactRequest>(r => r.ExternalIdentifier == ploeg.ExternalIdentifier)))
                .Returns(new ApiResultValues<Contact>(ploeg));

            _civiApiMock.Setup(
                src =>
                    src.EventGet(It.IsAny<string>(), It.IsAny<string>(),
                        It.Is<EventRequest>(
                            r => r.GapUitstapId == teBewarenBivak.UitstapID)))
                .Returns(new ApiResultValues<Event>
                {
                    Count = 0,
                    ErrorMessage = String.Empty,
                    Id = null,
                    IsError = 0,
                    Values = new Event[0],
                    Version = 3
                });

            _civiApiMock.Setup(
                src =>
                    src.EventSave(It.IsAny<string>(), It.IsAny<string>(),
                        It.Is<EventRequest>(
                            r => r.EventTypeId == (int) EvenementType.Bivak && r.OrganiserendePloeg1Id == ploeg.Id)))
                .Returns(
                    (string key1, string key2, EventRequest r) => Mapper.Map<EventRequest, ApiResultValues<Event>>(r))
                .Verifiable();

            // ACT

            var service = Factory.Maak<SyncService>();
            service.BivakBewaren(teBewarenBivak);

            // ASSERT

            _civiApiMock.Verify(
                src =>
                    src.EventSave(It.IsAny<string>(), It.IsAny<string>(),
                        It.Is<EventRequest>(r => r.EventTypeId == (int)EvenementType.Bivak && r.OrganiserendePloeg1Id == ploeg.Id)), Times.AtLeastOnce);
        }

        /// <summary>
        /// Als er meerdere bivakken bestaan met het GAP-ID van het te updaten bivak, dan
        /// verwachten we dat CiviSync er minstens 1 overschrijft.
        /// 
        /// Normaal bestaan er geen meerdere bivakken met hetzelfde GAP-ID, maar als dat in
        /// dev toch zo is, dan mag de sync het niet om zeep helpen.
        /// </summary>
        [TestMethod]
        public void MeerdereBivakkenZelfdeGapId()
        {
            // ARRANGE

            var ploeg = new Contact { ExternalIdentifier = "TST/0001", Id = 1, ContactType = ContactType.Organization };

            var bivak1 = new Bivak
            {
                DatumVan = new DateTime(2015, 07, 01),
                DatumTot = new DateTime(2015, 07, 10),
                Naam = "Trappistenkamp Achel",
                Opmerkingen = "Voor zolang er hier nog trappist is.",
                StamNummer = ploeg.ExternalIdentifier,
                UitstapID = 2,
                WerkJaar = HuidigWerkJaar
            };

            // Er zijn per ongeluk meerdere events gemaakt van dat bivak.
            var event1 = Mapper.Map<EventRequest, Event>(Mapper.Map<Bivak, EventRequest>(bivak1));
            event1.Id = 3;
            event1.OrganiserendePloeg1Id = ploeg.Id;
            var event2 = Mapper.Map<EventRequest, Event>(Mapper.Map<Bivak, EventRequest>(bivak1));
            event2.Id = 4;
            event1.OrganiserendePloeg2Id = ploeg.Id;
          
            _civiApiMock.Setup(
                src =>
                    src.ContactGetSingle(It.IsAny<string>(), It.IsAny<string>(),
                        It.Is<ContactRequest>(r => r.ExternalIdentifier == ploeg.ExternalIdentifier))).Returns(ploeg);

            // getsingle levert een leeg event op als er meerdere bestaan. Zo doet civi dat alleszins.
            _civiApiMock.Setup(
                src => src.EventGetSingle(It.IsAny<string>(), It.IsAny<string>(),
                    It.Is<EventRequest>(r => r.GapUitstapId == bivak1.UitstapID))).Returns(new Event());
            // met get krijg je ze allebei
            _civiApiMock.Setup(
                src => src.EventGet(It.IsAny<string>(), It.IsAny<string>(),
                    It.Is<EventRequest>(r => r.GapUitstapId == bivak1.UitstapID)))
                .Returns(new ApiResultValues<Event>
                {
                    Count = 2,
                    ErrorMessage = string.Empty,
                    Id = null,
                    IsError = 0,
                    Values = new[] {event1, event2},
                    Version = 3
                });

            _civiApiMock.Setup(
                src =>
                    src.EventSave(It.IsAny<string>(), It.IsAny<string>(),
                        It.Is<EventRequest>(r => r.Id == event1.Id || r.Id == event2.Id)))
                .Returns((string key1, string key2, EventRequest r) => Mapper.Map<EventRequest, ApiResultValues<Event>>(r))
                .Verifiable();

            // ACT

            var service = Factory.Maak<SyncService>();
            service.BivakBewaren(bivak1);

            // ASSERT

            _civiApiMock.Verify(
                src =>
                    src.EventSave(It.IsAny<string>(), It.IsAny<string>(),
                        It.Is<EventRequest>(r => r.EventTypeId == (int)EvenementType.Bivak)), Times.AtLeastOnce);
        }
    }
}
