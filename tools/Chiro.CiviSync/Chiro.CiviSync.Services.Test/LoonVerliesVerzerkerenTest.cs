﻿/*
   Copyright 2015, 2016 Chirojeugd-Vlaanderen vzw

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
using Moq;
using NUnit.Framework;

namespace Chiro.CiviSync.Services.Test
{
    [TestFixture]
    public class LoonVerliesVerzerkerenTest: SyncTest
    {
        private readonly DateTime _vandaagZogezegd = new DateTime(2015, 2, 6);
        private const int HuidigWerkJaar = 2014;

        /// <summary>
        /// Stel dat een persoon al member is in civi, en dat dat membership al is gefactureerd.
        /// Als loonverlies verzekerd moet worden, dan wordt het membership bijgewerkt, en dan moet de status van het
        /// bijgewerkte membership 'gedeeltelijk gefactureerd' worden.
        /// </summary>
        [Test]
        public void GefactureerdMembershipUpgradenMetLoonverlies()
        {
            // ARRANGE

            Mock<ICiviCrmApi> civiApiMock;
            Mock<IGapUpdateClient> updateHelperMock;
            using (var factory = TestHelper.IocOpzetten(_vandaagZogezegd, out civiApiMock, out updateHelperMock))
            {

                const int adNummer = 2;
                const int contactId = 4;
                DateTime beginDitWerkJaar = new DateTime(HuidigWerkJaar, 9, 1);
                DateTime eindeDitWerkJaar = new DateTime(HuidigWerkJaar + 1, 8, 31);

                var bestaandMembership = new Membership
                {
                    ContactId = contactId,
                    MembershipTypeId = (int) MembershipType.Aansluiting,
                    StartDate = beginDitWerkJaar,
                    EndDate = eindeDitWerkJaar,
                    JoinDate = beginDitWerkJaar.AddMonths(1),
                    VerzekeringLoonverlies = false,
                    FactuurStatus = FactuurStatus.FactuurOk,
                    MembershipPaymentResult = new ApiResultValues<MembershipPayment>(new MembershipPayment())
                };

                var persoon = new Contact
                {
                    Id = contactId,
                    ExternalIdentifier = adNummer.ToString(),
                    FirstName = "Kees",
                    LastName = "Flodder",
                    GapId = 3,
                    MembershipResult =
                        new ApiResultValues<Membership> {Count = 1, IsError = 0, Values = new[] {bestaandMembership}}
                };
                var groep = new Contact {ExternalIdentifier = "BLA/0000", Id = 5};

                civiApiMock.Setup(
                        src => src.ContactGet(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<ContactRequest>()))
                    .Returns(
                        (string key1, string key2, ContactRequest r) =>
                            new ApiResultValues<Contact>(r.ExternalIdentifier == groep.ExternalIdentifier ||
                                                         r.Id == groep.Id
                                ? groep
                                : persoon));

                civiApiMock.Setup(
                        src =>
                            src.MembershipSave(It.IsAny<string>(), It.IsAny<string>(),
                                It.Is<MembershipRequest>(
                                    r => r.FactuurStatus == FactuurStatus.ExtraVerzekeringTeFactureren)))
                    .Returns(
                        (string key1, string key2, MembershipRequest r) =>
                            TestHelper.Map<MembershipRequest, ApiResultValues<Membership>>(r))
                    .Verifiable();

                var service = factory.Maak<SyncService>();

                // ACT

                // loonverlies verzekeren.
                service.LoonVerliesVerzekeren(adNummer, groep.ExternalIdentifier, HuidigWerkJaar, false);

                // ASSERT

                civiApiMock.Verify(
                    src =>
                        src.MembershipSave(It.IsAny<string>(), It.IsAny<string>(),
                            It.Is<MembershipRequest>(r => r.FactuurStatus ==
                                                          FactuurStatus.ExtraVerzekeringTeFactureren)),
                    Times.AtLeastOnce);
            }
        }

        /// <summary>
        /// Als iemand vorig jaar member was, maar dit jaar nog niet, en je verzekert dit jaar 
        /// tegen loonverlies, dan mag dat nog niets doen. Die verzekering zal dan mee
        /// afgehandeld worden als het membership wordt gemaakt.
        /// </summary>
        [Test]
        public void LoonverliesNegerenAlsNogGeenMemberDitWerkjaar()
        {
            // ARRANGE

            Mock<ICiviCrmApi> civiApiMock;
            Mock<IGapUpdateClient> updateHelperMock;
            using (var factory = TestHelper.IocOpzetten(_vandaagZogezegd, out civiApiMock, out updateHelperMock))
            {

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
                    JoinDate = beginDitWerkJaar.AddMonths(-10),
                    VerzekeringLoonverlies = false,
                    FactuurStatus = FactuurStatus.FactuurOk
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
                var groep = new Contact {ExternalIdentifier = "BLA/0000", Id = 5};

                civiApiMock.Setup(
                        src => src.ContactGet(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<ContactRequest>()))
                    .Returns(
                        (string key1, string key2, ContactRequest r) =>
                            new ApiResultValues<Contact>(r.ExternalIdentifier == groep.ExternalIdentifier ||
                                                         r.Id == groep.Id
                                ? groep
                                : persoon));

                civiApiMock.Setup(
                        src =>
                            src.MembershipSave(It.IsAny<string>(), It.IsAny<string>(),
                                It.IsAny<MembershipRequest>()))
                    .Returns(
                        (string key1, string key2, MembershipRequest r) =>
                            TestHelper.Map<MembershipRequest, ApiResultValues<Membership>>(r))
                    .Verifiable();

                var service = factory.Maak<SyncService>();

                // ACT

                // loonverlies verzekeren.
                service.LoonVerliesVerzekeren(adNummer, groep.ExternalIdentifier, HuidigWerkJaar, false);

                // ASSERT

                civiApiMock.Verify(
                    src =>
                        src.MembershipSave(It.IsAny<string>(), It.IsAny<string>(),
                            It.IsAny<MembershipRequest>()),
                    Times.Never);
            }
        }
    }
}
