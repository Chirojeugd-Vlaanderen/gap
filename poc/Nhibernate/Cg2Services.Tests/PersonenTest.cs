using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Cg2.Core.Domain;
using Cg2.Validatie;
using System.ServiceModel;

namespace Cg2Services.Tests
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
        public void ServiceAanroepen()
        {
            using (PersonenServiceReference.PersonenServiceClient service = new PersonenServiceReference.PersonenServiceClient())
            {
                string antwoord = service.Hallo();
                Assert.IsTrue(antwoord.Length > 0);
            }
        }

        [TestMethod]
        public void PersoonOphalen()
        {
            using (PersonenServiceReference.PersonenServiceClient service = new PersonenServiceReference.PersonenServiceClient())
            {
                Persoon resultaat = service.Ophalen(1893);
                IValidator<Persoon> validator = new PersonenValidator();

                Assert.IsTrue(validator.Valideer(resultaat));
                Assert.IsTrue(resultaat.Communicatie.Count() == 0);
            }
        }

        [TestMethod]
        public void PersoonBewaren()
        {
            using (PersonenServiceReference.PersonenServiceClient service = new PersonenServiceReference.PersonenServiceClient())
            {
                Persoon nieuw = new Persoon { Naam = "Pienter", VoorNaam = "Piet", Geslacht = GeslachtsType.Man };

                int toegekendId = service.Bewaren(nieuw);

                Persoon opgehaald = service.Ophalen(toegekendId);

                // Vergelijken zou moeten werken zoals verwacht, dankzij 
                // aangepaste Equals in BasisEntiteit, die gebruik maakt
                // van een GUID.

                Assert.AreEqual(nieuw, opgehaald);
            }
        }

        [TestMethod]
        [ExpectedException(typeof(FaultException))]
        public void VermijdDubbeleInsert()
        {
            using (PersonenServiceReference.PersonenServiceClient service = new PersonenServiceReference.PersonenServiceClient())
            {
                Persoon nieuw = new Persoon { Naam = "Pienter", VoorNaam = "Piet", Geslacht = GeslachtsType.Man };

                int id1 = service.Bewaren(nieuw);
                int id2 = service.Bewaren(nieuw);

                Assert.IsFalse(id2 > id1);
            }
        }


        [TestMethod]
        public void PersoonOphalenMetCommunicatie()
        {
            using (PersonenServiceReference.PersonenServiceClient service = new PersonenServiceReference.PersonenServiceClient())
            {
                Persoon resultaat = service.OphalenMetCommunicatie(1893);

                Assert.IsTrue(resultaat.Naam.Length > 0);
                Assert.IsTrue(resultaat.Communicatie.Count() > 0);
            }
        }

        /// <summary>
        /// Deze test kijkt na of er extra communicatievormen toegevoegd
        /// kunnen worden aan een persoon.  (Dit zou  niet werken als
        /// collections als array worden doorgegeven ipv als lists.)
        /// </summary>
        [TestMethod]
        public void CommunicatieManipuleren()
        {
            using (PersonenServiceReference.PersonenServiceClient service = new PersonenServiceReference.PersonenServiceClient())
            {
                Persoon p = service.OphalenMetCommunicatie(1893);
                int aantal = p.Communicatie.Count();

                // Om een communicatievorm toe te voegen, moeten zowel de 
                // link persoon->communicatievorm als de link
                // communicatievorm->persoon ok zijn.  Vandaar dat we hiervoor
                // p.CommunicatieToevoegen gebruiken.

                p.CommunicatieToevoegen(CommunicatieType.Telefoon, "1207", false);

                Assert.IsTrue(p.Communicatie.Count() == aantal + 1);

                foreach (CommunicatieVorm cv in p.Communicatie)
                {
                    Assert.IsTrue(cv.PersoonID == p.ID);
                }
            }
        }
    }
}
