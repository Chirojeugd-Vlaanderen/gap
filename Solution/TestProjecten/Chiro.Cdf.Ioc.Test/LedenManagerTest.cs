using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
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
        public LedenManagerTest()
        {
            //
            // TODO: Add constructor logic here
            //
        }


        [ClassInitialize]
        static public void InitialiseerTests(TestContext tc)
        {
            Factory.ContainerInit();
        }

        [ClassCleanup]
        static public void AfsluitenTests()
        {
            Factory.Dispose();
        }

        /// <summary>
        /// Test instantieren LedenManager
        /// </summary>
        [TestMethod]
        public void InstantieerLedenManager()
        {
            LedenManager lm = Factory.Maak<LedenManager>();

            // Flauwe test, maar ik wil niet dat de IOC een
            // exception veroorzaakt.

            Assert.IsTrue(lm != null);
        }

    }
}
