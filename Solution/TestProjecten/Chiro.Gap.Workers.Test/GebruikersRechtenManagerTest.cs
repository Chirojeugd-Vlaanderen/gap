/*
 * Copyright 2008-2014 the GAP developers. See the NOTICE file at the 
 * top-level directory of this distribution, and at
 * https://develop.chiro.be/gap/wiki/copyright
 * Verfijnen gebruikersrechten Copyright 2015 Chirojeugd-Vlaanderen vzw
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
using System.Linq;
using Chiro.Gap.Domain;
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
        /// <summary>
        /// Test wijzigen van een bestaand gebruikersrecht.
        /// </summary>
        [TestMethod()]
        public void WijzigenTest()
        {
            // ARRANGE

            var gelieerdePersoon = new GelieerdePersoon
            {
                ID = 1,
                Groep = new ChiroGroep { ID = 3 },
                Persoon = new Persoon { ID = 2 }
            };
            gelieerdePersoon.Persoon.GelieerdePersoon.Add(gelieerdePersoon);

            var gebruikersrecht = new GebruikersRechtV2
                                  {
                                      Persoon = gelieerdePersoon.Persoon,
                                      Groep = gelieerdePersoon.Groep,
                                      VervalDatum = DateTime.Now.AddDays(-1)    // gisteren vervallen
                                  };
            gebruikersrecht.Persoon.GebruikersRechtV2.Add(gebruikersrecht);

            // ACT

            var target = new GebruikersRechtenManager();
            target.ToekennenOfWijzigen(gelieerdePersoon.Persoon, gelieerdePersoon.Groep, Permissies.Bewerken,
                Permissies.Bewerken, Permissies.Bewerken, Permissies.Bewerken);

            // ASSERT

            Assert.IsTrue(gebruikersrecht.VervalDatum > DateTime.Now);
        }

        /// <summary>
        /// Test toekennen van een nieuw gebruikersrecht.
        /// </summary>
        [TestMethod()]
        public void ToekennenTest()
        {
            // ARRANGE

            var gelieerdePersoon = new GelieerdePersoon
            {
                ID = 1,
                Groep = new ChiroGroep { ID = 3 },
                Persoon = new Persoon { ID = 2 }
            };
            gelieerdePersoon.Persoon.GelieerdePersoon.Add(gelieerdePersoon);

            // ACT

            var target = new GebruikersRechtenManager();
            target.ToekennenOfWijzigen(gelieerdePersoon.Persoon, gelieerdePersoon.Groep, Permissies.Bewerken,
                Permissies.Bewerken, Permissies.Bewerken, Permissies.Bewerken);
            var result = gelieerdePersoon.Persoon.GebruikersRechtV2.FirstOrDefault();

            // ASSERT

            Assert.IsNotNull(result);
            Assert.AreEqual(gelieerdePersoon.Groep, result.Groep);
            Assert.AreEqual(Permissies.Bewerken, result.PersoonsPermissies);
            Assert.AreEqual(Permissies.Bewerken, result.GroepsPermissies);
            Assert.AreEqual(Permissies.Bewerken, result.AfdelingsPermissies);
            Assert.AreEqual(Permissies.Bewerken, result.IedereenPermissies);
        }

    }
}
