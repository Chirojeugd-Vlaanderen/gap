/*
 * Copyright 2008-2013 the GAP developers. See the NOTICE file at the 
 * top-level directory of this distribution, and at
 * https://develop.chiro.be/gap/wiki/copyright
 * 
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 * 
 *     http://www.apache.org/licenses/LICENSE-2.0
 * 
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Chiro.Cdf.Ioc.Factory;
using Chiro.Cdf.ServiceHelper;
using Chiro.Gap.Domain;
using Chiro.Gap.ServiceContracts;
using Chiro.Gap.ServiceContracts.DataContracts;
using Chiro.Gap.WebApp.Controllers;
using Chiro.Gap.WebApp.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Resources = Chiro.Gap.WebApp.Properties.Resources;

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

        private const int GROEP_ID = 426;            // arbitrair
        private const int VORIG_WERKJAAR = 2010;     // vorig werkjaar was zogezegd 2010-2011
        private const int GROEPSWERKJAAR_ID = 2971;  // arbitrair groepswerkjaarid
        private const int AFDELING_ID = 1;           // arbitrair afdelingsid
        private const string AFDELING_NAAM = "Dingskes"; // arbitraire naam van de afdeling

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

            _veelGebruiktMock.Setup(vg => vg.GroepsWerkJaarOphalen(GROEP_ID)).Returns(new GroepsWerkJaarDetail
            {
                GroepID = GROEP_ID,
                WerkJaar = VORIG_WERKJAAR,
                WerkJaarID = GROEPSWERKJAAR_ID
            });
            _veelGebruiktMock.Setup(vg => vg.BivakStatusHuidigWerkjaarOphalen(GROEP_ID)).Returns(
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
        /// Als je een ongeldige afdelingsverdeling post bij het definieren van de afdelingsjaren, 
        /// dan moet je een nieuwe poging kunnen doen, waarbij nog wel de afdelingen getoond worden.
        /// </summary>
        [TestMethod()]
        public void Stap2AfdelingsJarenVerdelenTest()
        {
            Factory.InstantieRegistreren(_veelGebruiktMock.Object);

            var groepenServiceMock = new Mock<IGroepenService>();
            groepenServiceMock.Setup(svc => svc.AlleAfdelingenOphalen(GROEP_ID)).Returns(new List<AfdelingInfo>
                {new AfdelingInfo {ID = AFDELING_ID, Naam = AFDELING_NAAM}});
            var channelProviderMock = new Mock<IChannelProvider>();
            channelProviderMock.Setup(mock => mock.GetChannel<IGroepenService>()).Returns(groepenServiceMock.Object);

            Factory.InstantieRegistreren(channelProviderMock.Object);

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
            target.ViewData.ModelState.AddModelError("Afdelingen[0].GeboorteJaarVan", Properties.Resources.OngeldigeGeboorteJarenVoorAfdeling);

            var actual = target.Stap2AfdelingsJarenVerdelen(model, GROEP_ID);

            var actualModel = ((ViewResult) actual).ViewData.Model as JaarOvergangAfdelingsJaarModel;

            Assert.IsNotNull(actualModel);
            var afdelingDetail = actualModel.Afdelingen.FirstOrDefault();
            Assert.IsNotNull(afdelingDetail);
            Assert.AreEqual(afdelingDetail.AfdelingNaam, AFDELING_NAAM);
        }
    }
}
