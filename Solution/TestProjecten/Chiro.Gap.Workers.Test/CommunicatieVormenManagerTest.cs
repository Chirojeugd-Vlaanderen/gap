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

using System.Data.Objects.DataClasses;
using Chiro.Cdf.Ioc;
using Chiro.Gap.Poco.Model;
using Chiro.Gap.ServiceContracts.DataContracts;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Chiro.Gap.Workers.Test
{
    
    
    /// <summary>
    /// Dit is een testclass voor CommVormManagerTest,
    ///to contain all CommVormManagerTest Unit Tests
    /// </summary>
    [TestClass()]
    public class CommunicatieVormenManagerTest
    {


        private TestContext testContextInstance;

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
        // 
        //You can use the following additional attributes as you write your tests:
        //
        //Use ClassInitialize to run code before running the first test in the class

        /// <summary>
        /// Code uit te voeren voor de 1ste test uitgevoerd wordt.
        /// </summary>
        /// <param name="testContext"></param>
        [ClassInitialize]
        public static void MyClassInitialize(TestContext testContext)
        {
            Factory.ContainerInit();
        }

        //
        //Use ClassCleanup to run code after all tests in a class have run
        //[ClassCleanup()]
        //public static void MyClassCleanup()
        //{
        //}
        //
        //Use TestInitialize to run code before running each test
        //[TestInitialize]
        //public void MyTestInitialize()
        //{
        //}
        //
        //Use TestCleanup to run code after each test has run
        //[TestCleanup()]
        //public void MyTestCleanup()
        //{
        //}
        //
        #endregion


        /// <summary>
        /// Als een persoon al een voorkeurs-e-mailadres heeft, en er wordt een nieuw
        /// vooorkeurs-e-mailadres gekoppeld, dan moet het bestaande adres zijn voorkeur
        /// verliezen.
        /// </summary>
        [TestMethod]
        public void KoppelenVoorkeursCommunicatieTest()
        {
            // Arrange

            const int TESTGPID = 1234;      // arbitrair ID van een gelieerde persoon
            const int TESTCVID = 2345;      // en van een communicatievorm
            const int TESTCTID = 3;         // en diens communicatietype

            var testCommunicatieType = new CommunicatieType { ID = TESTCTID, Validatie = ".*" };
            var testCommunicatieVorm = new CommunicatieVorm
            {
                ID = TESTCVID,
                CommunicatieType = testCommunicatieType,
                Nummer = "jos@linux.be",
                Voorkeur = true
            };

            // Koppel gauw testCommunicatieVorm aan testGelieerdePersoon

            var testGelieerdePersoon = new GelieerdePersoon
                                           {
                                               ID = TESTGPID,
                                               Persoon = new Persoon(),
                                               Communicatie =
                                                   new EntityCollection<CommunicatieVorm>
                                                       {
                                                           testCommunicatieVorm
                                                       }
                                           };
            testCommunicatieVorm.GelieerdePersoon = testGelieerdePersoon;

            var target = Factory.Maak<CommunicatieVormenManager>();

            CommunicatieVorm nieuwecv = new CommunicatieVorm
            {
                CommunicatieType = testCommunicatieType,    // e-mail
                ID = 0,                    // nieuwe communicatievorm
                Nummer = "johan@linux.be", // arbitrair nieuw e-mailadres
                Voorkeur = true
            };

            // act

            target.Koppelen(testGelieerdePersoon, nieuwecv);

            // assert

            Assert.IsFalse(testCommunicatieVorm.Voorkeur);
        }

        /// <summary>
        /// Als een communicatievorm voorkeur wordt gemaakt voor zijn type, dan moet
        /// de huidige voorkeurscommunicatie aangepast worden, want die
        /// verliest zijn voorkeur.
        /// </summary>
        [TestMethod]
        public void CommunicatieVormVoorkeurMakenTest()
        {
            // Vroeger testten we of de andere communicatie wel degelijk gepersisteerd
            // werd. Vooral omdat we dat toen zelf moesten implementeren. Nu wordt
            // bewaard met 'Context.SaveChanges'. Ik vermoed niet dat we EntityFramework
            // moeten debuggen, dus ik paste deze test aan, zodat die gewoon nakijkt
            // of 

            // Arrange

            const int TESTGPID = 1234;      // arbitrair ID van een gelieerde persoon
            const int TESTCVID = 2345;      // en van een communicatievorm
            const int TESTCVID2 = 2346;     // en van een andere communicatievorm
            const int TESTCTID = 3;         // en hun communicatietype

            var testCommunicatieType = new CommunicatieType { ID = TESTCTID, Validatie = ".*" };

            var testGelieerdePersoon = new GelieerdePersoon
            {
                ID = TESTGPID,
                Persoon = new Persoon()
            };

            var testCommunicatieVorm1 = new CommunicatieVorm
            {
                ID = TESTCVID,
                CommunicatieType = testCommunicatieType,
                Nummer = "jos@linux.be",
                Voorkeur = true,
                GelieerdePersoon = testGelieerdePersoon
            };

            var testCommunicatieVorm2 = new CommunicatieVorm
            {
                ID = TESTCVID2,
                CommunicatieType = testCommunicatieType,
                Nummer = "johan@linux.be",
                Voorkeur = false,
                GelieerdePersoon = testGelieerdePersoon
            };

            testGelieerdePersoon.Communicatie = new[] {testCommunicatieVorm1, testCommunicatieVorm2};

            var info = new CommunicatieInfo
            {
                CommunicatieTypeID = TESTCTID,    // ID testcommunicatietype
                ID = TESTCVID2,                   // ID niet-voorkeurscommunicatie
                Nummer = "johan@linux.be",        // arbitrair nieuw e-mailadres
                Voorkeur = true                   // nu wel voorkeur
            };

            var target = Factory.Maak<CommunicatieVormenManager>();

            // Act
            target.Bijwerken(testCommunicatieVorm2,info);

            // Assert

            // We hebben communicatievorm2 aangepast, maar omdat die de voorkeur
            // krijgt, moet communicatievorm1 zijn voorkeur verliezen.

            Assert.IsFalse(testCommunicatieVorm1.Voorkeur);
        }
    }
}
