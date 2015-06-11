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

using System.Collections.Generic;
using System.Linq;
using Chiro.Cdf.Ioc;
using Chiro.Gap.Domain;
using Chiro.Gap.Poco.Model;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Chiro.Gap.Workers.Test
{
    /// <summary>
    /// Dit is een testclass voor Unit Tests van GroepsWerkJaarManagerTest
    /// </summary>
	[TestClass]
	public class GroepsWerkJarenManagerTest
	{


		private TestContext _testContextInstance;

		/// <summary>
		///Gets or sets the test context which provides
		///information about and functionality for the current test run.
		/// </summary>
		public TestContext TestContext
		{
			get
			{
				return _testContextInstance;
			}
			set
			{
				_testContextInstance = value;
			}
		}

		[ClassInitialize]
		public static void MyClassInitialize(TestContext testContext)
		{
			Factory.ContainerInit();
		}

        /// <summary>
        /// Snelle test die nakijkt of GroepsWerkJaarManager.AfdelingsJarenVoorstellen daadwerkelijk de geboortedata
        /// voor de nieuwe afdelingsjaren bijwerkt.
        ///</summary>
        [TestMethod()]
        public void AfdelingsJarenVoorstellenTest()
        {
            // ARRANGE: dom modelleke opbouwen
            var groep = new ChiroGroep{Afdeling = new List<Afdeling>(), GroepsWerkJaar = new List<GroepsWerkJaar>()};
            var groepsWerkJaar = new GroepsWerkJaar
                                     {
                                         WerkJaar = 2010,
                                         ID = 2971,
                                         Groep = groep,
                                         AfdelingsJaar = new List<AfdelingsJaar>()
                                     };
            groep.GroepsWerkJaar.Add(groepsWerkJaar);
            var afdeling = new Afdeling { ID = 2337, ChiroGroep = groep };
            groep.Afdeling.Add(afdeling);
            var afdelingsJaar = new AfdelingsJaar
                {GroepsWerkJaar = groepsWerkJaar, Afdeling = afdeling, GeboorteJaarVan = 2003, GeboorteJaarTot = 2004};
            groepsWerkJaar.AfdelingsJaar.Add(afdelingsJaar);

            // ACT: stel nieuwe afdelingsjaren voor
            var target = Factory.Maak<GroepsWerkJarenManager>();
            var actual = target.AfdelingsJarenVoorstellen(groep, groep.Afdeling.ToList(), 2011, new OfficieleAfdeling());

            // ASSERT
            Assert.IsNotNull(actual);
            Assert.IsNotNull(actual[0]);
            Assert.AreEqual(actual[0].GeboorteJaarVan, afdelingsJaar.GeboorteJaarVan + 1);
            Assert.AreEqual(actual[0].GeboorteJaarTot, afdelingsJaar.GeboorteJaarTot + 1);
        }

        /// <summary>
        /// Deze test moet nagaan of GroepsWerkjaarManager.AfdelingsJarenVoorstellen ook een
        /// officiele afdeling koppelt aan een afdelingsjaar voor een afdeling die vorig jaar
        /// niet actief was.
        ///</summary>
        [TestMethod()]
        public void AfdelingsJarenVoorstellenOfficieleAfdelingTest()
        {
            // -- Arrange --

            // Testsituatie opzetten

            var ribbels = new OfficieleAfdeling
                              {
                                  ID = (int) NationaleAfdeling.Ribbels,
                                  LeefTijdVan = 6,
                                  LeefTijdTot = 7
                              };

            // Een Chirogroep met een oud groepswerkjaar. Zonder afdelingen, why not.
            var groep = new ChiroGroep
                {GroepsWerkJaar = new List<GroepsWerkJaar> {new GroepsWerkJaar()}};

            // Dit jaar willen we een groep met 1 afdeling.
            var afdelingen = new List<Afdeling> {new Afdeling {ID = 1, ChiroGroep = groep}};

            const int NIEUW_WERKJAAR = 2012; // Jaartal is eigenlijk irrelevant voor deze test.

            // -- Act -- 
            var target = Factory.Maak<GroepsWerkJarenManager>();

            var actual = target.AfdelingsJarenVoorstellen(groep, afdelingen, NIEUW_WERKJAAR, ribbels);
            var afdelingsJaar = actual.FirstOrDefault();

            // -- Assert --

            Assert.IsNotNull(afdelingsJaar);
            Assert.IsNotNull(afdelingsJaar.OfficieleAfdeling);
        }

        /// <summary>
        /// Als een afdelingsjaar vroeger gemengd m/v was, dan moet er nu gemengd m/v/x
        /// voorgesteld worden.
        ///</summary>
        [TestMethod()]
        public void AfdelingsJarenVoorstellenMvxGemengdTest()
        {
            // -- Arrange --

            // Testsituatie opzetten

            var ribbels = new OfficieleAfdeling
            {
                ID = (int)NationaleAfdeling.Ribbels,
                LeefTijdVan = 6,
                LeefTijdTot = 7
            };

            // Een Chirogroep met een oud groepswerkjaar.
            var groep = new ChiroGroep ();
            var sloebers = new Afdeling {ID = 1, ChiroGroep = groep};
            var oudGroepsWerkJaar = new GroepsWerkJaar {Groep = groep};
            groep.GroepsWerkJaar.Add(oudGroepsWerkJaar);
            oudGroepsWerkJaar.AfdelingsJaar.Add(new AfdelingsJaar
            {
                Afdeling = sloebers,
                GroepsWerkJaar = oudGroepsWerkJaar,
                OfficieleAfdeling = ribbels,
                // Geslacht uit de oude doos:
                Geslacht = (GeslachtsType.Man | GeslachtsType.Vrouw)
            });

            // Dit jaar willen we een groep met 1 afdeling.
            var afdelingen = new List<Afdeling> { sloebers };

            const int NIEUW_WERKJAAR = 2012; // Jaartal is eigenlijk irrelevant voor deze test.

            // -- Act -- 
            var target = Factory.Maak<GroepsWerkJarenManager>();

            var actual = target.AfdelingsJarenVoorstellen(groep, afdelingen, NIEUW_WERKJAAR, ribbels);
            var afdelingsJaar = actual.FirstOrDefault();

            // -- Assert --

            Assert.IsNotNull(afdelingsJaar);
            Assert.AreEqual(afdelingsJaar.Geslacht, GeslachtsType.Gemengd);
        }
    }
}
