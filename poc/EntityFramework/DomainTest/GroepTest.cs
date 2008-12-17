using Cg2.Core.Domain;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace DomainTest
{
    
    
    /// <summary>
    ///This is a test class for GroepTest and is intended
    ///to contain all GroepTest Unit Tests
    ///</summary>
    [TestClass()]
    public class GroepTest
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
        ///A test for WebSite
        ///</summary>
        [TestMethod()]
        public void WebSiteTest()
        {
            Groep target = new Groep(); // TODO: Initialize to an appropriate value
            string expected = string.Empty; // TODO: Initialize to an appropriate value
            string actual;
            target.WebSite = expected;
            actual = target.WebSite;
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for OprichtingsJaar
        ///</summary>
        [TestMethod()]
        public void OprichtingsJaarTest()
        {
            Groep target = new Groep(); // TODO: Initialize to an appropriate value
            Nullable<int> expected = new Nullable<int>(); // TODO: Initialize to an appropriate value
            Nullable<int> actual;
            target.OprichtingsJaar = expected;
            actual = target.OprichtingsJaar;
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for Naam
        ///</summary>
        [TestMethod()]
        public void NaamTest()
        {
            Groep target = new Groep(); // TODO: Initialize to an appropriate value
            string expected = string.Empty; // TODO: Initialize to an appropriate value
            string actual;
            target.Naam = expected;
            actual = target.Naam;
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for Code
        ///</summary>
        [TestMethod()]
        public void CodeTest()
        {
            Groep target = new Groep(); // TODO: Initialize to an appropriate value
            string expected = string.Empty; // TODO: Initialize to an appropriate value
            string actual;
            target.Code = expected;
            actual = target.Code;
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for Groep Constructor
        ///</summary>
        [TestMethod()]
        public void GroepConstructorTest()
        {
            Groep target = new Groep();
            Assert.Inconclusive("TODO: Implement code to verify target");
        }
    }
}
