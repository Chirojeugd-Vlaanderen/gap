/*
 * Copyright 2009-2015 Chirojeugd-Vlaanderen vzw.
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
using Chiro.Gap.Domain;
using Chiro.Gap.Poco.Model;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Chiro.Gap.Workers.Test
{ 
    /// <summary>
    ///This is a test class for GelieerdePersonenManagerTest and is intended
    ///to contain all GelieerdePersonenManagerTest Unit Tests
    ///</summary>
    [TestClass()]
    public class GelieerdePersonenManagerTest
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
        ///Controleert of voorkeursadres goed wordt bewaardgezet door PersonenManager.AdresToevoegen
        ///</summary>
        [TestMethod()]
        public void AdresToevoegenVoorkeurTest()
        {
            // ARRANGE

            var gelieerdePersoon = new GelieerdePersoon { Persoon = new Persoon() };
            gelieerdePersoon.Persoon.GelieerdePersoon.Add(gelieerdePersoon);

            var adres = new BelgischAdres();

            // ACT

            var target = new GelieerdePersonenManager();
            target.AdresToevoegen(new List<GelieerdePersoon>{gelieerdePersoon}, adres, AdresTypeEnum.Thuis, true);
            
            // ASSERT

            // vind het persoonsadresobject
            var persoonsAdres = gelieerdePersoon.Persoon.PersoonsAdres.First();

            // weet het persoonsadresobject dat het voorkeursadres is?
            Assert.IsNotNull(persoonsAdres.GelieerdePersoon.FirstOrDefault());
        }

        /// <summary>
        /// Als een glieerde persoon een nieuw voorkeursadres krijgt, dan moet de link van het oude
        /// 'voorkeurspersoonsadres' naar de gelieerde persoon verdwijnen. Een test. :-)
        ///</summary>
        [TestMethod()]
        public void AdresToevoegenOudeVoorkeurTest()
        {
            // ARRANGE

            var gelieerdePersoon = new GelieerdePersoon { Persoon = new Persoon() };
            gelieerdePersoon.Persoon.GelieerdePersoon.Add(gelieerdePersoon);

            var oudVoorkeursAdres = new BelgischAdres {ID = 1};
            var oudPersoonsAdres = new PersoonsAdres
            {
                Persoon = gelieerdePersoon.Persoon,
                Adres = oudVoorkeursAdres,
                GelieerdePersoon = new List<GelieerdePersoon>{gelieerdePersoon}
            };
            // Het voorkeursadres is gekoppeld aan gelieerde persoon, alle adressen aan persoon.
            gelieerdePersoon.PersoonsAdres = oudPersoonsAdres;
            gelieerdePersoon.Persoon.PersoonsAdres.Add(oudPersoonsAdres);

            var nieuwVoorkeursAdres = new BelgischAdres {ID = 2};

            // ACT

            var target = new GelieerdePersonenManager();
            target.AdresToevoegen(new List<GelieerdePersoon> { gelieerdePersoon }, nieuwVoorkeursAdres, AdresTypeEnum.Thuis, true);

            // ASSERT

            // het oude voorkeursadres mag niet meer aan gelieerdePersoon gekoppeld zijn.
            // (wel nog aan persoon, natuurlijk)
            Assert.IsFalse(oudPersoonsAdres.GelieerdePersoon.Any());
        }

        /// <summary>
        /// Een test voor AdresGenotenUitZelfdeGroep
        ///</summary>
        [TestMethod()]
        public void AdresGenotenZelfdeGroepTest()
        {
            // ARRANGE

            var gelieerdePersoon1 = new GelieerdePersoon {ID = 2, Persoon = new Persoon {ID = 4}};
            gelieerdePersoon1.Persoon.GelieerdePersoon.Add(gelieerdePersoon1);

            var gelieerdePersoon2 = new GelieerdePersoon {ID = 3, Persoon = new Persoon {ID = 5}};
            gelieerdePersoon2.Persoon.GelieerdePersoon.Add(gelieerdePersoon2);
            
            var groep = new ChiroGroep
            {
                ID = 1,
                GelieerdePersoon = new List<GelieerdePersoon> {gelieerdePersoon1, gelieerdePersoon2}
            };
            gelieerdePersoon1.Groep = groep;
            gelieerdePersoon2.Groep = groep;

            var adres = new BelgischAdres();

            var persoonsAdres1 = new PersoonsAdres {ID = 6, Persoon = gelieerdePersoon1.Persoon, Adres = adres};
            var persoonsAdres2 = new PersoonsAdres {ID = 7, Persoon = gelieerdePersoon2.Persoon, Adres = adres};

            gelieerdePersoon1.Persoon.PersoonsAdres = new List<PersoonsAdres> {persoonsAdres1};
            gelieerdePersoon2.Persoon.PersoonsAdres = new List<PersoonsAdres> {persoonsAdres2};
            adres.PersoonsAdres = new List<PersoonsAdres> {persoonsAdres1, persoonsAdres2};

            // ACT

            var target = new GelieerdePersonenManager();
            var result = target.AdresGenotenUitZelfdeGroep(gelieerdePersoon1);

            // ASSERT

            Assert.IsTrue(result.Contains(gelieerdePersoon1));
            Assert.IsTrue(result.Contains(gelieerdePersoon2));
        }
    }
}
