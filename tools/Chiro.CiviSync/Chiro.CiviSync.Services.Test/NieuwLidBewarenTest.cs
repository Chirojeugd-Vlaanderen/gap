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
using Chiro.CiviCrm.Api.DataContracts.EntityRequests;
using Chiro.CiviCrm.Api.DataContracts.Requests;
using Chiro.Kip.ServiceContracts.DataContracts;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Chiro.CiviSync.Services.Test
{
    [TestClass]
    public class NieuwLidBewarenTest
    {
        private Mock<ICiviCrmApi> _civiApiMock;
        private static int _nextId = 1;

        [ClassInitialize]
        public static void InitialilzeTestClass(TestContext c)
        {
            TestHelper.MappingsCreeren();
        }

        [TestInitialize]
        public void InitializeTest()
        {
            _civiApiMock = TestHelper.IocOpzetten(new DateTime(2015, 02, 05));
        }

        /// <summary>
        /// Als een nieuwe persoon al gevonden is in de CiviCRM-database, dan wordt het GAP-ID
        /// van die persoon aangepast. Voor die call verwacht CiviCRM een ContactType.
        /// </summary>
        [TestMethod]
        public void ContactTypeBijGematchtLid()
        {
            // ARRANGE

            // Onze nepdatabase bevat 1 organisatie (TST/0000) en 1 contact (Kees Flodder)
            var ploeg = new Contact {ExternalIdentifier = "TST/0001", Id = 1, ContactType = ContactType.Organization};
            var persoon = new Contact {ExternalIdentifier = "2", FirstName = "Kees", LastName = "Flodder", GapId = 3};

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
                            Values = new[] {_civiApiMock.Object.ContactGetSingle(key1, key2, r)}
                        };
                        result.Id = result.Values.First().Id;
                        return result;
                    });

            // RelationshipSave moet gewoon doen of het werkt.
            _civiApiMock.Setup(
                src => src.RelationshipSave(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<RelationshipRequest>()))
                .Returns((string key1, string key2, RelationshipRequest r) =>
                    Mapper.Map<RelationshipRequest, ApiResultValues<Relationship>>(r));

            // Voor deze test moet elke 'ContactSave' een individual aanmaken.
            _civiApiMock.Setup(src => src.ContactSave(It.IsAny<string>(), It.IsAny<string>(),
                It.Is<ContactRequest>(r => r.ContactType == ContactType.Individual))).Returns(
                    (string key1, string key2, ContactRequest r) => SomeSaveResult(r)).Verifiable();

            var service = Factory.Maak<SyncService>();

            // ACT

            service.NieuwLidBewaren(
                new PersoonDetails
                {
                    Adres = new Adres {Straat = "Kipdorp", HuisNr = 30, PostNr = 2000, WoonPlaats = "Antwerpen"},
                    AdresType = AdresTypeEnum.Thuis,
                    Communicatie =
                        new[]
                        {
                            new CommunicatieMiddel
                            {
                                GeenMailings = true,
                                Type = CommunicatieType.TelefoonNummer,
                                Waarde = "03-231 07 95"
                            }
                        },
                    Persoon =
                        new Persoon
                        {
                            AdNummer = null,
                            GeboorteDatum = new DateTime(2003, 11, 8),
                            Geslacht = GeslachtsEnum.Vrouw,
                            ID = persoon.GapId ?? 0,
                            Naam = persoon.LastName,
                            VoorNaam = persoon.FirstName
                        }
                },
                new LidGedoe
                {
                    EindeInstapPeriode = new DateTime(2015, 02, 27),
                    LidType = LidTypeEnum.Kind,
                    OfficieleAfdelingen = new[] {AfdelingEnum.Rakwis},
                    StamNummer = ploeg.ExternalIdentifier,
                    WerkJaar = 2014
                });

            // ASSERT

            _civiApiMock.Verify(src => src.ContactSave(It.IsAny<string>(), It.IsAny<string>(),
                It.Is<ContactRequest>(r => r.ContactType == ContactType.Individual)), Times.AtLeastOnce);
        }

        /// <summary>
        /// Als CiviSync persoonsgegevens krijgt zonder AD-nummer of GAP-ID, dan probeert CiviSync zelf
        /// te matchen. We checken even of CiviSync op zoek gaat naar een persoon van het juiste geslacht
        /// (want dat deden we al wel eens mis.)
        /// </summary>
        [TestMethod]
        public void CorrectGeslachtBijTeMatchenLid()
        {
            // ARRANGE

            // We zullen straks de persoon kunnen matchen op zijn telefoonnummer.
            const string somePhoneNumber = "03-231 07 95";

            // Onze nepdatabase bevat 1 organisatie (TST/0000) en 1 contact (Kees Flodder)
            var ploeg = new Contact { ExternalIdentifier = "TST/0001", Id = 1, ContactType = ContactType.Organization };
            var persoon = new Contact
            {
                ExternalIdentifier = "2",
                FirstName = "Kees",
                LastName = "Flodder",
                Gender = Gender.Female,
                Id = 3,
                RelationshipResult = new ApiResultValues<Relationship>
                {
                    Count = 0,
                    IsError = 0
                },
                PhoneResult =
                    new ApiResultValues<Phone>
                    {
                        Count = 1,
                        IsError = 0,
                        Values = new[] {new Phone {PhoneNumber = somePhoneNumber}}
                    }
            };

            // Een request om 1 of meerdere contacts op te leveren, levert voor het gemak altijd
            // dezelfde persoon en dezelfde ploeg.

            // Verwacht dat er zeker op gender gezocht wordt. Lever persoon op.
            _civiApiMock.Setup(
                src =>
                    src.ContactGet(It.IsAny<string>(), It.IsAny<string>(),
                        It.Is<ContactRequest>(r => r.Gender == persoon.Gender && r.ContactType == ContactType.Individual)))
                .Returns(
                    new ApiResultValues<Contact> {Count = 1, IsError = 0, Values = new[] {persoon}}).Verifiable();
            // Zoeken op eender welke organisatie levert de ploeg op.
            _civiApiMock.Setup(
                src =>
                    src.ContactGet(It.IsAny<string>(), It.IsAny<string>(),
                        It.Is<ContactRequest>(r => r.ContactType == ContactType.Organization))).Returns(
                            new ApiResultValues<Contact> {Count = 1, IsError = 0, Values = new[] {ploeg}});

            // RelationshipSave moet gewoon doen of het werkt.
            _civiApiMock.Setup(
                src => src.RelationshipSave(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<RelationshipRequest>()))
                .Returns((string key1, string key2, RelationshipRequest r) =>
                    Mapper.Map<RelationshipRequest, ApiResultValues<Relationship>>(r));

            // Voor deze test moet elke 'ContactSave' een individual aanmaken.
            _civiApiMock.Setup(src => src.ContactSave(It.IsAny<string>(), It.IsAny<string>(),
                It.IsAny<ContactRequest>())).Returns(
                    (string key1, string key2, ContactRequest r) =>
                    {
                        Assert.AreEqual(ContactType.Individual, r.ContactType);
                        return SomeSaveResult(r);
                    });

            var service = Factory.Maak<SyncService>();

            // ACT

            service.NieuwLidBewaren(
                new PersoonDetails
                {
                    Adres = new Adres { Straat = "Kipdorp", HuisNr = 30, PostNr = 2000, WoonPlaats = "Antwerpen" },
                    AdresType = AdresTypeEnum.Thuis,
                    Communicatie =
                        new[]
                        {
                            new CommunicatieMiddel
                            {
                                GeenMailings = true,
                                Type = CommunicatieType.TelefoonNummer,
                                Waarde = somePhoneNumber
                            }
                        },
                    Persoon =
                        new Persoon
                        {
                            AdNummer = null,
                            GeboorteDatum = new DateTime(2003, 11, 8),
                            Geslacht = GeslachtsEnum.Vrouw,
                            ID = persoon.GapId ?? 0,
                            Naam = persoon.LastName,
                            VoorNaam = persoon.FirstName
                        }
                },
                new LidGedoe
                {
                    EindeInstapPeriode = new DateTime(2015, 02, 27),
                    LidType = LidTypeEnum.Kind,
                    OfficieleAfdelingen = new[] { AfdelingEnum.Rakwis },
                    StamNummer = ploeg.ExternalIdentifier,
                    WerkJaar = 2014
                });

            // ASSERT

            _civiApiMock.Verify(
                src =>
                    src.ContactGet(It.IsAny<string>(), It.IsAny<string>(),
                        It.Is<ContactRequest>(r => r.Gender == persoon.Gender && r.ContactType == ContactType.Individual)),
                Times.AtLeastOnce);
        }

        private ApiResultValues<Contact> SomeSaveResult(ContactRequest contactRequest)
        {
            return new ApiResultValues<Contact>
            {
                Count = 1,
                ErrorMessage = String.Empty,
                Id = _nextId++,
                IsError = 0,
                Values = new Contact[] {Mapper.Map<ContactRequest, Contact>(contactRequest)}
            };
        }
    }
}