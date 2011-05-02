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
    [TestClass()]
    public class LedenSyncTest
    {
        private TestContext testContextInstance;

        /// <summary>
        /// Gets or sets the test context which provides
        /// information about and functionality for the current test run.
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

        [ClassInitialize]
        static public void InitialiseerTests(TestContext tc)
        {
            Factory.ContainerInit();
            MappingHelper.MappingsDefinieren();
        }

        [ClassCleanup]
        static public void AfsluitenTests()
        {
            Factory.Dispose();
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
