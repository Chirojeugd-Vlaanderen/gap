using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Cg2.Orm;
using Cg2.Workers;
using Cg2.Ioc;
using Cg2.Orm.DataInterfaces;

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

        [ClassInitialize]
        static public void TestInitialiseren(TestContext context)
        {
            Factory.InitContainer();

            // Maak persoon aan, die bij het testen weer
            // verwijderd kan worden.

            int groepID = Properties.Settings.Default.TestGroepID;
            string naam = Properties.Settings.Default.TestNieuwePersoonNaam;
            string voornaam = Properties.Settings.Default.TestTeVerwijderenVoornaam;

            IGroepenDao gdao = Factory.Maak<IGroepenDao>();
            Groep g = gdao.Ophalen(groepID);

            GelieerdePersonenManager gpm = Factory.Maak<GelieerdePersonenManager>();

            // Creeer gloednieuwe persoon

            Persoon p = new Persoon();
            p.VoorNaam = voornaam;
            p.Naam = naam;

            // Koppel aan testgroep, Chiroleeftijd 0

            GelieerdePersoon gp = gpm.PersoonAanGroepKoppelen(p, g, 0);

            IGelieerdePersonenDao gpdao = Factory.Maak<IGelieerdePersonenDao>();
            // Hier moeten we via de DAO gaan, en niet via de worker, omdat 
            // we de DAO willen testen, en niet willen failen op fouten in
            // de worker.

            gp = gpdao.Bewaren(gp);
        }

        /// <summary>
        /// Opkuis na de test; verwijdert bijgemaakte personen
        /// </summary>
        /// <param name="context"></param>
        [ClassCleanup]
        static public void Opkuis()
        {
            int groepID = Properties.Settings.Default.TestGroepID;
            string naam = Properties.Settings.Default.TestNieuwePersoonNaam;
            string voornaam = Properties.Settings.Default.TestNieuwePersoonVoornaam;

            GelieerdePersonenManager mgr = Factory.Maak<GelieerdePersonenManager>();

            // nog niet alle functionaliteit wordt aangeboden in de worker,
            // dus ik werk hier en daar rechtstreeks op de dao.

            IGelieerdePersonenDao dao = Factory.Maak<IGelieerdePersonenDao>();

            IList<GelieerdePersoon> gevonden = dao.ZoekenOpNaam(groepID, naam + ' ' + voornaam);

            foreach (GelieerdePersoon gp in gevonden)
            {
                // Markeer geliererde persoon en alle aanhangsels als
                // 'te verwijderen' 
                mgr.VolledigVerwijderen(gp);

                // persisteer
                dao.Bewaren(gp, l=>l.Persoon, l=>l.Persoon.PersoonsAdres, l=>l.Communicatie);
            }
        }

        [TestMethod]
        public void ZoekenOpNaam()
        {
            // arrange
            string zoekString = Properties.Settings.Default.TestZoekNaam;
            int groepID = Properties.Settings.Default.TestGroepID;

            IGelieerdePersonenDao dao = Factory.Maak<IGelieerdePersonenDao>();

            // act
            IList<GelieerdePersoon> lijst = dao.ZoekenOpNaam(groepID, zoekString);

            // assert
            Assert.IsTrue(lijst.Count >= 2);
        }

        /// <summary>
        /// Nieuwe (gelieerde) persoon bewaren via GelieerdePersonenDAO.
        /// </summary>
        [TestMethod]
        public void NieuwePersoon()
        {
            #region Arrange

            int groepID = Properties.Settings.Default.TestGroepID;
            string naam = Properties.Settings.Default.TestNieuwePersoonNaam;
            string voornaam = Properties.Settings.Default.TestNieuwePersoonVoornaam;

            IGroepenDao gdao = Factory.Maak<IGroepenDao>();
            Groep g = gdao.Ophalen(groepID);

            GelieerdePersonenManager gpm = Factory.Maak<GelieerdePersonenManager>();
            
            // Creeer gloednieuwe persoon

            Persoon p = new Persoon();
            p.VoorNaam = voornaam;
            p.Naam = naam;

            // Koppel aan testgroep, Chiroleeftijd 0

            GelieerdePersoon gp = gpm.PersoonAanGroepKoppelen(p, g, 0);

            IGelieerdePersonenDao gpdao = Factory.Maak<IGelieerdePersonenDao>();
            #endregion

            #region Act
            // Hier moeten we via de DAO gaan, en niet via de worker, omdat 
            // we de DAO willen testen, en niet willen failen op fouten in
            // de worker.

            gp = gpdao.Bewaren(gp);
            #endregion

            #region Assert
            IList<GelieerdePersoon> gevonden = gpdao.ZoekenOpNaam(groepID, naam + ' ' + voornaam);
            Assert.IsTrue(gevonden.Count > 0);
            #endregion

        }


        /// <summary>
        /// Test om persoon te verwijderen.
        /// </summary>
        [TestMethod]
        public void VerwijderPersoon()
        {
            #region Arrange
            // zoek te verwijderen personen op

            int groepID = Properties.Settings.Default.TestGroepID;
            string naam = Properties.Settings.Default.TestNieuwePersoonNaam;
            string voornaam = Properties.Settings.Default.TestTeVerwijderenVoornaam;

            GelieerdePersonenManager mgr = Factory.Maak<GelieerdePersonenManager>();

            // nog niet alle functionaliteit wordt aangeboden in de worker,
            // dus ik werk hier en daar rechtstreeks op de dao.

            IGelieerdePersonenDao dao = Factory.Maak<IGelieerdePersonenDao>();

            IList<GelieerdePersoon> gevonden = dao.ZoekenOpNaam(groepID, naam + ' ' + voornaam);
            #endregion

            #region Act
            foreach (GelieerdePersoon gp in gevonden)
            {
                // Markeer geliererde persoon en alle aanhangsels als
                // 'te verwijderen' 
                mgr.VolledigVerwijderen(gp);

                // persisteer
                dao.Bewaren(gp, l => l.Persoon, l => l.Persoon.PersoonsAdres, l => l.Communicatie);
            }
            #endregion

            #region Assert
            IList<GelieerdePersoon> gevonden2 = dao.ZoekenOpNaam(groepID, naam + ' ' + voornaam);
            Assert.IsTrue(gevonden2.Count == 0);
            #endregion

        }

    }
}
