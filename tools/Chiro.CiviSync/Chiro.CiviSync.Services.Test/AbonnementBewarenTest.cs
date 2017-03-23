/*
   Copyright 2016 Chirojeugd-Vlaanderen vzw

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
    public class AbonnementBewarenTest: SyncTest
    {
        private readonly DateTime _vandaagZogezegd = new DateTime(2015, 8, 6);
        private const int HuidigWerkJaar = 2014;

        /// <summary>
        /// Nakijken data als je een abonnement vraagt voor werkjaar X terwijl je er al
        /// een hebt voor werkjaar X+1.
        /// </summary>
        [Test]
        public void AbonnementBewarenOuderDanHuidige()
        {
            // ARRANGE

            Mock<ICiviCrmApi> civiApiMock;
            Mock<IGapUpdateClient> updateHelperMock;
            using (var factory = TestHelper.IocOpzetten(_vandaagZogezegd, out civiApiMock, out updateHelperMock))
            {

                const int adNummer = 2;
                const int contactId = 4;
                DateTime beginBestaandAbonnement = new DateTime(HuidigWerkJaar + 1, 9, 1);
                DateTime eindeBestaandAbonnement = new DateTime(HuidigWerkJaar + 2, 9, 1);

                var bestaandAbonnement = new Membership
                {
                    ContactId = contactId,
                    MembershipTypeId = (int) MembershipType.DubbelpuntAbonnement,
                    StartDate = beginBestaandAbonnement,
                    EndDate = eindeBestaandAbonnement,
                    JoinDate = beginBestaandAbonnement
                };

                var persoon = new Contact
                {
                    Id = contactId,
                    ExternalIdentifier = adNummer.ToString(),
                    FirstName = "Kees",
                    LastName = "Flodder",
                    GapId = 3,
                    MembershipResult =
                        new ApiResultValues<Membership> {Count = 1, IsError = 0, Values = new[] {bestaandAbonnement}}
                };

                civiApiMock.Setup(
                        src => src.ContactGet(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<ContactRequest>()))
                    .Returns(new ApiResultValues<Contact>(persoon));

                // Neem meteen mee dat IsOverride niet gezet mag zijn (zie #4960).
                civiApiMock.Setup(
                        src =>
                            src.MembershipSave(It.IsAny<string>(), It.IsAny<string>(),
                                It.Is<MembershipRequest>(
                                    r =>
                                        r.StartDate == _vandaagZogezegd && r.EndDate == eindeBestaandAbonnement &&
                                        r.JoinDate == _vandaagZogezegd && r.IsOverride != true &&
                                        r.MembershipTypeId == (int) MembershipType.DubbelpuntAbonnement)))
                    .Returns(
                        (string key1, string key2, MembershipRequest r) =>
                            TestHelper.Map<MembershipRequest, ApiResultValues<Membership>>(r))
                    .Verifiable();

                var service = factory.Maak<SyncService>();

                // ACT

                service.AbonnementBewaren(adNummer, HuidigWerkJaar, AbonnementTypeEnum.Digitaal);

                // ASSERT

                civiApiMock.Verify(
                    src =>
                        src.MembershipSave(It.IsAny<string>(), It.IsAny<string>(),
                            It.Is<MembershipRequest>(
                                r => r.StartDate == _vandaagZogezegd && r.EndDate == eindeBestaandAbonnement &&
                                     r.JoinDate == _vandaagZogezegd && r.IsOverride != true &&
                                     r.MembershipTypeId == (int) MembershipType.DubbelpuntAbonnement)),
                    Times.AtLeastOnce);
            }
        }
    }
}
