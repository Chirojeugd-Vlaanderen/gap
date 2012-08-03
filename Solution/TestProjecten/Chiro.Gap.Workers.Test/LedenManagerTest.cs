using System.Data.Objects.DataClasses;

using Chiro.Gap.Domain;
using Chiro.Gap.Workers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using Chiro.Gap.Orm.DataInterfaces;
using Chiro.Gap.Orm.SyncInterfaces;
using Chiro.Gap.Orm;
using Chiro.Cdf.Ioc;

namespace Chiro.Gap.Workers.Test
{
    
    
    /// <summary>
    ///This is a test class for LedenManagerTest and is intended
    ///to contain all LedenManagerTest Unit Tests
    ///</summary>
    [TestClass()]
    public class LedenManagerTest
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
        /// Test of 'LedenManager.InschrijvingVoorstellen' rekening houdt met het geslacht van een persoon.
        ///</summary>
        [TestMethod()]
        public void InschrijvingVoorstellenTest()
        {
            // Arrange

            GelieerdePersoon gp = new GelieerdePersoon
                                      {
                                          Persoon =
                                              new Persoon
                                                  {
                                                      GeboorteDatum = new DateTime(1996, 03, 07),
                                                      Geslacht = GeslachtsType.Vrouw,
                                                  }
                                      };

            GroepsWerkJaar gwj = new GroepsWerkJaar();
            gwj.AfdelingsJaar.Add(new AfdelingsJaar
                                      {
                                          ID = 1,
                                          GeboorteJaarVan = 1996,
                                          GeboorteJaarTot = 1997,
                                          Geslacht = GeslachtsType.Man,
                                          OfficieleAfdeling = new OfficieleAfdeling()
                                      });
                        gwj.AfdelingsJaar.Add(new AfdelingsJaar
                                      {
                                          ID = 2,
                                          GeboorteJaarVan = 1996,
                                          GeboorteJaarTot = 1997,
                                          Geslacht = GeslachtsType.Vrouw,
                                          OfficieleAfdeling = new OfficieleAfdeling()
                                      });


            LedenManager target = Factory.Maak<LedenManager>();

            // Act
            
            var actual = target.InschrijvingVoorstellen(gp, gwj, false);

            // Assert

            Assert.AreEqual(actual.AfdelingsJaarIDs[0], 2);
        }

        /// <summary>
        /// Test of er een 'redelijke' afdeling wordt voorgesteld als er voor een persoon
        /// geen afdeling wordt gevonden waarin die 'natuurlijk' past.
        ///</summary>
        [TestMethod()]
        public void InschrijvingVoorstellenTest1()
        {
            // Arrange
            // Ik maak een persoon, en een werkjaar met 2 afdelingen, zodanig dat
            // de geboortedatum van die persoon buiten de afdelingen valt. We verwachten
            // de afdeling die het meest logisch is (kleinste verschil met geboortedatum)

            GelieerdePersoon gp = new GelieerdePersoon
            {
                Persoon =
                    new Persoon
                    {
                        GeboorteDatum = new DateTime(1996, 03, 07),
                        Geslacht = GeslachtsType.Vrouw,
                    }
            };

            GroepsWerkJaar gwj = new GroepsWerkJaar();
            gwj.AfdelingsJaar.Add(new AfdelingsJaar
            {
                ID = 1,
                GeboorteJaarVan = 1999,
                GeboorteJaarTot = 2000,
                Geslacht = GeslachtsType.Gemengd,
                OfficieleAfdeling = new OfficieleAfdeling()
            });
            gwj.AfdelingsJaar.Add(new AfdelingsJaar
            {
                ID = 2,
                GeboorteJaarVan = 1997,
                GeboorteJaarTot = 1998,
                Geslacht = GeslachtsType.Gemengd,
                OfficieleAfdeling = new OfficieleAfdeling()
            });


            LedenManager target = Factory.Maak<LedenManager>();

            // Act

            var actual = target.InschrijvingVoorstellen(gp, gwj, false);

            // Assert

            Assert.AreEqual(actual.AfdelingsJaarIDs[0], 2);
        }
    }
}
