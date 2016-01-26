/*
 * Copyright 2008-2013, 2015 the GAP developers. See the NOTICE file at the 
 * top-level directory of this distribution, and at
 * https://gapwiki.chiro.be/copyright
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
using System.ServiceModel;
using System.Web.Mvc;
using Chiro.Cdf.Ioc.Factory;
using Chiro.Cdf.ServiceHelper;
using Chiro.Gap.Domain;
using Chiro.Gap.ServiceContracts;
using Chiro.Gap.ServiceContracts.DataContracts;
using Chiro.Gap.ServiceContracts.FaultContracts;
using Chiro.Gap.WebApp.Controllers;
using Chiro.Gap.WebApp.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Chiro.Gap.WebApp.Test
{
    [TestClass]
    public class PersonenControllerTest
    {
        /// <summary>
        /// Initialize IOC container.
        /// </summary>
        /// <param name="testContext"></param>
        [ClassInitialize()]
        public static void MyClassInitialize(TestContext testContext)
        {
            Factory.ContainerInit();
        }

        /// <summary>
        /// Als je bij het aanvragen van de nieuwsbrief een ongeldig e-mailadres ingeeft,
        /// dan moet je opnieuw de view krijgen waarin je het adres kunt ingeven.
        /// </summary>
        [TestMethod]
        public void NieuwsbriefOngeldigAdresTest()
        {
            const int groepid = 426;            // arbitrair
            const int werkjaar = 2011;          // werkJaar 2011-2012, om iets te doen
            const int groepswerkjaarid = 2971;  // arbitrair
            const int gelieerdePersoonId = 1;   // arbitrair.

            // setup mocks
            // (dit is nogal hetzelfde in veel tests.  Best eens afsplitsen)

            var veelGebruiktMock = new Mock<IVeelGebruikt>();
            veelGebruiktMock.Setup(vg => vg.GroepsWerkJaarOphalen(groepid)).Returns(new GroepsWerkJaarDetail
            {
                GroepID = groepid,
                WerkJaar = werkjaar,
                WerkJaarID =
                    groepswerkjaarid
            });
            veelGebruiktMock.Setup(vg => vg.BivakStatusHuidigWerkjaarOphalen(groepid)).Returns(new BivakAangifteLijstInfo
            {
                AlgemeneStatus =
                    BivakAangifteStatus.NogNietVanBelang
            });

            var gelieerdePersonenServiceMock = new Mock<IGelieerdePersonenService>();
            gelieerdePersonenServiceMock.Setup(svc => svc.AlleDetailsOphalen(gelieerdePersoonId))
                .Returns(new PersoonLidInfo
                {
                    CommunicatieInfo = new List<CommunicatieDetail>(),
                    PersoonDetail = new PersoonDetail {NieuwsBrief = false}
                });
            gelieerdePersonenServiceMock.Setup(svc => svc.InschrijvenNieuwsBrief(gelieerdePersoonId, It.IsAny<string>(), true))
                .Throws(new FaultException<FoutNummerFault>(new FoutNummerFault {FoutNummer = FoutNummer.ValidatieFout}));

            var channelProviderMock = new Mock<IChannelProvider>();
            channelProviderMock.Setup(mock => mock.GetChannel<IGelieerdePersonenService>()).Returns(gelieerdePersonenServiceMock.Object);

            Factory.InstantieRegistreren(veelGebruiktMock.Object);
            Factory.InstantieRegistreren(channelProviderMock.Object);

            var target = Factory.Maak<PersonenController>();
            var model = new VoorkeursMailModel
            {
                PersoonLidInfo =
                    new PersoonLidInfo
                    {
                        CommunicatieInfo = new List<CommunicatieDetail>(),
                        PersoonDetail = new PersoonDetail { NieuwsBrief = true }
                    }
            };

            // act

            var result = target.NieuwsBrief(gelieerdePersoonId, groepid, model);

            // assert

            Assert.IsInstanceOfType(result, typeof (ViewResult));
            Assert.AreEqual(((ViewResult)result).ViewName, "NieuwsBrief");
        }

        /// <summary>
        /// Als je de nieuwsbriefinstellingen wilt wijzigen, dan is het handig dat op het
        /// formulier dat je krijgt, nieuwsbrief krijgen is aangevinkt als dat nog niet
        /// het geval was, en uitgevinkt als dat wel het geval was.
        /// </summary>
        [TestMethod]
        public void NieuwsbriefWijzigenVinkjeOpVoorhandGewijzigdTest()
        {
            const int groepid = 426;            // arbitrair
            const int werkjaar = 2011;          // werkJaar 2011-2012, om iets te doen
            const int groepswerkjaarid = 2971;  // arbitrair
            const int gelieerdePersoonId = 1;   // arbitrair.

            // setup mocks
            // (dit is nogal hetzelfde in veel tests.  Best eens afsplitsen)

            var veelGebruiktMock = new Mock<IVeelGebruikt>();
            veelGebruiktMock.Setup(vg => vg.GroepsWerkJaarOphalen(groepid)).Returns(new GroepsWerkJaarDetail
            {
                GroepID = groepid,
                WerkJaar = werkjaar,
                WerkJaarID =
                    groepswerkjaarid
            });
            veelGebruiktMock.Setup(vg => vg.BivakStatusHuidigWerkjaarOphalen(groepid)).Returns(new BivakAangifteLijstInfo
            {
                AlgemeneStatus =
                    BivakAangifteStatus.NogNietVanBelang
            });

            var gelieerdePersonenServiceMock = new Mock<IGelieerdePersonenService>();
            gelieerdePersonenServiceMock.Setup(svc => svc.AlleDetailsOphalen(gelieerdePersoonId))
                .Returns(new PersoonLidInfo
                {
                    CommunicatieInfo = new List<CommunicatieDetail>(),
                    PersoonDetail = new PersoonDetail { NieuwsBrief = false }
                });

            var channelProviderMock = new Mock<IChannelProvider>();
            channelProviderMock.Setup(mock => mock.GetChannel<IGelieerdePersonenService>()).Returns(gelieerdePersonenServiceMock.Object);

            Factory.InstantieRegistreren(veelGebruiktMock.Object);
            Factory.InstantieRegistreren(channelProviderMock.Object);

            var target = Factory.Maak<PersonenController>();

            // act

            var result = target.NieuwsBrief(gelieerdePersoonId, groepid);

            // assert

            Assert.IsInstanceOfType(result, typeof(ViewResult));
            Assert.IsInstanceOfType(((ViewResult) result).Model, typeof (VoorkeursMailModel));
            Assert.IsTrue(((VoorkeursMailModel)((ViewResult)result).Model).PersoonLidInfo.PersoonDetail.NieuwsBrief);
        }

        ///<summary>
        ///Test of je met de webinterface leiding kunt maken zonder afdeling
        ///</summary>
        [TestMethod()]
        public void LedenMakenTest()
        {
            // Het inschrijven van leden en leiding via de webinterface is nogal vreemd
            // geïmplementeerd.  Je mag namelijk maar 1 afdeling(sjaar) kiezen, en dat kan ook
            // 'geen' zijn.  Dat gekozen item moet dan geconverteerd worden naar een lijst met
            // afdelingsjaarID's, die dan eventueel nul mag zijn.
            //
            // Als leden maken herwerkt wordt, dan vervalt natuurlijk deze test.

            const int GROEPID = 426;            // arbitrair
            const int WERKJAAR = 2011;          // werkJaar 2011-2012, om iets te doen
            const int GROEPSWERKJAARID = 2971;  // arbitrair

            // setup mocks
            // (dit is nogal hetzelfde in veel tests.  Best eens afsplitsen)

            var veelGebruiktMock = new Mock<IVeelGebruikt>();
            veelGebruiktMock.Setup(vg => vg.GroepsWerkJaarOphalen(GROEPID)).Returns(new GroepsWerkJaarDetail
            {
                GroepID = GROEPID,
                WerkJaar = WERKJAAR,
                WerkJaarID =
                    GROEPSWERKJAARID
            });
            veelGebruiktMock.Setup(vg => vg.BivakStatusHuidigWerkjaarOphalen(GROEPID)).Returns(new BivakAangifteLijstInfo
            {
                AlgemeneStatus =
                    BivakAangifteStatus.NogNietVanBelang
            });

            var channelProviderMock = new Mock<IChannelProvider>();
            channelProviderMock.Setup(mock => mock.GetChannel<ILedenService>()).Returns(new FakeLedenService());


            Factory.InstantieRegistreren(veelGebruiktMock.Object);
            Factory.InstantieRegistreren(channelProviderMock.Object);

            // we doen het volgende:
            // We roepen LedenMaken aan met een model waarbij '0' geselecteerd is als enige afdeling voor
            // een leid(st)er, wat dus wil zeggen dat we een leid(st)er maken zonder afdeling.
            // We verifieren dat de ledenservice niet wordt aangeroepen met dat afdelingsjaar 0.

            // Ik maak een LedenController, omdat PersonenEnLedenController abstract is, en dus 
            // niet als dusdanig kan worden geïnstantieerd.

            var target = Factory.Maak<PersonenController>();
            var model = new InschrijvingsModel
            {
                Inschrijvingen = new List<InschrijfbaarLid>
                {
                    new InschrijfbaarLid
                    {
                        AfdelingsJaarIDs = new int[] {0},
                        LeidingMaken = true,
                        InTeSchrijven = true
                    }
                }
            };

            // act

            target.Inschrijven(GROEPID, model);

            // assert

            var probleem = from p in FakeLedenService.DoorgekregenInschrijving
                           where p.AfdelingsJaarIDs.Any(ajid => ajid == 0)
                           select p;

            Assert.IsNull(probleem.FirstOrDefault());

        }
    }
}
