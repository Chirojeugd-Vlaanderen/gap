using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Cg2.Workers;
using Chiro.Gap.Orm.DataInterfaces;
using Chiro.Cdf.Ioc;
using Chiro.Gap.Data.Ef;

namespace Cg2.Dao.Test
{
    /// <summary>
    /// Summary description for Authorisatietest
    /// </summary>
    [TestClass]
    public class AuthorisatieTest
    {
        public AuthorisatieTest()
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


        /// <summary>
        /// Uit te voeren voor de 1ste test:
        /// </summary>
        /// <param name="context"></param>
        [ClassInitialize]
        static public void TestInitialiseren(TestContext context)
        {
            Factory.ContainerInit();
        }

        /// <summary>
        /// Check of gebruiker die GAV is van groepswerkjaar gedetecteerd
        /// wordt door IsGavGroepsWerkJaar.
        /// </summary>
        [TestMethod]
        public void GroepsWerkJaarWelGav()
        {
            // Arrange

            // Ik gebruik hier geen IOC om de Authorisatiedao aan te maken,
            // aangezien er in de Web.Config een dummy gekoppeld wordt aan
            // IAuthorisatieDao.

            IAutorisatieDao dao = new AutorisatieDao();

            // Act

            bool resultaat = dao.IsGavGroepsWerkJaar(Properties.Settings.Default.TestGav1, Properties.Settings.Default.TestGroepsWerkJaarID);

            // Assert

            Assert.IsTrue(resultaat);
        }

        /// <summary>
        /// Check of gebruiker die geen GAV is van groepswerkjaar gedetecteerd
        /// wordt door IsGavGroepsWerkJaar.
        /// </summary>
        [TestMethod]
        public void GroepsWerkJaarGeenGav()
        {
            // Arrange

            // Ik gebruik hier geen IOC om de Authorisatiedao aan te maken,
            // aangezien er in de Web.Config een dummy gekoppeld wordt aan
            // IAuthorisatieDao.

            IAutorisatieDao dao = new AutorisatieDao();

            // Act

            bool resultaat = dao.IsGavGroepsWerkJaar(Properties.Settings.Default.TestGav2, Properties.Settings.Default.TestGroepsWerkJaarID);

            // Assert

            Assert.IsFalse(resultaat);
        }
    }
}
