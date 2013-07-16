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

using Chiro.Cdf.Ioc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using Chiro.Gap.WorkerInterfaces;
using Chiro.Gap.Poco.Model;
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

            // testgroep; toegang met mijnLogin net vervallen.
            const string mijnLogin = "MijnLogin";

            Groep groep = new ChiroGroep
            {
                GebruikersRecht = new[]
                                                        {
                                                            new GebruikersRecht
                                                                {
                                                                    Gav = new Gav {Login = "MijnLogin"},
                                                                    VervalDatum = DateTime.Today // net vervallen
                                                                }
                                                        }
            };

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
        ///Controleert of de GAV-check van een gelieerde persoon rekening houdt met de vervaldatum van gebruikersrechten.
        ///</summary>
        [TestMethod()]
        public void IsGavGelieerdePersoonTest()
        {
            // ARRANGE

            // testgroep met gelieerde persoon; toegang met mijnLogin net vervallen.
            const string mijnLogin = "MijnLogin";

            var gp = new GelieerdePersoon();

            var groep = new ChiroGroep
                              {
                                  GebruikersRecht = new[]
                                                        {
                                                            new GebruikersRecht
                                                                {
                                                                    Gav = new Gav {Login = "MijnLogin"},
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
