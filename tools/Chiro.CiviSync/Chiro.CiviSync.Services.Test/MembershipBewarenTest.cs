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
using Chiro.Gap.UpdateApi.Client;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Chiro.CiviSync.Services.Test
{
    [TestClass]
    public class MembershipBewarenTest
    {
        private Mock<ICiviCrmApi> _civiApiMock;
        private Mock<IGapUpdateClient> _updateHelperMock;

        private readonly DateTime _vandaagZogezegd = new DateTime(2015, 2, 6);
        private const int HuidigWerkJaar = 2014;

        [ClassInitialize]
        public static void InitialilzeTestClass(TestContext c)
        {
            TestHelper.MappingsCreeren();
        }

        [TestInitialize]
        public void InitializeTest()
        {
            TestHelper.IocOpzetten(_vandaagZogezegd, out _civiApiMock, out _updateHelperMock);
        }

        [TestMethod]
        public void MembershipBewaren()
        {
            // ARRANGE

            const int adNummer = 2;

            var persoon = new Contact { ExternalIdentifier = adNummer.ToString(), FirstName = "Kees", LastName = "Flodder", GapId = 3, Id = 4 };
            var groep = new Contact {ExternalIdentifier = "BLA/0000", Id = 5};
            DateTime beginDitWerkJaar = new DateTime(HuidigWerkJaar, 9, 1);
            DateTime eindeDitWerkJaar = new DateTime(HuidigWerkJaar + 1, 8, 31);

            _civiApiMock.Setup(
                src => src.ContactGet(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<ContactRequest>()))
                .Returns(
                    (string key1, string key2, ContactRequest r) =>
                    {
                        Contact result;
                        if (r.ExternalIdentifier == groep.ExternalIdentifier || r.Id == groep.Id)
                        {
                            result = groep;
                        }
                        else
                        {
                            result = persoon;
                        }
                        // Als relaties gevraagd zijn, lever dan gewoon een lege lijst op.
                        if (r.MembershipGetRequest != null)
                        {
                            result.MembershipResult = new ApiResultValues<Membership>
                            {
                                Count = 0,
                                IsError = 0
                            };
                        }
                        return new ApiResultValues<Contact>(result);
                    });

            _civiApiMock.Setup(
                src =>
                    src.MembershipSave(It.IsAny<string>(), It.IsAny<string>(),
                        It.Is<MembershipRequest>(
                            r =>
                                r.StartDate == beginDitWerkJaar && r.EndDate == eindeDitWerkJaar &&
                                r.MembershipTypeId == (int) MembershipType.Aansluiting)))
                .Returns(
                    (string key1, string key2, MembershipRequest r) =>
                        Mapper.Map<MembershipRequest, ApiResultValues<Membership>>(r)).Verifiable();

            var service = Factory.Maak<SyncService>();

            // ACT

            service.MembershipBewaren(adNummer, groep.ExternalIdentifier, HuidigWerkJaar, false);

            // ASSERT

            _civiApiMock.Verify(
                src =>
                    src.MembershipSave(It.IsAny<string>(), It.IsAny<string>(),
                        It.Is<MembershipRequest>(
                            r =>
                                r.StartDate == _vandaagZogezegd && r.EndDate == eindeDitWerkJaar &&
                                r.MembershipTypeId == (int) MembershipType.Aansluiting)), Times.AtLeastOnce);
        }

        /// <summary>
        /// In onze eigen specs staat ergens dat JoinDate de datum is dat het eerste membership van deze
        /// persoon (ever) geregistreerd is.
        /// </summary>
        [TestMethod]
        public void MembershipBewarenOudeJoinDate()
        {
            // ARRANGE

            const int adNummer = 2;
            const int contactId = 4;
            DateTime beginDitWerkJaar = new DateTime(HuidigWerkJaar, 9, 1);
            DateTime eindeDitWerkJaar = new DateTime(HuidigWerkJaar + 1, 8, 31);

            var oudMembership = new Membership
            {
                ContactId = contactId,
                MembershipTypeId = (int) MembershipType.Aansluiting,
                StartDate = beginDitWerkJaar.AddYears(-1),
                EndDate = eindeDitWerkJaar.AddYears(-1),
                JoinDate = beginDitWerkJaar.AddMonths(-10)
            };

            var persoon = new Contact
            {
                Id = contactId,
                ExternalIdentifier = adNummer.ToString(),
                FirstName = "Kees",
                LastName = "Flodder",
                GapId = 3,
                MembershipResult =
                    new ApiResultValues<Membership> {Count = 1, IsError = 0, Values = new[] {oudMembership}}
            };
            var groep = new Contact { ExternalIdentifier = "BLA/0000", Id = 5 };

            _civiApiMock.Setup(
                src => src.ContactGet(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<ContactRequest>()))
                .Returns(
                    (string key1, string key2, ContactRequest r) =>
                        new ApiResultValues<Contact>(r.ExternalIdentifier == groep.ExternalIdentifier ||
                                                     r.Id == groep.Id
                            ? groep
                            : persoon));

            _civiApiMock.Setup(
                src =>
                    src.MembershipSave(It.IsAny<string>(), It.IsAny<string>(),
                        It.Is<MembershipRequest>(
                            r =>
                                r.StartDate == beginDitWerkJaar && r.EndDate == eindeDitWerkJaar &&
                                r.JoinDate == oudMembership.JoinDate &&
                                r.MembershipTypeId == (int)MembershipType.Aansluiting)))
                .Returns(
                    (string key1, string key2, MembershipRequest r) =>
                        Mapper.Map<MembershipRequest, ApiResultValues<Membership>>(r)).Verifiable();

            var service = Factory.Maak<SyncService>();

            // ACT

            service.MembershipBewaren(adNummer, groep.ExternalIdentifier, HuidigWerkJaar, false);

            // ASSERT

            _civiApiMock.Verify(
                src =>
                    src.MembershipSave(It.IsAny<string>(), It.IsAny<string>(),
                        It.Is<MembershipRequest>(
                            r =>
                                r.StartDate == _vandaagZogezegd && r.EndDate == eindeDitWerkJaar &&
                                r.JoinDate == oudMembership.JoinDate &&
                                r.MembershipTypeId == (int)MembershipType.Aansluiting)), Times.AtLeastOnce);
        }

        /// <summary>
        /// Stel dat een persoon al member is in civi, maar dat dat membership nog niet is gefactureerd. Als
        /// dat membership dan geüpgradet wordt met een verzekering loonverlies, dan moet de status van het
        /// bijgewerkte membership nog steeds 'niet gefactureerd' zijn.
        /// </summary>
        [TestMethod]
        public void TeFacturerenMembershipUpgradenMetLoonverlies()
        {
            // ARRANGE

            const int adNummer = 2;
            const int contactId = 4;
            DateTime beginDitWerkJaar = new DateTime(HuidigWerkJaar, 9, 1);
            DateTime eindeDitWerkJaar = new DateTime(HuidigWerkJaar + 1, 8, 31);

            var bestaandMembership = new Membership
            {
                ContactId = contactId,
                MembershipTypeId = (int)MembershipType.Aansluiting,
                StartDate = beginDitWerkJaar,
                EndDate = eindeDitWerkJaar,
                JoinDate = beginDitWerkJaar.AddMonths(1),
                VerzekeringLoonverlies = false,
                FactuurStatus = FactuurStatus.VolledigTeFactureren
            };

            var persoon = new Contact
            {
                Id = contactId,
                ExternalIdentifier = adNummer.ToString(),
                FirstName = "Kees",
                LastName = "Flodder",
                GapId = 3,
                MembershipResult =
                    new ApiResultValues<Membership> { Count = 1, IsError = 0, Values = new[] { bestaandMembership } }
            };
            var groep = new Contact { ExternalIdentifier = "BLA/0000", Id = 5 };

            _civiApiMock.Setup(
                src => src.ContactGet(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<ContactRequest>()))
                .Returns(
                    (string key1, string key2, ContactRequest r) =>
                        new ApiResultValues<Contact>(r.ExternalIdentifier == groep.ExternalIdentifier ||
                                                     r.Id == groep.Id
                            ? groep
                            : persoon));

            _civiApiMock.Setup(
                src =>
                    src.MembershipSave(It.IsAny<string>(), It.IsAny<string>(),
                        It.Is<MembershipRequest>(r => r.FactuurStatus == FactuurStatus.VolledigTeFactureren)))
                .Returns(
                    (string key1, string key2, MembershipRequest r) =>
                        Mapper.Map<MembershipRequest, ApiResultValues<Membership>>(r))
                .Verifiable();

            var service = Factory.Maak<SyncService>();

            // ACT

            // pas membership aan met loonverlies.
            service.MembershipBewaren(adNummer, groep.ExternalIdentifier, HuidigWerkJaar, true);

            // ASSERT

            _civiApiMock.Verify(
                src =>
                    src.MembershipSave(It.IsAny<string>(), It.IsAny<string>(),
                        It.Is<MembershipRequest>(r => r.FactuurStatus == FactuurStatus.VolledigTeFactureren)),
                Times.AtLeastOnce);
        }
    }
}
