using System.Linq;
using Chiro.Gap.Workers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using Chiro.Gap.WorkerInterfaces;
using Chiro.Gap.SyncInterfaces;
using Chiro.Gap.Poco.Model;
using System.Collections.Generic;
using Chiro.Gap.Domain;

namespace Chiro.Gap.Workers.Test
{
    
    
    /// <summary>
    ///This is a test class for GelieerdePersonenManagerTest and is intended
    ///to contain all GelieerdePersonenManagerTest Unit Tests
    ///</summary>
    [TestClass()]
    public class GelieerdePersonenManagerTest
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
        ///Controleert of voorkeursadres goed wordt bewaardgezet door PersonenManager.AdresToevoegen
        ///</summary>
        [TestMethod()]
        public void AdresToevoegenVoorkeurTest()
        {
            // ARRANGE

            var gelieerdePersoon = new GelieerdePersoon { Persoon = new Persoon() };
            gelieerdePersoon.Persoon.GelieerdePersoon.Add(gelieerdePersoon);

            var adres = new BelgischAdres();

            // ACT

            var target = new GelieerdePersonenManager();
            target.AdresToevoegen(new List<GelieerdePersoon>{gelieerdePersoon}, adres, AdresTypeEnum.Thuis, true);
            
            // ASSERT

            // vind het persoonsadresobject
            var persoonsAdres = gelieerdePersoon.Persoon.PersoonsAdres.First();

            // weet het persoonsadresobject dat het voorkeursadres is?
            Assert.IsNotNull(persoonsAdres.GelieerdePersoon.FirstOrDefault());
        }
    }
}
