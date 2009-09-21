using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Cg2.Orm.DataInterfaces;
using Cg2.Orm;
using Cg2.Fouten.Exceptions;

namespace Cg2.Workers.Test
{
    /// <summary>
    /// Tests op de security van de workers.
    /// </summary>
    [TestClass]
    public class AutorisatieTest
    {
        public AutorisatieTest()
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

        [ClassInitialize]
        static public void InitialiseerTests(TestContext context)
        {
            // we doen even niets.
        }

        /// <summary>
        /// Probeert ledenlijst op te halen voor niet-GAV.
        /// Verwacht een exception.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(GeenGavException))]
        public void LijstLedenGeenGav()
        {
            // Arrange

            var ledenDaoMock = new Mock<ILedenDao>();
            var autorisatieMgrMock = new Mock<IAutorisatieManager>();

            ledenDaoMock.Setup(foo => foo.PaginaOphalen(Properties.Settings.Default.TestGroepsWerkJaarID)).Returns(new List<Lid>());
            autorisatieMgrMock.Setup(foo => foo.IsGavGroepsWerkJaar(Properties.Settings.Default.TestGroepsWerkJaarID)).Returns(false);

            LedenDaoCollectie daos = new LedenDaoCollectie(ledenDaoMock.Object, null, null, null, null, null);

            LedenManager lm = new LedenManager(daos, autorisatieMgrMock.Object);

            // Act

            lm.PaginaOphalen(Properties.Settings.Default.TestGroepsWerkJaarID);

            // Verwacht exception
        }

        /// <summary>
        /// Probeert ledenlijst op te halen als wel GAV
        /// Verwacht geen exception.
        /// </summary>
        [TestMethod]
        public void LijstLedenGav()
        {
            // Arrange

            var ledenDaoMock = new Mock<ILedenDao>();
            var autorisatieMgrMock = new Mock<IAutorisatieManager>();

            ledenDaoMock.Setup(foo => foo.PaginaOphalen(Properties.Settings.Default.TestGroepsWerkJaarID)).Returns(new List<Lid>());
            autorisatieMgrMock.Setup(foo => foo.IsGavGroepsWerkJaar(Properties.Settings.Default.TestGroepsWerkJaarID)).Returns(true);

            LedenDaoCollectie daos = new LedenDaoCollectie(ledenDaoMock.Object, null, null, null, null, null);

            LedenManager lm = new LedenManager(daos, autorisatieMgrMock.Object);

            // Act

            IList<Lid> lijst = lm.PaginaOphalen(Properties.Settings.Default.TestGroepsWerkJaarID);

            // Assert

            Assert.IsTrue(lijst != null);
        }
    }
}
