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
using System.Linq;
using Chiro.Cdf.Ioc;
using Chiro.CiviCrm.Api;
using Chiro.CiviCrm.Api.DataContracts;
using Chiro.CiviCrm.Api.DataContracts.Entities;
using Chiro.CiviCrm.Api.DataContracts.Requests;
using Chiro.CiviSync.Test.Mapping;
using Chiro.Gap.UpdateApi.Client;
using Moq;
using NUnit.Framework;

namespace Chiro.CiviSync.Services.Test
{
    [TestFixture]
    public class LidUitschrijvenTest: SyncTest
    {
        private readonly DateTime _vandaagZogezegd = new DateTime(2015, 2, 6);
        private const int HuidigWerkJaar = 2014;

        /// <summary>
        /// Uitschrijfdatum - 1 = einddatum relatie, zie #5367.
        /// </summary>
        [Test]
        public void LidUitschrijven()
        {
            // ARRANGE

            Mock<ICiviCrmApi> civiApiMock;
            Mock<IGapUpdateClient> updateHelperMock;
            using (var factory = TestHelper.IocOpzetten(_vandaagZogezegd, out civiApiMock, out updateHelperMock))
            {

                const int adNummer = 2;

                DateTime uitschrijfDatum = new DateTime(HuidigWerkJaar, 11, 20);

                // Onze nepdatabase bevat 1 organisatie (TST/0000) en 1 contact (Kees Flodder)
                var ploeg = new Contact
                {
                    ExternalIdentifier = "TST/0001",
                    Id = 1,
                    ContactType = ContactType.Organization
                };
                var persoon = new Contact
                {
                    ExternalIdentifier = adNummer.ToString(),
                    FirstName = "Kees",
                    LastName = "Flodder",
                    GapId = 3,
                    Id = 4
                };
                var relatie = new Relationship
                {
                    Id = 5,
                    Afdeling = Afdeling.Titos,
                    ContactIdA = persoon.Id,
                    ContactIdB = ploeg.Id,
                    StartDate = new DateTime(HuidigWerkJaar, 10, 14),
                    EndDate = null,
                    IsActive = true
                };

                // Een request om 1 of meerdere contacts op te leveren, levert voor het gemak altijd
                // dezelfde persoon en dezelfde ploeg.

                civiApiMock.Setup(
                        src => src.ContactGetSingle(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<ContactRequest>()))
                    .Returns(
                        (string key1, string key2, ContactRequest r) =>
                        {
                            var result = r.ContactType == ContactType.Organization
                                ? ploeg
                                : persoon;
                            // We veronderstellen dat de persoon al lid was dit werkjaar.
                            if (r.RelationshipGetRequest != null)
                            {
                                result.RelationshipResult = new ApiResultValues<Relationship>
                                {
                                    Count = 1,
                                    IsError = 0,
                                    Id = relatie.Id,
                                    Values = new[] {relatie}
                                };
                            }
                            return result;
                        });

                civiApiMock.Setup(
                        src => src.ContactGet(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<ContactRequest>()))
                    .Returns(
                        (string key1, string key2, ContactRequest r) =>
                        {
                            var result = new ApiResultValues<Contact>
                            {
                                Count = 1,
                                IsError = 0,
                                Values = new[] {civiApiMock.Object.ContactGetSingle(key1, key2, r)}
                            };
                            result.Id = result.Values.First().Id;
                            return result;
                        });

                civiApiMock.Setup(
                        src =>
                            src.RelationshipSave(It.IsAny<string>(), It.IsAny<string>(),
                                It.Is<RelationshipRequest>(
                                    r => r.Id == relatie.Id && r.EndDate == uitschrijfDatum.Date.AddDays(-1))))
                    .Returns(new ApiResultValues<Relationship>(relatie))
                    .Verifiable();

                var service = factory.Maak<SyncService>();

                // ACT

                service.LidUitschrijven(adNummer, ploeg.ExternalIdentifier, uitschrijfDatum);

                civiApiMock.Verify(
                    // We zullen een lid maken voor een werkjaar dat nog niet begonnen is.
                    // LidMaken moet de startdatum dan op 1 september zetten.
                    src =>
                        src.RelationshipSave(It.IsAny<string>(), It.IsAny<string>(),
                            It.Is<RelationshipRequest>(
                                r => r.Id == relatie.Id && r.EndDate == uitschrijfDatum.Date.AddDays(-1))),
                    Times.AtLeastOnce);
            }
        }
    }
}
