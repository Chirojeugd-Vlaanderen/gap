/*
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
using Moq;
using Chiro.Gap.UpdateApi.Client;
using Chiro.CiviCrm.Api;
using Chiro.CiviCrm.Api.DataContracts.Requests;
using Chiro.CiviCrm.Api.DataContracts.Entities;
using Chiro.CiviCrm.Api.DataContracts;
using Chiro.CiviSync.Services.Test;
using Chiro.Kip.ServiceContracts.DataContracts;
using NUnit.Framework;

namespace Chiro.CiviSync.Workers.Test
{
    [TestFixture]
    public class MembershipWorkerTest: SyncTest
    {
        private readonly DateTime _vandaagZogezegd = new DateTime(2015, 11, 6);

        /// <summary>
        /// Als een membership bijgewerkt wordt, is het niet de bedoeling dat er
        /// plots een verzekering loonverlies bijkomt. (Zie #4413)
        /// </summary>
        [Test]
        public void BestaandeBijwerkenTest()
        {
            // ARRANGE

            Mock<ICiviCrmApi> civiApiMock;
            Mock<IGapUpdateClient> updateHelperMock;
            using (var factory = TestHelper.IocOpzetten(_vandaagZogezegd, out civiApiMock, out updateHelperMock))
            {

                var myCiviContact = new Contact
                {
                    Id = 1,
                    FirstName = "Joe",
                    LastName = "Schmoe",
                    ExternalIdentifier = "2"
                };

                var myPloeg = new Contact
                {
                    Id = 2,
                    ExternalIdentifier = "TST/0000"
                };

                civiApiMock.Setup(
                        src => src.ContactGet(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<ContactRequest>()))
                    .Returns(new ApiResultValues<Contact>(new[] {myCiviContact, myPloeg}));
                civiApiMock.Setup(
                        src =>
                            src.MembershipSave(It.IsAny<string>(), It.IsAny<string>(),
                                It.Is<MembershipRequest>(r => r.VerzekeringLoonverlies.Value == false)))
                    .Returns(new ApiResultValues<Membership>())
                    .Verifiable();

                var membership = new Membership
                {
                    ContactId = myCiviContact.Id,
                    VerzekeringLoonverlies = false,
                    StartDate = new DateTime(2015, 10, 15),
                    EndDate = new DateTime(2016, 8, 31),
                    MembershipPaymentResult = new ApiResultValues<MembershipPayment>(new MembershipPayment())
                };

                // Dit nam ik over van ContactWorkerTest:
                var membershipWorker = factory.Maak<MembershipWorker>();

                // ACT
                membershipWorker.BestaandeBijwerken(membership,
                    new MembershipGedoe
                    {
                        MetLoonVerlies = false,
                        StamNummer = myPloeg.ExternalIdentifier
                    });

                // ASSERT
                civiApiMock.Verify(src =>
                    src.MembershipSave(It.IsAny<string>(), It.IsAny<string>(),
                        It.Is<MembershipRequest>(r => r.VerzekeringLoonverlies.Value == false)), Times.AtLeastOnce);
            }
        }

        /// <summary>
        /// Als je al een gratis membership hebt (kaderploeg), maar je nieuwe membership is betalend,
        /// dan moet het membership bijgewerkt worden.
        /// </summary>
        [Test]
        public void UpdateNaarBetalendTest()
        {
            // ARRANGE

            Mock<ICiviCrmApi> civiApiMock;
            Mock<IGapUpdateClient> updateHelperMock;
            using (var factory = TestHelper.IocOpzetten(_vandaagZogezegd, out civiApiMock, out updateHelperMock))
            {

                var myCiviContact = new Contact
                {
                    Id = 1,
                    FirstName = "Joe",
                    LastName = "Schmoe",
                    ExternalIdentifier = "2"
                };

                var myPloeg = new Contact
                {
                    Id = 2,
                    ExternalIdentifier = "TST/0101",
                    KaderNiveau = KaderNiveau.PlaatselijkeGroep
                };

                var myGewest = new Contact
                {
                    Id = 3,
                    ExternalIdentifier = "TST/0100",
                    KaderNiveau = KaderNiveau.Gewest
                };

                // Kaderniveau hierboven is in principe niet relevant, want het al dan
                // niet gratis zijn van een membership wordt bepaald aan de hand van
                // factuurstatus en betalingen.
                // Het staat er enkel voor de duidelijkheid.

                civiApiMock.Setup(
                        src => src.ContactGet(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<ContactRequest>()))
                    .Returns(new ApiResultValues<Contact>(new[] {myCiviContact, myPloeg}));
                civiApiMock.Setup(
                        src =>
                            src.MembershipSave(It.IsAny<string>(), It.IsAny<string>(),
                                It.Is<MembershipRequest>(
                                    r => r.VerzekeringLoonverlies.Value == false &&
                                         r.FactuurStatus == FactuurStatus.VolledigTeFactureren)))
                    .Returns(new ApiResultValues<Membership>())
                    .Verifiable();

                var membership = new Membership
                {
                    ContactId = myGewest.Id,
                    VerzekeringLoonverlies = false,
                    FactuurStatus = FactuurStatus.FactuurOk,
                    // Vandaag is het zogezegd _vandaagzogezegd, en dat valt dan in het onderstaande interval.
                    StartDate = new DateTime(2015, 10, 15),
                    EndDate = new DateTime(2016, 8, 31),
                    MembershipPaymentResult = new ApiResultValues<MembershipPayment>(),
                };

                // Dit nam ik over van ContactWorkerTest:
                var membershipWorker = factory.Maak<MembershipWorker>();

                // ACT

                // Als we onderweg een invalid api-result hebben, zou het kunnen dat de condities niet voldaan zijn
                // om de juiste mocking te triggeren.
                membershipWorker.BestaandeBijwerken(membership,
                    new MembershipGedoe
                    {
                        MetLoonVerlies = false,
                        StamNummer = myPloeg.ExternalIdentifier
                    });

                // ASSERT
                civiApiMock.Verify(src =>
                    src.MembershipSave(It.IsAny<string>(), It.IsAny<string>(),
                        It.Is<MembershipRequest>(r => r.VerzekeringLoonverlies.Value == false)), Times.AtLeastOnce);
            }
        }
    }
}
