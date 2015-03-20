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
using Chiro.Cdf.Ioc;
using Chiro.CiviCrm.Api;
using Chiro.CiviCrm.Api.DataContracts;
using Chiro.CiviCrm.Api.DataContracts.Entities;
using Chiro.CiviCrm.Api.DataContracts.Requests;
using Chiro.CiviSync.Helpers;
using Chiro.Kip.ServiceContracts.DataContracts;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Chiro.CiviSync.Services.Test
{
    [TestClass]
    public class LidVerwijderenTest
    {
        private Mock<ICiviCrmApi> _civiApiMock;
        private Mock<IGapUpdateHelper> _updateHelperMock;

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

        /// <summary>
        /// Roept LidVerwijderen de CiviCRM API aan?
        /// </summary>
        [TestMethod]
        public void LidVerwijderen()
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
                    src.RelationshipDelete(It.IsAny<string>(), It.IsAny<string>(),
                        It.Is<IdRequest>(
                            r => r.Id == relatie.Id)))
                .Returns(new ApiResult()).Verifiable();

            var service = Factory.Maak<SyncService>();

            // ACT

            service.LidVerwijderen(adNummer, ploeg.ExternalIdentifier, HuidigWerkJaar, uitschrijfDatum);

            _civiApiMock.Verify(
                // We zullen een lid maken voor een werkjaar dat nog niet begonnen is.
                // LidMaken moet de startdatum dan op 1 september zetten.
                src =>
                    src.RelationshipDelete(It.IsAny<string>(), It.IsAny<string>(),
                        It.Is<IdRequest>(
                            r => r.Id == relatie.Id)), Times.AtLeastOnce);
        }

        /// <summary>
        /// Als CiviSync een lid moet verwijderen met een ongeldig AD-nummer, dan moet dat AD-nummer als ongeldig
        /// terug naar het GAP.
        /// </summary>
        [TestMethod]
        public void LidVerwijderenOngeldigAdNummer()
        {
            // ARRANGE

            const int adNummer = 2;

            // Onze nepdatabase bevat 1 organisatie (TST/0000)
            var ploeg = new Contact { ExternalIdentifier = "TST/0001", Id = 1, ContactType = ContactType.Organization };

            // We mocken ook GapUpdateHelper.

            _updateHelperMock.Setup(src => src.OngeldigAdNaarGap(It.Is<Int32>(ad => ad == adNummer))).Verifiable();

            _civiApiMock.Setup(
                src =>
                    src.ContactGetSingle(It.IsAny<string>(), It.IsAny<string>(),
                        It.Is<ContactRequest>(r => r.ExternalIdentifier == ploeg.ExternalIdentifier))).Returns(ploeg);

            // Imiteer het gedrag van de CiviCRM-API bij een niet-gevonden contact:
            _civiApiMock.Setup(
                src =>
                    src.ContactGetSingle(It.IsAny<string>(), It.IsAny<string>(),
                        It.Is<ContactRequest>(r => r.ExternalIdentifier == adNummer.ToString()))).Returns(new Contact());

            var service = Factory.Maak<SyncService>();

            // ACT

            service.LidVerwijderen(adNummer, ploeg.ExternalIdentifier, HuidigWerkJaar, _vandaagZogezegd);

            // ASSERT

            _updateHelperMock.Verify(src => src.OngeldigAdNaarGap(It.Is<Int32>(ad => ad == adNummer)), Times.AtLeastOnce);
        }

        /// <summary>
        /// Roept NieuwLidVerwijderen (van een lid zonder AD-nummer) de CiviCRM API aan?
        /// </summary>
        [TestMethod]
        public void NieuwLidVerwijderen()
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
                    src.RelationshipDelete(It.IsAny<string>(), It.IsAny<string>(),
                        It.Is<IdRequest>(
                            r => r.Id == relatie.Id)))
                .Returns(new ApiResult()).Verifiable();

            var service = Factory.Maak<SyncService>();

            // ACT

            var details = new PersoonDetails
            {
                Persoon = new Persoon
                {
                    VoorNaam = persoon.FirstName,
                    Naam = persoon.LastName,
                    Geslacht = GeslachtsEnum.Vrouw,
                    GeboorteDatum = persoon.BirthDate,
                    ID = persoon.GapId ?? 0
                }
            };

            service.NieuwLidVerwijderen(details, ploeg.ExternalIdentifier, HuidigWerkJaar, uitschrijfDatum);

            _civiApiMock.Verify(
                // We zullen een lid maken voor een werkjaar dat nog niet begonnen is.
                // LidMaken moet de startdatum dan op 1 september zetten.
                src =>
                    src.RelationshipDelete(It.IsAny<string>(), It.IsAny<string>(),
                        It.Is<IdRequest>(
                            r => r.Id == relatie.Id)), Times.AtLeastOnce);
        }
    }
}
