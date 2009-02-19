using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Cg2.Validatie;
using Cg2.Orm;

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

        /// <summary>
        /// Test of een persoon en een groep wel verschillend zijn
        /// </summary>
        [TestMethod]
        public void ObjectIdentity2()
        {
            Persoon p;
            Groep g;

            using (PersonenServiceReference.PersonenServiceClient ps = new WebServices.Test.PersonenServiceReference.PersonenServiceClient())
            {
                p = ps.Ophalen(307);
            }

            using (GroepenServiceReference.GroepenServiceClient gs = new WebServices.Test.GroepenServiceReference.GroepenServiceClient())
            {
                g = gs.Ophalen(307);
            }

            Assert.IsTrue(!p.Equals(g));
        }

    }
}
