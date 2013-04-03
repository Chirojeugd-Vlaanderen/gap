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
﻿using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using WebServices.Test.GroepenServiceReference;
using Cg2.Core.Domain;

namespace WebServices.Test
{
    /// <summary>
    /// Summary description for GroepenServiceTest
    /// </summary>
    [TestClass]
    public class GroepenServiceTest
    {
        public GroepenServiceTest()
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
        // [ClassInitialize()]
        // public static void MyClassInitialize(TestContext testContext) { }
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

        [TestMethod]
        public void ServiceAanroepen()
        {
            using (GroepenServiceReference.GroepenServiceClient service = new GroepenServiceReference.GroepenServiceClient())
            {
                string antwoord = service.Hallo();

                Assert.IsTrue(antwoord.Length > 0);
            }
        }

        [TestMethod]
        public void GroepOphalen()
        {
            using (GroepenServiceReference.GroepenServiceClient service = new GroepenServiceReference.GroepenServiceClient())
            {
                Groep antwoord = service.Ophalen(310);

                Assert.IsTrue(antwoord.Naam.Length > 0);
            }
        }

        [TestMethod]
        public void GroepUpdatenMetOrigineel()
        {
            using (GroepenServiceReference.GroepenServiceClient service = new GroepenServiceReference.GroepenServiceClient())
            {
                Groep orig = service.Ophalen(310);
                Groep g = service.Ophalen(310);

                string oudeNaam = g.Naam;

                if (oudeNaam == "Valentijn")
                {
                    g.Naam = "Viersel";
                }
                else
                {
                    g.Naam = "Valentijn";
                }

                // Updaten MET origineel
                service.Updaten(g, orig);

                // Opnieuw opghalen

                Groep h = service.Ophalen(310);

                Assert.IsTrue(oudeNaam != h.Naam);
            }
        }

        [TestMethod]
        public void GroepUpdaten()
        {
            using (GroepenServiceReference.GroepenServiceClient service = new GroepenServiceReference.GroepenServiceClient())
            {
                Groep g = service.Ophalen(310);

                string oudeNaam = g.Naam;

                if (oudeNaam == "Valentijn")
                {
                    g.Naam = "Viersel";
                }
                else
                {
                    g.Naam = "Valentijn";
                }

                // Updaten ZONDER origineel
                service.Updaten(g, null);

                // Opnieuw opghalen

                Groep h = service.Ophalen(310);

                Assert.IsTrue(oudeNaam != h.Naam);
            }
        }

        [TestMethod]
        public void GroepConcurrencyMetOrigineel()
        {
            bool exceptieGevangen = false;

            using (GroepenServiceClient service = new GroepenServiceClient())
            {
                Groep orig = service.Ophalen(310);
                Groep g = service.Ophalen(310);
                Groep h = service.Ophalen(310);

                if (g.Naam == "Valentijn")
                {
                    g.Naam = "Viersel";
                    h.Naam = "Viersel2";
                }
                else
                {
                    g.Naam = "Valentijn";
                    h.Naam = "Valentijn2";
                }

                // updates MET origineel

                service.Updaten(g, orig);

                try
                {
                    service.Updaten(h, orig);
                }
                catch (System.ServiceModel.FaultException)
                {
                    // TODO: fatsoenlijke exceptie vangen
                    exceptieGevangen = true;
                }


                Assert.IsTrue(exceptieGevangen);

            }
        }

        [TestMethod]
        public void GroepConcurrency()
        {
            bool exceptieGevangen = false;

            using (GroepenServiceClient service = new GroepenServiceClient())
            {
                Groep g = service.Ophalen(310);
                Groep h = service.Ophalen(310);

                if (g.Naam == "Valentijn")
                {
                    g.Naam = "Viersel";
                    h.Naam = "Viersel2";
                }
                else
                {
                    g.Naam = "Valentijn";
                    h.Naam = "Valentijn2";
                }

                // updates ZONDER origineel

                service.Updaten(g, null);

                try
                {
                    service.Updaten(h, null);
                }
                catch (System.ServiceModel.FaultException)
                {
                    // TODO: fatsoenlijke exceptie vangen
                    exceptieGevangen = true;
                }


                Assert.IsTrue(exceptieGevangen);

            }
        }

    }
}
