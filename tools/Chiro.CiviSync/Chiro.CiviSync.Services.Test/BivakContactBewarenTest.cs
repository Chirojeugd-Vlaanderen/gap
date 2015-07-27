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
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Chiro.CiviSync.Services.Test
{
    [TestClass]
    public class BivakContactBewarenTest
    {
        private Mock<ICiviCrmApi> _civiApiMock;
        private Mock<IGapUpdateClient> _gapUpdateClientMock;

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
            TestHelper.IocOpzetten(_vandaagZogezegd, out _civiApiMock, out _gapUpdateClientMock);
        }

        /// <summary>
        /// Bij het bewaren van een contact moet het goede contact-ID naar CiviCRM gestuurd worden.
        /// </summary>
        [TestMethod]
        public void ContactBewaren()
        {
            // ARRANGE

            const int adNummer = 2;
            const int persoonContactId = 5;
            const int uitstapId = 4;

            // prepareer het event dat we gaan opleveren.
            var ploeg = new Contact {ExternalIdentifier = "TST/0001", Id = 1, ContactType = ContactType.Organization};
            var persoon = new Contact {ExternalIdentifier = adNummer.ToString(), Id = persoonContactId, ContactType = ContactType.Individual};

            var bivak = new Event
            {
                Id = 3,
                GapUitstapId = uitstapId,
                ContactResult = Mapper.Map<Contact, ApiResultValues<Contact>>(ploeg)
            };

            // Lever persoon of groep op als dat wordt gevraagd.
            _civiApiMock.Setup(
                src =>
                    src.ContactGet(It.IsAny<string>(), It.IsAny<string>(),
                        It.Is<ContactRequest>(r => r.ExternalIdentifier == adNummer.ToString())))
                .Returns(new ApiResultValues<Contact>(persoon));
            _civiApiMock.Setup(
                src => src.EventGet(It.IsAny<string>(), It.IsAny<string>(),
                    It.Is<EventRequest>(r => r.GapUitstapId == bivak.GapUitstapId)))
                .Returns(Mapper.Map<Event, ApiResultValues<Event>>(bivak));

            // Verwacht dat het juiste ContactID naar CiviCRM gaat.
            _civiApiMock.Setup(
                src =>
                    src.EventSave(It.IsAny<string>(), It.IsAny<string>(),
                        It.Is<EventRequest>(r => r.OrganiserendePersoon1Id == persoonContactId))).Verifiable();

            var service = Factory.Maak<SyncService>();
            service.CacheInvalideren();

            // ACT

            service.BivakContactBewaren(uitstapId, adNummer);

            // ASSERT

            _civiApiMock.Verify(src => src.EventSave(It.IsAny<string>(), It.IsAny<string>(),
                It.Is<EventRequest>(r => r.OrganiserendePersoon1Id == persoonContactId)), Times.AtLeastOnce);
        }

        /// <summary>
        /// Als CiviSync een contact moet vastleggen voor een bivak, maar een ongeldig
        /// AD-nummer krijgt, dan moet CiviSync dat aan GAP laten weten.
        /// </summary>
        [TestMethod]
        public void ContactBewarenOngeldigAdNummer()
        {
            // ARRANGE

            const int adNummer = 2;
            const int uitstapId = 4;

            // prepareer het event dat we gaan opleveren.
            var ploeg = new Contact {ExternalIdentifier = "TST/0001", Id = 1, ContactType = ContactType.Organization};
            var bivak = new Event
            {
                Id = 3,
                GapUitstapId = uitstapId,
                ContactResult = Mapper.Map<Contact, ApiResultValues<Contact>>(ploeg)
            };

            _civiApiMock.Setup(
                src =>
                    src.ContactGet(It.IsAny<string>(), It.IsAny<string>(),
                        It.Is<ContactRequest>(r => r.ExternalIdentifier == adNummer.ToString()))).Returns(new ApiResultValues<Contact>());

            // Lever braaf het bivak op als het gevraagd wordt.
            _civiApiMock.Setup(
                src => src.EventGet(It.IsAny<string>(), It.IsAny<string>(),
                    It.Is<EventRequest>(r => r.GapUitstapId == bivak.GapUitstapId)))
                .Returns(Mapper.Map<Event, ApiResultValues<Event>>(bivak));

            // Verwacht dat het foute AD-nummer terug naar GAP gaat.
            _gapUpdateClientMock.Setup(src => src.OngeldigAdNaarGap(It.Is<Int32>(ad => ad == adNummer))).Verifiable();

            // ACT

            var service = Factory.Maak<SyncService>();
            service.CacheInvalideren();
            service.BivakContactBewaren(uitstapId, adNummer);

            // ASSERT

            _gapUpdateClientMock.Verify(src => src.OngeldigAdNaarGap(It.Is<Int32>(ad => ad == adNummer)), Times.AtLeastOnce);
        }
    }
}
