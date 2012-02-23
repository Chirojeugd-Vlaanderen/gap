using Chiro.Gap.Workers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using Chiro.Gap.Orm;
using Chiro.Cdf.Data;
using Chiro.Gap.Orm.DataInterfaces;
using Chiro.Gap.Orm.SyncInterfaces;
using System.Data.Objects.DataClasses;
using Chiro.Cdf.Ioc;

namespace Chiro.Gap.Workers.Test
{
    
    
    /// <summary>
    /// Dit is een testclass voor CommVormManagerTest,
    ///to contain all CommVormManagerTest Unit Tests
    /// </summary>
    [TestClass()]
    public class CommVormManagerTest
    {


        private TestContext testContextInstance;

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        /// </summary>
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

        /// <summary>
        /// Code uit te voeren voor de 1ste test uitgevoerd wordt.
        /// </summary>
        /// <param name="testContext"></param>
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
        //[TestInitialize]
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
        /// Als een persoon al een voorkeurs-e-mailadres heeft, en er wordt een nieuw
        /// vooorkeurs-e-mailadres gekoppeld, dan moet het bestaande adres zijn voorkeur
        /// verliezen.
        /// </summary>
        [TestMethod]
        public void KoppelenVoorkeursCommunicatieTest()
        {
            // Arrange

            const int TESTGPID = 1234;      // arbitrair ID van een gelieerde persoon
            const int TESTCVID = 2345;      // en van een communicatievorm
            const int TESTCTID = 3;         // en diens communicatietype

            var testCommunicatieType = new CommunicatieType { ID = TESTCTID, Validatie = ".*" };
            var testCommunicatieVorm = new CommunicatieVorm
            {
                ID = TESTCVID,
                CommunicatieType = testCommunicatieType,
                Nummer = "jos@linux.be",
                Voorkeur = true
            };

            // Koppel gauw testCommunicatieVorm aan testGelieerdePersoon

            var testGelieerdePersoon = new GelieerdePersoon
                                           {
                                               ID = TESTGPID,
                                               Persoon = new Persoon(),
                                               Communicatie =
                                                   new EntityCollection<CommunicatieVorm>
                                                       {
                                                           testCommunicatieVorm
                                                       }
                                           };
            testCommunicatieVorm.GelieerdePersoon = testGelieerdePersoon;

            var target = Factory.Maak<CommVormManager>();

            CommunicatieVorm nieuwecv = new CommunicatieVorm
            {
                CommunicatieType = testCommunicatieType,    // e-mail
                ID = 0,                    // nieuwe communicatievorm
                Nummer = "johan@linux.be", // arbitrair nieuw e-mailadres
                Voorkeur = true
            };

            // act

            target.Koppelen(testGelieerdePersoon, nieuwecv);

            // assert

            Assert.IsFalse(testCommunicatieVorm.Voorkeur);
        }
    }
}
