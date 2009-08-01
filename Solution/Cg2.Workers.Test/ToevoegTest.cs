using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Cg2.Data.Ef;
using Cg2.Orm;
using Cg2.Ioc;

namespace Cg2.Workers.Test
{
    /// <summary>
    /// Summary description for UnitTest1
    /// </summary>
    [TestClass]
    public class ToevoegTest
    {
        public ToevoegTest()
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
        public void TestMethod1()
        {
            GroepenDao gdao = new GroepenDao();
            Groep g = gdao.Ophalen(310);
            
            Persoon p = Persoon.CreatePersoon("Broes", 0, 0);
            //Persoon p = new Persoon();
            //p.VoorNaam = "Broes";
            //p.Naam = "De Cat";

            /*PersonenDao pdao = new PersonenDao();
            pdao.Bewaren(p);*/

            GelieerdePersoon gp = GelieerdePersoon.CreateGelieerdePersoon(10, 0);

            gp.Persoon = p;
            p.GelieerdePersoon.Add(gp);
            gp.Groep = g;

            GelieerdePersonenDao gpdao = new GelieerdePersonenDao();
            int number = gpdao.AllenOphalen(310).Count;
            
            gpdao.Bewaren(gp);

            int number2 = gpdao.AllenOphalen(310).Count;

            Assert.AreEqual(number + 1, number2);

            /*
             * todo: 
             * kan ik groep ophalen zonder al zijn gelieerde personen, maar toch aan de collectie toevoegen?
             * verwijderen van persoon nog testen
             * vastleggen welke velden van persoon not null zijn en welke uniek moeten zijn (alleen AD geloof ik)
             * creeren als methode overal verwijderen?
             * checken of overal in bewaren nieuwe objecten juist worden afgehandeld (nog niet zo voor gelieerdepersoon
             *                   namelijk niet als de persoon als bestaat maar de gelieerdepersoon nieuw is)
             */


            //gp.TeVerwijderen = true;
            //gpdao.Bewaren(gp);
        }
    }
}
