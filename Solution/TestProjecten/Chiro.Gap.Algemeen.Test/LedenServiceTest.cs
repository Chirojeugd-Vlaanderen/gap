﻿using Chiro.Gap.Services;
using NUnit.Framework;
using System;
using NUnit.Framework.Web;
using Chiro.Gap.WorkerInterfaces;
using Chiro.Cdf.Poco;
using Chiro.Gap.SyncInterfaces;
using Chiro.Gap.ServiceContracts.DataContracts;
using System.Collections.Generic;

namespace Chiro.Gap.Algemeen.Test
{
    
    
    /// <summary>
    ///This is a test class for LedenServiceTest and is intended
    ///to contain all LedenServiceTest Unit Tests
    ///</summary>
    [TestFixture]
    public class LedenServiceTest
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
        //[TestFixtureSetUp]
        //public static void MyClassInitialize(TestContext testContext)
        //{
        //}
        //
        //Use ClassCleanup to run code after all tests in a class have run
        //[TestFixtureTearDown()]
        //public static void MyClassCleanup()
        //{
        //}
        //
        //Use TestInitialize to run code before running each test
        //[SetUp]
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
        ///A test for Inschrijven
        ///</summary>
        // TODO: Ensure that the UrlToTest attribute specifies a URL to an ASP.NET page (for example,
        // http://.../Default.aspx). This is necessary for the unit test to be executed on the web server,
        // whether you are testing a page, web service, or a WCF service.
        [Test]
        [HostType("ASP.NET")]
        [AspNetDevelopmentServerHost("C:\\Users\\johanv\\dev\\gap\\Solution\\Chiro.Gap.Services", "/")]
        [UrlToTest("http://localhost:2734/")]
        public void InschrijvenGestoptTest()
        {
            IAutorisatieManager autorisatieMgr = null; // TODO: Initialize to an appropriate value
            IVerzekeringenManager verzekeringenMgr = null; // TODO: Initialize to an appropriate value
            ILedenManager ledenMgr = null; // TODO: Initialize to an appropriate value
            IGroepsWerkJarenManager groepsWerkJarenMgr = null; // TODO: Initialize to an appropriate value
            IGroepenManager groepenMgr = null; // TODO: Initialize to an appropriate value
            IFunctiesManager functiesMgr = null; // TODO: Initialize to an appropriate value
            IRepositoryProvider repositoryProvider = null; // TODO: Initialize to an appropriate value
            ILedenSync ledenSync = null; // TODO: Initialize to an appropriate value
            IVerzekeringenSync verzekeringenSync = null; // TODO: Initialize to an appropriate value
            LedenService target = new LedenService(autorisatieMgr, verzekeringenMgr, ledenMgr, groepsWerkJarenMgr, groepenMgr, functiesMgr, repositoryProvider, ledenSync, verzekeringenSync); // TODO: Initialize to an appropriate value
            InTeSchrijvenLid[] inschrijfInfo = null; // TODO: Initialize to an appropriate value
            string foutBerichten = string.Empty; // TODO: Initialize to an appropriate value
            string foutBerichtenExpected = string.Empty; // TODO: Initialize to an appropriate value
            IEnumerable<int> expected = null; // TODO: Initialize to an appropriate value
            IEnumerable<int> actual;
            actual = target.Inschrijven(inschrijfInfo, out foutBerichten);
            Assert.AreEqual(foutBerichtenExpected, foutBerichten);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }
    }
}
