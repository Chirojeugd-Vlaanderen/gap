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
using Chiro.Kip.ServiceContracts.DataContracts;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Chiro.CiviSync.Services.Test
{
    [TestClass]
    public class LidMakenTest
    {
        private Mock<ICiviCrmApi> _civiApiMock;
        private static int _nextId = 1;

        private readonly DateTime _vandaagZogezegd = new DateTime(2015, 2, 6);
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

        [TestMethod]
        public void TestMethod1()
        {
            // ARRANGE

            const int adNummer = 2;

            // Onze nepdatabase bevat 1 organisatie (TST/0000) en 1 contact (Kees Flodder)
            var ploeg = new Contact { ExternalIdentifier = "TST/0001", Id = 1, ContactType = ContactType.Organization };
            var persoon = new Contact { ExternalIdentifier = adNummer.ToString(), FirstName = "Kees", LastName = "Flodder", GapId = 3 };

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
                src => src.RelationshipSave(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<RelationshipRequest>()))
                .Returns((string key1, string key2, RelationshipRequest r) =>
                {
                    DateTime beginVolgendWerkjaar = new DateTime(VolgendWerkjaar, 9, 1);
                    Assert.AreEqual(beginVolgendWerkjaar, r.StartDate);
                    return Mapper.Map<RelationshipRequest, ApiResultValues<Relationship>>(r);
                });

            var service = Factory.Maak<SyncService>();

            // ACT

            service.LidBewaren(adNummer, new LidGedoe
            {
                EindeInstapPeriode = new DateTime(2015, 02, 28),
                LidType = LidTypeEnum.Kind,
                OfficieleAfdelingen = new[] {AfdelingEnum.Rakwis},
                StamNummer = ploeg.ExternalIdentifier,
                WerkJaar = VolgendWerkjaar
            });

            // ASSERT:
            // Als er niets crasht, is het altijd goed, want kipsync geeft geen feedback.
            Assert.IsTrue(true);
        }
    }
}
