/*
 * Copyright 2014 Chirojeugd-Vlaanderen vzw. See the NOTICE file at the 
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

using System;
using System.Collections.Generic;
using Chiro.Cdf.Ioc;
using Chiro.Gap.Domain;
using Chiro.Gap.Poco.Model;
using Chiro.Gap.WorkerInterfaces;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Chiro.Gap.Validatie.Test
{
    /// <summary>
    /// Testclass LidVoorstelValidator
    /// </summary>
    [TestClass]
    public class LidVoorstelValidatorTest
    {
        /// <summary>
        /// Run code before running each test
        /// </summary>
        [TestInitialize]
        public void MyTestInitialize()
        {
            Factory.ContainerInit();
        }

        /// <summary>
        /// Kleuters moeten gedetecteerd worden bij validatie lidvoorstel.
        ///</summary>
        [TestMethod()]
        public void ValidatieLidVoorstelKleutersTest()
        {
            // Arrange
            // Ik maak een persoon, en een werkjaar met 2 afdelingen.  Maar dat is hier
            // niet relevant. Wat wel van belang is, is dat de persoon te jong is om
            // lid te worden.

            var gwj = new GroepsWerkJaar { WerkJaar = 2012, Groep = new ChiroGroep() };  // Werkjaar 2012-2013

            var gp = new GelieerdePersoon
            {
                Persoon =
                    new Persoon
                    {
                        GeboorteDatum = new DateTime(2010, 06, 21), // Geboren in 2010
                        Geslacht = GeslachtsType.Vrouw,
                    },
                Groep = gwj.Groep
            };

            gwj.AfdelingsJaar.Add(new AfdelingsJaar
            {
                ID = 1,
                GeboorteJaarVan = 1999,
                GeboorteJaarTot = 2000,
                Geslacht = GeslachtsType.Gemengd,
                OfficieleAfdeling = new OfficieleAfdeling()
            });

            gwj.AfdelingsJaar.Add(new AfdelingsJaar
            {
                ID = 2,
                GeboorteJaarVan = 1997,
                GeboorteJaarTot = 1998,
                Geslacht = GeslachtsType.Gemengd,
                OfficieleAfdeling = new OfficieleAfdeling()
            });

            var lidVoorstel = new LidVoorstel
            {
                GelieerdePersoon = gp,
                GroepsWerkJaar = gwj,
                AfdelingsJarenIrrelevant = true,
                LeidingMaken = false
            };

            var target = new LidVoorstelValidator();

            // Act

            var foutNummer = target.FoutNummer(lidVoorstel);

            // Assert

            Assert.AreEqual(FoutNummer.LidTeJong, foutNummer);
        }

        /// <summary>
        /// Personen zonder (voorkeurs)adres moeten gedetecteerd worden bij validatie lidvoorstel.
        ///</summary>
        [TestMethod()]
        public void ValidatieLidVoorstelZonderAdresTest()
        {
            // Arrange
            // Ik maak een persoon, en een werkjaar met 2 afdelingen.  Maar dat is hier
            // niet relevant. Wat wel van belang is, is dat de persoon te jong is om
            // lid te worden. We verwachten dat het maken van een lidvoorstel crasht.

            var gwj = new GroepsWerkJaar { WerkJaar = 2014, Groep = new ChiroGroep() };  // Werkjaar 2012-2013

            var gp = new GelieerdePersoon
            {
                Persoon =
                    new Persoon
                    {
                        GeboorteDatum = new DateTime(2000, 06, 21),
                        Geslacht = GeslachtsType.Vrouw,
                    },
                Groep = gwj.Groep,
                PersoonsAdres = null    // Geen Adres
            };

            gwj.AfdelingsJaar.Add(new AfdelingsJaar
            {
                ID = 1,
                GeboorteJaarVan = 1999,
                GeboorteJaarTot = 2000,
                Geslacht = GeslachtsType.Gemengd,
                OfficieleAfdeling = new OfficieleAfdeling()
            });

            gwj.AfdelingsJaar.Add(new AfdelingsJaar
            {
                ID = 2,
                GeboorteJaarVan = 1997,
                GeboorteJaarTot = 1998,
                Geslacht = GeslachtsType.Gemengd,
                OfficieleAfdeling = new OfficieleAfdeling()
            });

            var lidVoorstel = new LidVoorstel
            {
                GelieerdePersoon = gp,
                GroepsWerkJaar = gwj,
                AfdelingsJarenIrrelevant = true,
                LeidingMaken = false
            };

            var target = new LidVoorstelValidator();

            // Act

            var foutNummer = target.FoutNummer(lidVoorstel);

            // Assert

            Assert.AreEqual(FoutNummer.AdresOntbreekt, foutNummer);
        }

        /// <summary>
        /// Personen zonder telefoonnummer moeten gedetecteerd worden bij validatie lidvoorstel.
        ///</summary>
        [TestMethod()]
        public void ValidatieLidVoorstelZonderTelefoonNummer()
        {
            // Arrange
            // Ik maak een persoon, en een werkjaar met 2 afdelingen.  Maar dat is hier
            // niet relevant. Wat wel van belang is, is dat de persoon te jong is om
            // lid te worden. We verwachten dat het maken van een lidvoorstel crasht.

            var gwj = new GroepsWerkJaar { WerkJaar = 2014, Groep = new ChiroGroep() };  // Werkjaar 2012-2013

            var gp = new GelieerdePersoon
            {
                Persoon =
                    new Persoon
                    {
                        GeboorteDatum = new DateTime(2000, 06, 21),
                        Geslacht = GeslachtsType.Vrouw,
                    },
                Groep = gwj.Groep,
                PersoonsAdres = new PersoonsAdres(),
                Communicatie = new List<CommunicatieVorm>() // geen telefoonnummer
            };

            gwj.AfdelingsJaar.Add(new AfdelingsJaar
            {
                ID = 1,
                GeboorteJaarVan = 1999,
                GeboorteJaarTot = 2000,
                Geslacht = GeslachtsType.Gemengd,
                OfficieleAfdeling = new OfficieleAfdeling()
            });

            gwj.AfdelingsJaar.Add(new AfdelingsJaar
            {
                ID = 2,
                GeboorteJaarVan = 1997,
                GeboorteJaarTot = 1998,
                Geslacht = GeslachtsType.Gemengd,
                OfficieleAfdeling = new OfficieleAfdeling()
            });

            var lidVoorstel = new LidVoorstel
            {
                GelieerdePersoon = gp,
                GroepsWerkJaar = gwj,
                AfdelingsJarenIrrelevant = true,
                LeidingMaken = false
            };

            var target = new LidVoorstelValidator();

            // Act

            var foutNummer = target.FoutNummer(lidVoorstel);

            // Assert

            Assert.AreEqual(FoutNummer.TelefoonNummerOntbreekt, foutNummer);
        }


    }
}
