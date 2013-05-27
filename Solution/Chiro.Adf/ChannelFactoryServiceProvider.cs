/*
 * Copyright 2008-2013 the GAP developers. See the NOTICE file at the 
 * top-level directory of this distribution, and at
 * https://develop.chiro.be/gap/wiki/copyright
 * 
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 * 
 *     http://www.apache.org/licenses/LICENSE-2.0
 * 
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */
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
        /// Gebruikt WCF om een proxy naar de service met interface I te instantieren, volgens een in de config benoemd endpoint
        /// </summary>
        /// <typeparam name="I">Service interface</typeparam>
        /// <returns>Een instantie van <typeparamref name="I"/>.  Als die niet geresolved kon worden, dan <c>null</c></returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope")] // Disposing gebeurt verder in <cref>Chiro.Adf.ServiceModel</cref>
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
        /// Probe for the service, and return it when found
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
        /// Niet-generische versie van  <see cref="TryGetService&lt;I&gt;" />
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