/*
 * Copyright 2008-2013 the GAP developers. See the NOTICE file at the 
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

using System.Collections.Generic;
using System.ServiceModel;
using Chiro.Cdf.Poco;
using Chiro.Gap.Dummies;
using Chiro.Gap.Poco.Model;
using Chiro.Gap.ServiceContracts.DataContracts;
using Chiro.Gap.ServiceContracts.FaultContracts;
using Chiro.Gap.Test;
using NUnit.Framework;
using Moq;

namespace Chiro.Gap.Services.Test
{
    /// <summary>
    /// Summary description for CategorieToevoegen
    /// </summary>
    [TestFixture]
    public class CategorieVerwijderen: ChiroTest
    {
        #region initialisatie en afronding

        [SetUp]
        public void SetUp()
        {
            // Dit gebeurt normaal gesproken bij het starten van de service,
            // maar blijkbaar is het moeilijk de service te herstarten bij het testen.
            // Vandaar op deze manier:
            PermissionHelper.FixPermissions();
        }

        #endregion

        /// <summary>
        /// Verwijderen van een lege categorie
        /// </summary>
        [Test]
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
        [Test]
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

            // ASSERT

            var groepenService = Factory.Maak<GroepenService>();
            // Verwijder categorie zonder te forceren
            Assert.Throws<FaultException<BlokkerendeObjectenFault<PersoonDetail>>>(
                () => groepenService.CategorieVerwijderen(categorie.ID, false));
        }

        /// <summary>
        /// Geforceerd een categorie met personen verwijderen
        /// </summary>
        [Test]
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
