// <copyright company="Chirojeugd-Vlaanderen vzw">
// Copyright (c) 2007-2010
// Mail naar informatica@chiro.be voor alle info over deze broncode
// </copyright>

using System;
using System.Collections.Generic;
using System.Text;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Dispatcher;
using Chiro.Cdf.Ioc;

namespace Chiro.Cdf.DependencyInjection
{
	/// <summary>
	/// Dependency injection instance provider. 
	/// This kind of behavior controls the lifecycle of a WCF service instance, so it is the best place
	/// to inject the service dependencies.
	/// </summary>
	public class DIInstanceProvider : IInstanceProvider
	{
		private Type serviceType;
		List<TypeMapping> typeMappings;

		/// <summary>
		/// Instantieert een DIInstanceProvider-object
		/// </summary>
		/// <param name="serviceType">Service implementation type</param>
		/// <param name="typeMappings">Type mappings</param>
		public DIInstanceProvider(Type serviceType, List<TypeMapping> typeMappings)
		{
			this.serviceType = serviceType;
			this.typeMappings = typeMappings;
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

			return Factory.Maak(this.serviceType);
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