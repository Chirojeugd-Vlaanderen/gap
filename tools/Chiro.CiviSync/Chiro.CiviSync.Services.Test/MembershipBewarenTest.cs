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
using AutoMapper;
using Chiro.Cdf.Ioc;
using Chiro.CiviCrm.Api;
using Chiro.CiviCrm.Api.DataContracts;
using Chiro.CiviCrm.Api.DataContracts.Entities;
using Chiro.CiviCrm.Api.DataContracts.Requests;
using Chiro.Gap.UpdateApi.Client;
using Chiro.Kip.ServiceContracts.DataContracts;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Chiro.CiviSync.Services.Test
{
    [TestClass]
    public class MembershipBewarenTest
    {
        private readonly DateTime _vandaagZogezegd = new DateTime(2015, 2, 6);
        private const int HuidigWerkJaar = 2014;

        [ClassInitialize]
        public static void InitialilzeTestClass(TestContext c)
        {
            TestHelper.MappingsCreeren();
        }

        /// <summary>
        /// Deze test controleert voornamelijk de datums.
        /// 
        /// Als een membership gesynct wordt, dan is de begindatum van dat membership
        /// de huidige datum (tenminste als het werkjaar al bezig is).
        /// Zie #3417.
        /// </summary>
        [TestMethod]
        public void MembershipBewaren()
        {
            // ARRANGE

            Mock<ICiviCrmApi> civiApiMock;
            Mock<IGapUpdateClient> updateHelperMock;
            IDiContainer factory;
            TestHelper.IocOpzetten(_vandaagZogezegd, out factory, out civiApiMock, out updateHelperMock);

            const int adNummer = 2;

            var persoon = new Contact { ExternalIdentifier = adNummer.ToString(), FirstName = "Kees", LastName = "Flodder", GapId = 3, Id = 4 };
            var groep = new Contact {ExternalIdentifier = "BLA/0000", Id = 5};
            new DateTime(HuidigWerkJaar, 9, 1);
            DateTime eindeDitWerkJaar = new DateTime(HuidigWerkJaar + 1, 8, 31);

            civiApiMock.Setup(
                src => src.ContactGet(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<ContactRequest>()))
                .Returns(
                    (string key1, string key2, ContactRequest r) =>
                    {
                        Contact result;
                        if (r.ExternalIdentifier == groep.ExternalIdentifier || r.Id == groep.Id)
                        {
                            result = groep;
                        }
                        else
                        {
                            result = persoon;
                        }
                        // Als relaties gevraagd zijn, lever dan gewoon een lege lijst op.
                        if (r.MembershipGetRequest != null)
                        {
                            result.MembershipResult = new ApiResultValues<Membership>
                            {
                                Count = 0,
                                IsError = 0
                            };
                        }
                        return new ApiResultValues<Contact>(result);
                    });

            civiApiMock.Setup(
                src =>
                    src.MembershipSave(It.IsAny<string>(), It.IsAny<string>(),
                        It.Is<MembershipRequest>(
                            r =>
                                r.StartDate == _vandaagZogezegd && r.EndDate == eindeDitWerkJaar &&
                                r.MembershipTypeId == (int) MembershipType.Aansluiting)))
                .Returns(
                    (string key1, string key2, MembershipRequest r) =>
                        Mapper.Map<MembershipRequest, ApiResultValues<Membership>>(r)).Verifiable();

            var service = factory.Maak<SyncService>();

            // ACT

            service.MembershipBewaren(adNummer, HuidigWerkJaar,
                new MembershipGedoe {Gratis = false, MetLoonVerlies = false, StamNummer = groep.ExternalIdentifier});

            // ASSERT

            civiApiMock.Verify(
                src =>
                    src.MembershipSave(It.IsAny<string>(), It.IsAny<string>(),
                        It.Is<MembershipRequest>(
                            r =>
                                r.StartDate == _vandaagZogezegd && r.EndDate == eindeDitWerkJaar &&
                                r.MembershipTypeId == (int) MembershipType.Aansluiting)), Times.AtLeastOnce);
        }

        /// <summary>
        /// In onze eigen specs staat ergens dat JoinDate de datum is dat het eerste membership van deze
        /// persoon (ever) geregistreerd is.
        /// </summary>
        [TestMethod]
        public void MembershipBewarenOudeJoinDate()
        {
            // ARRANGE

            Mock<ICiviCrmApi> civiApiMock;
            Mock<IGapUpdateClient> updateHelperMock;
            IDiContainer factory;
            TestHelper.IocOpzetten(_vandaagZogezegd, out factory, out civiApiMock, out updateHelperMock);

            const int adNummer = 2;
            const int contactId = 4;
            DateTime beginDitWerkJaar = new DateTime(HuidigWerkJaar, 9, 1);
            DateTime eindeDitWerkJaar = new DateTime(HuidigWerkJaar + 1, 8, 31);

            var oudMembership = new Membership
            {
                ContactId = contactId,
                MembershipTypeId = (int) MembershipType.Aansluiting,
                StartDate = beginDitWerkJaar.AddYears(-1),
                EndDate = eindeDitWerkJaar.AddYears(-1),
                JoinDate = beginDitWerkJaar.AddMonths(-10)
            };

            var persoon = new Contact
            {
                Id = contactId,
                ExternalIdentifier = adNummer.ToString(),
                FirstName = "Kees",
                LastName = "Flodder",
                GapId = 3,
                MembershipResult =
                    new ApiResultValues<Membership> {Count = 1, IsError = 0, Values = new[] {oudMembership}}
            };
            var groep = new Contact { ExternalIdentifier = "BLA/0000", Id = 5 };

            civiApiMock.Setup(
                src => src.ContactGet(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<ContactRequest>()))
                .Returns(
                    (string key1, string key2, ContactRequest r) =>
                        new ApiResultValues<Contact>(r.ExternalIdentifier == groep.ExternalIdentifier ||
                                                     r.Id == groep.Id
                            ? groep
                            : persoon));

            civiApiMock.Setup(
                src =>
                    src.MembershipSave(It.IsAny<string>(), It.IsAny<string>(),
                        It.Is<MembershipRequest>(
                            r =>
                                r.StartDate == _vandaagZogezegd && r.EndDate == eindeDitWerkJaar &&
                                r.JoinDate == oudMembership.JoinDate &&
                                r.MembershipTypeId == (int)MembershipType.Aansluiting)))
                .Returns(
                    (string key1, string key2, MembershipRequest r) =>
                        Mapper.Map<MembershipRequest, ApiResultValues<Membership>>(r)).Verifiable();

            var service = factory.Maak<SyncService>();

            // ACT

            service.MembershipBewaren(adNummer, HuidigWerkJaar,
                new MembershipGedoe {Gratis = false, MetLoonVerlies = false, StamNummer = groep.ExternalIdentifier});

            // ASSERT

            civiApiMock.Verify(
                src =>
                    src.MembershipSave(It.IsAny<string>(), It.IsAny<string>(),
                        It.Is<MembershipRequest>(
                            r =>
                                r.StartDate == _vandaagZogezegd && r.EndDate == eindeDitWerkJaar &&
                                r.JoinDate == oudMembership.JoinDate &&
                                r.MembershipTypeId == (int)MembershipType.Aansluiting)), Times.AtLeastOnce);
        }

        /// <summary>
        /// Stel dat een persoon al member is in civi, maar dat dat membership nog niet is gefactureerd. Als
        /// dat membership dan geüpgradet wordt met een verzekering loonverlies, dan moet de status van het
        /// bijgewerkte membership nog steeds 'niet gefactureerd' zijn.
        /// </summary>
        [TestMethod]
        public void TeFacturerenMembershipUpgradenMetLoonverlies()
        {
            // ARRANGE

            Mock<ICiviCrmApi> civiApiMock;
            Mock<IGapUpdateClient> updateHelperMock;
            IDiContainer factory;
            TestHelper.IocOpzetten(_vandaagZogezegd, out factory, out civiApiMock, out updateHelperMock);

            const int adNummer = 2;
            const int contactId = 4;
            DateTime beginDitWerkJaar = new DateTime(HuidigWerkJaar, 9, 1);
            DateTime eindeDitWerkJaar = new DateTime(HuidigWerkJaar + 1, 8, 31);

            var bestaandMembership = new Membership
            {
                ContactId = contactId,
                MembershipTypeId = (int)MembershipType.Aansluiting,
                StartDate = beginDitWerkJaar,
                EndDate = eindeDitWerkJaar,
                JoinDate = beginDitWerkJaar.AddMonths(1),
                VerzekeringLoonverlies = false,
                FactuurStatus = FactuurStatus.VolledigTeFactureren,
                MembershipPaymentResult = new ApiResultValues<MembershipPayment>(new MembershipPayment())
            };

            var persoon = new Contact
            {
                Id = contactId,
                ExternalIdentifier = adNummer.ToString(),
                FirstName = "Kees",
                LastName = "Flodder",
                GapId = 3,
                MembershipResult =
                    new ApiResultValues<Membership> { Count = 1, IsError = 0, Values = new[] { bestaandMembership } }
            };
            var groep = new Contact { ExternalIdentifier = "BLA/0000", Id = 5 };

            civiApiMock.Setup(
                src => src.ContactGet(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<ContactRequest>()))
                .Returns(
                    (string key1, string key2, ContactRequest r) =>
                        new ApiResultValues<Contact>(r.ExternalIdentifier == groep.ExternalIdentifier ||
                                                     r.Id == groep.Id
                            ? groep
                            : persoon));

            civiApiMock.Setup(
                src =>
                    src.MembershipSave(It.IsAny<string>(), It.IsAny<string>(),
                        It.Is<MembershipRequest>(r => r.FactuurStatus == FactuurStatus.VolledigTeFactureren)))
                .Returns(
                    (string key1, string key2, MembershipRequest r) =>
                        Mapper.Map<MembershipRequest, ApiResultValues<Membership>>(r))
                .Verifiable();

            var service = factory.Maak<SyncService>();

            // ACT

            // pas membership aan met loonverlies.
            service.MembershipBewaren(adNummer, HuidigWerkJaar, new MembershipGedoe { Gratis = false, MetLoonVerlies = true, StamNummer = groep.ExternalIdentifier });

            // ASSERT

            civiApiMock.Verify(
                src =>
                    src.MembershipSave(It.IsAny<string>(), It.IsAny<string>(),
                        It.Is<MembershipRequest>(r => r.FactuurStatus == FactuurStatus.VolledigTeFactureren)),
                Times.AtLeastOnce);
        }

        /// <summary>
        /// Kaderleden krijgen geen factuur. Een test.
        /// </summary>
        [TestMethod]
        public void MembershipBewarenKader()
        {
            // ARRANGE

            Mock<ICiviCrmApi> civiApiMock;
            Mock<IGapUpdateClient> updateHelperMock;
            IDiContainer factory;
            TestHelper.IocOpzetten(_vandaagZogezegd, out factory, out civiApiMock, out updateHelperMock);

            const int adNummer = 2;

            var persoon = new Contact
            {
                ExternalIdentifier = adNummer.ToString(),
                FirstName = "Kees",
                LastName = "Flodder",
                GapId = 3,
                Id = 4
            };
            var groep = new Contact {ExternalIdentifier = "BLA/0000", Id = 5, KaderNiveau = KaderNiveau.Gewest};
            DateTime beginDitWerkJaar = new DateTime(HuidigWerkJaar, 9, 1);
            DateTime eindeDitWerkJaar = new DateTime(HuidigWerkJaar + 1, 8, 31);

            civiApiMock.Setup(
                src => src.ContactGet(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<ContactRequest>()))
                .Returns(
                    (string key1, string key2, ContactRequest r) =>
                    {
                        Contact result;
                        if (r.ExternalIdentifier == groep.ExternalIdentifier || r.Id == groep.Id)
                        {
                            result = groep;
                        }
                        else
                        {
                            result = persoon;
                        }
                        // Als relaties gevraagd zijn, lever dan gewoon een lege lijst op.
                        if (r.MembershipGetRequest != null)
                        {
                            result.MembershipResult = new ApiResultValues<Membership>
                            {
                                Count = 0,
                                IsError = 0
                            };
                        }
                        return new ApiResultValues<Contact>(result);
                    });

            civiApiMock.Setup(
                src =>
                    src.MembershipSave(It.IsAny<string>(), It.IsAny<string>(),
                        It.Is<MembershipRequest>(
                            r =>
                                r.StartDate == _vandaagZogezegd && r.EndDate == eindeDitWerkJaar &&
                                r.MembershipTypeId == (int) MembershipType.Aansluiting &&
                                r.FactuurStatus == FactuurStatus.FactuurOk)))
                .Returns(
                    (string key1, string key2, MembershipRequest r) =>
                        Mapper.Map<MembershipRequest, ApiResultValues<Membership>>(r)).Verifiable();

            var service = factory.Maak<SyncService>();

            // ACT

            service.MembershipBewaren(adNummer, HuidigWerkJaar,
                new MembershipGedoe {Gratis = true, MetLoonVerlies = false, StamNummer = groep.ExternalIdentifier});

            // ASSERT

            civiApiMock.Verify(
                src =>
                    src.MembershipSave(It.IsAny<string>(), It.IsAny<string>(),
                        It.Is<MembershipRequest>(
                            r =>
                                r.StartDate == _vandaagZogezegd && r.EndDate == eindeDitWerkJaar &&
                                r.MembershipTypeId == (int) MembershipType.Aansluiting &&
                                r.FactuurStatus == FactuurStatus.FactuurOk)), Times.AtLeastOnce);
        }
    }
}
