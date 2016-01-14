/*
 * Copyright 2013-2015 Chirojeugd-Vlaanderen vzw. See the NOTICE file at the 
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
using System.Collections.Generic;
using Chiro.Gap.Poco.Model;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Chiro.Gap.Workers.Test
{
    
    
    /// <summary>
    ///This is a test class for GebruikersRechtenManagerTest and is intended
    ///to contain all GebruikersRechtenManagerTest Unit Tests
    ///</summary>
    [TestClass()]
    public class GebruikersRechtenManagerTest
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
        //[ClassInitialize()]
        //public static void MyClassInitialize(TestContext testContext)
        //{
        //}
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
        ///A test for ToekennenOfVerlengen
        ///</summary>
        [TestMethod()]
        public void ToekennenOfVerlengenTest()
        {
            // ARRANGE

            var gav = new Gav();
            var gelieerdePersoon = new GelieerdePersoon
            {
                ID = 1,
                Groep = new ChiroGroep { ID = 3 },
                Persoon = new Persoon { ID = 2, Gav = new List<Gav> { gav } }
            };
            gelieerdePersoon.Persoon.GelieerdePersoon.Add(gelieerdePersoon);
            gav.Persoon.Add(gelieerdePersoon.Persoon);
            var gebruikersrecht = new GebruikersRecht
                                  {
                                      Gav = gav,
                                      Groep = gelieerdePersoon.Groep,
                                      VervalDatum = DateTime.Now.AddDays(-1)    // gisteren vervallen
                                  };
            gav.GebruikersRecht.Add(gebruikersrecht);

            // ACT

            var target = new GebruikersRechtenManager();
            var actual = target.ToekennenOfVerlengen(gav, gelieerdePersoon.Groep);

            // ASSERT

            Assert.IsTrue(gebruikersrecht.VervalDatum > DateTime.Now);
        }
    }
}
