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
using System.Linq;
using AutoMapper;
using Chiro.Cdf.Ioc;
using Chiro.CiviCrm.Api;
using Chiro.CiviCrm.Api.DataContracts;
using Chiro.CiviCrm.Api.DataContracts.Entities;
using Chiro.CiviCrm.Api.DataContracts.Requests;
using Chiro.CiviSync.Mapping;
using Chiro.Gap.UpdateApi.Client;
using Chiro.Kip.ServiceContracts.DataContracts;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Chiro.CiviSync.Services.Test
{
    [TestClass]
    public class PersonenBewarenTest
    {
        private static int _nextId = 1;

        [ClassInitialize]
        public static void InitialilzeTestClass(TestContext c)
        {
            // creer mappings voor de service
            MappingHelper.MappingsDefinieren();
            // creer mappings voor de tests
            TestHelper.MappingsCreeren();
        }

        /// <summary>
        /// Als een nieuwe persoon al gevonden is in de CiviCRM-database, dan wordt het GAP-ID
        /// van die persoon aangepast. Voor die call verwacht CiviCRM een ContactType.
        /// </summary>
        [TestMethod]
        public void ContactTypeBijGematchtLid()
        {
            // ARRANGE

            Mock<ICiviCrmApi> civiApiMock;
            Mock<IGapUpdateClient> updateHelperMock;
            IDiContainer factory;
            TestHelper.IocOpzetten(new DateTime(2015, 02, 05), out factory, out civiApiMock, out updateHelperMock);

            // Onze nepdatabase bevat 1 organisatie (TST/0000) en 1 contact (Kees Flodder)
            var ploeg = new Contact {ExternalIdentifier = "TST/0001", Id = 1, ContactType = ContactType.Organization};
            var persoon = new Contact
            {
                ExternalIdentifier = "2",
                FirstName = "Kees",
                LastName = "Flodder",
                GapId = 3,
                Id = 5,
                // Voor het gemak heeft ons contact nog geen bestaande releationships.
                RelationshipResult = new ApiResultValues<Relationship>(),
            };

            // Een request om 1 of meerdere contacts op te leveren, levert voor het gemak altijd
            // dezelfde persoon en dezelfde ploeg.

            civiApiMock.Setup(
                src =>
                    src.ContactGet(It.IsAny<string>(), It.IsAny<string>(),
                        It.Is<ContactRequest>(
                            cr =>
                                cr.ExternalIdentifier == ploeg.ExternalIdentifier ||
                                cr.ContactType == ContactType.Organization)))
                .Returns(new ApiResultValues<Contact>(ploeg));

            civiApiMock.Setup(
                src =>
                    src.ContactGet(It.IsAny<string>(), It.IsAny<string>(),
                        It.Is<ContactRequest>(
                            cr =>
                                cr.ExternalIdentifier == persoon.ExternalIdentifier ||
                                cr.ContactType == ContactType.Individual || cr.GapId == persoon.GapId)))
                .Returns(new ApiResultValues<Contact>(persoon));


            // RelationshipSave moet gewoon doen of het werkt.
            civiApiMock.Setup(
                src => src.RelationshipSave(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<RelationshipRequest>()))
                .Returns((string key1, string key2, RelationshipRequest r) =>
                    Mapper.Map<RelationshipRequest, ApiResultValues<Relationship>>(r));

            // Voor deze test moet elke 'ContactSave' een individual aanmaken.
            civiApiMock.Setup(src => src.ContactSave(It.IsAny<string>(), It.IsAny<string>(),
                It.Is<ContactRequest>(r => r.ContactType == ContactType.Individual))).Returns(
                    (string key1, string key2, ContactRequest r) => SomeSaveResult(r)).Verifiable();

            var service = factory.Maak<SyncService>();

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

            civiApiMock.Verify(src => src.ContactSave(It.IsAny<string>(), It.IsAny<string>(),
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

            Mock<ICiviCrmApi> civiApiMock;
            Mock<IGapUpdateClient> updateHelperMock;
            IDiContainer factory;
            TestHelper.IocOpzetten(new DateTime(2015, 02, 05), out factory, out civiApiMock, out updateHelperMock);

            // We zullen straks de persoon kunnen matchen op zijn telefoonnummer.
            const string somePhoneNumber = "03-231 07 95";

            // Onze nepdatabase bevat 1 organisatie (TST/0000) en 1 contact (Kees Flodder)
            var ploeg = new Contact { ExternalIdentifier = "TST/0001", Id = 1, ContactType = ContactType.Organization };
            var persoon = new Contact
            {
                // De persoon in civi heeft uiteraard wel een AD-nummer
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

            // Zoeken op het gevraagde stamnummer levert de ploeg op.
            civiApiMock.Setup(
                src =>
                    src.ContactGet(It.IsAny<string>(), It.IsAny<string>(),
                        It.Is<ContactRequest>(r => r.ExternalIdentifier == ploeg.ExternalIdentifier))).Returns(
                            new ApiResultValues<Contact> { Count = 1, IsError = 0, Values = new[] { ploeg } });

            // Als er gezocht wordt op AD-nummer, vinden we de persoon:
            civiApiMock.Setup(
                src =>
                    src.ContactGet(It.IsAny<string>(), It.IsAny<string>(),
                        It.Is<ContactRequest>(r => r.ExternalIdentifier == persoon.ExternalIdentifier)))
                .Returns(
                    new ApiResultValues<Contact> { Count = 1, IsError = 0, Values = new[] { persoon } }).Verifiable();

            // Verwacht dat er zeker op gender gezocht wordt. Lever persoon op.
            civiApiMock.Setup(
                src =>
                    src.ContactGet(It.IsAny<string>(), It.IsAny<string>(),
                        It.Is<ContactRequest>(r => r.Gender == persoon.Gender && r.ContactType == ContactType.Individual)))
                .Returns(
                    new ApiResultValues<Contact> {Count = 1, IsError = 0, Values = new[] {persoon}}).Verifiable();

            // RelationshipSave moet gewoon doen of het werkt.
            civiApiMock.Setup(
                src => src.RelationshipSave(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<RelationshipRequest>()))
                .Returns((string key1, string key2, RelationshipRequest r) =>
                    Mapper.Map<RelationshipRequest, ApiResultValues<Relationship>>(r));

            // Voor deze test moet elke 'ContactSave' een individual aanmaken.
            civiApiMock.Setup(src => src.ContactSave(It.IsAny<string>(), It.IsAny<string>(),
                It.IsAny<ContactRequest>())).Returns(
                    (string key1, string key2, ContactRequest r) =>
                    {
                        Assert.AreEqual(ContactType.Individual, r.ContactType);
                        return SomeSaveResult(r);
                    });

            var service = factory.Maak<SyncService>();

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

            civiApiMock.Verify(
                src =>
                    src.ContactGet(It.IsAny<string>(), It.IsAny<string>(),
                        It.Is<ContactRequest>(r => r.Gender == persoon.Gender && r.ContactType == ContactType.Individual)),
                Times.AtLeastOnce);
        }


        /// <summary>
        /// Doet 'PersoonUpdaten' een ContactSave op de API?
        /// </summary>
        [TestMethod]
        public void PersoonUpdaten()
        {
            // ARRANGE

            Mock<ICiviCrmApi> civiApiMock;
            Mock<IGapUpdateClient> updateHelperMock;
            IDiContainer factory;
            TestHelper.IocOpzetten(new DateTime(2015, 02, 05), out factory, out civiApiMock, out updateHelperMock);

            const int adNummer = 2;
            var persoon = new Contact
            {
                Id = 4,
                ExternalIdentifier = adNummer.ToString(),
                FirstName = "Kees",
                LastName = "Flodder",
                GapId = 3
            };

            civiApiMock.Setup(
                src => src.ContactGet(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<ContactRequest>()))
                .Returns(new ApiResultValues<Contact>(persoon));
            civiApiMock.Setup(
                src =>
                    src.ContactSave(It.IsAny<string>(), It.IsAny<string>(),
                        It.Is<ContactRequest>(r => r.ExternalIdentifier == persoon.ExternalIdentifier)))
                .Returns(
                    (string key1, string key2, ContactRequest request) =>
                        Mapper.Map<ContactRequest, ApiResultValues<Contact>>(request))
                .Verifiable();

            var service = factory.Maak<SyncService>();

            // ACT

            service.PersoonUpdaten(new Persoon
            {
                AdNummer = adNummer,
                GeboorteDatum = new DateTime(1977, 03, 08)
            });

            // ASSERT

            civiApiMock.Verify(
                src =>
                    src.ContactSave(It.IsAny<string>(), It.IsAny<string>(),
                        It.Is<ContactRequest>(r => r.ExternalIdentifier == persoon.ExternalIdentifier)),
                Times.AtLeastOnce);
        }

        /// <summary>
        /// Werkt PersoonUpdaten ook als het GAP geen AD-nummer kent? (#3684)
        /// </summary>
        [TestMethod]
        public void PersoonUpdatenZonderAd()
        {
            // ARRANGE

            Mock<ICiviCrmApi> civiApiMock;
            Mock<IGapUpdateClient> updateHelperMock;
            IDiContainer factory;
            TestHelper.IocOpzetten(new DateTime(2015, 02, 05), out factory, out civiApiMock, out updateHelperMock);

            const int adNummer = 2;
            var persoon = new Contact
            {
                Id = 4,
                ExternalIdentifier = adNummer.ToString(),
                FirstName = "Kees",
                LastName = "Flodder",
                GapId = 3
            };

            civiApiMock.Setup(
                src =>
                    src.ContactGet(It.IsAny<string>(), It.IsAny<string>(),
                        It.Is<ContactRequest>(r => r.ExternalIdentifier == persoon.ExternalIdentifier)))
                .Returns(new ApiResultValues<Contact>(persoon));
            civiApiMock.Setup(
                src =>
                    src.ContactGet(It.IsAny<string>(), It.IsAny<string>(),
                        It.Is<ContactRequest>(r => r.GapId == persoon.GapId)))
                .Returns(Mapper.Map<Contact, ApiResultValues<Contact>>(persoon));
            civiApiMock.Setup(
                src =>
                    src.ContactSave(It.IsAny<string>(), It.IsAny<string>(),
                        It.Is<ContactRequest>(r => r.ExternalIdentifier == persoon.ExternalIdentifier)))
                .Returns(
                    (string key1, string key2, ContactRequest request) =>
                        Mapper.Map<ContactRequest, ApiResultValues<Contact>>(request))
                .Verifiable();

            var service = factory.Maak<SyncService>();

            // ACT

            Assert.IsNotNull(persoon.GapId);
            service.PersoonUpdaten(new Persoon
            {
                AdNummer = null,
                ID = persoon.GapId.Value,
                GeboorteDatum = new DateTime(1977, 03, 08)
            });

            // ASSERT

            civiApiMock.Verify(
                src =>
                    src.ContactSave(It.IsAny<string>(), It.IsAny<string>(),
                        It.Is<ContactRequest>(r => r.ExternalIdentifier == persoon.ExternalIdentifier)),
                Times.AtLeastOnce);
        }

        /// <summary>
        /// UpdatenOfMaken moet kunnen maken als er geen adres is. (#4111)
        /// </summary>
        [TestMethod]
        public void UpdatenOfMakenZonderAdres()
        {
            // ARRANGE

            Mock<ICiviCrmApi> civiApiMock;
            Mock<IGapUpdateClient> updateHelperMock;
            IDiContainer factory;
            TestHelper.IocOpzetten(new DateTime(2015, 02, 05), out factory, out civiApiMock, out updateHelperMock);

            var persoon = new PersoonDetails
            {
                Persoon = new Persoon
                {
                   Naam = "Flodder",
                   VoorNaam = "Kees",
                   GeboorteDatum = new DateTime(1977, 03, 08)
                }
            };

            // Zorg ervoor dat de persoon niet gevonden wordt.
            civiApiMock.Setup(
                src =>
                    src.ContactGet(It.IsAny<string>(), It.IsAny<string>(),
                        It.IsAny<ContactRequest>()))
                .Returns(new ApiResultValues<Contact>());

            // Zet dummy save op.
            civiApiMock.Setup(
                src =>
                    src.ContactSave(It.IsAny<string>(), It.IsAny<string>(),
                        It.IsAny<ContactRequest>()))
                .Returns(
                    (string key1, string key2, ContactRequest request) =>
                        Mapper.Map<ContactRequest, ApiResultValues<Contact>>(request))
                .Verifiable();

            var service = factory.Maak<SyncService>();

            // ACT

            service.UpdatenOfMaken(persoon);

            // ASSERT

            civiApiMock.Verify(
                src => src.ContactSave(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<ContactRequest>()),
                Times.AtLeastOnce);
        }

        /// <summary>
        /// PersoonUpdaten voor een onbekend persoon moet een error loggen. (#3684)
        /// </summary>
        [TestMethod]
        public void PersoonUpdatenOnbekend()
        {
            // ARRANGE

            Mock<ICiviCrmApi> civiApiMock;
            Mock<IGapUpdateClient> updateHelperMock;
            IDiContainer factory;
            TestHelper.IocOpzetten(new DateTime(2015, 02, 05), out factory, out civiApiMock, out updateHelperMock);

            // De API levert niets op:
            civiApiMock.Setup(
                src => src.ContactGetSingle(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<ContactRequest>()))
                .Returns(new Contact());
            civiApiMock.Setup(
                src =>
                    src.ContactGet(It.IsAny<string>(), It.IsAny<string>(),
                        It.IsAny<ContactRequest>()))
                .Returns(new ApiResultValues<Contact>());

            // Bewaren verifiable maken, zodat we kunnen controleren dat het niet gebeurt.
            civiApiMock.Setup(
                src =>
                    src.ContactSave(It.IsAny<string>(), It.IsAny<string>(),
                        It.IsAny<ContactRequest>()))
                .Returns(
                    (string key1, string key2, ContactRequest request) =>
                        Mapper.Map<ContactRequest, ApiResultValues<Contact>>(request))
                .Verifiable();

            var service = factory.Maak<SyncService>();

            // ACT

            service.PersoonUpdaten(new Persoon
            {
                AdNummer = null,
                ID = 1,
                GeboorteDatum = new DateTime(1977, 03, 08)
            });

            // ASSERT

            civiApiMock.Verify(
                src =>
                    src.ContactSave(It.IsAny<string>(), It.IsAny<string>(),
                        It.IsAny<ContactRequest>()),
                Times.Never);
        }

        /// <summary>
        /// PersoonUpdaten voor een persoon met fout AD-nummer (#3688)
        /// </summary>
        [TestMethod]
        public void PersoonUpdatenFoutAd()
        {
            // ARRANGE

            Mock<ICiviCrmApi> civiApiMock;
            Mock<IGapUpdateClient> updateHelperMock;
            IDiContainer factory;
            TestHelper.IocOpzetten(new DateTime(2015, 02, 05), out factory, out civiApiMock, out updateHelperMock);

            // Zo gezegd ongeldig AD-nummer
            const int adNummer = 1;

            // De API levert niets op:
            civiApiMock.Setup(
                src => src.ContactGetSingle(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<ContactRequest>()))
                .Returns(new Contact());
            civiApiMock.Setup(
                src =>
                    src.ContactGet(It.IsAny<string>(), It.IsAny<string>(),
                        It.IsAny<ContactRequest>()))
                .Returns(new ApiResultValues<Contact>());

            // We mocken ook GapUpdateClient.
            updateHelperMock.Setup(src => src.OngeldigAdNaarGap(It.Is<Int32>(ad => ad == adNummer))).Verifiable();

            var service = factory.Maak<SyncService>();

            // ACT

            service.PersoonUpdaten(new Persoon
            {
                AdNummer = adNummer,
                ID = 1,
                GeboorteDatum = new DateTime(1977, 03, 08)
            });

            // ASSERT

            updateHelperMock.Verify(src => src.OngeldigAdNaarGap(It.Is<Int32>(ad => ad == adNummer)), Times.AtLeastOnce);
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
