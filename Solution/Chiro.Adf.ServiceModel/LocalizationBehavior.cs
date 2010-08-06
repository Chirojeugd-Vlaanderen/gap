using System.Globalization;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.ServiceModel.Dispatcher;
using System.Threading;

namespace Chiro.Adf.ServiceModel
{
    /// <summary>
    /// TODO (#190): Documenteren!
    /// </summary>
	public class LocalizationBehavior : IClientMessageInspector, IDispatchMessageInspector, IEndpointBehavior
	{
		private const string HEADER_NAME = "localizationHeader";
		private const string NAMESPACE = "urn:schemas-capgemini-be:localization";

        /// <summary>
        /// TODO (#190): Documenteren!
        /// </summary>
        /// <param name="endpoint"></param>
        /// <param name="clientRuntime"></param>
		public void ApplyClientBehavior(ServiceEndpoint endpoint, ClientRuntime clientRuntime)
		{
			clientRuntime.MessageInspectors.Add(this);
		}

        /// <summary>
        /// TODO (#190): Documenteren!
        /// </summary>
        /// <param name="request"></param>
        /// <param name="channel"></param>
        /// <returns></returns>
		public object BeforeSendRequest(ref Message request, IClientChannel channel)
		{
			request.Headers.Add(MessageHeader.CreateHeader(HEADER_NAME, NAMESPACE, Thread.CurrentThread.CurrentUICulture.Name));

			return null; // no correlation required
		}

        /// <summary>
        /// TODO (#190): Documenteren!
        /// </summary>
        /// <param name="endpoint"></param>
        /// <param name="endpointDispatcher"></param>
		public void ApplyDispatchBehavior(ServiceEndpoint endpoint, EndpointDispatcher endpointDispatcher)
		{
			endpointDispatcher.DispatchRuntime.MessageInspectors.Add(this);
		}

        /// <summary>
        /// TODO (#190): Documenteren!
        /// </summary>
        /// <param name="request"></param>
        /// <param name="channel"></param>
        /// <param name="instanceContext"></param>
        /// <returns></returns>
		public object AfterReceiveRequest(ref Message request, IClientChannel channel, InstanceContext instanceContext)
		{
			var cultureName = request.Headers.GetHeader<string>(HEADER_NAME, NAMESPACE);
			
			Thread.CurrentThread.CurrentUICulture = CultureInfo.GetCultureInfo(cultureName);
			Thread.CurrentThread.CurrentCulture = CultureInfo.CreateSpecificCulture(cultureName);

			return null; // no correlation required
		}

        /// <summary>
        /// TODO (#190): Documenteren!
        /// </summary>
        /// <param name="reply"></param>
        /// <param name="correlationState"></param>
		public void AfterReceiveReply(ref Message reply, object correlationState) { }

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
        /// TODO (#190): Documenteren1
        /// </summary>
        /// <param name="reply"></param>
        /// <param name="correlationState"></param>
		public void BeforeSendReply(ref Message reply, object correlationState) { }
	}
}