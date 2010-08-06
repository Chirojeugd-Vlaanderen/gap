using System;
using System.Collections.ObjectModel;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.ServiceModel.Dispatcher;

namespace Chiro.Adf.ServiceModel.Extensions
{
    /// <summary>
    /// TODO (#190): Documenteren!
    /// </summary>
    public class CallContextBehavior : Attribute, IServiceBehavior, IEndpointBehavior, IOperationBehavior
    {
        private readonly Type _contextType;

        /// <summary>
        /// TODO (#190): Documenteren
        /// </summary>
        /// <param name="type"></param>
        public CallContextBehavior(Type type)
        {
            _contextType = type;
        }

        void IServiceBehavior.ApplyDispatchBehavior(ServiceDescription serviceDescription, ServiceHostBase serviceHostBase)
        {
            foreach (ChannelDispatcher cd in serviceHostBase.ChannelDispatchers)
                foreach (var ed in cd.Endpoints)
                    foreach (var operation in ed.DispatchRuntime.Operations)
                    {
                        operation.CallContextInitializers.Add(new CallContextInitializer(_contextType));
                    }
        }

        void IEndpointBehavior.ApplyDispatchBehavior(ServiceEndpoint endpoint, EndpointDispatcher endpointDispatcher)
        {
            foreach (var operation in endpointDispatcher.DispatchRuntime.Operations)
            {
                operation.CallContextInitializers.Add(new CallContextInitializer(_contextType));
            }
        }

        void IOperationBehavior.ApplyDispatchBehavior(OperationDescription operationDescription, DispatchOperation dispatchOperation)
        {
            dispatchOperation.CallContextInitializers.Add(new CallContextInitializer(_contextType));
        }

        /// <summary>
        /// TODO (#190): Documenteren!
        /// </summary>
        /// <param name="serviceDescription"></param>
        /// <param name="serviceHostBase"></param>
        public void Validate(ServiceDescription serviceDescription, ServiceHostBase serviceHostBase) { }

        /// <summary>
        /// TODO (#190): Documenteren!
        /// </summary>
        /// <param name="serviceDescription"></param>
        /// <param name="serviceHostBase"></param>
        /// <param name="endpoints"></param>
        /// <param name="bindingParameters"></param>
        public void AddBindingParameters(ServiceDescription serviceDescription, ServiceHostBase serviceHostBase, Collection<ServiceEndpoint> endpoints, BindingParameterCollection bindingParameters) { }

        /// <summary>
        /// TODO (#190): Documenteren!
        /// </summary>
        /// <param name="endpoint"></param>
        public void Validate(ServiceEndpoint endpoint) { }

        /// <summary>
        /// TODO (#190): Documenteren!
        /// </summary>
        /// <param name="endpoint"></param>
        /// <param name="bindingParameters"></param>
        public void AddBindingParameters(ServiceEndpoint endpoint, BindingParameterCollection bindingParameters) { }

        /// <summary>
        /// TODO (#190): Documenteren!
        /// </summary>
        /// <param name="endpoint"></param>
        /// <param name="clientRuntime"></param>
        public void ApplyClientBehavior(ServiceEndpoint endpoint, ClientRuntime clientRuntime) { }

        /// <summary>
        /// TODO (#190): Documenteren!
        /// </summary>
        /// <param name="operationDescription"></param>
        public void Validate(OperationDescription operationDescription) { }

        /// <summary>
        /// TODO (#190): Documenteren!
        /// </summary>
        /// <param name="operationDescription"></param>
        /// <param name="clientOperation"></param>
        public void ApplyClientBehavior(OperationDescription operationDescription, ClientOperation clientOperation) { }

        /// <summary>
        /// TODO (#190): Documenteren!
        /// </summary>
        /// <param name="operationDescription"></param>
        /// <param name="bindingParameters"></param>
        public void AddBindingParameters(OperationDescription operationDescription, BindingParameterCollection bindingParameters) { }
    }
}