using Chiro.Cdf.Ioc;
using Chiro.Gap.Orm.DataInterfaces;
using Chiro.Gap.Workers.Exceptions;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using Chiro.Gap.WorkerInterfaces;
using Chiro.Gap.Orm;

using Moq;

namespace Chiro.Gap.Workers.Test
{
    
    
    /// <summary>
    ///This is a test class for GroepenManagerTest and is intended
    ///to contain all GroepenManagerTest Unit Tests
    ///</summary>
    [TestClass()]
    public class GroepenManagerTest
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
        [ClassInitialize()]
        public static void MyClassInitialize(TestContext testContext)
        {
            Factory.ContainerInit();
        }
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
        /// Deze test moet nakijken of ik niet stiekem het stamnummer van een groep kan wijzigen.
        ///</summary>
        [TestMethod()]
        [ExpectedException(typeof(GeenGavException))]
        public void BewarenTest()
        {
            // arrabge

            const int GROEP_ID = 1;


            var veelGebruiktMock = new Mock<IVeelGebruikt>();
            veelGebruiktMock.Setup(che => che.GroepsWerkJaarOphalen(GROEP_ID)).Returns(new GroepsWerkJaar
                {Groep = new ChiroGroep {ID = GROEP_ID, Code = "TST/0001"}});
            Factory.InstantieRegistreren(veelGebruiktMock.Object);

            var groepenDaoMock = new Mock<IGroepenDao>();
            groepenDaoMock.Setup(dao => dao.Ophalen(GROEP_ID)).Returns(new ChiroGroep {ID = GROEP_ID, Code = "TST/0001"});
            Factory.InstantieRegistreren(groepenDaoMock.Object);

            var target = Factory.Maak<GroepenManager>();

            Groep g = target.Ophalen(GROEP_ID);

            // act

            g.Code = "TST/0002";
            target.Bewaren(g);

            // assert

            Assert.Fail();  // we verwachtten een exception; komen we hier, dan is het niet gelukt
        }
    }
}
