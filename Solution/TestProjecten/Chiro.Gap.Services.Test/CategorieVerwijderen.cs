using Chiro.Gap.ServiceContracts.DataContracts;

using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Security.Principal;
using System.ServiceModel;
using System.Text;
using System.Threading;

using Chiro.Cdf.Ioc;
using Chiro.Gap.Orm;
using Chiro.Gap.Orm.DataInterfaces;
using Chiro.Gap.ServiceContracts;
using Chiro.Gap.ServiceContracts.FaultContracts;
using Chiro.Gap.ServiceContracts.Mappers;
using Chiro.Gap.Services;
using Chiro.Gap.TestDbInfo;

namespace Chiro.Gap.Services.Test
{
    using System.Transactions;

    using Chiro.Gap.Domain;

    /// <summary>
    /// Summary description for CategorieToevoegen
    /// </summary>
    [TestClass]
    public class CategorieVerwijderen
    {
        private IGroepenService _groepenSvc;
        private IGelieerdePersonenService _personenSvc;

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
            // Zorg ervoor dat de PrincipalPermissionAttributes op de service methods
            // geen excepties genereren, door te doen alsof de service aangeroepen is met de goede

            var identity = new GenericIdentity(Properties.Settings.Default.TestUser);
            var roles = new[] { Properties.Settings.Default.TestSecurityGroep };
            var principal = new GenericPrincipal(identity, roles);
            Thread.CurrentPrincipal = principal;

            _groepenSvc = Factory.Maak<GroepenService>(); // Geen interface hier, want dat werkt hij niet
            _personenSvc = Factory.Maak<GelieerdePersonenService>(); // Geen interface hier, want dat werkt hij niet

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
            using (new TransactionScope())
            {
                var catID = _groepenSvc.CategorieToevoegen(TestInfo.GROEP_ID, "CategorieVerwijderenNormaal", "TempCat3");

                // Act: verwijder de categorie met gegeven ID, en probeer categorie opnieuw op te halen

                _groepenSvc.CategorieVerwijderen(catID, false);

                var catIDTest = _groepenSvc.CategorieIDOphalen(
                    TestInfo.GROEP_ID,
                    "TempCat3");

                // Assert: categorie niet meer gevonden.
                Assert.IsTrue(catIDTest == 0);

            }

        }

        /// <summary>
        /// Probeert een categorie te verwijderen waaraan een persoon gekoppeld is.  
        /// Er wordt een exception verwacht.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(FaultException<BlokkerendeObjectenFault<PersoonDetail>>))]
        public void CategorieVerwijderenMetPersoon()
        {
            // Arrange: categorie opzoeken, en een nieuwe persoon toevoegen.

            using (new TransactionScope())
            {
                // Maak een nieuwe persoon
                var gp =
                    _personenSvc.AanmakenForceer(
                        new PersoonInfo
                            {
                                AdNummer = null,
                                ChiroLeefTijd = 0,
                                GeboorteDatum = new DateTime(2003, 5, 8),
                                Geslacht = GeslachtsType.Vrouw,
                                Naam = "CategorieVerwijderenMetPersoon",
                                VoorNaam = "Anneke",
                            },
                        groepID: TestInfo.GROEP_ID,
                        forceer: true);

                var persoonID = gp.PersoonID;
                var gelPersoonID = gp.GelieerdePersoonID;

                var catID = _groepenSvc.CategorieToevoegen(TestInfo.GROEP_ID, "CategorieVerwijderenMetPersoon", "TempCat");

                _personenSvc.CategorieKoppelen(
                    new List<int> { gelPersoonID },
                    new List<int> { catID });

                // Act

                // Verwijder categorie zonder te forceren
                _groepenSvc.CategorieVerwijderen(catID, false);

                // Assert

                // Als we hier geraken, liep er iets mis.
                Assert.IsTrue(false);
            }

        }

        /// <summary>
        /// Geforceerd een categorie met personen verwijderen
        /// </summary>
        [TestMethod]
        public void CategorieVerwijderenMetPersoonForceer()
        {

            using (new TransactionScope())
            {
                // Arrange: categorie maken, en persoon toevoegen.
                var catIDDieVerwijderdZalWorden = _groepenSvc.CategorieToevoegen(TestInfo.GROEP_ID, "CategorieVerwijderenMetPersoonForceer", "TempCat2");

                // Maak een nieuwe persoon
                var gp =
                    _personenSvc.AanmakenForceer(
                        new PersoonInfo
                        {
                            AdNummer = null,
                            ChiroLeefTijd = 0,
                            GeboorteDatum = new DateTime(2005, 5, 8),
                            Geslacht = GeslachtsType.Man,
                            Naam = "CategorieVerwijderenMetPersoonForceer",
                            VoorNaam = "Pierre",
                        },
                        groepID: TestInfo.GROEP_ID,
                        forceer: true);


                _personenSvc.CategorieKoppelen(
                    new List<int> { gp.GelieerdePersoonID },
                    new List<int> { catIDDieVerwijderdZalWorden });

                // Act
                _groepenSvc.CategorieVerwijderen(catIDDieVerwijderdZalWorden, true);

                // Assert

                // Probeer categorie terug op te halen.  Dat moet failen.
                var newCatID = _groepenSvc.CategorieIDOphalen(
                    TestInfo.GROEP_ID,
                    "TempCat2");
                Assert.IsTrue(newCatID == 0);

                // Controleer ook of de gelieerde persoon niet per ongeluk mee is verwijderd
                var persoon = _personenSvc.DetailOphalen(gp.GelieerdePersoonID);
                Assert.IsNotNull(persoon);
            }

        }
    }
}
