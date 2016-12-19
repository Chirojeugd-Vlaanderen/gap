/*
 * Copyright 2016 Chirojeugd-Vlaanderen vzw. See the NOTICE file at the 
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

using System;
using Chiro.Ad.DirectoryInterface;
using Chiro.Ad.Domain;
using Chiro.Cdf.Ioc.Factory;
using Chiro.Cdf.Mailer;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Chiro.Ad.Workers.Test
{
    [TestClass]
    public class LoginManagerTest
    {
        [ClassInitialize]
        public static void MyClassInitialize(TestContext testContext)
        {
            Factory.ContainerInit();
        }

        [TestMethod]
        public void ZoekenOfMakenTest()
        {
            // ARRANGE

            const int adnr = 1;
            const string voornaam = "Jos";
            const string familienaam = "Pros";
            const string verwachteLogin = "jos.pros";
            const string mailadres = "jos@pros.com";

            var mailerMoq = new Mock<IMailer>();
            var adMoq = new Mock<IDirectoryAccess>();

            // Onze gemockte Active Directory zal de gebruiker niet vinden op AD-nummer
            adMoq.Setup(src => src.GebruikerZoeken(It.IsAny<string>(), It.IsAny<int>())).Returns((Chirologin) null);
            // en ook niet op naam en voornaam (er moeten dus geen extra cijfers toegevoegd worden aan login)
            adMoq.Setup(src => src.GebruikerZoeken(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                .Returns((Chirologin) null);
            // Verwacht een vraag om de login te bewaren.
            adMoq.Setup(src => src.NieuweGebruikerBewaren(It.IsAny<Chirologin>(), It.IsAny<string>())).Verifiable();

            Factory.InstantieRegistreren(mailerMoq.Object);
            Factory.InstantieRegistreren(adMoq.Object);

            // ACT
            var worker = Factory.Maak<LoginManager>();
            var result = worker.ZoekenOfMaken(DomeinEnum.Wereld, adnr, voornaam, familienaam, mailadres);

            // ASSERT

            Assert.AreEqual(result.AdNr, adnr);
            Assert.AreEqual(result.Voornaam, voornaam);
            Assert.AreEqual(result.Familienaam, familienaam);
            Assert.AreEqual(result.Login, verwachteLogin);
            Assert.AreEqual(result.Mailadres, mailadres);
            Assert.IsNotNull(result.Domein);
            // we gaan ervan uit dat de lijst met securitygroepen altijd bestaat.
            Assert.IsNotNull(result.SecurityGroepen);
        }
    }
}
