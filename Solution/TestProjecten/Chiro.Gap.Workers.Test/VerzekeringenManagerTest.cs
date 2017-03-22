/*
 * Copyright 2015,2016 Chirojeugd-Vlaanderen vzw. See the NOTICE file at the 
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
using NUnit.Framework;
using Chiro.Gap.Poco.Model;
using Chiro.Gap.Poco.Model.Exceptions;
using Chiro.Gap.Test;
using Chiro.Gap.WorkerInterfaces;

namespace Chiro.Gap.Workers.Test
{
    /// <summary>
    /// Summary description for VerzekeringenManagerTest
    /// </summary>
    [TestFixture]
    public class VerzekeringenManagerTest: ChiroTest
    {
        /// <summary>
        /// Nieuwe verzekering, bestaande is voorbij.
        /// </summary>
        [Test]
        public void WerkjaarVerzekeringGeenOverlap()
        {
            // ARRANGE
            var groepsWerkJarenManager = Factory.Maak<IGroepsWerkJarenManager>();

            var vtype = new VerzekeringsType
            {
                TotEindeWerkJaar = true
            };

            var groepsWerkJaar = new GroepsWerkJaar
            {
                Groep = new ChiroGroep(),
                WerkJaar = 2012
            };
            var vorigGroepsWerkJaar = new GroepsWerkJaar
            {
                Groep = new ChiroGroep(),
                WerkJaar = 2011
            };
            groepsWerkJaar.Groep.GroepsWerkJaar.Add(groepsWerkJaar);
            groepsWerkJaar.Groep.GroepsWerkJaar.Add(vorigGroepsWerkJaar);

            var persoon = new Persoon
            {
                PersoonsVerzekering = new List<PersoonsVerzekering>
                {
                    new PersoonsVerzekering
                    {
                        VerzekeringsType = vtype,
                        Van = new DateTime(2011, 10, 1),
                        // De einddatum moeten we via de groepsWerkJarenManager laten
                        // berekenen, want het einde van een groepswerkjaar valt bijv.
                        // anders bij staging als bij dev.
                        Tot = groepsWerkJarenManager.EindDatum(vorigGroepsWerkJaar)
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
            verzekeringenManager.Verzekeren(lid1, vtype, new DateTime(2012, 10, 1), groepsWerkJarenManager.EindDatum(groepsWerkJaar));

            // ASSERT

            Assert.AreEqual(2, persoon.PersoonsVerzekering.Count);
        }

        /// <summary>
        /// Nieuwe verzekering voor volgend werkjaar, bestaande is nog niet voorbij (#1781).
        /// </summary>
        [Test]
        public void WerkjaarVerzekeringWelOverlap()
        {
            // ARRANGE
            var groepsWerkJarenManager = Factory.Maak<IGroepsWerkJarenManager>();

            var vtype = new VerzekeringsType
            {
                TotEindeWerkJaar = true
            };

            var groepsWerkJaar = new GroepsWerkJaar
            {
                Groep = new ChiroGroep(),
                WerkJaar = 2012
            };
            var vorigGroepsWerkJaar = new GroepsWerkJaar
            {
                Groep = new ChiroGroep(),
                WerkJaar = 2011
            };
            groepsWerkJaar.Groep.GroepsWerkJaar.Add(groepsWerkJaar);
            groepsWerkJaar.Groep.GroepsWerkJaar.Add(vorigGroepsWerkJaar);

            var persoon = new Persoon
            {
                PersoonsVerzekering = new List<PersoonsVerzekering>
                {
                    new PersoonsVerzekering
                    {
                        VerzekeringsType = vtype,
                        Van = new DateTime(2011, 10, 1),
                        // De einddatum moeten we via de groepsWerkJarenManager laten
                        // berekenen, want het einde van een groepswerkjaar valt bijv.
                        // anders bij staging als bij dev.
                        Tot = groepsWerkJarenManager.EindDatum(vorigGroepsWerkJaar)
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
            verzekeringenManager.Verzekeren(lid1, vtype, new DateTime(2012, 7, 28), groepsWerkJarenManager.EindDatum(groepsWerkJaar));

            // ASSERT

            Assert.AreEqual(2, persoon.PersoonsVerzekering.Count);
        }

        /// <summary>
        /// Nieuwe verzekering, niet per werkjaar, bestaande is nog niet voorbij.
        /// </summary>
        [Test]
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

            // ASSERT

            var verzekeringenManager = Factory.Maak<VerzekeringenManager>();
            Assert.Throws<BlokkerendeObjectenException<PersoonsVerzekering>>(() => verzekeringenManager.Verzekeren(lid1,
                vtype, new DateTime(2012, 8, 1), new DateTime(2013, 8, 31)));
        }
    }
}
