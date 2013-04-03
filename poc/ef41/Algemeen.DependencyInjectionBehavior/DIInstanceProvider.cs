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
using System.Collections.Generic;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Dispatcher;
using Algemeen.IoC;

namespace Algemeen.DependencyInjectionBehavior
{
	/// <summary>
	/// Dependency injection instance provider. 
	/// This kind of behavior controls the lifecycle of a WCF service instance, so it is the best place
	/// to inject the service dependencies.
	/// </summary>
	public class DIInstanceProvider : IInstanceProvider
	{
		private readonly Type _serviceType;
// ReSharper disable UnaccessedField.Local
		List<TypeMapping> _typeMappings;
// ReSharper restore UnaccessedField.Local

		/// <summary>
		/// Instantieert een DIInstanceProvider-object
		/// </summary>
		/// <param name="serviceType">Service implementation type</param>
		/// <param name="typeMappings">Type mappings</param>
		public DIInstanceProvider(Type serviceType, List<TypeMapping> typeMappings)
		{
			_serviceType = serviceType;
			_typeMappings = typeMappings;
		}

		/// <summary>
		/// Gets a fresh service instance
		/// </summary>
		/// <param name="instanceContext"></param>
		/// <returns></returns>
		public object GetInstance(InstanceContext instanceContext)
		{
			return GetInstance(instanceContext, null);
		}

		/// <summary>
		/// Gets a fresh service instance
		/// </summary>
		/// <param name="instanceContext"></param>
		/// <param name="message"></param>
		/// <returns></returns>
		public object GetInstance(InstanceContext instanceContext, Message message)
		{
			// DependencyContainer container = new DependencyContainer();
			// foreach (TypeMapping typeMapping in this.typeMappings)
			// {
			//    container.RegisterTypeMapping(typeMapping.TypeRequested, typeMapping.TypeToBuild);
			// }

			return Factory.Maak(_serviceType);
		}

		/// <summary>
		/// Releases the specified service instance
		/// </summary>
		/// <param name="instanceContext"></param>
		/// <param name="instance"></param>
		public void ReleaseInstance(InstanceContext instanceContext, object instance)
		{
		}
	}
}