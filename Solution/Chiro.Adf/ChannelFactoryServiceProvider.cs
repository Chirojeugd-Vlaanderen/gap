using System;
using System.Collections;
using System.Collections.Generic;
using System.ServiceModel;

namespace Chiro.Adf
{
	/// <summary>
	/// Resolves WCF service implementations.
	/// </summary>
	public class ChannelFactoryServiceProvider : IServiceProvider
	{
		/// <summary>
		/// Creates the service proxy for the default endpoint of interface I.
		/// </summary>
		/// <typeparam name="I"></typeparam>
		/// <returns></returns>
		public I GetService<I>() where I : class
		{
			return GetService<I>(string.Empty); // Use the default endpoint from the config (emtpy string, not null!)
		}

		/// <summary>
		/// Throws a NotSupportedException.
		/// </summary>
		/// <typeparam name="I"></typeparam>
		/// <param name="arguments"></param>
		/// <returns></returns>
		public I GetService<I>(object arguments) where I : class { throw new NotSupportedException(); }

        /// <summary>
        /// Gebruikt WCF om een proxy naar de service met interface I te instantieren.
        /// </summary>
        /// <typeparam name="I">Service interface</typeparam>
        /// <returns>Een instantie van <typeparamref name="I"/>.  Als die niet geresolved kon worden, dan <c>null</c></returns>
		public I GetService<I>(string instanceName) where I : class
		{
			if (typeof(I).IsDefined(typeof(ServiceContractAttribute), true))
			{
			    ChannelFactory<I> factory;
                try
                {
                    factory = new ChannelFactory<I>(instanceName); // use the named endpoint
                }
                catch(InvalidOperationException)
                {
                    return null;
                }
			    return factory.CreateChannel();
			}

			return null;
		}

		/// <summary>
		/// Throws a NotSupportedException.
		/// </summary>
		/// <typeparam name="I"></typeparam>
		/// <param name="instanceName"></param>
		/// <param name="arguments"></param>
		/// <returns></returns>
		public I GetService<I>(string instanceName, object arguments) where I : class { throw new NotSupportedException(); }

		/// <summary>
		/// Gets all configured service instances of the specified interface type.
		/// </summary>
		/// <typeparam name="I"></typeparam>
		/// <returns></returns>
		public IEnumerable<I> GetServices<I>() where I : class
		{
			return null; // not supported, only one implementation per interface
		}

		/// <summary>
		/// Gets the configured service implementation for the specified interface.
		/// </summary>
		/// <param name="serviceType">The interface, type to get the service for</param>
		/// <returns></returns>
		public object GetService(Type serviceType) 
		{
			return GetService(serviceType, null);
		}

		/// <summary>
		/// Gets the configured service implementation for the specified interface and instance name.
		/// </summary>
		/// <param name="type"></param>
		/// <param name="instanceName"></param>
		/// <returns></returns>
		public object GetService(Type type, string instanceName)
		{
			// not supported, only one implementation per interface

			if (type.IsDefined(typeof(ServiceContractAttribute), true))
				throw new NotSupportedException();

			return null;
		}

		/// <summary>
		/// Gets all service instances for the specified interface.
		/// </summary>
		/// <param name="serviceType"></param>
		/// <returns></returns>
        public IEnumerable GetServices(Type serviceType)
		{
			// not supported, only one implementation per interface

			if (serviceType.IsDefined(typeof(ServiceContractAttribute), true))
				throw new NotSupportedException();

			return null;
		}

        /// <summary>
        /// TODO (#190): Documenteren
        /// </summary>
        /// <typeparam name="I"></typeparam>
        /// <param name="service"></param>
        /// <returns></returns>
		public bool TryGetService<I>(out I service) where I : class
		{
			service = GetService<I>();
			return service != null;
		}

        /// <summary>
        /// TODO (#190): Documenteren
        /// </summary>
        /// <param name="type"></param>
        /// <param name="service"></param>
        /// <returns></returns>
		public bool TryGetService(Type type, out object service)
		{
			service = GetService(type);
			return service != null;
		}
	}
}