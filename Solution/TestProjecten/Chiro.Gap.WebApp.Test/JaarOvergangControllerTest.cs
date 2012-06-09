using System.Linq;

using Chiro.Cdf.Ioc;
using Chiro.Gap.Domain;
using Chiro.Gap.ServiceContracts;
using Chiro.Gap.ServiceContracts.DataContracts;
using Chiro.Gap.WebApp.Controllers;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using System.Web.Mvc;

using Chiro.Gap.WebApp.Models;

using Moq;

namespace Chiro.Gap.WebApp.Test
{
    
    
    /// <summary>
    /// Dit is een testclass voor JaarOvergangControllerTest,
    ///to contain all JaarOvergangControllerTest Unit Tests
    /// </summary>
    [TestClass]
    public class JaarOvergangControllerTest
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
        //[ClassInitialize]
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

        /// <summary>
        /// Run code before running each test
        /// </summary>
        [TestInitialize]
        public void MyTestInitialize()
        {
            // Omdat ik hier en daar mocks ga registreren, wil ik de IOC-container
            // iedere keer resetten.

            Factory.ContainerInit();
        }

        //
        //Use TestCleanup to run code after each test has run
        //[TestCleanup()]
        //public void MyTestCleanup()
        //{
        //}
        //
        #endregion


        /// <summary>
        /// Kijkt na of de gesuggereerde geboortejaren voor de nieuwe afdelingsjaren wel juist zijn bij de jaarovergang.
        /// </summary>
        [TestMethod]
        public void Stap1AfdelingenSelecterenTest()
        {
            const int GROEPID = 426;            // arbitrair

            const int VORIGWERKJAAR = 2010;     // vorig werkjaar was zogezegd 2010-2011
            const int GROEPSWERKJAARID = 2971;  // arbitrair groepswerkjaarid

            const int NIEUWWERKJAAR = 2011;     // nieuw werkjaar, zogezegd 2011-2012

            const int AFDELINGID = 2337;            // ook arbitrair; bij wijze van test slechts 1 afdeling
            const int OUDGEBOORTEJAARVAN = 2003;    // geboortejaar van en tot in vorig werkjaar
            const int OUDGEBOORTEJAARTOT = 2004;

            // setup mocks

            var veelGebruiktMock = new Mock<IVeelGebruikt>();
            veelGebruiktMock.Setup(vg => vg.GroepsWerkJaarOphalen(GROEPID)).Returns(new GroepsWerkJaarDetail
                                                                                        {
                                                                                            GroepID = GROEPID,
                                                                                            WerkJaar = VORIGWERKJAAR,
                                                                                            WerkJaarID = GROEPSWERKJAARID
                                                                                        });
            veelGebruiktMock.Setup(vg => vg.BivakStatusHuidigWerkjaarOphalen(GROEPID)).Returns(
                new BivakAangifteLijstInfo {AlgemeneStatus = BivakAangifteStatus.NogNietVanBelang});

            var groepenServiceMock = new Mock<IGroepenService>();
            groepenServiceMock.Setup(svc => svc.NieuwWerkJaarOphalen()).Returns(NIEUWWERKJAAR);
            groepenServiceMock.Setup(svc => svc.AlleAfdelingenOphalen(GROEPID)).Returns(new[]
                                                                                            {
                                                                                                new AfdelingInfo
                                                                                                    {ID = AFDELINGID}
                                                                                            });
            groepenServiceMock.Setup(svc => svc.ActieveAfdelingenOphalen(GROEPSWERKJAARID)).Returns(new[]
                                                                                               {
                                                                                                   new AfdelingDetail
                                                                                                       {
                                                                                                           AfdelingID =
                                                                                                               AFDELINGID,
                                                                                                           GeboorteJaarVan
                                                                                                               =
                                                                                                               OUDGEBOORTEJAARVAN,
                                                                                                           GeboorteJaarTot
                                                                                                               =
                                                                                                               OUDGEBOORTEJAARTOT
                                                                                                       }
                                                                                               });

            Factory.InstantieRegistreren(veelGebruiktMock.Object);
            Factory.InstantieRegistreren(groepenServiceMock.Object);

            // arrange

            var target = Factory.Maak<JaarOvergangController>();
            var model1 = new JaarOvergangAfdelingsModel
                             {
                                 GekozenAfdelingsIDs = new[] {AFDELINGID}   // doe maar iets
                             };
            int groepID = GROEPID;

            // act

            var actualModel =
                ((ViewResult) target.Stap1AfdelingenSelecteren(model1, groepID)).ViewData.Model as
                JaarOvergangAfdelingsJaarModel;

            // assert

            const int EXPECTEDGEBOORTEJAARVAN = OUDGEBOORTEJAARVAN + (NIEUWWERKJAAR - VORIGWERKJAAR);
            const int EXPECTEDGEBOORTEJAARTOT = OUDGEBOORTEJAARTOT + (NIEUWWERKJAAR - VORIGWERKJAAR);

            Assert.IsNotNull(actualModel);
            Assert.AreEqual(EXPECTEDGEBOORTEJAARVAN, actualModel.Afdelingen.First().GeboorteJaarVan);
            Assert.AreEqual(EXPECTEDGEBOORTEJAARTOT, actualModel.Afdelingen.First().GeboorteJaarTot);
        }
    }
}
