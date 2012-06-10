using System.Globalization;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.ServiceModel.Dispatcher;
using System.Threading;

namespace Chiro.Adf.ServiceModel
{

    /// <summary>
    /// 
    /// </summary>
    /// <remarks></remarks>
	public class LocalizationBehavior : IClientMessageInspector, IDispatchMessageInspector, IEndpointBehavior
	{
        /// <summary>
        /// 
        /// </summary>
		private const string HEADER_NAME = "localizationHeader";
        /// <summary>
        /// 
        /// </summary>
		private const string NAMESPACE = "urn:schemas-capgemini-be:localization";

        /// <summary>
        /// Implements a modification or extension of the client across an endpoint.
        /// </summary>
        /// <param name="endpoint">The endpoint that is to be customized.</param>
        /// <param name="clientRuntime">The client runtime to be customized.</param>
        /// <remarks></remarks>
		public void ApplyClientBehavior(ServiceEndpoint endpoint, ClientRuntime clientRuntime)
		{
			clientRuntime.MessageInspectors.Add(this);
		}

        /// <summary>
        /// Enables inspection or modification of a message before a request message is sent to a service.
        /// </summary>
        /// <param name="request">The message to be sent to the service.</param>
        /// <param name="channel">The  client object channel.</param>
        /// <returns>The object that is returned as the correlationState argument of the <see cref="M:System.ServiceModel.Dispatcher.IClientMessageInspector.AfterReceiveReply(System.ServiceModel.Channels.Message@,System.Object)"/> method. This is null if no correlation state is used.The best practice is to make this a <see cref="T:System.Guid"/> to ensure that no two correlationState objects are the same.</returns>
        /// <remarks></remarks>
		public object BeforeSendRequest(ref Message request, IClientChannel channel)
		{
			request.Headers.Add(MessageHeader.CreateHeader(HEADER_NAME, NAMESPACE, Thread.CurrentThread.CurrentUICulture.Name));

			return null; // no correlation required
		}

        /// <summary>
        /// Implements a modification or extension of the service across an endpoint.
        /// </summary>
        /// <param name="endpoint">The endpoint that exposes the contract.</param>
        /// <param name="endpointDispatcher">The endpoint dispatcher to be modified or extended.</param>
        /// <remarks></remarks>
		public void ApplyDispatchBehavior(ServiceEndpoint endpoint, EndpointDispatcher endpointDispatcher)
		{
			endpointDispatcher.DispatchRuntime.MessageInspectors.Add(this);
		}

        /// <summary>
        /// Called after an inbound message has been received but before the message is dispatched to the intended operation.
        /// </summary>
        /// <param name="request">The request message.</param>
        /// <param name="channel">The incoming channel.</param>
        /// <param name="instanceContext">The current service instance.</param>
        /// <returns>The object used to correlate state. This object is passed back in the <see cref="M:System.ServiceModel.Dispatcher.IDispatchMessageInspector.BeforeSendReply(System.ServiceModel.Channels.Message@,System.Object)"/> method.</returns>
        /// <remarks></remarks>
		public object AfterReceiveRequest(ref Message request, IClientChannel channel, InstanceContext instanceContext)
		{
			var cultureName = request.Headers.GetHeader<string>(HEADER_NAME, NAMESPACE);
			
			Thread.CurrentThread.CurrentUICulture = CultureInfo.GetCultureInfo(cultureName);
			Thread.CurrentThread.CurrentCulture = CultureInfo.CreateSpecificCulture(cultureName);

			return null; // no correlation required
		}

        /// <summary>
        /// Enables inspection or modification of a message after a reply message is received but prior to passing it back to the client application.
        /// </summary>
        /// <param name="reply">The message to be transformed into types and handed back to the client application.</param>
        /// <param name="correlationState">Correlation state data.</param>
        /// <remarks></remarks>
		public void AfterReceiveReply(ref Message reply, object correlationState) { }

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
        /// Called after the operation has returned but before the reply message is sent.
        /// </summary>
        /// <param name="reply">The reply message. This value is null if the operation is one way.</param>
        /// <param name="correlationState">The correlation object returned from the <see cref="M:System.ServiceModel.Dispatcher.IDispatchMessageInspector.AfterReceiveRequest(System.ServiceModel.Channels.Message@,System.ServiceModel.IClientChannel,System.ServiceModel.InstanceContext)"/> method.</param>
        /// <remarks></remarks>
		public void BeforeSendReply(ref Message reply, object correlationState) { }
	}
}