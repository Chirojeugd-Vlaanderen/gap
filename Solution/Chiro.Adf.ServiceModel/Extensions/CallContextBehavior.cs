using System;
using System.Collections.ObjectModel;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.ServiceModel.Dispatcher;

namespace Chiro.Adf.ServiceModel.Extensions
{

    /// <summary>
    /// 
    /// </summary>
    /// <remarks></remarks>
    public class CallContextBehavior : Attribute, IServiceBehavior, IEndpointBehavior, IOperationBehavior
    {
        /// <summary>
        /// 
        /// </summary>
        private readonly Type _contextType;

        /// <summary>
        /// Initializes a new instance of the <see cref="CallContextBehavior"/> class.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <remarks></remarks>
        public CallContextBehavior(Type type)
        {
            _contextType = type;
        }

        /// <summary>
        /// Provides the ability to change run-time property values or insert custom extension objects such as error handlers, message or parameter interceptors, security extensions, and other custom extension objects.
        /// </summary>
        /// <param name="serviceDescription">The service description.</param>
        /// <param name="serviceHostBase">The host that is currently being built.</param>
        /// <remarks></remarks>
        void IServiceBehavior.ApplyDispatchBehavior(ServiceDescription serviceDescription, ServiceHostBase serviceHostBase)
        {
            foreach (ChannelDispatcher cd in serviceHostBase.ChannelDispatchers)
                foreach (var ed in cd.Endpoints)
                    foreach (var operation in ed.DispatchRuntime.Operations)
                    {
                        operation.CallContextInitializers.Add(new CallContextInitializer(_contextType));
                    }
        }

        /// <summary>
        /// Implements a modification or extension of the service across an endpoint.
        /// </summary>
        /// <param name="endpoint">The endpoint that exposes the contract.</param>
        /// <param name="endpointDispatcher">The endpoint dispatcher to be modified or extended.</param>
        /// <remarks></remarks>
        void IEndpointBehavior.ApplyDispatchBehavior(ServiceEndpoint endpoint, EndpointDispatcher endpointDispatcher)
        {
            foreach (var operation in endpointDispatcher.DispatchRuntime.Operations)
            {
                operation.CallContextInitializers.Add(new CallContextInitializer(_contextType));
            }
        }

        /// <summary>
        /// Implements a modification or extension of the service across an operation.
        /// </summary>
        /// <param name="operationDescription">The operation being examined. Use for examination only. If the operation description is modified, the results are undefined.</param>
        /// <param name="dispatchOperation">The run-time object that exposes customization properties for the operation described by <paramref name="operationDescription"/>.</param>
        /// <remarks></remarks>
        void IOperationBehavior.ApplyDispatchBehavior(OperationDescription operationDescription, DispatchOperation dispatchOperation)
        {
            dispatchOperation.CallContextInitializers.Add(new CallContextInitializer(_contextType));
        }


        /// <summary>
        /// Provides the ability to inspect the service host and the service description to confirm that the service can run successfully.
        /// </summary>
        /// <param name="serviceDescription">The service description.</param>
        /// <param name="serviceHostBase">The service host that is currently being constructed.</param>
        /// <remarks></remarks>
        public void Validate(ServiceDescription serviceDescription, ServiceHostBase serviceHostBase) { }


        /// <summary>
        /// Provides the ability to pass custom data to binding elements to support the contract implementation.
        /// </summary>
        /// <param name="serviceDescription">The service description of the service.</param>
        /// <param name="serviceHostBase">The host of the service.</param>
        /// <param name="endpoints">The service endpoints.</param>
        /// <param name="bindingParameters">Custom objects to which binding elements have access.</param>
        /// <remarks></remarks>
        public void AddBindingParameters(ServiceDescription serviceDescription, ServiceHostBase serviceHostBase, Collection<ServiceEndpoint> endpoints, BindingParameterCollection bindingParameters) { }


        /// <summary>
        /// Implement to confirm that the endpoint meets some intended criteria.
        /// </summary>
        /// <param name="endpoint">The endpoint to validate.</param>
        /// <remarks></remarks>
        public void Validate(ServiceEndpoint endpoint) { }


        /// <summary>
        /// Implement to pass data at runtime to bindings to support custom behavior.
        /// </summary>
        /// <param name="endpoint">The endpoint to modify.</param>
        /// <param name="bindingParameters">The objects that binding elements require to support the behavior.</param>
        /// <remarks></remarks>
        public void AddBindingParameters(ServiceEndpoint endpoint, BindingParameterCollection bindingParameters) { }


        /// <summary>
        /// Implements a modification or extension of the client across an endpoint.
        /// </summary>
        /// <param name="endpoint">The endpoint that is to be customized.</param>
        /// <param name="clientRuntime">The client runtime to be customized.</param>
        /// <remarks></remarks>
        public void ApplyClientBehavior(ServiceEndpoint endpoint, ClientRuntime clientRuntime) { }


        /// <summary>
        /// Implement to confirm that the operation meets some intended criteria.
        /// </summary>
        /// <param name="operationDescription">The operation being examined. Use for examination only. If the operation description is modified, the results are undefined.</param>
        /// <remarks></remarks>
        public void Validate(OperationDescription operationDescription) { }


        /// <summary>
        /// Implements a modification or extension of the client across an operation.
        /// </summary>
        /// <param name="operationDescription">The operation being examined. Use for examination only. If the operation description is modified, the results are undefined.</param>
        /// <param name="clientOperation">The run-time object that exposes customization properties for the operation described by <paramref name="operationDescription"/>.</param>
        /// <remarks></remarks>
        public void ApplyClientBehavior(OperationDescription operationDescription, ClientOperation clientOperation) { }


        /// <summary>
        /// Implement to pass data at runtime to bindings to support custom behavior.
        /// </summary>
        /// <param name="operationDescription">The operation being examined. Use for examination only. If the operation description is modified, the results are undefined.</param>
        /// <param name="bindingParameters">The collection of objects that binding elements require to support the behavior.</param>
        /// <remarks></remarks>
        public void AddBindingParameters(OperationDescription operationDescription, BindingParameterCollection bindingParameters) { }
    }
}