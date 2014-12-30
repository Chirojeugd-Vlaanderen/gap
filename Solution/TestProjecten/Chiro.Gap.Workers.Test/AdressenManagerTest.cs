using System.Collections.Generic;
using Chiro.Cdf.Ioc;
using Chiro.Gap.Workers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using Chiro.Gap.WorkerInterfaces;
using Chiro.Gap.Domain;
using Chiro.Gap.Poco.Model;
using System.Linq;

namespace Chiro.Gap.Workers.Test
{
    
    
    /// <summary>
    ///This is a test class for AdressenManagerTest and is intended
    ///to contain all AdressenManagerTest Unit Tests
    ///</summary>
    [TestClass()]
    public class AdressenManagerTest
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
        [TestInitialize()]
        public void MyTestInitialize()
        {
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
        /// Controleert of ZoekenOfMaken wel degelijk rekening houdt
        /// met busnummers.
        ///</summary>
        [TestMethod()]
        public void ZoekenOfMakenBusTest1()
        {
            // ARRANGE

            var adres = new BelgischAdres
                        {
                            ID = 1,
                            StraatNaam = new StraatNaam {Naam = "Kipdorp", PostNummer = 2000},
                            HuisNr = 30,
                            WoonPlaats = new WoonPlaats {Naam = "Antwerpen", PostNummer = 2000}
                        };

            // ACT

            var target = Factory.Maak<AdressenManager>();
            var adresInfo = new AdresInfo
                            {
                                StraatNaamNaam = adres.StraatNaam.Naam,
                                HuisNr = adres.HuisNr,
                                Bus = "B", // iets anders.
                                PostNr = adres.StraatNaam.PostNummer,
                                WoonPlaatsNaam = adres.WoonPlaats.Naam
                            };

            var adressen = new List<Adres> {adres}.AsQueryable();
            var straatNamen = new List<StraatNaam> {adres.StraatNaam}.AsQueryable();
            var woonPlaatsen = new List<WoonPlaats> {adres.WoonPlaats}.AsQueryable();
            var landen = new List<Land>().AsQueryable();

            var actual = target.ZoekenOfMaken(adresInfo, adressen, straatNamen, woonPlaatsen, landen);

            // ASSERT

            Assert.AreNotEqual(adres, actual);
        }
        /// <summary>
        /// Controleert of ZoekenOfMaken voor de busnummers null en leeg gelijkschakelt
        ///</summary>
        [TestMethod()]
        public void ZoekenOfMakenBusTest2()
        {
            // ARRANGE

            var adres = new BelgischAdres
            {
                ID = 1,
                StraatNaam = new StraatNaam { Naam = "Kipdorp", PostNummer = 2000 },
                HuisNr = 30,
                Bus = null,
                WoonPlaats = new WoonPlaats { Naam = "Antwerpen", PostNummer = 2000 }
            };

            // ACT

            var target = Factory.Maak<AdressenManager>();
            var adresInfo = new AdresInfo
            {
                StraatNaamNaam = adres.StraatNaam.Naam,
                HuisNr = adres.HuisNr,
                Bus = "",
                PostNr = adres.StraatNaam.PostNummer,
                WoonPlaatsNaam = adres.WoonPlaats.Naam
            };

            var adressen = new List<Adres> { adres }.AsQueryable();
            var straatNamen = new List<StraatNaam> { adres.StraatNaam }.AsQueryable();
            var woonPlaatsen = new List<WoonPlaats> { adres.WoonPlaats }.AsQueryable();
            var landen = new List<Land>().AsQueryable();

            var actual = target.ZoekenOfMaken(adresInfo, adressen, straatNamen, woonPlaatsen, landen);

            // ASSERT

            Assert.AreEqual(adres, actual);
        }
    }
}
