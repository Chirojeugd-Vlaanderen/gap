﻿/*
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
using System.Linq;
using AutoMapper;
using Chiro.Cdf.Ioc;
using Chiro.CiviCrm.Api;
using Chiro.CiviCrm.Api.DataContracts;
using Chiro.CiviCrm.Api.DataContracts.Entities;
using Chiro.CiviCrm.Api.DataContracts.Requests;
using Chiro.Kip.ServiceContracts.DataContracts;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Chiro.CiviSync.Services.Test
{
    [TestClass]
    public class LidBewarenTest
    {
        private Mock<ICiviCrmApi> _civiApiMock;
        private static int _nextId = 1;

        private readonly DateTime _vandaagZogezegd = new DateTime(2015, 2, 6);
        private const int VorigWerkJaar = 2013;
        private const int HuidigWerkJaar = 2014;
        private const int VolgendWerkjaar = 2015;

        [ClassInitialize]
        public static void InitialilzeTestClass(TestContext c)
        {
            TestHelper.MappingsCreeren();
        }

        [TestInitialize]
        public void InitializeTest()
        {
            _civiApiMock = TestHelper.IocOpzetten(_vandaagZogezegd);
        }

        /// <summary>
        /// Als een lid wordt ingeschreven voor een volgend werkjaar, dan moet de begindatum
        /// 1 september zijn.
        /// </summary>
        [TestMethod]
        public void DatumsLidVolgendWerkjaar()
        {
            // ARRANGE

            const int adNummer = 2;

            // Onze nepdatabase bevat 1 organisatie (TST/0000) en 1 contact (Kees Flodder)
            var ploeg = new Contact { ExternalIdentifier = "TST/0001", Id = 1, ContactType = ContactType.Organization };
            var persoon = new Contact { ExternalIdentifier = adNummer.ToString(), FirstName = "Kees", LastName = "Flodder", GapId = 3 };

            DateTime beginVolgendWerkjaar = new DateTime(VolgendWerkjaar, 9, 1);
            DateTime eindeVolgendWerkJaar = new DateTime(VolgendWerkjaar + 1, 8, 31);

            // Een request om 1 of meerdere contacts op te leveren, levert voor het gemak altijd
            // dezelfde persoon en dezelfde ploeg.

            _civiApiMock.Setup(
                src => src.ContactGetSingle(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<ContactRequest>()))
                .Returns(
                    (string key1, string key2, ContactRequest r) =>
                    {
                        var result = r.ContactType == ContactType.Organization
                            ? ploeg
                            : persoon;
                        // Als relaties gevraagd zijn, lever dan gewoon een lege lijst op.
                        if (r.RelationshipGetRequest != null)
                        {
                            result.RelationshipResult = new ApiResultValues<Relationship>
                            {
                                Count = 0,
                                IsError = 0
                            };
                        }
                        return result;
                    });

            _civiApiMock.Setup(
                src => src.ContactGet(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<ContactRequest>()))
                .Returns(
                    (string key1, string key2, ContactRequest r) =>
                    {
                        var result = new ApiResultValues<Contact>
                        {
                            Count = 1,
                            IsError = 0,
                            Values = new[] { _civiApiMock.Object.ContactGetSingle(key1, key2, r) }
                        };
                        result.Id = result.Values.First().Id;
                        return result;
                    });

            _civiApiMock.Setup(
                // We zullen een lid maken voor een werkjaar dat nog niet begonnen is.
                // LidMaken moet de startdatum dan op 1 september zetten.
                // Future relationships shoud be inactive, see http://forum.civicrm.org/index.php?topic=21327.0
                src =>
                    src.RelationshipSave(It.IsAny<string>(), It.IsAny<string>(),
                        It.Is<RelationshipRequest>(
                            r =>
                                r.StartDate == beginVolgendWerkjaar && r.EndDate == eindeVolgendWerkJaar &&
                                r.IsActive == false)))
                .Returns(
                    (string key1, string key2, RelationshipRequest r) =>
                        Mapper.Map<RelationshipRequest, ApiResultValues<Relationship>>(r)).Verifiable();

            var service = Factory.Maak<SyncService>();

            // ACT

            service.LidBewaren(adNummer, new LidGedoe
            {
                LidType = LidTypeEnum.Kind,
                OfficieleAfdelingen = new[] {AfdelingEnum.Rakwis},
                StamNummer = ploeg.ExternalIdentifier,
                WerkJaar = VolgendWerkjaar
            });

            // ASSERT:

            _civiApiMock.Verify(
                // We zullen een lid maken voor een werkjaar dat nog niet begonnen is.
                // LidMaken moet de startdatum dan op 1 september zetten.
                // Future relationships shoud be inactive, see http://forum.civicrm.org/index.php?topic=21327.0
                src =>
                    src.RelationshipSave(It.IsAny<string>(), It.IsAny<string>(),
                        It.Is<RelationshipRequest>(
                            r =>
                                r.StartDate == beginVolgendWerkjaar && r.EndDate == eindeVolgendWerkJaar &&
                                r.IsActive == false)), Times.AtLeastOnce);
        }

        /// <summary>
        /// Als een lid wordt ingeschreven voor dit werkjaar, dan verwachten we als begindatum
        /// van de lidrelatie vandaag, en de einddatum op 31 augustus.
        /// </summary>
        [TestMethod]
        public void DatumsLidHuidigWerkjaar()
        {
            // ARRANGE

            const int adNummer = 2;

            // Onze nepdatabase bevat 1 organisatie (TST/0000) en 1 contact (Kees Flodder)
            var ploeg = new Contact { ExternalIdentifier = "TST/0001", Id = 1, ContactType = ContactType.Organization };
            var persoon = new Contact { ExternalIdentifier = adNummer.ToString(), FirstName = "Kees", LastName = "Flodder", GapId = 3 };

            DateTime eindeDitWerkJaar = new DateTime(HuidigWerkJaar + 1, 8, 31);

            // Een request om 1 of meerdere contacts op te leveren, levert voor het gemak altijd
            // dezelfde persoon en dezelfde ploeg.

            _civiApiMock.Setup(
                src => src.ContactGetSingle(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<ContactRequest>()))
                .Returns(
                    (string key1, string key2, ContactRequest r) =>
                    {
                        var result = r.ContactType == ContactType.Organization
                            ? ploeg
                            : persoon;
                        // Als relaties gevraagd zijn, lever dan gewoon een lege lijst op.
                        if (r.RelationshipGetRequest != null)
                        {
                            result.RelationshipResult = new ApiResultValues<Relationship>
                            {
                                Count = 0,
                                IsError = 0
                            };
                        }
                        return result;
                    });

            _civiApiMock.Setup(
                src => src.ContactGet(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<ContactRequest>()))
                .Returns(
                    (string key1, string key2, ContactRequest r) =>
                    {
                        var result = new ApiResultValues<Contact>
                        {
                            Count = 1,
                            IsError = 0,
                            Values = new[] { _civiApiMock.Object.ContactGetSingle(key1, key2, r) }
                        };
                        result.Id = result.Values.First().Id;
                        return result;
                    });

            _civiApiMock.Setup(
                src =>
                    src.RelationshipSave(It.IsAny<string>(), It.IsAny<string>(),
                        It.Is<RelationshipRequest>(
                            r => r.StartDate == _vandaagZogezegd && r.EndDate == eindeDitWerkJaar && r.IsActive == true)))
                .Returns(
                    (string key1, string key2, RelationshipRequest r) =>
                        Mapper.Map<RelationshipRequest, ApiResultValues<Relationship>>(r)).Verifiable();

            var service = Factory.Maak<SyncService>();

            // ACT

            service.LidBewaren(adNummer, new LidGedoe
            {
                LidType = LidTypeEnum.Kind,
                OfficieleAfdelingen = new[] { AfdelingEnum.Rakwis },
                StamNummer = ploeg.ExternalIdentifier,
                WerkJaar = HuidigWerkJaar
            });

            // ASSERT

            _civiApiMock.Verify(
                src =>
                    src.RelationshipSave(It.IsAny<string>(), It.IsAny<string>(),
                        It.Is<RelationshipRequest>(
                            r => r.StartDate == _vandaagZogezegd && r.EndDate == eindeDitWerkJaar && r.IsActive == true)),
                Times.AtLeastOnce);
        }

        /// <summary>
        /// Als iemand al (inactief) lid was, en die persoon wordt nu opnieuw lid,
        /// dan verwachten we een actief lid met einddatum 31 augustus.
        /// </summary>
        [TestMethod]
        public void UpdateOpnieuwLidHuidigWerkjaar()
        {
            // ARRANGE

            const int adNummer = 2;

            // Onze nepdatabase bevat 1 organisatie (TST/0000) en 1 contact (Kees Flodder)
            var ploeg = new Contact { ExternalIdentifier = "TST/0001", Id = 1, ContactType = ContactType.Organization };
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
                EndDate = new DateTime(HuidigWerkJaar, 11, 25),
                IsActive = false
            };
            DateTime eindeDitWerkJaar = new DateTime(HuidigWerkJaar + 1, 8, 31);
            
            // Een request om 1 of meerdere contacts op te leveren, levert voor het gemak altijd
            // dezelfde persoon en dezelfde ploeg.

            _civiApiMock.Setup(
                src => src.ContactGetSingle(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<ContactRequest>()))
                .Returns(
                    (string key1, string key2, ContactRequest r) =>
                    {
                        var result = r.ContactType == ContactType.Organization
                            ? ploeg
                            : persoon;
                        // We veronderstellen dat de persoon al (inactief) lid was dit werkjaar.
                        if (r.RelationshipGetRequest != null)
                        {
                            result.RelationshipResult = new ApiResultValues<Relationship>
                            {
                                Count = 1,
                                IsError = 0,
                                Values = new [] { relatie }
                            };
                        }
                        return result;
                    });

            _civiApiMock.Setup(
                src => src.ContactGet(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<ContactRequest>()))
                .Returns(
                    (string key1, string key2, ContactRequest r) =>
                    {
                        var result = new ApiResultValues<Contact>
                        {
                            Count = 1,
                            IsError = 0,
                            Values = new[] { _civiApiMock.Object.ContactGetSingle(key1, key2, r) }
                        };
                        result.Id = result.Values.First().Id;
                        return result;
                    });

            _civiApiMock.Setup(
                // We zullen een lid maken voor een werkjaar dat nog niet begonnen is.
                // LidMaken moet de startdatum dan op 1 september zetten.
                src =>
                    src.RelationshipSave(It.IsAny<string>(), It.IsAny<string>(),
                        It.Is<RelationshipRequest>(
                            r =>
                                r.StartDate == relatie.StartDate && r.EndDate == eindeDitWerkJaar && r.Id == relatie.Id &&
                                r.IsActive == true)))
                .Returns(
                    (string key1, string key2, RelationshipRequest r) =>
                        Mapper.Map<RelationshipRequest, ApiResultValues<Relationship>>(r)).Verifiable();

            var service = Factory.Maak<SyncService>();

            // ACT

            service.LidBewaren(adNummer, new LidGedoe
            {
                LidType = LidTypeEnum.Kind,
                OfficieleAfdelingen = new[] { AfdelingEnum.Rakwis },
                StamNummer = ploeg.ExternalIdentifier,
                WerkJaar = HuidigWerkJaar
            });

            // ASSERT:
            // Als er niets crasht, is het altijd goed, want kipsync geeft geen feedback.
            _civiApiMock.Verify(
                // We zullen een lid maken voor een werkjaar dat nog niet begonnen is.
                // LidMaken moet de startdatum dan op 1 september zetten.
                src =>
                    src.RelationshipSave(It.IsAny<string>(), It.IsAny<string>(),
                        It.Is<RelationshipRequest>(
                            r =>
                                r.StartDate == relatie.StartDate && r.EndDate == eindeDitWerkJaar && r.Id == relatie.Id &&
                                r.IsActive == true)), Times.AtLeastOnce);
        }

        /// <summary>
        /// Als een lid wordt ingeschreven voor een werkjaar dat al voorbij is,
        /// dan zijn start- en eineddatum 31 augustus (om toch iets te doen).
        /// </summary>
        [TestMethod]
        public void DatumsLidVorigWerkjaar()
        {
            // ARRANGE

            const int adNummer = 2;

            // Onze nepdatabase bevat 1 organisatie (TST/0000) en 1 contact (Kees Flodder)
            var ploeg = new Contact { ExternalIdentifier = "TST/0001", Id = 1, ContactType = ContactType.Organization };
            var persoon = new Contact { ExternalIdentifier = adNummer.ToString(), FirstName = "Kees", LastName = "Flodder", GapId = 3 };
            DateTime eindeVorigWerkJaar = new DateTime(VorigWerkJaar + 1, 8, 31);

            // Een request om 1 of meerdere contacts op te leveren, levert voor het gemak altijd
            // dezelfde persoon en dezelfde ploeg.

            _civiApiMock.Setup(
                src => src.ContactGetSingle(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<ContactRequest>()))
                .Returns(
                    (string key1, string key2, ContactRequest r) =>
                    {
                        var result = r.ContactType == ContactType.Organization
                            ? ploeg
                            : persoon;
                        // Als relaties gevraagd zijn, lever dan gewoon een lege lijst op.
                        if (r.RelationshipGetRequest != null)
                        {
                            result.RelationshipResult = new ApiResultValues<Relationship>
                            {
                                Count = 0,
                                IsError = 0
                            };
                        }
                        return result;
                    });

            _civiApiMock.Setup(
                src => src.ContactGet(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<ContactRequest>()))
                .Returns(
                    (string key1, string key2, ContactRequest r) =>
                    {
                        var result = new ApiResultValues<Contact>
                        {
                            Count = 1,
                            IsError = 0,
                            Values = new[] { _civiApiMock.Object.ContactGetSingle(key1, key2, r) }
                        };
                        result.Id = result.Values.First().Id;
                        return result;
                    });

            _civiApiMock.Setup(
                // We zullen een lid maken voor een werkjaar dat al voorbij is.
                // We verwachten dat start- en einddatum 31 autustus zullen zijn.
                src =>
                    src.RelationshipSave(It.IsAny<string>(), It.IsAny<string>(),
                        It.Is<RelationshipRequest>(
                            r =>
                                r.StartDate == eindeVorigWerkJaar && r.EndDate == eindeVorigWerkJaar &&
                                r.IsActive == false)))
                .Returns(
                    (string key1, string key2, RelationshipRequest r) =>
                        Mapper.Map<RelationshipRequest, ApiResultValues<Relationship>>(r)).Verifiable();

            var service = Factory.Maak<SyncService>();

            // ACT

            service.LidBewaren(adNummer, new LidGedoe
            {
                LidType = LidTypeEnum.Kind,
                OfficieleAfdelingen = new[] { AfdelingEnum.Rakwis },
                StamNummer = ploeg.ExternalIdentifier,
                WerkJaar = VorigWerkJaar
            });

            // ASSERT:

            _civiApiMock.Verify(
                // We zullen een lid maken voor een werkjaar dat al voorbij is.
                // We verwachten dat start- en einddatum 31 autustus zullen zijn.
                src =>
                    src.RelationshipSave(It.IsAny<string>(), It.IsAny<string>(),
                        It.Is<RelationshipRequest>(
                            r =>
                                r.StartDate == eindeVorigWerkJaar && r.EndDate == eindeVorigWerkJaar &&
                                r.IsActive == false)), Times.AtLeastOnce);
        }

        /// <summary>
        /// Controleer omzetting afdelingen leden.
        /// </summary>
        [TestMethod]
        public void AfdelingLidHuidigWerkjaar()
        {
            // ARRANGE

            const int adNummer = 2;

            // Onze nepdatabase bevat 1 organisatie (TST/0000) en 1 contact (Kees Flodder)
            var ploeg = new Contact { ExternalIdentifier = "TST/0001", Id = 1, ContactType = ContactType.Organization };
            var persoon = new Contact { ExternalIdentifier = adNummer.ToString(), FirstName = "Kees", LastName = "Flodder", GapId = 3 };

            // We gaan inschrijven bij de rakwi's, en nakijken of de juiste afdeling naar CiviCRM gaat.
            AfdelingEnum gapAfdeling = AfdelingEnum.Rakwis;
            Afdeling civiAfdeling = Afdeling.Rakwis;

            // Een request om 1 of meerdere contacts op te leveren, levert voor het gemak altijd
            // dezelfde persoon en dezelfde ploeg.

            _civiApiMock.Setup(
                src => src.ContactGetSingle(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<ContactRequest>()))
                .Returns(
                    (string key1, string key2, ContactRequest r) =>
                    {
                        var result = r.ContactType == ContactType.Organization
                            ? ploeg
                            : persoon;
                        // Als relaties gevraagd zijn, lever dan gewoon een lege lijst op.
                        if (r.RelationshipGetRequest != null)
                        {
                            result.RelationshipResult = new ApiResultValues<Relationship>
                            {
                                Count = 0,
                                IsError = 0
                            };
                        }
                        return result;
                    });

            _civiApiMock.Setup(
                src => src.ContactGet(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<ContactRequest>()))
                .Returns(
                    (string key1, string key2, ContactRequest r) =>
                    {
                        var result = new ApiResultValues<Contact>
                        {
                            Count = 1,
                            IsError = 0,
                            Values = new[] { _civiApiMock.Object.ContactGetSingle(key1, key2, r) }
                        };
                        result.Id = result.Values.First().Id;
                        return result;
                    });

            _civiApiMock.Setup(
                src =>
                    src.RelationshipSave(It.IsAny<string>(), It.IsAny<string>(),
                        It.Is<RelationshipRequest>(r => r.Afdeling == civiAfdeling)))
                .Returns(
                    (string key1, string key2, RelationshipRequest r) =>
                        Mapper.Map<RelationshipRequest, ApiResultValues<Relationship>>(r)).Verifiable();

            var service = Factory.Maak<SyncService>();

            // ACT

            service.LidBewaren(adNummer, new LidGedoe
            {
                LidType = LidTypeEnum.Kind,
                OfficieleAfdelingen = new[] { gapAfdeling },
                StamNummer = ploeg.ExternalIdentifier,
                WerkJaar = HuidigWerkJaar
            });

            // ASSERT

            _civiApiMock.Verify(
                src =>
                    src.RelationshipSave(It.IsAny<string>(), It.IsAny<string>(),
                        It.Is<RelationshipRequest>(r => r.Afdeling == civiAfdeling)), Times.AtLeastOnce);
        }

        /// <summary>
        /// Controleer omzetting afdelingen leiding
        /// </summary>
        [TestMethod]
        public void AfdelingenLeidingHuidigWerkjaar()
        {
            // ARRANGE

            const int adNummer = 2;

            // Onze nepdatabase bevat 1 organisatie (TST/0000) en 1 contact (Kees Flodder)
            var ploeg = new Contact { ExternalIdentifier = "TST/0001", Id = 1, ContactType = ContactType.Organization };
            var persoon = new Contact { ExternalIdentifier = adNummer.ToString(), FirstName = "Kees", LastName = "Flodder", GapId = 3 };

            // We gaan inschrijven bij de rakwi's, en nakijken of de juiste afdeling naar CiviCRM gaat.
            AfdelingEnum gapAfdeling = AfdelingEnum.Rakwis;
            Afdeling civiAfdeling = Afdeling.Rakwis;

            // Een request om 1 of meerdere contacts op te leveren, levert voor het gemak altijd
            // dezelfde persoon en dezelfde ploeg.

            _civiApiMock.Setup(
                src => src.ContactGetSingle(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<ContactRequest>()))
                .Returns(
                    (string key1, string key2, ContactRequest r) =>
                    {
                        var result = r.ContactType == ContactType.Organization
                            ? ploeg
                            : persoon;
                        // Als relaties gevraagd zijn, lever dan gewoon een lege lijst op.
                        if (r.RelationshipGetRequest != null)
                        {
                            result.RelationshipResult = new ApiResultValues<Relationship>
                            {
                                Count = 0,
                                IsError = 0
                            };
                        }
                        return result;
                    });

            _civiApiMock.Setup(
                src => src.ContactGet(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<ContactRequest>()))
                .Returns(
                    (string key1, string key2, ContactRequest r) =>
                    {
                        var result = new ApiResultValues<Contact>
                        {
                            Count = 1,
                            IsError = 0,
                            Values = new[] { _civiApiMock.Object.ContactGetSingle(key1, key2, r) }
                        };
                        result.Id = result.Values.First().Id;
                        return result;
                    });

            _civiApiMock.Setup(
                src =>
                    src.RelationshipSave(It.IsAny<string>(), It.IsAny<string>(),
                        It.Is<RelationshipRequest>(
                            r => r.Afdeling == Afdeling.Leiding && r.LeidingVan.First() == civiAfdeling)))
                .Returns(
                    (string key1, string key2, RelationshipRequest r) =>
                        Mapper.Map<RelationshipRequest, ApiResultValues<Relationship>>(r)).Verifiable();

            var service = Factory.Maak<SyncService>();

            // ACT

            service.LidBewaren(adNummer, new LidGedoe
            {
                LidType = LidTypeEnum.Leiding,
                OfficieleAfdelingen = new[] { gapAfdeling },
                StamNummer = ploeg.ExternalIdentifier,
                WerkJaar = HuidigWerkJaar
            });

            // ASSERT

            _civiApiMock.Verify(
                // LidMaken moet de startdatum dan op 1 september zetten.
                src =>
                    src.RelationshipSave(It.IsAny<string>(), It.IsAny<string>(),
                        It.Is<RelationshipRequest>(
                            r => r.Afdeling == Afdeling.Leiding && r.LeidingVan.First() == civiAfdeling)),
                Times.AtLeastOnce);
        }

        /// <summary>
        /// Een inactief lid bewaren moet de uitschrijfdatum zetten.
        /// </summary>
        [TestMethod]
        public void UitschrijvenLidHuidigWerkjaar()
        {
            // ARRANGE

            const int adNummer = 2;

            DateTime eindeDitWerkJaar = new DateTime(HuidigWerkJaar + 1, 8, 31);
            DateTime uitschrijfDatum = new DateTime(HuidigWerkJaar, 11, 20);

            // Onze nepdatabase bevat 1 organisatie (TST/0000) en 1 contact (Kees Flodder)
            var ploeg = new Contact { ExternalIdentifier = "TST/0001", Id = 1, ContactType = ContactType.Organization };
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
                EndDate = eindeDitWerkJaar,
                IsActive = true
            };

            // Een request om 1 of meerdere contacts op te leveren, levert voor het gemak altijd
            // dezelfde persoon en dezelfde ploeg.

            _civiApiMock.Setup(
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
                                Values = new[] { relatie }
                            };
                        }
                        return result;
                    });

            _civiApiMock.Setup(
                src => src.ContactGet(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<ContactRequest>()))
                .Returns(
                    (string key1, string key2, ContactRequest r) =>
                    {
                        var result = new ApiResultValues<Contact>
                        {
                            Count = 1,
                            IsError = 0,
                            Values = new[] { _civiApiMock.Object.ContactGetSingle(key1, key2, r) }
                        };
                        result.Id = result.Values.First().Id;
                        return result;
                    });

            _civiApiMock.Setup(
                src =>
                    src.RelationshipSave(It.IsAny<string>(), It.IsAny<string>(),
                        It.Is<RelationshipRequest>(
                            r =>
                                r.StartDate == relatie.StartDate && r.EndDate == uitschrijfDatum && r.Id == relatie.Id &&
                                r.IsActive == false)))
                .Returns(
                    (string key1, string key2, RelationshipRequest r) =>
                        Mapper.Map<RelationshipRequest, ApiResultValues<Relationship>>(r)).Verifiable();

            var service = Factory.Maak<SyncService>();

            // ACT

            service.LidBewaren(adNummer, new LidGedoe
            {
                LidType = LidTypeEnum.Kind,
                OfficieleAfdelingen = new[] { AfdelingEnum.Rakwis },
                StamNummer = ploeg.ExternalIdentifier,
                WerkJaar = HuidigWerkJaar,
                UitschrijfDatum = uitschrijfDatum
            });

            _civiApiMock.Verify(
                // We zullen een lid maken voor een werkjaar dat nog niet begonnen is.
                // LidMaken moet de startdatum dan op 1 september zetten.
                src =>
                    src.RelationshipSave(It.IsAny<string>(), It.IsAny<string>(),
                        It.Is<RelationshipRequest>(
                            r =>
                                r.StartDate == relatie.StartDate && r.EndDate == uitschrijfDatum && r.Id == relatie.Id &&
                                r.IsActive == false)), Times.AtLeastOnce);
        }
    }
}