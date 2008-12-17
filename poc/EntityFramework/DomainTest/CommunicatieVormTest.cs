using Cg2.Core.Domain;
using Microsoft.VisualStudio.TestTools.UnitTesting;
namespace DomainTest
{
    
    
    /// <summary>
    ///This is a test class for CommunicatieVormTest and is intended
    ///to contain all CommunicatieVormTest Unit Tests
    ///</summary>
    [TestClass()]
    public class CommunicatieVormTest
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
        ///A test for Voorkeur
        ///</summary>
        [TestMethod()]
        public void VoorkeurTest()
        {
            CommunicatieVorm target = new CommunicatieVorm(); // TODO: Initialize to an appropriate value
            bool expected = false; // TODO: Initialize to an appropriate value
            bool actual;
            target.Voorkeur = expected;
            actual = target.Voorkeur;
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for TypeInt
        ///</summary>
        [TestMethod()]
        public void TypeIntTest()
        {
            CommunicatieVorm target = new CommunicatieVorm(); // TODO: Initialize to an appropriate value
            int expected = 0; // TODO: Initialize to an appropriate value
            int actual;
            target.TypeInt = expected;
            actual = target.TypeInt;
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for Type
        ///</summary>
        [TestMethod()]
        public void TypeTest()
        {
            CommunicatieVorm target = new CommunicatieVorm(); // TODO: Initialize to an appropriate value
            CommunicatieType expected = new CommunicatieType(); // TODO: Initialize to an appropriate value
            CommunicatieType actual;
            target.Type = expected;
            actual = target.Type;
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for Persoon
        ///</summary>
        [TestMethod()]
        public void PersoonTest()
        {
            CommunicatieVorm target = new CommunicatieVorm(); // TODO: Initialize to an appropriate value
            Persoon expected = null; // TODO: Initialize to an appropriate value
            Persoon actual;
            target.Persoon = expected;
            actual = target.Persoon;
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for Nummer
        ///</summary>
        [TestMethod()]
        public void NummerTest()
        {
            CommunicatieVorm target = new CommunicatieVorm(); // TODO: Initialize to an appropriate value
            string expected = string.Empty; // TODO: Initialize to an appropriate value
            string actual;
            target.Nummer = expected;
            actual = target.Nummer;
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for Nota
        ///</summary>
        [TestMethod()]
        public void NotaTest()
        {
            CommunicatieVorm target = new CommunicatieVorm(); // TODO: Initialize to an appropriate value
            string expected = string.Empty; // TODO: Initialize to an appropriate value
            string actual;
            target.Nota = expected;
            actual = target.Nota;
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for IsGezinsGebonden
        ///</summary>
        [TestMethod()]
        public void IsGezinsGebondenTest()
        {
            CommunicatieVorm target = new CommunicatieVorm(); // TODO: Initialize to an appropriate value
            bool expected = false; // TODO: Initialize to an appropriate value
            bool actual;
            target.IsGezinsGebonden = expected;
            actual = target.IsGezinsGebonden;
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for ID
        ///</summary>
        [TestMethod()]
        public void IDTest()
        {
            CommunicatieVorm target = new CommunicatieVorm(); // TODO: Initialize to an appropriate value
            int expected = 0; // TODO: Initialize to an appropriate value
            int actual;
            target.ID = expected;
            actual = target.ID;
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for CommunicatieVorm Constructor
        ///</summary>
        [TestMethod()]
        public void CommunicatieVormConstructorTest()
        {
            CommunicatieVorm target = new CommunicatieVorm();
            Assert.Inconclusive("TODO: Implement code to verify target");
        }
    }
}
