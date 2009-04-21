using System;
using System.Collections.ObjectModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Dispatcher;
using System.ServiceModel.Description;
using System.ServiceModel;
using Capgemini.Adf.ServiceModel.Extensions;

namespace Capgemini.Adf.ServiceModel.Extensions
{
public class CallContextBehavior : Attribute, IServiceBehavior, IEndpointBehavior, IOperationBehavior
{
	private readonly Type contextType;

	public CallContextBehavior(Type type)
	{
		contextType = type;
	}

	void IServiceBehavior.ApplyDispatchBehavior(ServiceDescription serviceDescription, ServiceHostBase serviceHostBase)
	{
		foreach (ChannelDispatcher cd in serviceHostBase.ChannelDispatchers)
		foreach (var ed in cd.Endpoints)
		foreach (var operation in ed.DispatchRuntime.Operations)
		{
			operation.CallContextInitializers.Add(new CallContextInitializer(contextType));
		}
	}

	void IEndpointBehavior.ApplyDispatchBehavior(ServiceEndpoint endpoint, EndpointDispatcher endpointDispatcher)
	{
		foreach (var operation in endpointDispatcher.DispatchRuntime.Operations)
		{
			operation.CallContextInitializers.Add(new CallContextInitializer(contextType));
		}
	}

	void IOperationBehavior.ApplyDispatchBehavior(OperationDescription operationDescription, DispatchOperation dispatchOperation)
	{
		dispatchOperation.CallContextInitializers.Add(new CallContextInitializer(contextType));
	}

	public void Validate(ServiceDescription serviceDescription, ServiceHostBase serviceHostBase) { }
	public void AddBindingParameters(ServiceDescription serviceDescription, ServiceHostBase serviceHostBase, Collection<ServiceEndpoint> endpoints, BindingParameterCollection bindingParameters) { }
	
	public void Validate(ServiceEndpoint endpoint) {  }
	public void AddBindingParameters(ServiceEndpoint endpoint, BindingParameterCollection bindingParameters) { }
	public void ApplyClientBehavior(ServiceEndpoint endpoint, ClientRuntime clientRuntime) {  }
	
	public void Validate(OperationDescription operationDescription) { }
    public void ApplyClientBehavior(OperationDescription operationDescription, ClientOperation clientOperation) { }
    public void AddBindingParameters(OperationDescription operationDescription, BindingParameterCollection bindingParameters) { }
}
}