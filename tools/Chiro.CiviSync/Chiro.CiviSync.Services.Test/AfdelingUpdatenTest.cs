using System;
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
    public class AfdelingUpdatenTest
    {
        private Mock<ICiviCrmApi> _civiApiMock;
        private Mock<IGapUpdateClient> _updateHelperMock;

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

        [TestInitialize]
        public void InitializeTest()
        {
            TestHelper.IocOpzetten(_vandaagZogezegd, out _civiApiMock, out _updateHelperMock);
        }

        /// <summary>
        /// Controleer of de nieuwe afdelingen wel mee naar de API gaan.
        /// </summary>
        [TestMethod]
        public void AfdelingenInRequestTest()
        {
            // ARRANGE

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
                EndDate = new DateTime(werkJaar + 1, 08, 31)
            };
            // We gaan de afdeling van die speelclubber veranderen
            // naar rakwi's.
            const AfdelingEnum gapAfdeling = AfdelingEnum.Rakwis;
            const Afdeling civiAfdeling = Afdeling.Rakwis;

            // External ID's omzetten naar ID's gebeurt via
            // een GetSingle. Let's mock that.
            _civiApiMock.Setup(
                src =>
                    src.ContactGetSingle(It.IsAny<string>(), It.IsAny<string>(),
                        It.Is<ContactRequest>(r => r.ExternalIdentifier == ploeg.ExternalIdentifier))).Returns(ploeg);
            _civiApiMock.Setup(
                src =>
                    src.ContactGetSingle(It.IsAny<string>(), It.IsAny<string>(),
                        It.Is<ContactRequest>(r => r.ExternalIdentifier == persoon.ExternalIdentifier)))
                .Returns(persoon);
            // Relatie wordt normaal gezien gezocht op basis van de contact-ID's
            _civiApiMock.Setup(
                src =>
                    src.RelationshipGet(It.IsAny<string>(), It.IsAny<string>(),
                        It.Is<RelationshipRequest>(
                            r =>
                                r.ContactIdA == persoon.Id && r.ContactIdB == ploeg.Id &&
                                r.RelationshipTypeId == (int) RelatieType.LidVan)))
                .Returns(new ApiResultValues<Relationship>(bestaandLid));
            // We verwachten dat de relatie opnieuw bewaard wordt met de juiste afdeling
            // (de return value van  deze mock is niet helemaal juist, maar zeis hier ook niet zo relevant.)
            _civiApiMock.Setup(
                src => src.RelationshipSave(It.IsAny<string>(), It.IsAny<string>(), It.Is<RelationshipRequest>(
                    r => r.Id == bestaandLid.Id && r.Afdeling == civiAfdeling)))
                .Returns(new ApiResultValues<Relationship>(bestaandLid))
                .Verifiable();

            var service = Factory.Maak<SyncService>();

            // ACT

            service.AfdelingenUpdaten(kipPersoon, ploeg.ExternalIdentifier, werkJaar,
                new[] {gapAfdeling});

            // ASSERT

            _civiApiMock.Verify(
                src => src.RelationshipSave(It.IsAny<string>(), It.IsAny<string>(), It.Is<RelationshipRequest>(
                    r => r.Id == bestaandLid.Id && r.Afdeling == civiAfdeling)), Times.AtLeastOnce);
        }
    }
}
