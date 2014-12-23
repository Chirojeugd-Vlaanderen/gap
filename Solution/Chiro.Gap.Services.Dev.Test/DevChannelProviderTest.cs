using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Chiro.Cdf.Ioc;
using Chiro.Cdf.ServiceHelper;

namespace Chiro.Gap.Services.Dev.Test
{
    [TestClass]
    public class DevChannelProviderTest
    {
        /// <summary>
        /// Run code before running each test
        /// </summary>
        [TestInitialize]
        public void MyTestInitialize()
        {
            Factory.ContainerInit();
        }

        [TestMethod]
        public void CreateDevChannelProviderTest()
        {
            var instantie = Factory.Maak<IChannelProvider>();

            Assert.IsInstanceOfType(instantie, typeof(DevChannelProvider));
        }
    }
}
