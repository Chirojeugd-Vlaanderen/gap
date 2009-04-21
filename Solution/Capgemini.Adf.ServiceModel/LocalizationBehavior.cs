using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.ServiceModel.Dispatcher;
using System.Threading;
using System.Globalization;

namespace Capgemini.Adf.ServiceModel
{
	public class LocalizationBehavior : IClientMessageInspector, IDispatchMessageInspector, IEndpointBehavior
	{
		private const string headerName = "localizationHeader";
		private const string @namespace = "urn:schemas-capgemini-be:localization";

		public void ApplyClientBehavior(ServiceEndpoint endpoint, ClientRuntime clientRuntime)
		{
			clientRuntime.MessageInspectors.Add(this);
		}

		public object BeforeSendRequest(ref Message request, IClientChannel channel)
		{
			request.Headers.Add(MessageHeader.CreateHeader(headerName, @namespace, Thread.CurrentThread.CurrentUICulture.Name));

			return null; // no correlation required
		}

		public void ApplyDispatchBehavior(ServiceEndpoint endpoint, EndpointDispatcher endpointDispatcher)
		{
			endpointDispatcher.DispatchRuntime.MessageInspectors.Add(this);
		}

		public object AfterReceiveRequest(ref Message request, IClientChannel channel, InstanceContext instanceContext)
		{
			var cultureName = request.Headers.GetHeader<string>(headerName, @namespace);
			
			Thread.CurrentThread.CurrentUICulture = CultureInfo.GetCultureInfo(cultureName);
			Thread.CurrentThread.CurrentCulture = CultureInfo.CreateSpecificCulture(cultureName);

			return null; // no correlation required
		}

		public void AfterReceiveReply(ref Message reply, object correlationState) { }
		public void Validate(ServiceEndpoint endpoint) { }
		public void AddBindingParameters(ServiceEndpoint endpoint, BindingParameterCollection bindingParameters) { }
		public void BeforeSendReply(ref Message reply, object correlationState) { }
	}
}