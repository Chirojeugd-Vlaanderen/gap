// <copyright company="Chirojeugd-Vlaanderen vzw">
// Copyright (c) 2007-2010
// Mail naar informatica@chiro.be voor alle info over deze broncode
// </copyright>

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.ServiceModel.Dispatcher;

namespace Chiro.Cdf.DependencyInjection
{
	/// <summary>
	/// Dependency injection service behavior. This behavior is only used to hook up the instance
	/// provider in the WCF dispatcher at runtime.
	/// </summary>
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
	public class DIServiceBehavior : Attribute, IServiceBehavior
	{
		List<TypeMapping> typeMappings = new List<TypeMapping>();

		public DIServiceBehavior()
		{
		}

		public DIServiceBehavior(IList<TypeMapping> typeMappings)
		{
			this.typeMappings.AddRange(typeMappings);
		}

		public void ApplyDispatchBehavior(ServiceDescription serviceDescription, ServiceHostBase serviceHostBase)
		{
			foreach (ChannelDispatcherBase cdb in serviceHostBase.ChannelDispatchers)
			{
				var cd = cdb as ChannelDispatcher;

				if (cd != null)
				{
					foreach (EndpointDispatcher ed in cd.Endpoints)
					{
						ed.DispatchRuntime.InstanceProvider =
							new DIInstanceProvider(serviceDescription.ServiceType, typeMappings);
					}
				}
			}
		}

		public void AddBindingParameters(ServiceDescription serviceDescription, ServiceHostBase serviceHostBase, Collection<ServiceEndpoint> endpoints, BindingParameterCollection bindingParameters)
		{
		}

		public void Validate(ServiceDescription serviceDescription, ServiceHostBase serviceHostBase)
		{
		}
	}
}
