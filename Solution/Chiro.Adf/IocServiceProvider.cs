using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Chiro.Cdf.Ioc;

namespace Chiro.Adf
{
    /// <summary>
    /// Service provider die IOC gebruikt om implementaties van de service-interface te genereren.
    /// </summary>
    public class IocServiceProvider: IServiceProvider
    {
        public I GetService<I>() where I : class
        {
            return Factory.Maak<I>();
        }

        public I GetService<I>(object arguments) where I : class
        {
            throw new NotImplementedException();
        }

        public I GetService<I>(string instanceName) where I : class
        {
            throw new NotImplementedException();
        }

        public I GetService<I>(string instanceName, object arguments) where I : class
        {
            throw new NotImplementedException();
        }

        public IEnumerable<I> GetServices<I>() where I : class
        {
            throw new NotImplementedException();
        }

        public object GetService(Type serviceType)
        {
            throw new NotImplementedException();
        }

        public object GetService(Type type, string instanceName)
        {
            throw new NotImplementedException();
        }

        public IEnumerable GetServices(Type serviceType)
        {
            throw new NotImplementedException();
        }

        public bool TryGetService<I>(out I service) where I : class
        {
            throw new NotImplementedException();
        }

        public bool TryGetService(Type type, out object service)
        {
            throw new NotImplementedException();
        }
    }
}
