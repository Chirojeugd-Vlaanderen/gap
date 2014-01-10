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

using System.Linq;
using Chiro.Cdf.Poco;
using Chiro.Gap.Dummies;
using Chiro.Gap.Poco.Model;
using Chiro.Gap.ServiceContracts.DataContracts;

using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.ServiceModel;
using Chiro.Cdf.Ioc;
using Chiro.Gap.ServiceContracts.FaultContracts;
using Chiro.Gap.ServiceContracts.Mappers;
using Moq;

namespace Chiro.Gap.Services.Test
{
    /// <summary>
    /// Summary description for CategorieToevoegen
    /// </summary>
    [TestClass]
    public class CategorieVerwijderen
    {
        #region initialisatie en afronding

        [ClassInitialize]
        static public void InitialiseerTests(TestContext tc)
        {
            // Dit gebeurt normaal gesproken bij het starten van de service,
            // maar blijkbaar is het moeilijk de service te herstarten bij het testen.
            // Vandaar op deze manier:

            MappingHelper.MappingsDefinieren();
            Factory.ContainerInit();
        }

        [ClassCleanup]
        static public void TestsAfsluiten()
        {
        }

        [TestInitialize]
        public void SetUp()
        {
        }

        /// <summary>
        /// Verwijder eventuele overblijvende categorieën
        /// </summary>
        [TestCleanup]
        public void TearDown()
        {
        }

        #endregion

        /// <summary>
        /// Verwijderen van een lege categorie
        /// </summary>
        [TestMethod]
        public void CategorieVerwijderenNormaal()
        {
            // ARRANGE

            #region testdata

            var groep = new ChiroGroep();

            var categorie = new Categorie
            {
                ID = 1,
                Groep = groep
            };
            groep.Categorie.Add(categorie);

            #endregion

            #region dependency injection

            var categorieenRepo = new DummyRepo<Categorie>(new List<Categorie> { categorie });

            var repositoryProviderMock = new Mock<IRepositoryProvider>();
            repositoryProviderMock.Setup(src => src.RepositoryGet<Categorie>())
                                  .Returns(categorieenRepo);

            Factory.InstantieRegistreren(repositoryProviderMock.Object);
            #endregion

            // ACT

            var groepenService = Factory.Maak<GroepenService>();
            // Verwijder categorie zonder te forceren
            groepenService.CategorieVerwijderen(categorie.ID, false);

            // ASSERT

            Assert.IsNull(categorieenRepo.ByID(categorie.ID));  // categorie weg
        }

        /// <summary>
        /// Probeert een categorie te verwijderen waaraan een persoon gekoppeld is.  
        /// Er wordt een exception verwacht.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(FaultException<BlokkerendeObjectenFault<PersoonDetail>>))]
        public void CategorieVerwijderenMetPersoon()
        {
            // ARRANGE

            #region testdata
            var categorie = new Categorie
                                {
                                    ID = 1,
                                    GelieerdePersoon = new List<GelieerdePersoon> { new GelieerdePersoon { Persoon = new Persoon(), Groep = new ChiroGroep() } }
                                };
            #endregion

            #region dependency injection
            var repositoryProviderMock = new Mock<IRepositoryProvider>();
            repositoryProviderMock.Setup(src => src.RepositoryGet<Categorie>())
                                  .Returns(new DummyRepo<Categorie>(new List<Categorie> {categorie}));

            Factory.InstantieRegistreren(repositoryProviderMock.Object);
            #endregion

            // ACT

            var groepenService = Factory.Maak<GroepenService>();
            // Verwijder categorie zonder te forceren
            groepenService.CategorieVerwijderen(categorie.ID, false);

            // ASSERT

            // Als we hier geraken, liep er iets mis.
            Assert.IsTrue(false);

        }

        /// <summary>
        /// Geforceerd een categorie met personen verwijderen
        /// </summary>
        [TestMethod]
        public void CategorieVerwijderenMetPersoonForceer()
        {
            // ARRANGE

            #region testdata

            var groep = new ChiroGroep();

            var categorie = new Categorie
            {
                ID = 1,
                Groep = groep
            };
            groep.Categorie.Add(categorie);

            var gelieerdePersoon = new GelieerdePersoon {Groep = groep};
            groep.GelieerdePersoon.Add(gelieerdePersoon);
            categorie.GelieerdePersoon.Add(gelieerdePersoon);
            gelieerdePersoon.Categorie.Add(categorie);

            #endregion

            #region dependency injection

            var categorieenRepo = new DummyRepo<Categorie>(new List<Categorie> {categorie});

            var repositoryProviderMock = new Mock<IRepositoryProvider>();
            repositoryProviderMock.Setup(src => src.RepositoryGet<Categorie>())
                                  .Returns(categorieenRepo);

            Factory.InstantieRegistreren(repositoryProviderMock.Object);
            #endregion

            // ACT

            var groepenService = Factory.Maak<GroepenService>();
            // Verwijder categorie door te forceren
            groepenService.CategorieVerwijderen(categorie.ID, true);

            // ASSERT

            Assert.IsNull(categorieenRepo.ByID(categorie.ID));  // categorie weg
        }
    }
}
