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
using Chiro.Cdf.Ioc;
using Chiro.CiviCrm.Api;
using Chiro.CiviCrm.Api.DataContracts;
using Chiro.CiviCrm.Api.DataContracts.Entities;
using Chiro.CiviCrm.Api.DataContracts.Requests;
using Chiro.CiviSync.Mapping;
using Chiro.Gap.Log;
using Chiro.Gap.UpdateApi.Client;
using Chiro.Kip.ServiceContracts.DataContracts;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Chiro.CiviSync.Services.Test
{
    [TestClass]
    public class AfdelingUpdatenTest
    {
        private readonly DateTime _vandaagZogezegd = new DateTime(2015, 2, 6);
        private const int HuidigWerkJaar = 2014;

        [ClassInitialize]
        public static void InitialilzeTestClass(TestContext c)
        {
            // creer mappings voor de service
            MappingHelper.MappingsDefinieren();
            // creer mappings voor de tests
            TestHelper.MappingsCreeren();
        }

        /// <summary>
        /// Controleer of de nieuwe afdelingen wel mee naar de API gaan.
        /// </summary>
        [TestMethod]
        public void AfdelingenInRequestTest()
        {
            // ARRANGE

            Mock<ICiviCrmApi> civiApiMock;
            Mock<IGapUpdateClient> updateHelperMock;
            IDiContainer factory;

            TestHelper.IocOpzetten(_vandaagZogezegd, out factory, out civiApiMock, out updateHelperMock);

            const int adNummer = 2;
            const int werkJaar = 2014;

            // Dummy gegevens voor test:

            var kipPersoon = new Persoon
            {
                AdNummer = adNummer
            };

            var ploeg = new Contact {ExternalIdentifier = "TST/0001", Id = 1, ContactType = ContactType.Organization};
            var persoon = new Contact
            {
                ExternalIdentifier = adNummer.ToString(),
                FirstName = "Kees",
                LastName = "Flodder",
                GapId = 3,
                Id = 4,
            };
            var bestaandLid = new Relationship
            {
                ContactIdA = persoon.Id,
                ContactIdB = ploeg.Id,
                RelationshipTypeId = (int) RelatieType.LidVan,
                Id = 5,
                Afdeling = Afdeling.Speelclub,
                StartDate = new DateTime(werkJaar, 09, 01),
                EndDate = null
            };
            // We gaan de afdeling van die speelclubber veranderen
            // naar rakwi's.
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
            // Relatie wordt normaal gezien gezocht op basis van de contact-ID's
            civiApiMock.Setup(
                src =>
                    src.RelationshipGet(It.IsAny<string>(), It.IsAny<string>(),
                        It.Is<RelationshipRequest>(
                            r =>
                                r.ContactIdA == persoon.Id && r.ContactIdB == ploeg.Id &&
                                r.RelationshipTypeId == (int) RelatieType.LidVan)))
                .Returns(new ApiResultValues<Relationship>(bestaandLid));
            // We verwachten dat de relatie opnieuw bewaard wordt met de juiste afdeling
            // (de return value van  deze mock is niet helemaal juist, maar zeis hier ook niet zo relevant.)
            civiApiMock.Setup(
                src => src.RelationshipSave(It.IsAny<string>(), It.IsAny<string>(), It.Is<RelationshipRequest>(
                    r => r.Id == bestaandLid.Id && r.Afdeling == civiAfdeling)))
                .Returns(new ApiResultValues<Relationship>(bestaandLid))
                .Verifiable();

            var service = factory.Maak<SyncService>();

            // ACT

            service.AfdelingenUpdaten(kipPersoon, ploeg.ExternalIdentifier,
                new[] {gapAfdeling});

            // ASSERT

            civiApiMock.Verify(
                src => src.RelationshipSave(It.IsAny<string>(), It.IsAny<string>(), It.Is<RelationshipRequest>(
                    r => r.Id == bestaandLid.Id && r.Afdeling == civiAfdeling)), Times.AtLeastOnce);
        }

        /// <summary>
        /// Als het lid onbestaand is, mag het niet crashen (#3721).
        /// </summary>
        [TestMethod]
        public void AfdelingenBijwerkenOnbestaandLidTest()
        {
            // ARRANGE

            Mock<ICiviCrmApi> civiApiMock;
            Mock<IGapUpdateClient> updateHelperMock;
            IDiContainer factory;

            TestHelper.IocOpzetten(_vandaagZogezegd, out factory, out civiApiMock, out updateHelperMock);


            const int adNummer = 2;

            // Mock ook log
            var logMock = new Mock<IMiniLog>();
            logMock.Setup(
                src =>
                    src.Loggen(Niveau.Error, It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int?>(), It.IsAny<int?>()))
                .Verifiable();
            factory.InstantieRegistreren(logMock.Object);

            // Dummy gegevens voor test:

            var kipPersoon = new Persoon
            {
                AdNummer = adNummer
            };

            var ploeg = new Contact { ExternalIdentifier = "TST/0001", Id = 1, ContactType = ContactType.Organization };
            var persoon = new Contact
            {
                ExternalIdentifier = adNummer.ToString(),
                FirstName = "Kees",
                LastName = "Flodder",
                GapId = 3,
                Id = 4,
            };
            const AfdelingEnum gapAfdeling = AfdelingEnum.Rakwis;

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
            // Api vindt geen relatie
            civiApiMock.Setup(
                src =>
                    src.RelationshipGet(It.IsAny<string>(), It.IsAny<string>(),
                        It.Is<RelationshipRequest>(
                            r =>
                                r.ContactIdA == persoon.Id && r.ContactIdB == ploeg.Id &&
                                r.RelationshipTypeId == (int)RelatieType.LidVan)))
                .Returns(new ApiResultValues<Relationship>());
            // We verwachten dat de relatie niet opnieuw bewaard wordt. We zetten een mock op
            // om dat te verifieren.
            civiApiMock.Setup(
                src => src.RelationshipSave(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<RelationshipRequest>()))
                .Returns(new ApiResultValues<Relationship>())
                .Verifiable();

            var service = factory.Maak<SyncService>();

            // ACT

            service.AfdelingenUpdaten(kipPersoon, ploeg.ExternalIdentifier,
                new[] { gapAfdeling });

            // ASSERT

            civiApiMock.Verify(
                src => src.RelationshipSave(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<RelationshipRequest>()), Times.Never);
            logMock.Verify(
                src =>
                    src.Loggen(Niveau.Error, It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int?>(), It.IsAny<int?>()),
                Times.AtLeastOnce);
        }
    }
}
