using System.Data.Objects.DataClasses;
using System.Linq;

using Chiro.Cdf.Ioc;
using Chiro.Gap.Domain;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using Chiro.Gap.Orm.DataInterfaces;
using Chiro.Gap.Orm;
using System.Collections.Generic;

using Moq;

namespace Chiro.Gap.Workers.Test
{
    
    
    /// <summary>
    ///This is a test class for GroepsWerkJaarManagerTest and is intended
    ///to contain all GroepsWerkJaarManagerTest Unit Tests
    ///</summary>
    [TestClass()]
    public class GroepsWerkJaarManagerTest
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
        /// Snelle test die nakijkt of GroepsWerkJaarManager.AfdelingsJarenVoorstellen daadwerkelijk de geboortedata
        /// voor de nieuwe afdelingsjaren bijwerkt.
        ///</summary>
        [TestMethod()]
        public void AfdelingsJarenVoorstellenTest()
        {
            var target = Factory.Maak<GroepsWerkJaarManager>();

            var groep = new ChiroGroep();
            var groepsWerkJaar = new GroepsWerkJaar {WerkJaar = 2010, ID = 2971, Groep = groep};
            var afdeling = new Afdeling {ID = 2337, ChiroGroep = groep};
            var afdelingsJaar = new AfdelingsJaar
                {GroepsWerkJaar = groepsWerkJaar, Afdeling = afdeling, GeboorteJaarVan = 2003, GeboorteJaarTot = 2004};

            const int NIEUW_WERKJAAR = 2011;

            var actual = target.AfdelingsJarenVoorstellen(groep, groep.Afdeling.ToArray(), NIEUW_WERKJAAR);

            Assert.IsNotNull(actual);
            Assert.IsNotNull(actual[0]);
            Assert.AreEqual(actual[0].GeboorteJaarVan, afdelingsJaar.GeboorteJaarVan + 1);
            Assert.AreEqual(actual[0].GeboorteJaarTot, afdelingsJaar.GeboorteJaarTot + 1);
        }

        /// <summary>
        /// Deze test moet nagaan of GroepsWerkjaarManager.AfdelingsJarenVoorstellen ook een
        /// officiele afdeling koppelt aan een afdelingsjaar voor een afdeling die vorig jaar
        /// niet actief was.
        ///</summary>
        [TestMethod()]
        public void AfdelingsJarenVoorstellenTest1()
        {
            // -- Arrange --

            var afdelingenDaoMock = new Mock<IAfdelingenDao>();
            afdelingenDaoMock.Setup(dao => dao.OfficieleAfdelingOphalen((int)NationaleAfdeling.Ribbels)).Returns(
                new OfficieleAfdeling {ID = (int) NationaleAfdeling.Ribbels, LeefTijdVan = 6, LeefTijdTot = 7});
            Factory.InstantieRegistreren(afdelingenDaoMock.Object);

            var target = Factory.Maak<GroepsWerkJaarManager>();

            // Een Chirogroep met een oud groepswerkjaar. Zonder afdelingen, why not.
            var groep = new ChiroGroep
                {GroepsWerkJaar = new EntityCollection<GroepsWerkJaar> {new GroepsWerkJaar()}};

            // Dit jaar willen we een groep met 1 afdeling.
            var afdelingen = new[] {new Afdeling {ID = 1, ChiroGroep = groep}};

            const int NIEUW_WERKJAAR = 2012; // Jaartal is eigenlijk irrelevant voor deze test.

            // -- Act -- 
            var actual = target.AfdelingsJarenVoorstellen(groep, afdelingen, NIEUW_WERKJAAR);
            var afdelingsJaar = actual.FirstOrDefault();

            // -- Assert --

            Assert.IsNotNull(afdelingsJaar);
            Assert.IsNotNull(afdelingsJaar.OfficieleAfdeling);
        }
    }
}
