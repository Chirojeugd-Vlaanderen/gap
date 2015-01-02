using Chiro.Ad.ServiceContracts;
using Chiro.Cdf.ServiceHelper;
using Chiro.Kip.ServiceContracts;
using Moq;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Chiro.Gap.Services.Dev
{
    /// <summary>
    /// De DevChannelProvider vermijdt dat Kipadmin of Active Directory worden aangesproken tijdens
    /// het ontwikkelen/debuggen/testen.
    /// </summary>
    /// <remarks>Uiteraard mag deze niet gebruikt worden in de live omgeving!</remarks>
    public class DevChannelProvider: IChannelProvider
    {
        public I GetChannel<I>() where I : class
        {
            if (typeof(I) == typeof(IAdService))
            {
                // De AdServiceMock genereert pseude-gebruikersnamen.
                return new AdServiceMock() as I;
            }
            if (typeof(I) == typeof(ISyncPersoonService))
            {
                // Deze mock doet niets. Dat is ook niet nodig, want we zijn een message queue.
                return new Mock<ISyncPersoonService>().Object as I;
            }
            // We verwachten hier geen andere services. In principe zou ik die generiek kunnen
            // mocken, maar ik gooi hier een exception, opdat we zouden merken dat er iets mis is.
            throw new NotImplementedException();
        }

        public I GetChannel<I>(string instanceName) where I : class
        {
            return GetChannel<I>();
        }

        public bool TryGetChannel<I>(out I channel) where I : class
        {
            channel = GetChannel<I>();
            return (channel != null);
        }
    }
}
