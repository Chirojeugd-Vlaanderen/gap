using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.ServiceModel;

using Chiro.Ad.ServiceContracts;
using Chiro.Adf.ServiceModel;

namespace Chiro.Ad.Test
{
    // Testmogelijkheden zijn momenteel beperkt, omdat er nog geen IOC ondersteund wordt.

    [TestClass]
    public class UnitTest1
    {
        /// <summary>
        /// Een zeer domme test om te kijken of de WCF-service iets doet.
        /// Probeer een account aan te maken met een ongeldig adres, en verwacht een exception.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(FaultException))]
        public void ServiceAanroepTest()
        {
            ServiceHelper.CallService<IAdService, string>(client => client.GapLoginAanvragen(39198, "Johan", "Vervloet", "johan.vervloet@chiro"));
        }
    }
}
