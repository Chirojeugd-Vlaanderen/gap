using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using WebServices.Test.PersonenServiceReference;
using Cg2.Core.Domain;
using Cg2.Validatie;

namespace WebServices.Test
{
    /// <summary>
    /// Summary description for PersonenServiceTest
    /// </summary>
    [TestClass]
    public class PersonenServiceTest
    {
        public PersonenServiceTest()
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
            using (PersonenServiceReference.PersonenServiceClient service = new WebServices.Test.PersonenServiceReference.PersonenServiceClient())
            {
                string antwoord = service.Hallo();
                Assert.IsTrue(antwoord.Length > 0);
            }
        }

        [TestMethod]
        public void PersoonOphalen()
        {
            using (PersonenServiceReference.PersonenServiceClient service = new WebServices.Test.PersonenServiceReference.PersonenServiceClient())
            {
                Persoon resultaat = service.Ophalen(1893);
                PersonenValidator validator = new PersonenValidator();

                Assert.IsTrue(validator.Valideer(resultaat));
                Assert.IsTrue(resultaat.Communicatie.Count() == 0);
            }
        }

        [TestMethod]
        public void PersoonOphalenMetCommunicatie()
        {
            using (PersonenServiceReference.PersonenServiceClient service = new WebServices.Test.PersonenServiceReference.PersonenServiceClient())
            {
                Persoon resultaat = service.OphalenMetCommunicatie(1893);
                PersonenValidator validator = new PersonenValidator();

                Assert.IsTrue(validator.Valideer(resultaat));
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
            using (PersonenServiceReference.PersonenServiceClient service = new WebServices.Test.PersonenServiceReference.PersonenServiceClient())
            {
                Persoon p = service.OphalenMetCommunicatie(1893);
                int aantal = p.Communicatie.Count();

                p.CommunicatieToevoegen(CommunicatieType.Telefoon, "1207", false);

                Assert.IsTrue(p.Communicatie.Count() == aantal+1);
            }
        }



        /// <summary>
        /// Test op 'object identity' van 'detached' entity's
        /// </summary>
        [TestMethod]
        public void ObjectIdentity()
        {
            using (PersonenServiceReference.PersonenServiceClient service = new WebServices.Test.PersonenServiceReference.PersonenServiceClient())
            {
                Persoon p = new Persoon { Naam = "Fluwijn", VoorNaam = "Piet", Geslacht = GeslachtsType.Man };

                int newId = service.Bewaren(p);

                Persoon q = service.Ophalen(newId);

                Assert.IsTrue(p.BusinessKey == q.BusinessKey);
                Assert.IsTrue(p.Equals(q));

                service.Verwijderen(q);
            }
        }

    }
}
