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
using System.Linq;
using Chiro.Cdf.Ioc;
using Chiro.CiviCrm.Api;
using Chiro.CiviCrm.Api.DataContracts;
using Chiro.CiviCrm.Api.DataContracts.Entities;
using Chiro.CiviCrm.Api.DataContracts.Requests;
using Chiro.CiviSync.Mapping;
using Chiro.Gap.UpdateApi.Client;
using Chiro.Kip.ServiceContracts.DataContracts;
using Moq;
using NUnit.Framework;

namespace Chiro.CiviSync.Services.Test
{
    [TestFixture]
    public class LidBewarenTest: SyncTest
    {
        private readonly DateTime _vandaagZogezegd = new DateTime(2015, 2, 6);
        private const int HuidigWerkJaar = 2014;

        /// <summary>
        /// De begindatum van een actieve lidrelatie is sinds #5282 altijd vandaag of in het verleden,
        /// de einddatum is NULL.
        /// 
        /// We testen hier op begindatum vandaag, einddatum NULL.
        /// </summary>
        [Test]
        public void DatumsLidVolgendWerkjaar()
        {
            // ARRANGE

            Mock<ICiviCrmApi> civiApiMock;
            Mock<IGapUpdateClient> updateHelperMock;
            using (var factory = TestHelper.IocOpzetten(_vandaagZogezegd, out civiApiMock, out updateHelperMock))
            {

                const int adNummer = 2;

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
                    Id = 4,
                    RelationshipResult = new ApiResultValues<Relationship>
                    {
                        Count = 0,
                        IsError = 0
                    }
                };

                civiApiMock.Setup(
                        src =>
                            src.ContactGet(It.IsAny<string>(), It.IsAny<string>(),
                                It.Is<ContactRequest>(r => r.ExternalIdentifier == ploeg.ExternalIdentifier)))
                    .Returns(new ApiResultValues<Contact>(ploeg));
                civiApiMock.Setup(
                        src =>
                            src.ContactGet(It.IsAny<string>(), It.IsAny<string>(),
                                It.Is<ContactRequest>(r => r.ExternalIdentifier == persoon.ExternalIdentifier)))
                    .Returns(new ApiResultValues<Contact>(persoon));

                civiApiMock.Setup(
                        // We zullen een lid maken voor een werkjaar dat nog niet begonnen is.
                        // LidMaken moet de startdatum dan op 1 september zetten.
                        // Future relationships shoud be inactive, see http://forum.civicrm.org/index.php?topic=21327.0
                        src =>
                            src.RelationshipSave(It.IsAny<string>(), It.IsAny<string>(),
                                It.Is<RelationshipRequest>(
                                    r =>
                                        r.StartDate == _vandaagZogezegd && r.EndDate == DateTime.MinValue &&
                                        r.IsActive == true && r.ContactIdA == persoon.Id && r.ContactIdB == ploeg.Id)))
                    .Returns(
                        (string key1, string key2, RelationshipRequest r) =>
                            TestHelper.Map<RelationshipRequest, ApiResultValues<Relationship>>(r))
                    .Verifiable();

                var service = factory.Maak<SyncService>();

                // ACT

                service.LidBewaren(adNummer, new LidGedoe
                {
                    LidType = LidTypeEnum.Kind,
                    OfficieleAfdelingen = new[] {AfdelingEnum.Rakwis},
                    StamNummer = ploeg.ExternalIdentifier
                });

                // ASSERT:

                civiApiMock.Verify(
                    // We zullen een lid maken voor een werkjaar dat nog niet begonnen is.
                    // LidMaken moet de startdatum dan op 1 september zetten.
                    // Future relationships shoud be inactive, see http://forum.civicrm.org/index.php?topic=21327.0
                    src =>
                        src.RelationshipSave(It.IsAny<string>(), It.IsAny<string>(),
                            It.Is<RelationshipRequest>(
                                r =>
                                    r.StartDate == _vandaagZogezegd && r.EndDate == DateTime.MinValue &&
                                    r.IsActive == true && r.ContactIdA == persoon.Id && r.ContactIdB == ploeg.Id)),
                    Times.AtLeastOnce);
            }
        }

        /// <summary>
        /// Als iemand een inactieve relatie heeft, die nog niet zo lang geleden begonnen was,
        /// en die iemand krijgt nu opnieuw een relatie, dan verwachten we dat de oude gerecupereerd wordt.
        /// </summary>
        [Test]
        public void UpdateOpnieuwLidHuidigWerkjaar()
        {
            // ARRANGE

            Mock<ICiviCrmApi> civiApiMock;
            Mock<IGapUpdateClient> updateHelperMock;
            using (var factory = TestHelper.IocOpzetten(_vandaagZogezegd, out civiApiMock, out updateHelperMock))
            {

                const int adNummer = 2;

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
                    Id = 4,
                    RelationshipResult = new ApiResultValues<Relationship>
                    {
                        Count = 1,
                        IsError = 0
                    }
                };
                var relatie = new Relationship
                {
                    Id = 5,
                    Afdeling = Afdeling.Titos,
                    ContactIdA = persoon.Id,
                    ContactIdB = ploeg.Id,
                    StartDate = _vandaagZogezegd.AddDays(-2),
                    EndDate = _vandaagZogezegd.AddDays(-1),
                    IsActive = false
                };
                persoon.RelationshipResult.Values = new[] {relatie};

                DateTime eindeDitWerkJaar = new DateTime(HuidigWerkJaar + 1, 8, 31);

                civiApiMock.Setup(
                        src =>
                            src.ContactGet(It.IsAny<string>(), It.IsAny<string>(),
                                It.Is<ContactRequest>(r => r.ExternalIdentifier == ploeg.ExternalIdentifier)))
                    .Returns(new ApiResultValues<Contact>(ploeg));
                civiApiMock.Setup(
                        src =>
                            src.ContactGet(It.IsAny<string>(), It.IsAny<string>(),
                                It.Is<ContactRequest>(r => r.ExternalIdentifier == persoon.ExternalIdentifier)))
                    .Returns(new ApiResultValues<Contact>(persoon));

                civiApiMock.Setup(
                        // We zullen een lid maken voor een werkjaar dat nog niet begonnen is.
                        // LidMaken moet de startdatum dan op 1 september zetten.
                        src =>
                            src.RelationshipSave(It.IsAny<string>(), It.IsAny<string>(),
                                It.Is<RelationshipRequest>(
                                    r =>
                                        r.StartDate == relatie.StartDate && r.EndDate == DateTime.MinValue &&
                                        r.Id == relatie.Id &&
                                        r.IsActive == true && r.ContactIdA == persoon.Id && r.ContactIdB == ploeg.Id)))
                    .Returns(
                        (string key1, string key2, RelationshipRequest r) =>
                            TestHelper.Map<RelationshipRequest, ApiResultValues<Relationship>>(r))
                    .Verifiable();

                var service = factory.Maak<SyncService>();

                // ACT

                service.LidBewaren(adNummer, new LidGedoe
                {
                    LidType = LidTypeEnum.Kind,
                    OfficieleAfdelingen = new[] {AfdelingEnum.Rakwis},
                    StamNummer = ploeg.ExternalIdentifier
                });

                // ASSERT:
                // Als er niets crasht, is het altijd goed, want kipsync geeft geen feedback.
                civiApiMock.Verify(
                    // We zullen een lid maken voor een werkjaar dat nog niet begonnen is.
                    // LidMaken moet de startdatum dan op 1 september zetten.
                    src =>
                        src.RelationshipSave(It.IsAny<string>(), It.IsAny<string>(),
                            It.Is<RelationshipRequest>(
                                r =>
                                    r.StartDate == relatie.StartDate && r.EndDate == DateTime.MinValue &&
                                    r.Id == relatie.Id &&
                                    r.IsActive == true && r.ContactIdA == persoon.Id && r.ContactIdB == ploeg.Id)),
                    Times.AtLeastOnce);
            }
        }

        /// <summary>
        /// Controleer omzetting afdelingen leden.
        /// </summary>
        [Test]
        public void AfdelingLidHuidigWerkjaar()
        {
            // ARRANGE

            Mock<ICiviCrmApi> civiApiMock;
            Mock<IGapUpdateClient> updateHelperMock;
            using (var factory = TestHelper.IocOpzetten(_vandaagZogezegd, out civiApiMock, out updateHelperMock))
            {

                const int adNummer = 2;

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
                    Id = 4,
                    RelationshipResult = new ApiResultValues<Relationship>
                    {
                        Count = 0,
                        IsError = 0
                    }
                };

                // We gaan inschrijven bij de rakwi's, en nakijken of de juiste afdeling naar CiviCRM gaat.
                const AfdelingEnum gapAfdeling = AfdelingEnum.Rakwis;
                const Afdeling civiAfdeling = Afdeling.Rakwis;

                civiApiMock.Setup(
                        src =>
                            src.ContactGet(It.IsAny<string>(), It.IsAny<string>(),
                                It.Is<ContactRequest>(r => r.ExternalIdentifier == ploeg.ExternalIdentifier)))
                    .Returns(new ApiResultValues<Contact>(ploeg));
                civiApiMock.Setup(
                        src =>
                            src.ContactGet(It.IsAny<string>(), It.IsAny<string>(),
                                It.Is<ContactRequest>(r => r.ExternalIdentifier == persoon.ExternalIdentifier)))
                    .Returns(new ApiResultValues<Contact>(persoon));


                civiApiMock.Setup(
                        src =>
                            src.RelationshipSave(It.IsAny<string>(), It.IsAny<string>(),
                                It.Is<RelationshipRequest>(
                                    r => r.Afdeling == civiAfdeling && r.ContactIdA == persoon.Id &&
                                         r.ContactIdB == ploeg.Id)))
                    .Returns(
                        (string key1, string key2, RelationshipRequest r) =>
                            TestHelper.Map<RelationshipRequest, ApiResultValues<Relationship>>(r))
                    .Verifiable();

                var service = factory.Maak<SyncService>();

                // ACT

                service.LidBewaren(adNummer, new LidGedoe
                {
                    LidType = LidTypeEnum.Kind,
                    OfficieleAfdelingen = new[] {gapAfdeling},
                    StamNummer = ploeg.ExternalIdentifier
                });

                // ASSERT

                civiApiMock.Verify(
                    src =>
                        src.RelationshipSave(It.IsAny<string>(), It.IsAny<string>(),
                            It.Is<RelationshipRequest>(
                                r => r.Afdeling == civiAfdeling && r.ContactIdA == persoon.Id &&
                                     r.ContactIdB == ploeg.Id)),
                    Times.AtLeastOnce);
            }
        }

        /// <summary>
        /// Controleer omzetting afdelingen leiding
        /// </summary>
        [Test]
        public void AfdelingenLeidingHuidigWerkjaar()
        {
            // ARRANGE

            Mock<ICiviCrmApi> civiApiMock;
            Mock<IGapUpdateClient> updateHelperMock;
            using (var factory = TestHelper.IocOpzetten(_vandaagZogezegd, out civiApiMock, out updateHelperMock))
            {

                const int adNummer = 2;

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
                    Id = 4,
                    RelationshipResult = new ApiResultValues<Relationship>
                    {
                        Count = 0,
                        IsError = 0
                    }
                };

                // We gaan inschrijven bij de rakwi's, en nakijken of de juiste afdeling naar CiviCRM gaat.
                const AfdelingEnum gapAfdeling = AfdelingEnum.Rakwis;
                const Afdeling civiAfdeling = Afdeling.Rakwis;

                // Een request om 1 of meerdere contacts op te leveren, levert voor het gemak altijd
                // dezelfde persoon en dezelfde ploeg.

                civiApiMock.Setup(
                        src =>
                            src.ContactGet(It.IsAny<string>(), It.IsAny<string>(),
                                It.Is<ContactRequest>(r => r.ExternalIdentifier == ploeg.ExternalIdentifier)))
                    .Returns(new ApiResultValues<Contact>(ploeg));
                civiApiMock.Setup(
                        src =>
                            src.ContactGet(It.IsAny<string>(), It.IsAny<string>(),
                                It.Is<ContactRequest>(r => r.ExternalIdentifier == persoon.ExternalIdentifier)))
                    .Returns(new ApiResultValues<Contact>(persoon));

                civiApiMock.Setup(
                        src =>
                            src.RelationshipSave(It.IsAny<string>(), It.IsAny<string>(),
                                It.Is<RelationshipRequest>(
                                    r =>
                                        r.Afdeling == Afdeling.Leiding && r.LeidingVan.First() == civiAfdeling &&
                                        r.ContactIdA == persoon.Id && r.ContactIdB == ploeg.Id)))
                    .Returns(
                        (string key1, string key2, RelationshipRequest r) =>
                            TestHelper.Map<RelationshipRequest, ApiResultValues<Relationship>>(r))
                    .Verifiable();

                var service = factory.Maak<SyncService>();

                // ACT

                service.LidBewaren(adNummer, new LidGedoe
                {
                    LidType = LidTypeEnum.Leiding,
                    OfficieleAfdelingen = new[] {gapAfdeling},
                    StamNummer = ploeg.ExternalIdentifier
                });

                // ASSERT

                civiApiMock.Verify(
                    // LidMaken moet de startdatum dan op 1 september zetten.
                    src =>
                        src.RelationshipSave(It.IsAny<string>(), It.IsAny<string>(),
                            It.Is<RelationshipRequest>(
                                r =>
                                    r.Afdeling == Afdeling.Leiding && r.LeidingVan.First() == civiAfdeling &&
                                    r.ContactIdA == persoon.Id && r.ContactIdB == ploeg.Id)),
                    Times.AtLeastOnce);
            }
        }

        /// <summary>
        /// Een inactief lid bewaren moet de uitschrijfdatum zetten.
        /// </summary>
        [Test]
        public void UitschrijvenLidHuidigWerkjaar()
        {
            // ARRANGE

            Mock<ICiviCrmApi> civiApiMock;
            Mock<IGapUpdateClient> updateHelperMock;
            using (var factory = TestHelper.IocOpzetten(_vandaagZogezegd, out civiApiMock, out updateHelperMock))
            {

                const int adNummer = 2;

                DateTime eindeDitWerkJaar = new DateTime(HuidigWerkJaar + 1, 8, 31);
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
                    Id = 4,
                    RelationshipResult = new ApiResultValues<Relationship>
                    {
                        Count = 1,
                        IsError = 0
                    }
                };

                var relatie = new Relationship
                {
                    Id = 5,
                    Afdeling = Afdeling.Titos,
                    ContactIdA = persoon.Id,
                    ContactIdB = ploeg.Id,
                    StartDate = new DateTime(HuidigWerkJaar, 10, 14),
                    EndDate = eindeDitWerkJaar,
                    IsActive = true
                };

                persoon.RelationshipResult.Values = new[] {relatie};

                civiApiMock.Setup(
                        src =>
                            src.ContactGet(It.IsAny<string>(), It.IsAny<string>(),
                                It.Is<ContactRequest>(r => r.ExternalIdentifier == ploeg.ExternalIdentifier)))
                    .Returns(new ApiResultValues<Contact>(ploeg));
                civiApiMock.Setup(
                        src =>
                            src.ContactGet(It.IsAny<string>(), It.IsAny<string>(),
                                It.Is<ContactRequest>(r => r.ExternalIdentifier == persoon.ExternalIdentifier)))
                    .Returns(new ApiResultValues<Contact>(persoon));

                civiApiMock.Setup(
                        src =>
                            src.RelationshipSave(It.IsAny<string>(), It.IsAny<string>(),
                                It.Is<RelationshipRequest>(
                                    r =>
                                        r.StartDate == relatie.StartDate &&
                                        r.EndDate == uitschrijfDatum.Date.AddDays(-1) && r.Id == relatie.Id &&
                                        r.IsActive == false && r.ContactIdA == persoon.Id && r.ContactIdB == ploeg.Id)))
                    .Returns(
                        (string key1, string key2, RelationshipRequest r) =>
                            TestHelper.Map<RelationshipRequest, ApiResultValues<Relationship>>(r))
                    .Verifiable();

                var service = factory.Maak<SyncService>();

                // ACT

                service.LidBewaren(adNummer, new LidGedoe
                {
                    LidType = LidTypeEnum.Kind,
                    OfficieleAfdelingen = new[] {AfdelingEnum.Rakwis},
                    StamNummer = ploeg.ExternalIdentifier,
                    UitschrijfDatum = uitschrijfDatum
                });

                civiApiMock.Verify(
                    // We zullen een lid maken voor een werkjaar dat nog niet begonnen is.
                    // LidMaken moet de startdatum dan op 1 september zetten.
                    src =>
                        src.RelationshipSave(It.IsAny<string>(), It.IsAny<string>(),
                            It.Is<RelationshipRequest>(
                                r =>
                                    r.StartDate == relatie.StartDate && r.EndDate == uitschrijfDatum.Date.AddDays(-1) &&
                                    r.Id == relatie.Id &&
                                    r.IsActive == false && r.ContactIdA == persoon.Id && r.ContactIdB == ploeg.Id)),
                    Times.AtLeastOnce);
            }
        }

        /// <summary>
        /// Als CiviSync een lid moet maken met een ongeldig AD-nummer, dan moet dat AD-nummer als ongeldig
        /// terug naar het GAP.
        /// </summary>
        [Test]
        public void LidBewarenOngeldigAdNummer()
        {
            // ARRANGE

            Mock<ICiviCrmApi> civiApiMock;
            Mock<IGapUpdateClient> updateHelperMock;
            using (var factory = TestHelper.IocOpzetten(_vandaagZogezegd, out civiApiMock, out updateHelperMock))
            {

                const int adNummer = 2;

                // Onze nepdatabase bevat 1 organisatie (TST/0000) en 1 contact (Kees Flodder)
                var ploeg = new Contact
                {
                    ExternalIdentifier = "TST/0001",
                    Id = 1,
                    ContactType = ContactType.Organization
                };

                // We mocken ook GapUpdateClient, die een dummy task oplevert.
                updateHelperMock.Setup(src => src.OngeldigAdNaarGap(It.Is<int>(ad => ad == adNummer))).Verifiable();

                civiApiMock.Setup(
                        src =>
                            src.ContactGet(It.IsAny<string>(), It.IsAny<string>(),
                                It.Is<ContactRequest>(r => r.ExternalIdentifier == ploeg.ExternalIdentifier)))
                    .Returns(new ApiResultValues<Contact>(ploeg));

                // Niet gevonden contact mocken:
                civiApiMock.Setup(
                        src =>
                            src.ContactGet(It.IsAny<string>(), It.IsAny<string>(),
                                It.Is<ContactRequest>(r => r.ExternalIdentifier == adNummer.ToString())))
                    .Returns(new ApiResultValues<Contact>());

                var service = factory.Maak<SyncService>();

                // ACT

                service.LidBewaren(adNummer, new LidGedoe
                {
                    LidType = LidTypeEnum.Kind,
                    OfficieleAfdelingen = new[] {AfdelingEnum.Rakwis},
                    StamNummer = ploeg.ExternalIdentifier
                });

                // ASSERT

                updateHelperMock.Verify(src => src.OngeldigAdNaarGap(It.Is<Int32>(ad => ad == adNummer)),
                    Times.AtLeastOnce);
            }
        }
    }
}
