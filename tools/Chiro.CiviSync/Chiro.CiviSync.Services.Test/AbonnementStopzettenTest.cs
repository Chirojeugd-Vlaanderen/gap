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
using Moq;
using NUnit.Framework;

namespace Chiro.CiviSync.Services.Test
{
    [TestFixture]
    public class AbonnementStopzettenTest: SyncTest
    {
        private readonly DateTime _vandaagZogezegd = new DateTime(2016, 2, 6);
        private const int HuidigWerkJaar = 2015;

        /// <summary>
        /// Test of een abonnement wel stopgezet wordt.
        /// </summary>
        /// <remarks>
        /// Deze test zou beter verplaatst worden naar de WorkersTest.
        /// </remarks>
        [Test]
        public void AbonnementStopzetten()
        {
            // ARRANGE

            Mock<ICiviCrmApi> civiApiMock;
            Mock<IGapUpdateClient> updateHelperMock;
            using (var factory = TestHelper.IocOpzetten(_vandaagZogezegd, out civiApiMock, out updateHelperMock))
            {

                const int adNummer = 2;
                const int contactId = 4;
                DateTime beginBestaandAbonnement = new DateTime(HuidigWerkJaar, 9, 1);
                DateTime eindeBestaandAbonnement = new DateTime(HuidigWerkJaar + 1, 9, 1);

                var bestaandAbonnement = new Membership
                {
                    Id = 5,
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

                civiApiMock.Setup(
                        src =>
                            src.MembershipSave(It.IsAny<string>(), It.IsAny<string>(),
                                It.Is<MembershipRequest>(r => r.Id == 5 && r.EndDate <= _vandaagZogezegd)))
                    .Returns(
                        (string key1, string key2, MembershipRequest r) =>
                            TestHelper.Map<MembershipRequest, ApiResultValues<Membership>>(r))
                    .Verifiable();

                var service = factory.Maak<SyncService>();

                // ACT


                service.AbonnementStopzetten(adNummer);

                // ASSERT

                civiApiMock.Verify(
                    src =>
                        src.MembershipSave(It.IsAny<string>(), It.IsAny<string>(),
                            It.Is<MembershipRequest>(r => r.Id == 5 && r.EndDate <= _vandaagZogezegd)),
                    Times.AtLeastOnce);
            }
        }

        /// <summary>
        /// Als je een abonnement vandaag maakt, en vandaag terug stopzet,
        /// dan moet het verwijderd worden uit de Civi.
        /// </summary>
        /// <remarks>
        /// Deze test zou beter verplaatst worden naar de WorkersTest.
        /// </remarks>
        [Test]
        public void AbonnementDirectStopzetten()
        {
            // ARRANGE

            Mock<ICiviCrmApi> civiApiMock;
            Mock<IGapUpdateClient> updateHelperMock;
            using (var factory = TestHelper.IocOpzetten(_vandaagZogezegd, out civiApiMock, out updateHelperMock))
            {

                const int adNummer = 2;
                const int contactId = 4;
                DateTime beginBestaandAbonnement = _vandaagZogezegd;
                DateTime eindeBestaandAbonnement = new DateTime(HuidigWerkJaar + 1, 9, 1);

                var bestaandAbonnement = new Membership
                {
                    Id = 5,
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

                civiApiMock.Setup(
                        src =>
                            src.MembershipDelete(It.IsAny<string>(), It.IsAny<string>(),
                                It.Is<DeleteRequest>(r => r.Id == bestaandAbonnement.Id)))
                    .Returns(new ApiResult())
                    .Verifiable();

                var service = factory.Maak<SyncService>();

                // ACT


                service.AbonnementStopzetten(adNummer);

                // ASSERT

                civiApiMock.Verify(
                    src =>
                        src.MembershipDelete(It.IsAny<string>(), It.IsAny<string>(),
                            It.Is<DeleteRequest>(r => r.Id == bestaandAbonnement.Id)), Times.AtLeastOnce);
            }
        }
    }

}
