using System;
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
    	[TestMethod]
    	public void AdresUpdatedTest()
    	{
    		var target = new SyncPersoonService(); // TODO: Initialize to an appropriate value

    		var a = new Adres
    		        	{
    		        		Straat = "Kipdorp",
    		        		HuisNr = 28,
    		        		PostNr = 2000,
    		        		WoonPlaats = "Antwerpen",
    		        		Land = ""
    		        	};
    		target.VoorkeurAdresUpdated(a, new Bewoner[] {
			new Bewoner{AdNummer = 85111, AdresType = AdresTypeEnum.Kot},
			new Bewoner{AdNummer = 39198, AdresType = AdresTypeEnum.Werk}});
    	}

	/// <summary>
	///A test for VoorkeurCommunicatieUpdated
	///</summary>
	[TestMethod()]
	public void CommunicatieUpdatedTest()
	{
		var target = new SyncPersoonService(); // TODO: Initialize to an appropriate value
		int adNr = 17903;

		IEnumerable<CommunicatieMiddel> communicatiemiddelen = new CommunicatieMiddel[]
		                                                       	{
		                                                       		new CommunicatieMiddel
		                                                       			{
		                                                       				GeenMailings = true,
		                                                       				Type = CommunicatieType.Email,
		                                                       				Waarde = "tommy@microsoft.com"
		                                                       			},
		                                                       		new CommunicatieMiddel
		                                                       			{
		                                                       				GeenMailings = false,
		                                                       				Type = CommunicatieType.Email,
		                                                       				Waarde = "haepeto@chiro.be"
		                                                       			},
		                                                       		new CommunicatieMiddel
		                                                       			{
		                                                       				Type = CommunicatieType.TelefoonNummer,
		                                                       				Waarde = "03-297 61 10"
		                                                       			}
		                                                       	};

		target.CommunicatieToevoegen(adNr, communicatiemiddelen);
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
                                      ID = 205,
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
                ID = 2000,
                Naam = "HaepersNew",
                Sterfdatum = null,
                Voornaam = "Benny"
            };

            target.PersoonUpdated(persoon);

        }
    }
}
