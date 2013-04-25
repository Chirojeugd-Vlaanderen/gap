using Chiro.Gap.Domain;
using Chiro.Gap.Validatie;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace Chiro.Gap.Algemeen.Test
{
    
    
    /// <summary>
    ///This is a test class for ValidatorTest and is intended
    ///to contain all ValidatorTest Unit Tests
    ///</summary>
    [TestClass()]
    public class ValidatorTest
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
        /// Domme validator die altijd zegt dat het OK is.
        /// </summary>
        private class MijnValidator: Validator<int>
        {
            public override FoutNummer? FoutNummer(int teValideren)
            {
                return null;
            }
        }

        /// <summary>
        /// Controleert of Validator.Valideer <c>true</c> is als het foutnummer <c>null</c> is.
        /// </summary>
        [TestMethod()]
        public void ValideerTest()
        {
            // Arrange

            var a = new MijnValidator();

            // Assert

            Assert.IsTrue(a.Valideer(3));
        }
    }
}
