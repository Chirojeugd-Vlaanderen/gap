using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Cg2.Data.Ef;
using Cg2.Orm;

namespace Cg2.Dao.Test
{
    /// <summary>
    /// Summary description for PersonenTest
    /// </summary>
    [TestClass]
    public class PersonenTest
    {
        public PersonenTest()
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
        public void ZoekenOpNaam()
        {
            // arrange
            string zoekString = Properties.Settings.Default.TestZoekNaam;
            int groepID = Properties.Settings.Default.TestGroepID;

            GelieerdePersonenDao dao = new GelieerdePersonenDao();

            // act
            IList<GelieerdePersoon> lijst = dao.ZoekenOpNaam(groepID, zoekString);

            // assert
            Assert.IsTrue(lijst.Count >= 2);
        }

        /// <summary>
        /// Broes' test om te verifieren of de GelieerdePersonenDao 
        /// een nieuwe persoon kan toevoegen.
        /// </summary>
        [TestMethod]
        public void NieuwePersoon()
        {
            GroepenDao gdao = new GroepenDao();
            Groep g = gdao.Ophalen(310);

            //// Onderstaande assert is niet relevant voor de test.
            // Assert.AreEqual(0, g.GelieerdePersoon.Count);

            Persoon p = new Persoon();
            p.VoorNaam = "Broes";
            p.Naam = "De Cat";

            //// Ik weet niet precies waarom Broes de GP als volgt aanmaakte:
            //
            // GelieerdePersoon gp = GelieerdePersoon.CreateGelieerdePersoon(0, 0);
            //
            //// Dit is alleszins properder:

            GelieerdePersoon gp = new GelieerdePersoon();

            // Onderstaande operaties zouden beter gebeuren in de personenmanager.

            gp.Persoon = p;
            p.GelieerdePersoon.Add(gp);
            gp.Groep = g;

            //// Deze assert is ook niet relevant
            // Assert.AreEqual(1, g.GelieerdePersoon.Count);

            // Er gebeurt dus GEEN loading van de andere gelieerde personen, maar blijkbaar kan het wel
            // zonder probleem worden toegevoegd
            // Johan: Zo is dat.  AttachObjectGraph kijkt alleen naar de beschikbare objecten.
            // Vandaar dat de 'TeVerwijderen'-vlag nodig is.  Alles wat bij het opnieuw attachen niet
            // beschikbaar is, wordt beschouwd als niet geladen.

            GelieerdePersonenDao gpdao = new GelieerdePersonenDao();

            // TODO: Dao voorzien van functionaliteit om het aantal personen te tellen.
            // Onderstaande is nogal 'duur':

            int number = gpdao.AllenOphalen(310).Count;

            gp = gpdao.Bewaren(gp);

            int number2 = gpdao.AllenOphalen(310).Count;

            Assert.AreEqual(number + 1, number2);
            Assert.AreNotEqual(number, 0);
            Assert.AreNotEqual(0, gp.ID);

            // Bij wijze van cleanup gelieerde persoon opnieuw verwijderen.

            /*
             * todo: 
             * vastleggen welke velden van persoon not null zijn en welke uniek moeten zijn (alleen AD geloof ik)
             * creeren als methode overal verwijderen?
             * checken of overal in bewaren nieuwe objecten juist worden afgehandeld (nog niet zo voor gelieerdepersoon
             *                   namelijk niet als de persoon als bestaat maar de gelieerdepersoon nieuw is)
             */

            gp.TeVerwijderen = true;
            gp.Persoon.TeVerwijderen = true;
            gpdao.Bewaren(gp);

            int number3 = gpdao.AllenOphalen(310).Count;

            // Eigenlijk testen we het onderstaande niet, maar in dit geval
            // komt het wel van pas.

            Assert.AreEqual(number, number3);
        }
    }
}
