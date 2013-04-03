using Chiro.Gap.WorkerInterfaces;

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Chiro.Gap.Workers;

namespace Chiro.Cdf.Ioc.Test
{
    /// <summary>
    /// Test of een ledenmanager goed wordt aangemaakt
    /// </summary>
    [TestClass]
    public class LedenManagerTest
    {
    	[ClassInitialize]
        static public void InitialiseerTests(TestContext tc)
        {
            Factory.ContainerInit();
        }

        [ClassCleanup]
        static public void AfsluitenTests()
        {
        }

        /// <summary>
        /// Test instantieren LedenManager
        /// </summary>
        [TestMethod]
        public void InstantieerLedenManager()
        {
			var lm = Factory.Maak<LedenManager>();

            // Flauwe test, maar ik wil niet dat de IOC een
            // exception veroorzaakt.

            Assert.IsTrue(lm != null);
        }

    }
}
