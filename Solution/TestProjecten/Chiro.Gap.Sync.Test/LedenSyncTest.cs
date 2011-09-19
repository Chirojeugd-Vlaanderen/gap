// <copyright company="Chirojeugd-Vlaanderen vzw">
// Copyright (c) 2007-2011
// Mail naar informatica@chiro.be voor alle info over deze broncode
// </copyright>

using System.Collections.Generic;
using Chiro.Cdf.Ioc;
using Chiro.Gap.Dummies;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Chiro.Gap.Sync.SyncService;
using Moq;
using Persoon = Chiro.Gap.Sync.SyncService.Persoon;

namespace Chiro.Gap.Sync.Test
{
    /// <summary>
    /// Tests voor de ledensync
    /// </summary>
    [TestClass]
    public class LedenSyncTest
    {
        /// <summary>
        /// Gets or sets the test context which provides
        /// information about and functionality for the current test run.
        /// </summary>
        public TestContext TestContext { get; set; }

        [ClassInitialize]
        static public void InitialiseerTests(TestContext tc)
        {
            Factory.ContainerInit();
            MappingHelper.MappingsDefinieren();
        }

        [ClassCleanup]
        static public void AfsluitenTests()
        {
            
        }

        /// <summary>
        /// Test voor syncen van geüpdatete kadertests
        /// </summary>
        [TestMethod]
        public void FunctiesUpdatenKaderTest()
        {
            var data = new DummyData();

            // Verwacht dat de syncservice wordt aangeroepen met functie-id's
            var svcMock = new Mock<ISyncPersoonService>();
            svcMock.Setup(
                svc =>
                svc.FunctiesUpdaten(It.IsAny<Persoon>(),
                                    data.DummyGewest.Code,
                                    data.GwjGewest.WerkJaar,
                                    It.IsAny<List<FunctieEnum>>())).Verifiable();
            Factory.InstantieRegistreren(svcMock.Object);

            // Maak ledensync

            var target = Factory.Maak<LedenSync>();

            target.FunctiesUpdaten(data.KaderJos);

            svcMock.VerifyAll();
        }
    }
}
