/*
 * Copyright 2008 Capgemini - Accelerated Delivery Framework - http://www.be.capgemini.com/
 * Copyright 2008-2014 the GAP developers. See the NOTICE file at the 
 * top-level directory of this distribution, and at
 * https://gapwiki.chiro.be/copyright
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

namespace Chiro.Cdf.ServiceHelper
{
	/// <summary>
	/// Resolves WCF service implementations.
	/// </summary>
	public class ChannelFactoryChannelProvider : IChannelProvider
	{
		/// <summary>
		/// Creates the service proxy for the default endpoint of interface I.
		/// </summary>
		/// <typeparam name="I"></typeparam>
		/// <returns></returns>
		public I GetChannel<I>() where I : class
		{
			return GetChannel<I>(string.Empty); // Use the default endpoint from the config (emtpy string, not null!)
		}

        /// <summary>
        /// Gebruikt WCF om een proxy naar de service met interface I te instantieren, volgens een in de config benoemd endpoint
        /// </summary>
        /// <typeparam name="I">Service interface</typeparam>
        /// <returns>Een instantie van <typeparamref name="I"/>.  Als die niet geresolved kon worden, dan <c>null</c></returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope")]
        // Disposen gebeurt in Chiro.Cdf.ServiceHelper.ServiceHelper
        public I GetChannel<I>(string instanceName) where I : class
		{
			if (typeof(I).IsDefined(typeof(ServiceContractAttribute), true))
			{
                ChannelFactory<I> factory;
                // If an exception occurs, just throw.
                factory = new ChannelFactory<I>(instanceName); // use the named endpoint
			    return factory.CreateChannel();
			}
            throw new InvalidOperationException("Not a ServiceContract.");
		}

        /// <summary>
        /// Probe for the service, and return it when found
        /// </summary>
        /// <typeparam name="I"></typeparam>
        /// <param name="service"></param>
        /// <returns><c>true</c> if a service was found, <c>false</c> otherwise.</returns>
		public bool TryGetChannel<I>(out I service) where I : class
		{
            try
            {
                service = GetChannel<I>();
            }
            catch (InvalidOperationException)
            {
                service = null;
                return false;
            }
			return service != null;
		}
	}
}