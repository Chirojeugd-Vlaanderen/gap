﻿using System;
using Chiro.Kip.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Chiro.Kip.Services.DataContracts;
using System.Collections.Generic;

namespace Chiro.Kip.Services.Test
{


    /// <summary>
    ///This is a test class for SyncPersoonServiceTest and is intended
    ///to contain all SyncPersoonServiceTest Unit Tests
    ///</summary>
    [TestClass()]
    public class SyncPersoonServiceTest
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
        ///A test for AdresUpdated
        ///</summary>
        [TestMethod()]
        public void AdresUpdatedTest()
        {
            var target = new SyncPersoonService(); // TODO: Initialize to an appropriate value
            var persoon = new Persoon()
            {
                AdNr = 17903,
                Geboortedatum = new DateTime(1971, 5, 14),
                Geslacht = GeslachtsEnum.Man,
                Id = 205,
                Naam = "Haepers",
                Sterfdatum = null,
                Voornaam = "Tommy"
            };

            var adreslijst = new List<Adres>()
                                 {
                                     new Adres()

                                         {
                                             AdresType = AdresTypeEnum.Thuis,
                                             Straat = "Nijverheidsstraat",
                                               HuisNr = 58,
                                                Postnummer = 2840,
                                             Woonplaats = "Rumst",
                                            Land = "België"
                                         },
                                         new Adres()
                                             {
                                                 AdresType = AdresTypeEnum.Werk,
                                             Straat = "Bessenveldstraat",
                                               HuisNr = 19,
                                                Postnummer = 1831,
                                             Woonplaats = "Diegem",
                                            Land = "België"
                                             }
                                 }

                ;


            IEnumerable<Adres> adreses = adreslijst;
            target.AdresUpdated(persoon, adreses);
            
        }

        /// <summary>
        ///A test for CommunicatieUpdated
        ///</summary>
        [TestMethod()]
        public void CommunicatieUpdatedTest()
        {
            var target = new SyncPersoonService(); // TODO: Initialize to an appropriate value
            var persoon = new Persoon()
            {
                AdNr = 17903,
                Geboortedatum = new DateTime(1971, 5, 14),
                Geslacht = GeslachtsEnum.Man,
                Id = 205,
                Naam = "Haepers",
                Sterfdatum = null,
                Voornaam = "Tommy"
            };

            IEnumerable<Communicatiemiddel> communicatiemiddelen = null; // TODO: Initialize to an appropriate value
            target.CommunicatieUpdated(persoon, communicatiemiddelen);
            Assert.Inconclusive("A method that does not return a value cannot be verified.");
        }

        /// <summary>
        ///A test for PersoonUpdated
        ///</summary>
        [TestMethod()]
        public void PersoonUpdatedTest()
        {
            var target = new SyncPersoonService(); // TODO: Initialize to an appropriate value
            var persoon = new Persoon()
                                  {
                                      AdNr = 17903,
                                      Geboortedatum = new DateTime(1971, 5, 14),
                                      Geslacht = GeslachtsEnum.Man,
                                      Id = 205,
                                      Naam = "Haepers",
                                      Sterfdatum = null,
                                      Voornaam = "Tommy"
                                  };

            target.PersoonUpdated(persoon);

        }

        /// <summary>
        ///A test for PersoonUpdated
        ///</summary>
        [TestMethod()]
        public void PersoonUpdatedNewTest()
        {
            var target = new SyncPersoonService(); // TODO: Initialize to an appropriate value
            var persoon = new Persoon()
            {
                Geboortedatum = new DateTime(1971, 5, 14),
                Geslacht = GeslachtsEnum.Man,
                Id = 2000,
                Naam = "HaepersNew",
                Sterfdatum = null,
                Voornaam = "Benny"
            };

            target.PersoonUpdated(persoon);

        }
    }
}