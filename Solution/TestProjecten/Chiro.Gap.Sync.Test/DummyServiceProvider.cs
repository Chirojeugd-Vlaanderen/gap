using Chiro.Cdf.Ioc;
using Chiro.Cdf.ServiceHelper;
using Chiro.Kip.ServiceContracts;
using Moq;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Chiro.Gap.Sync.Test
{
    /// <summary>
    /// Voor deze tests hebben we geen echte service providers nodig.
    /// </summary>
    public class DummyChannelProvider: IChannelProvider
    {
        private readonly ISyncPersoonService _syncPersoonService;

        public DummyChannelProvider(ISyncPersoonService syncPersoonService)
        {
            _syncPersoonService = syncPersoonService;
        }

        public I GetChannel<I>() where I : class
        {
            if (typeof(I) == typeof(ISyncPersoonService))
            {
                return _syncPersoonService as I;
            }
            else throw new System.NotImplementedException();
        }

        public I GetChannel<I>(string instanceName) where I : class
        {
            return GetChannel<I>();
        }

        public bool TryGetChannel<I>(out I service) where I : class
        {
            service = GetChannel<I>();
            return (service != null);
        }
    }
}
