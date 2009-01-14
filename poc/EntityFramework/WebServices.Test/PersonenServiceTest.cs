using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using WebServices.Test.PersonenServiceReference;

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

                Assert.IsTrue(resultaat.Naam.Length > 0);
                Assert.IsTrue(resultaat.Communicatie.Count() == 0);
            }
        }

        [TestMethod]
        public void PersoonOphalenMetCommunicatie()
        {
            using (PersonenServiceReference.PersonenServiceClient service = new WebServices.Test.PersonenServiceReference.PersonenServiceClient())
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
            using (PersonenServiceReference.PersonenServiceClient service = new WebServices.Test.PersonenServiceReference.PersonenServiceClient())
            {
                Persoon p = service.OphalenMetCommunicatie(1893);
                int aantal = p.Communicatie.Count();
                CommunicatieVorm v = new CommunicatieVorm();

                v.Type = CommunicatieType.Telefoon;
                v.Nummer = "1207";
                v.Voorkeur = false;
                v.IsGezinsGebonden = false;

                p.Communicatie.Add(v);

                Assert.IsTrue(p.Communicatie.Count() == aantal+1);
            }
        }

    }
}
