using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using WebServices.Test.GroepenServiceReference;
using Cg2.Orm;

namespace WebServices.Test
{
    /// <summary>
    /// Summary description for GroepenServiceTest
    /// </summary>
    [TestClass]
    public class GroepenServiceTest
    {
        public GroepenServiceTest()
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
        public void ServiceAanroepen()
        {
            using (GroepenServiceReference.GroepenServiceClient service = new GroepenServiceReference.GroepenServiceClient())
            {
                string antwoord = service.Hallo();

                Assert.IsTrue(antwoord.Length > 0);
            }
        }

        [TestMethod]
        public void GroepOphalen()
        {
            using (GroepenServiceReference.GroepenServiceClient service = new GroepenServiceReference.GroepenServiceClient())
            {
                Groep antwoord = service.Ophalen(310);

                Assert.IsTrue(antwoord.Naam.Length > 0);
            }
        }

        [TestMethod]
        public void GroepUpdatenMetOrigineel()
        {
            using (GroepenServiceReference.GroepenServiceClient service = new GroepenServiceReference.GroepenServiceClient())
            {
                Groep orig = service.Ophalen(310);
                Groep g = service.Ophalen(310);

                string oudeNaam = g.Naam;

                if (oudeNaam == "Valentijn")
                {
                    g.Naam = "Viersel";
                }
                else
                {
                    g.Naam = "Valentijn";
                }

                // Updaten MET origineel
                service.Updaten(g, orig);

                // Opnieuw opghalen

                Groep h = service.Ophalen(310);

                Assert.IsTrue(oudeNaam != h.Naam);
            }
        }

        [TestMethod]
        public void GroepUpdaten()
        {
            using (GroepenServiceReference.GroepenServiceClient service = new GroepenServiceReference.GroepenServiceClient())
            {
                Groep g = service.Ophalen(310);

                string oudeNaam = g.Naam;

                if (oudeNaam == "Valentijn")
                {
                    g.Naam = "Viersel";
                }
                else
                {
                    g.Naam = "Valentijn";
                }

                // Updaten ZONDER origineel
                service.Updaten(g, null);

                // Opnieuw opghalen

                Groep h = service.Ophalen(310);

                Assert.IsTrue(oudeNaam != h.Naam);
            }
        }

        [TestMethod]
        public void GroepConcurrencyMetOrigineel()
        {
            bool exceptieGevangen = false;

            using (GroepenServiceClient service = new GroepenServiceClient())
            {
                Groep orig = service.Ophalen(310);
                Groep g = service.Ophalen(310);
                Groep h = service.Ophalen(310);

                if (g.Naam == "Valentijn")
                {
                    g.Naam = "Viersel";
                    h.Naam = "Viersel2";
                }
                else
                {
                    g.Naam = "Valentijn";
                    h.Naam = "Valentijn2";
                }

                // updates MET origineel

                service.Updaten(g, orig);

                try
                {
                    service.Updaten(h, orig);
                }
                catch (System.ServiceModel.FaultException)
                {
                    // TODO: fatsoenlijke exceptie vangen
                    exceptieGevangen = true;
                }


                Assert.IsTrue(exceptieGevangen);

            }
        }

        [TestMethod]
        public void GroepConcurrency()
        {
            bool exceptieGevangen = false;

            using (GroepenServiceClient service = new GroepenServiceClient())
            {
                Groep g = service.Ophalen(310);
                Groep h = service.Ophalen(310);

                if (g.Naam == "Valentijn")
                {
                    g.Naam = "Viersel";
                    h.Naam = "Viersel2";
                }
                else
                {
                    g.Naam = "Valentijn";
                    h.Naam = "Valentijn2";
                }

                // updates ZONDER origineel

                service.Updaten(g, null);

                try
                {
                    service.Updaten(h, null);
                }
                catch (System.ServiceModel.FaultException)
                {
                    // TODO: fatsoenlijke exceptie vangen
                    exceptieGevangen = true;
                }


                Assert.IsTrue(exceptieGevangen);

            }
        }

    }
}
