/*
 * Copyright 2008-2014 the GAP developers. See the NOTICE file at the 
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
using System.ServiceModel;

namespace Chiro.Cdf.ServiceHelper
{
	/// <summary>
	/// Provides helper methods to call operations on service proxies and making sure the proxy is 
	/// disposed or aborted in case of timeout or communication exceptions.
	/// </summary>
	public class ServiceHelper
	{
        private readonly IServiceProvider _serviceProvider;

        /// <summary>
        /// Creates a new ServiceHelper.
        /// </summary>
        /// <param name="serviceProvider">A service provider that creates service channels.</param>
        public ServiceHelper(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

		/// <summary>
		/// Calls the specified operation on a service instance and safely disposes or aborts the service 
		/// (proxy) in case of Timeout or communication exceptions.
		/// </summary>
		/// <typeparam name="I">The service type</typeparam>
		/// <param name="service">The service instance to perform the opreation on.</param>
		/// <param name="operation">The operation to invoke.</param>
		// ReSharper disable InconsistentNaming
		private void Call<I>(I service, Action<I> operation) where I : class
		// ReSharper restore InconsistentNaming
		{
			if (service == null)
				throw new ArgumentNullException("service");
			if (operation == null)
				throw new ArgumentNullException("operation");

			try
			{
				operation.Invoke(service);
			}
			catch (TimeoutException)
			{
			    var clientChannel = service as IClientChannel;
			    if (clientChannel != null)
			        (clientChannel).Abort();
				throw;
			}
			catch (FaultException)
			{
			    var clientChannel = service as IClientChannel;
			    if (clientChannel != null)
			        (clientChannel).Close();
				throw;
			}
			catch (CommunicationException)
			{
			    var clientChannel = service as IClientChannel;
			    if (clientChannel != null)
			        (clientChannel).Abort();
				throw;
			}
			finally
			{
				DisposeServiceInstance(service);
			}
		}

		/// <summary>
		/// Calls the specified operation on a service instance and safely disposes or aborts the service (proxy) in case of Timeout or communication exceptions.
		/// </summary>
		/// <typeparam name="I">The type of service.</typeparam>
		/// <typeparam name="T">The return value type</typeparam>
		/// <param name="service">The serivice instance to call the operation on.</param>
		/// <param name="operation">The service operation to invoke.</param>
		/// <returns>the return value of the service operation.</returns>
		// ReSharper disable InconsistentNaming
		private T Call<I, T>(I service, Func<I, T> operation) where I : class
		// ReSharper restore InconsistentNaming
		{
			if (service == null)
				throw new ArgumentNullException("service");
			if (operation == null)
				throw new ArgumentNullException("operation");

			try
			{
				return operation.Invoke(service);
			}
			catch (TimeoutException)
			{
				((IClientChannel)service).Abort();
				throw;
			}
			catch (FaultException)
			{
				if (((IClientChannel) service).State == CommunicationState.Faulted)
				{
					((IClientChannel) service).Abort();
				}
				else
				{
					((IClientChannel) service).Close();
				}
				throw;
			}
			catch (CommunicationException)
			{
				((IClientChannel)service).Abort();
				throw;
			}
			finally
			{
				DisposeServiceInstance(service);
			}
		}

		/// <summary>
		/// Gebruik deze method om een method van een service aan te roepen.
		/// </summary>
		/// <typeparam name="I">ServiceContract aan te roepen service</typeparam>
		/// <param name="operation">Lambda-expressie die de vertrekkende van <typeparamref name="I"/>
		/// de gewenste method oproept.</param>
		/// <example>
		/// <c>CallService &lt;IMijnService, string&gt; (svc =&gt; svc.IetsDoen(id))</c>
		/// </example>
		// ReSharper disable InconsistentNaming
		public void CallService<I>(Action<I> operation) where I : class
		// ReSharper restore InconsistentNaming
		{
			Call(_serviceProvider.GetService<I>(), operation);
		}

		/// <summary>
		/// Gebruik deze method om een functie aan te roepen van een service.
		/// </summary>
		/// <typeparam name="I">ServiceContract aan te roepen service</typeparam>
		/// <typeparam name="T">Type van het resultaat van <paramref name="operation"/></typeparam>
		/// <param name="operation">Lambda-expressie die de <typeparamref name="I"/> afbeeldt op het gewenste
		/// resultaat.</param>
		/// <returns>Resultaat van de functie-aanroep</returns>
		/// <example>
		/// <c>string = 
		///	StaticAdfServiceHelper.CallService &lt;IMijnService, string&gt; (svc =&gt; svc.IetsDoen(id))</c>
		/// </example>
		// ReSharper disable InconsistentNaming
		public T CallService<I, T>(Func<I, T> operation) where I : class
		// ReSharper restore InconsistentNaming
		{
			return Call(_serviceProvider.GetService<I>(), operation);
		}

		private void DisposeServiceInstance(object instance)
		{
			var client = instance as IClientChannel;
			if (client == null || client.State == CommunicationState.Closed)
				return;

			try
			{
				client.Close();
			}
			catch
			{
				client.Abort();
			}
		}
	}
}