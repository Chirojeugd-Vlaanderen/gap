using System;
using AutoMapper;
using Chiro.Cdf.Ioc;
using Chiro.CiviCrm.Api;
using Chiro.CiviCrm.Api.DataContracts;
using Chiro.CiviCrm.Api.DataContracts.Entities;
using Chiro.CiviCrm.Api.DataContracts.Requests;
using Chiro.CiviSync.Helpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Chiro.CiviSync.Services.Test
{
    [TestClass]
    public class BivakContactBewarenTest
    {
        private Mock<ICiviCrmApi> _civiApiMock;
        private Mock<IGapUpdateHelper> _updateHelperMock;

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
        /// Als CiviSync een contact moet vastleggen voor een bivak, maar een ongeldig
        /// AD-nummer krijgt, dan moet CiviSync dat aan GAP laten weten.
        /// </summary>
        [TestMethod]
        public void ContactBewarenOngeldigAdNummer()
        {
            // ARRANGE

            const int adNummer = 2;
            const int uitstapId = 4;

            // prepareer het event dat we gaan opleveren.
            var ploeg = new Contact {ExternalIdentifier = "TST/0001", Id = 1, ContactType = ContactType.Organization};
            var bivak = new Event
            {
                Id = 3,
                GapUitstapId = uitstapId,
                ContactResult = Mapper.Map<Contact, ApiResultValues<Contact>>(ploeg)
            };

            // De persoon wordt opgeroepen met GetSingle, het bivak met Get. Het is omdat we
            // dat weten, dat we dat kunnen faken. Niet zo proper, maar het kan ermee door.

            // Imiteer het gedrag van de CiviCRM-API bij een niet-gevonden persoon:
            _civiApiMock.Setup(
                src =>
                    src.ContactGetSingle(It.IsAny<string>(), It.IsAny<string>(),
                        It.Is<ContactRequest>(r => r.ExternalIdentifier == adNummer.ToString()))).Returns(new Contact());

            // Lever braaf het bivak op als het gevraagd wordt.
            _civiApiMock.Setup(
                src => src.EventGet(It.IsAny<string>(), It.IsAny<string>(),
                    It.Is<EventRequest>(r => r.GapUitstapId == bivak.GapUitstapId)))
                .Returns(Mapper.Map<Event, ApiResultValues<Event>>(bivak));

            // Verwacht dat het foute AD-nummer terug naar GAP gaat.
            _updateHelperMock.Setup(src => src.OngeldigAdNaarGap(It.Is<Int32>(ad => ad == adNummer))).Verifiable();

            // ACT

            var service = Factory.Maak<SyncService>();
            service.BivakContactBewaren(uitstapId, adNummer);

            // ASSERT

            _updateHelperMock.Verify(src => src.OngeldigAdNaarGap(It.Is<Int32>(ad => ad == adNummer)), Times.AtLeastOnce);
        }
    }
}
