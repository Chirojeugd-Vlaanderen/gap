/*
 * Copyright 2008-2013 the GAP developers. See the NOTICE file at the 
 * top-level directory of this distribution, and at
 * https://develop.chiro.be/gap/wiki/copyright
 * Bijgewerkte authenticatie Copyright 2014 Johan Vervloet
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
using Chiro.Cdf.Ioc;
using Chiro.Gap.Poco.Model;
using Chiro.Gap.WorkerInterfaces;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Chiro.Gap.Workers.Test
{
    
    
    /// <summary>
    ///This is a test class for AutorisatieManagerTest and is intended
    ///to contain all AutorisatieManagerTest Unit Tests
    ///</summary>
    [TestClass()]
    public class AutorisatieManagerTest
    {


        private TestContext testContextInstance;

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
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
        [ClassInitialize()]
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
        //[TestInitialize()]
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
        ///Controleert of de GAV-check van een groep rekening houdt met de vervaldatum van gebruikersrechten.
        ///</summary>
        [TestMethod()]
        public void IsGavGroepTest()
        {
            // ARRANGE

            // testgroep; toegang voor deze persoon net vervallen.
            const int mijnAdNummer = 12345;

            Groep groep = new ChiroGroep
            {
                GebruikersRechtV2 = new[]
                                                        {
                                                            new GebruikersRechtV2
                                                                {
                                                                    Persoon = new Persoon {AdNummer = mijnAdNummer},
                                                                    VervalDatum = DateTime.Today // net vervallen
                                                                }
                                                        }
            };

            // Zet mock op voor het opleveren van gebruikersnaam
            var authenticatieManagerMock = new Mock<IAuthenticatieManager>();
            authenticatieManagerMock.Setup(src => src.AdNummerGet()).Returns(mijnAdNummer);
            Factory.InstantieRegistreren(authenticatieManagerMock.Object);


            // ACT

            var target = Factory.Maak<AutorisatieManager>();
            var actual = target.IsGav(groep);


            // ASSERT

            Assert.IsFalse(actual);
        }

        /// <summary>
        /// Als je gebruikersrechten hebt, maar geen GAV-permissies, dan mag IsGav ook niet zeggen
        /// dat je GAV bent.
        /// </summary>
        [TestMethod()]
        public void RechtenMaarGeenGavTest()
        {
            // ARRANGE

            var gebruikersRecht = new GebruikersRechtV2
            {
                Persoon = new Persoon { ID = 1, AdNummer = 2 },
                Groep = new ChiroGroep { ID = 3 },
                Permissies = Domain.Permissies.Geen,
                VervalDatum = DateTime.Now.AddDays(1)       // geldig tot morgen.
            };
            gebruikersRecht.Persoon.GebruikersRechtV2.Add(gebruikersRecht);
            gebruikersRecht.Groep.GebruikersRechtV2.Add(gebruikersRecht);

            var authenticatieManagerMock = new Mock<IAuthenticatieManager>();
            authenticatieManagerMock.Setup(mgr => mgr.AdNummerGet()).Returns(gebruikersRecht.Persoon.AdNummer);
            Factory.InstantieRegistreren(authenticatieManagerMock.Object);

            // ACT

            var target = Factory.Maak<AutorisatieManager>();
            bool actual = target.IsGav(gebruikersRecht.Groep);

            // ASSERT

            Assert.IsFalse(actual);
        }


        /// <summary>
        ///Controleert of de GAV-check van een gelieerde persoon rekening houdt met de vervaldatum van gebruikersrechten.
        ///</summary>
        [TestMethod()]
        public void IsGavGelieerdePersoonTest()
        {
            // ARRANGE

            // testgroep met gelieerde persoon; toegang met mijnLogin net vervallen.
            const string mijnLogin = "MijnLogin";

            var gp = new GelieerdePersoon { Persoon = new Persoon() };

            var groep = new ChiroGroep
                              {
                                  GebruikersRechtV2 = new[]
                                                        {
                                                            new GebruikersRechtV2
                                                                {
                                                                    Persoon = gp.Persoon,
                                                                    VervalDatum = DateTime.Today // net vervallen
                                                                }
                                                        },
                                  GelieerdePersoon = new[]
                                                         {
                                                            gp
                                                         }
                              };

            gp.Groep = groep;

            // Zet mock op voor het opleveren van gebruikersnaam
            var authenticatieManagerMock = new Mock<IAuthenticatieManager>();
            authenticatieManagerMock.Setup(src => src.GebruikersNaamGet()).Returns(mijnLogin);
            Factory.InstantieRegistreren(authenticatieManagerMock.Object);


            // ACT

            var target = Factory.Maak<AutorisatieManager>();
            var actual = target.IsGav(groep);


            // ASSERT

            Assert.IsFalse(actual);
        }

        /// <summary>
        /// Zelfs om te testen zijn we geen super-GAV
        /// </summary>
        [TestMethod()]
        public void IsSuperGavTest()
        {
            var target = Factory.Maak<AutorisatieManager>();

            Assert.IsFalse(target.IsSuperGav());
        }
    }
}
