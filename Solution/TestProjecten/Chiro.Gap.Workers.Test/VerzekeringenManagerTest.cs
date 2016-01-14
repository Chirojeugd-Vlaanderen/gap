/*
 * Copyright 2015 Chirojeugd-Vlaanderen vzw. See the NOTICE file at the 
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
using System.Text;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Chiro.Gap.Poco.Model;
using Chiro.Cdf.Ioc.Factory;
using Chiro.Gap.WorkerInterfaces;
using System.Linq;
using Chiro.Gap.Poco.Model.Exceptions;

namespace Chiro.Gap.Workers.Test
{
    /// <summary>
    /// Summary description for VerzekeringenManagerTest
    /// </summary>
    [TestClass]
    public class VerzekeringenManagerTest
    {
        public VerzekeringenManagerTest()
        {
            //
            // TODO: Add constructor logic here
            //
        }

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
        // You can use the following additional attributes as you write your tests:
        //
        // Use ClassInitialize to run code before running the first test in the class
        [ClassInitialize()]
        public static void MyClassInitialize(TestContext testContext)
        {
            Factory.ContainerInit();
        }

        //
        // Use ClassCleanup to run code after all tests in a class have run
        // [ClassCleanup()]
        // public static void MyClassCleanup() { }
        //
        // Use TestInitialize to run code before running each test 
        // [TestInitialize()]
        // public void MyTestInitialize() { }
        //
        // Use TestCleanup to run code after each test has run
        // [TestCleanup()]
        // public void MyTestCleanup() { }
        //
        #endregion


        /// <summary>
        /// Nieuwe verzekering, bestaande is voorbij.
        /// </summary>
        [TestMethod]
        public void WerkjaarVerzekeringGeenOverlap()
        {
            // ARRANGE
            var vtype = new VerzekeringsType
            {
                TotEindeWerkJaar = true
            };

            var groepsWerkJaar = new GroepsWerkJaar
            {
                Groep = new ChiroGroep(),
                WerkJaar = 2012
            };
            groepsWerkJaar.Groep.GroepsWerkJaar.Add(groepsWerkJaar);

            var persoon = new Persoon
            {
                PersoonsVerzekering = new List<PersoonsVerzekering>
                {
                    new PersoonsVerzekering
                    {
                        VerzekeringsType = vtype,
                        Van = new DateTime(2011, 10, 1),
                        Tot = new DateTime(2012, 8, 31)
                    }
                }
            };

            var lid1 = new Leiding
            {
                GroepsWerkJaar = groepsWerkJaar,
                GelieerdePersoon = new GelieerdePersoon { Persoon = persoon }
            };

            // ACT

            var verzekeringenManager = Factory.Maak<VerzekeringenManager>();
            verzekeringenManager.Verzekeren(lid1, vtype, new DateTime(2012, 10, 1), new DateTime(2013, 8, 31));

            // ASSERT

            Assert.AreEqual(2, persoon.PersoonsVerzekering.Count);
        }

        /// <summary>
        /// Nieuwe verzekering voor volgend werkjaar, bestaande is nog niet voorbij (#1781).
        /// </summary>
        [TestMethod]
        public void WerkjaarVerzekeringWelOverlap()
        {
            // ARRANGE
            var vtype = new VerzekeringsType
            {
                TotEindeWerkJaar = true
            };

            var groepsWerkJaar = new GroepsWerkJaar
            {
                Groep = new ChiroGroep(),
                WerkJaar = 2012
            };
            groepsWerkJaar.Groep.GroepsWerkJaar.Add(groepsWerkJaar);

            var persoon = new Persoon
            {
                PersoonsVerzekering = new List<PersoonsVerzekering>
                {
                    new PersoonsVerzekering
                    {
                        VerzekeringsType = vtype,
                        Van = new DateTime(2011, 10, 1),
                        Tot = new DateTime(2012, 8, 31)
                    }
                }
            };

            var lid1 = new Leiding
            {
                GroepsWerkJaar = groepsWerkJaar,
                GelieerdePersoon = new GelieerdePersoon { Persoon = persoon }
            };

            // ACT

            var verzekeringenManager = Factory.Maak<VerzekeringenManager>();
            verzekeringenManager.Verzekeren(lid1, vtype, new DateTime(2012, 8, 1), new DateTime(2013, 8, 31));

            // ASSERT

            Assert.AreEqual(2, persoon.PersoonsVerzekering.Count);
        }

        /// <summary>
        /// Nieuwe verzekering, niet per werkjaar, bestaande is nog niet voorbij.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(BlokkerendeObjectenException<PersoonsVerzekering>))]
        public void NietWerkjaarVerzekeringWelOverlap()
        {
            // ARRANGE
            var vtype = new VerzekeringsType
            {
                TotEindeWerkJaar = false
            };

            var groepsWerkJaar = new GroepsWerkJaar
            {
                Groep = new ChiroGroep(),
                WerkJaar = 2012
            };
            groepsWerkJaar.Groep.GroepsWerkJaar.Add(groepsWerkJaar);

            var persoon = new Persoon
            {
                PersoonsVerzekering = new List<PersoonsVerzekering>
                {
                    new PersoonsVerzekering
                    {
                        VerzekeringsType = vtype,
                        Van = new DateTime(2011, 10, 1),
                        Tot = new DateTime(2012, 8, 31)
                    }
                }
            };

            var lid1 = new Leiding
            {
                GroepsWerkJaar = groepsWerkJaar,
                GelieerdePersoon = new GelieerdePersoon { Persoon = persoon }
            };

            // ACT

            var verzekeringenManager = Factory.Maak<VerzekeringenManager>();
            verzekeringenManager.Verzekeren(lid1, vtype, new DateTime(2012, 8, 1), new DateTime(2013, 8, 31));
        }
    }
}
