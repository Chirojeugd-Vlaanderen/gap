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
﻿using System;
using System.Collections;
using System.Collections.Generic;

namespace Chiro.Adf
{
	/// <summary>
	/// Defines methods to obtain configuration-based instances of interface implementations.
	/// </summary>
	public interface IServiceProvider
	{
		/// <summary>
		/// Lever een service-implementatie op voor servicecontract <typeparamref name="I"/>.
		/// </summary>
		/// <typeparam name="I">Servicecontract</typeparam>
		/// <returns>Implementatie van het gevraagde service-contract, of <c>null</c> als er geen implementatie
		/// beschikbaar is.</returns>
		I GetService<I>() where I : class;

		/// <summary>
		/// Gets the configure service implementation for the specified interface.
		/// </summary>
		/// <typeparam name="I"></typeparam>
		/// <param name="arguments">.ctor arguments</param>
		/// <returns></returns>
		I GetService<I>(object arguments) where I : class;

		/// <summary>
		/// Gets the configured service implementation instance with the specified name.
		/// </summary>
		/// <typeparam name="I">The interface or type to get the service for.</typeparam>
		/// <param name="instanceName">The name of the service instance.</param>
		/// <returns></returns>
		I GetService<I>(string instanceName) where I : class;

        /// <summary>
        /// Gets the service.
        /// </summary>
        /// <typeparam name="I"></typeparam>
        /// <param name="instanceName">Name of the instance.</param>
        /// <param name="arguments">The arguments.</param>
        /// <returns></returns>
        /// <remarks></remarks>
		I GetService<I>(string instanceName, object arguments) where I : class;
		
		/// <summary>
		/// Gets all configured service instances of the specified interface type.
		/// </summary>
		/// <typeparam name="I"></typeparam>
		/// <returns></returns>
		IEnumerable<I> GetServices<I>() where I : class;
		
		/// <summary>
		/// Gets the configured service implementation for the specified interface.
		/// </summary>
		/// <param name="serviceType">The interface, type to get the service for</param>
		/// <returns></returns>
		object GetService(Type serviceType);

		/// <summary>
		/// Gets the configured service implementation for the specified interface and instance name.
		/// </summary>
		/// <param name="type"></param>
		/// <param name="instanceName"></param>
		/// <returns></returns>
		object GetService(Type type, string instanceName);

		/// <summary>
		/// Gets all service instances for the specified interface.
		/// </summary>
		/// <param name="serviceType"></param>
		/// <returns></returns>
		IEnumerable GetServices(Type serviceType);

		/// <summary>
		/// Try and get the requested service
		/// </summary>
		/// <typeparam name="I"></typeparam>
		/// <param name="service"></param>
		/// <returns></returns>
		bool TryGetService<I>(out I service) where I : class;

        /// <summary>
        /// Tries the get service.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <param name="service">The service.</param>
        /// <returns></returns>
        /// <remarks></remarks>
		bool TryGetService(Type type, out object service);
	}
}
