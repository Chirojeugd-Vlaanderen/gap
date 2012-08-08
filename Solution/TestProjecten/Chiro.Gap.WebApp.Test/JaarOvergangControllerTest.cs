using System.Collections.Generic;
using System.Linq;

using Chiro.Cdf.Ioc;
using Chiro.Gap.Domain;
using Chiro.Gap.ServiceContracts;
using Chiro.Gap.ServiceContracts.DataContracts;
using Chiro.Gap.WebApp.Controllers;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using System.Web.Mvc;

using Chiro.Gap.WebApp.Models;

using Moq;
using System;
using Microsoft.VisualStudio.TestTools.UnitTesting.Web;
using Chiro.Gap.WebApp;

namespace Chiro.Gap.WebApp.Test
{
    
    
    /// <summary>
    /// Dit is een testclass voor JaarOvergangControllerTest,
    ///to contain all JaarOvergangControllerTest Unit Tests
    /// </summary>
    [TestClass]
    public class JaarOvergangControllerTest
    {
        private TestContext testContextInstance;

        const int GROEPID = 426;            // arbitrair
        const int VORIGWERKJAAR = 2010;     // vorig werkjaar was zogezegd 2010-2011
        const int GROEPSWERKJAARID = 2971;  // arbitrair groepswerkjaarid

        private static Mock<IVeelGebruikt> _veelGebruiktMock;

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        /// </summary>
        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }
            set
            {
                testContextInstance = value;
            }
        }

        #region Additional test attributes

        [ClassInitialize]
        public static void MyClassInitialize(TestContext testContext)
        {    
            _veelGebruiktMock = new Mock<IVeelGebruikt>();

            _veelGebruiktMock.Setup(vg => vg.GroepsWerkJaarOphalen(GROEPID)).Returns(new GroepsWerkJaarDetail
            {
                GroepID = GROEPID,
                WerkJaar = VORIGWERKJAAR,
                WerkJaarID = GROEPSWERKJAARID
            });
            _veelGebruiktMock.Setup(vg => vg.BivakStatusHuidigWerkjaarOphalen(GROEPID)).Returns(
                new BivakAangifteLijstInfo { AlgemeneStatus = BivakAangifteStatus.NogNietVanBelang });
        }

        /// <summary>
        /// Run code before running each test
        /// </summary>
        [TestInitialize]
        public void MyTestInitialize()
        {
            // Omdat ik hier en daar mocks ga registreren, wil ik de IOC-container
            // iedere keer resetten.

            Factory.ContainerInit();
        }

        #endregion


        /// <summary>
        /// Kijkt na of de gesuggereerde geboortejaren voor de nieuwe afdelingsjaren wel juist zijn bij de jaarovergang.
        /// </summary>
        [TestMethod]
        public void Stap1AfdelingenSelecterenTest()
        {
            const int NIEUWWERKJAAR = 2011;     // nieuw werkjaar, zogezegd 2011-2012

            const int AFDELINGID = 2337;            // ook arbitrair; bij wijze van test slechts 1 afdeling
            const int OUDGEBOORTEJAARVAN = 2003;    // geboortejaar van en tot in vorig werkjaar
            const int OUDGEBOORTEJAARTOT = 2004;

            // setup mocks

            var groepenServiceMock = new Mock<IGroepenService>();
            groepenServiceMock.Setup(svc => svc.NieuwWerkJaarOphalen(GROEPID)).Returns(NIEUWWERKJAAR);
            groepenServiceMock.Setup(svc => svc.AlleAfdelingenOphalen(GROEPID)).Returns(new[]
                                                                                            {
                                                                                                new AfdelingInfo
                                                                                                    {ID = AFDELINGID}
                                                                                            });
            groepenServiceMock.Setup(svc => svc.ActieveAfdelingenOphalen(GROEPSWERKJAARID)).Returns(new[]
                                                                                               {
                                                                                                   new AfdelingDetail
                                                                                                       {
                                                                                                           AfdelingID =
                                                                                                               AFDELINGID,
                                                                                                           GeboorteJaarVan
                                                                                                               =
                                                                                                               OUDGEBOORTEJAARVAN,
                                                                                                           GeboorteJaarTot
                                                                                                               =
                                                                                                               OUDGEBOORTEJAARTOT
                                                                                                       }
                                                                                               });

            Factory.InstantieRegistreren(_veelGebruiktMock.Object);
            Factory.InstantieRegistreren(groepenServiceMock.Object);

            // arrange

            var target = Factory.Maak<JaarOvergangController>();
            var model1 = new JaarOvergangAfdelingsModel
                             {
                                 GekozenAfdelingsIDs = new[] {AFDELINGID}   // doe maar iets
                             };
            int groepID = GROEPID;

            // act

            var actualModel =
                ((ViewResult) target.Stap1AfdelingenSelecteren(model1, groepID)).ViewData.Model as
                JaarOvergangAfdelingsJaarModel;

            // assert

            const int EXPECTEDGEBOORTEJAARVAN = OUDGEBOORTEJAARVAN + (NIEUWWERKJAAR - VORIGWERKJAAR);
            const int EXPECTEDGEBOORTEJAARTOT = OUDGEBOORTEJAARTOT + (NIEUWWERKJAAR - VORIGWERKJAAR);

            Assert.IsNotNull(actualModel);
            Assert.AreEqual(EXPECTEDGEBOORTEJAARVAN, actualModel.Afdelingen.First().GeboorteJaarVan);
            Assert.AreEqual(EXPECTEDGEBOORTEJAARTOT, actualModel.Afdelingen.First().GeboorteJaarTot);
        }

        /// <summary>
        /// Als je een ongeldige afdelingsverdeling post bij het definieren van de afdelingsjaren, 
        /// dan moet je een nieuwe poging kunnen doen, waarbij nog wel de afdelingen getoond worden.
        /// </summary>
        [TestMethod()]
        public void Stap2AfdelingsJarenVerdelenTest()
        {
            Factory.InstantieRegistreren(_veelGebruiktMock.Object);

            var groepenServiceMock = new Mock<IGroepenService>();
            Factory.InstantieRegistreren(groepenServiceMock.Object);

            var target = Factory.Maak<JaarOvergangController>();

            var model = new JaarOvergangAfdelingsJaarModel
                {
                    Afdelingen =
                        new List<AfdelingDetail>
                            {
                                new AfdelingDetail
                                    {
                                        AfdelingID = 1,
                                        OfficieleAfdelingID = 1,
                                        GeboorteJaarVan = 0,    // foute geboortejaren,
                                        GeboorteJaarTot = 0,    // ik doe maar iets
                                        Geslacht = GeslachtsType.Gemengd
                                    }
                            }
                };

            // zorg ervoor dat de controller actie zodadelijk weet dat er iets mis is
            target.ViewData.ModelState.AddModelError("Afdelingen[0].GeboorteJaarVan", Properties.Resources.OngeldigeGeboortejarenVoorAfdeling);

            var actual = target.Stap2AfdelingsJarenVerdelen(model, GROEPID);

            var actualModel = ((ViewResult) actual).ViewData.Model as JaarOvergangAfdelingsJaarModel;

            Assert.IsNotNull(actualModel);
            var afdelingDetail = actualModel.Afdelingen.FirstOrDefault();
            Assert.IsNotNull(afdelingDetail);
            Assert.IsNotNull(afdelingDetail.AfdelingNaam);
        }
    }
}
