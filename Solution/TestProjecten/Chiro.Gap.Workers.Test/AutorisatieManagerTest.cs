using System.Collections.ObjectModel;
using Chiro.Cdf.Ioc;
using Chiro.Gap.Workers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using Chiro.Gap.WorkerInterfaces;
using Chiro.Gap.Poco.Model;
using Moq;
using System.Collections.Generic;

namespace Chiro.Gap.Workers.Test
{
    
    
    /// <summary>
    ///This is a test class for AutorisatieManagerTest and is intended
    ///to contain all AutorisatieManagerTest Unit Tests
    ///</summary>
    [TestClass()]
    public class AutorisatieManagerTest
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
        ///Controleert of de GAV-check van een groep rekening houdt met de vervaldatum van gebruikersrechten.
        ///</summary>
        [TestMethod()]
        public void IsGavGroepTest()
        {
            // ARRANGE

            // testgroep; toegang met mijnLogin net vervallen.
            const string mijnLogin = "MijnLogin";

            Groep groep = new ChiroGroep
            {
                GebruikersRecht = new[]
                                                        {
                                                            new GebruikersRecht
                                                                {
                                                                    Gav = new Gav {Login = "MijnLogin"},
                                                                    VervalDatum = DateTime.Today // net vervallen
                                                                }
                                                        }
            };

            // Zet mock op voor het opleveren van gebruikersnaam
            var authenticatieManagerMock = new Mock<IAuthenticatieManager>();
            authenticatieManagerMock.Setup(src => src.GebruikersNaamGet()).Returns(mijnLogin);
            Factory.InstantieRegistreren(authenticatieManagerMock.Object);


            // ACT

            var target = Factory.Maak<AutorisatieManager>();
            var actual = target.IsGav(groep);


            // ASSERT

            Assert.IsFalse(actual);
        }

        /// <summary>
        ///Controleert of de GAV-check van een gelieerde persoon rekening houdt met de vervaldatum van gebruikersrechten.
        ///</summary>
        [TestMethod()]
        public void IsGavGelieerdePersoonTest()
        {
            // ARRANGE

            // testgroep met gelieerde persoon; toegang met mijnLogin net vervallen.
            const string mijnLogin = "MijnLogin";

            var gp = new GelieerdePersoon();

            var groep = new ChiroGroep
                              {
                                  GebruikersRecht = new[]
                                                        {
                                                            new GebruikersRecht
                                                                {
                                                                    Gav = new Gav {Login = "MijnLogin"},
                                                                    VervalDatum = DateTime.Today // net vervallen
                                                                }
                                                        },
                                  GelieerdePersoon = new[]
                                                         {
                                                            gp
                                                         }
                              };

            gp.Groep = groep;

            // Zet mock op voor het opleveren van gebruikersnaam
            var authenticatieManagerMock = new Mock<IAuthenticatieManager>();
            authenticatieManagerMock.Setup(src => src.GebruikersNaamGet()).Returns(mijnLogin);
            Factory.InstantieRegistreren(authenticatieManagerMock.Object);


            // ACT

            var target = Factory.Maak<AutorisatieManager>();
            var actual = target.IsGav(groep);


            // ASSERT

            Assert.IsFalse(actual);
        }
    }
}
