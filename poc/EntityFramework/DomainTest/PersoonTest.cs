using Cg2.Core.Domain;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Data.Objects.DataClasses;

namespace DomainTest
{
    
    
    /// <summary>
    ///This is a test class for PersoonTest and is intended
    ///to contain all PersoonTest Unit Tests
    ///</summary>
    [TestClass()]
    public class PersoonTest
    {


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
        //You can use the following additional attributes as you write your tests:
        //
        //Use ClassInitialize to run code before running the first test in the class
        //[ClassInitialize()]
        //public static void MyClassInitialize(TestContext testContext)
        //{
        //}
        //
        //Use ClassCleanup to run code after all tests in a class have run
        //[ClassCleanup()]
        //public static void MyClassCleanup()
        //{
        //}
        //
        //Use TestInitialize to run code before running each test
        //[TestInitialize()]
        //public void MyTestInitialize()
        //{
        //}
        //
        //Use TestCleanup to run code after each test has run
        //[TestCleanup()]
        //public void MyTestCleanup()
        //{
        //}
        //
        #endregion


        /// <summary>
        ///A test for VoorNaam
        ///</summary>
        [TestMethod()]
        public void VoorNaamTest()
        {
            Persoon target = new Persoon(); // TODO: Initialize to an appropriate value
            string expected = string.Empty; // TODO: Initialize to an appropriate value
            string actual;
            target.VoorNaam = expected;
            actual = target.VoorNaam;
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for SterfDatum
        ///</summary>
        [TestMethod()]
        public void SterfDatumTest()
        {
            Persoon target = new Persoon(); // TODO: Initialize to an appropriate value
            Nullable<DateTime> expected = new Nullable<DateTime>(); // TODO: Initialize to an appropriate value
            Nullable<DateTime> actual;
            target.SterfDatum = expected;
            actual = target.SterfDatum;
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for Naam
        ///</summary>
        [TestMethod()]
        public void NaamTest()
        {
            Persoon target = new Persoon(); // TODO: Initialize to an appropriate value
            string expected = string.Empty; // TODO: Initialize to an appropriate value
            string actual;
            target.Naam = expected;
            actual = target.Naam;
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for ID
        ///</summary>
        [TestMethod()]
        public void IDTest()
        {
            Persoon target = new Persoon(); // TODO: Initialize to an appropriate value
            int expected = 0; // TODO: Initialize to an appropriate value
            int actual;
            target.ID = expected;
            actual = target.ID;
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for GeslachtsInt
        ///</summary>
        [TestMethod()]
        public void GeslachtsIntTest()
        {
            Persoon target = new Persoon(); // TODO: Initialize to an appropriate value
            int expected = 0; // TODO: Initialize to an appropriate value
            int actual;
            target.GeslachtsInt = expected;
            actual = target.GeslachtsInt;
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for Geslacht
        ///</summary>
        [TestMethod()]
        public void GeslachtTest()
        {
            Persoon target = new Persoon(); // TODO: Initialize to an appropriate value
            GeslachtsType expected = new GeslachtsType(); // TODO: Initialize to an appropriate value
            GeslachtsType actual;
            target.Geslacht = expected;
            actual = target.Geslacht;
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for GeboorteDatum
        ///</summary>
        [TestMethod()]
        public void GeboorteDatumTest()
        {
            Persoon target = new Persoon(); // TODO: Initialize to an appropriate value
            Nullable<DateTime> expected = new Nullable<DateTime>(); // TODO: Initialize to an appropriate value
            Nullable<DateTime> actual;
            target.GeboorteDatum = expected;
            actual = target.GeboorteDatum;
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for Communicatie
        ///</summary>
        [TestMethod()]
        public void CommunicatieTest()
        {
            Persoon target = new Persoon(); // TODO: Initialize to an appropriate value
            EntityCollection<CommunicatieVorm> actual;
            actual = target.Communicatie;
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for AdNummer
        ///</summary>
        [TestMethod()]
        public void AdNummerTest()
        {
            Persoon target = new Persoon(); // TODO: Initialize to an appropriate value
            Nullable<int> expected = new Nullable<int>(); // TODO: Initialize to an appropriate value
            Nullable<int> actual;
            target.AdNummer = expected;
            actual = target.AdNummer;
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for CommunicatieToevoegen
        ///</summary>
        [TestMethod()]
        public void CommunicatieToevoegenTest()
        {
            Persoon target = new Persoon(); // TODO: Initialize to an appropriate value
            CommunicatieType type = new CommunicatieType(); // TODO: Initialize to an appropriate value
            string nr = string.Empty; // TODO: Initialize to an appropriate value
            bool voorkeur = false; // TODO: Initialize to an appropriate value
            target.CommunicatieToevoegen(type, nr, voorkeur);
            Assert.Inconclusive("A method that does not return a value cannot be verified.");
        }

        /// <summary>
        ///A test for Persoon Constructor
        ///</summary>
        [TestMethod()]
        public void PersoonConstructorTest()
        {
            Persoon target = new Persoon();
            Assert.Inconclusive("TODO: Implement code to verify target");
        }
    }
}
