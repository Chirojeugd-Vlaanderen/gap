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
    public class DummyServiceProvider: IServiceProvider
    {
        private readonly ISyncPersoonService _syncPersoonService;

        public DummyServiceProvider(ISyncPersoonService syncPersoonService)
        {
            _syncPersoonService = syncPersoonService;
        }

        public I GetService<I>() where I : class
        {
            if (typeof(I) == typeof(ISyncPersoonService))
            {
                return _syncPersoonService as I;
            }
            else throw new System.NotImplementedException();
        }

        public I GetService<I>(string instanceName) where I : class
        {
            return GetService<I>();
        }

        public bool TryGetService<I>(out I service) where I : class
        {
            service = GetService<I>();
            return (service != null);
        }
    }
}
