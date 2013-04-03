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

using Chiro.Cdf.Data.Entity;

namespace Chiro.Cdf.Data.Test
{
    /// <summary>
    /// Summary description for VerwijderTest
    /// </summary>
    [TestClass]
    public class VerwijderTest
    {
        public VerwijderTest()
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

        /// <summary>
        /// Verwijdert een adres van een persoon
        /// </summary>
        [TestMethod]
        public void AdresVerwijderen()
        {
            #region Arrange
            int pid = TestHelpers.PersoonMetTweeAdressenMaken();
            GelieerdePersoon p = TestHelpers.PersoonOphalen(pid);

            int adrid1 = p.PersoonsAdres.First().Adres.ID;
            #endregion

            #region Act
            p.PersoonsAdres.Last().TeVerwijderen = true;

            using (Entities db = new Entities())
            {
                db.AttachObjectGraph(p, bla => bla.PersoonsAdres, bla => bla.PersoonsAdres.First().Adres);
                db.SaveChanges();
            }
            #endregion

            #region Assert
            GelieerdePersoon q = TestHelpers.PersoonOphalen(pid);
            Assert.AreEqual(q.PersoonsAdres.Count(), 1);
            Assert.AreEqual(q.PersoonsAdres.First().Adres.ID, adrid1);
            #endregion
        }

        /// <summary>
        /// De bedoeling is dat de persoon losgekoppeld wordt van de categorie.
        /// </summary>
        [TestMethod]
        public void CategorieVerwijderen()
        {
            #region arrange
            int gpid = TestHelpers.PersoonMetCategorieMaken();
            GelieerdePersoon gp = TestHelpers.PersoonOphalenMetCategorieen(gpid);
            #endregion

            #region act
            gp.Categorie.First().TeVerwijderen = true;

            using (Entities db = new Entities())
            {
                db.AttachObjectGraph(gp, foo => foo.Categorie);
                db.SaveChanges();
            }
            #endregion

            #region assert
            GelieerdePersoon gp2 = TestHelpers.PersoonOphalenMetCategorieen(gpid);
            Assert.AreEqual(gp2.Categorie.Count(), 0);
            #endregion
        }
    }
}
