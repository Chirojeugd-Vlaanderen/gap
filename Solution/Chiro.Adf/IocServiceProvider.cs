using System;
using System.Collections;
using System.Collections.Generic;

using Chiro.Cdf.Ioc;

using Microsoft.Practices.Unity;

namespace Chiro.Adf
{
    /// <summary>
    /// Service provider die IOC gebruikt om implementaties van de service-interface te genereren.
    /// </summary>
    /// <remarks></remarks>
    public class IocServiceProvider: IServiceProvider
    {
        /// <summary>
        /// Gebruikt IoC om de service met interface I te instantieren.
        /// </summary>
        /// <typeparam name="I">Service interface</typeparam>
        /// <returns>Een instantie van <typeparamref name="I"/>.  Als die niet geresolved kon worden, dan <c>null</c></returns>
        /// <remarks></remarks>
        public I GetService<I>() where I : class
        {
            try
            {
                return Factory.Maak<I>();
            }
            catch (ResolutionFailedException)
            {
                return null;
            }
        }

        /// <summary>
        /// Gets the configure service implementation for the specified interface.
        /// </summary>
        /// <typeparam name="I"></typeparam>
        /// <param name="arguments">.ctor arguments</param>
        /// <returns></returns>
        /// <remarks></remarks>
        public I GetService<I>(object arguments) where I : class
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Gets the configured service implementation instance with the specified name.
        /// </summary>
        /// <typeparam name="I">The interface or type to get the service for.</typeparam>
        /// <param name="instanceName">The name of the service instance.</param>
        /// <returns></returns>
        /// <remarks></remarks>
        public I GetService<I>(string instanceName) where I : class
        {
            throw new NotImplementedException();
        }


        /// <summary>
        /// Gets the service.
        /// </summary>
        /// <typeparam name="I"></typeparam>
        /// <param name="instanceName">Name of the instance.</param>
        /// <param name="arguments">The arguments.</param>
        /// <returns></returns>
        /// <remarks></remarks>
        public I GetService<I>(string instanceName, object arguments) where I : class
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Gets all configured service instances of the specified interface type.
        /// </summary>
        /// <typeparam name="I"></typeparam>
        /// <returns></returns>
        /// <remarks></remarks>
        public IEnumerable<I> GetServices<I>() where I : class
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Gets the configured service implementation for the specified interface. Most simplified non-generic version.
        /// </summary>
        /// <param name="serviceType">The interface to get the service for</param>
        /// <returns></returns>
        /// <remarks></remarks>
        public object GetService(Type serviceType)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Gets the configured service implementation for the specified interface and instance name.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <param name="instanceName">Name of the instance.</param>
        /// <returns></returns>
        /// <remarks></remarks>
        public object GetService(Type type, string instanceName)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Gets all service instances for the specified interface.
        /// </summary>
        /// <param name="serviceType">Type of the service.</param>
        /// <returns></returns>
        /// <remarks></remarks>
        public IEnumerable GetServices(Type serviceType)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Try and get the requested service
        /// </summary>
        /// <typeparam name="I"></typeparam>
        /// <param name="service">The service.</param>
        /// <returns></returns>
        /// <remarks></remarks>
        public bool TryGetService<I>(out I service) where I : class
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Tries the get service.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <param name="service">The service.</param>
        /// <returns></returns>
        /// <remarks></remarks>
        public bool TryGetService(Type type, out object service)
        {
            throw new NotImplementedException();
        }
    }
}
