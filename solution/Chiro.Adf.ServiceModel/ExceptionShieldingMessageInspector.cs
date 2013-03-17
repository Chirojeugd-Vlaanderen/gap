using System;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Dispatcher;

namespace Chiro.Adf.ServiceModel
{

    /// <summary>
    /// 
    /// </summary>
    /// <remarks></remarks>
	public class ExceptionShieldingMessageInspector : IClientMessageInspector
	{
        /// <summary>
        /// 
        /// </summary>
        private readonly Type[] _knownFaultTypes;

        /// <summary>
        /// Initializes a new instance of the <see cref="ExceptionShieldingMessageInspector"/> class.
        /// </summary>
        /// <param name="knownFaultTypes">The known fault types.</param>
        /// <remarks></remarks>
		public ExceptionShieldingMessageInspector(Type[] knownFaultTypes)
		{
			_knownFaultTypes = knownFaultTypes;
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
			return null; // no correlation required
		}


        /// <summary>
        /// Enables inspection or modification of a message after a reply message is received but prior to passing it back to the client application.
        /// </summary>
        /// <param name="reply">The message to be transformed into types and handed back to the client application.</param>
        /// <param name="correlationState">Correlation state data.</param>
        /// <remarks></remarks>
		public void AfterReceiveReply(ref Message reply, object correlationState)
		{
			if (!reply.IsFault) return;

			var action = reply.Headers.Action;
			var faultType = _knownFaultTypes.FirstOrDefault(t => t.Name == action);

			if (faultType != null)
			{
				var detail = ReadFaultDetail(MessageFault.CreateFault(reply, int.MaxValue), faultType);
				var exceptionType = typeof(FaultException<>).MakeGenericType(faultType);
				var faultException = Activator.CreateInstance(exceptionType, detail, "Server exception has been shielded.") as Exception;

				if (faultException != null)
				{
					throw faultException;
				}
			}
		}


        /// <summary>
        /// Reads the fault detail.
        /// </summary>
        /// <param name="reply">The reply.</param>
        /// <param name="faultType">Type of the fault.</param>
        /// <returns></returns>
        /// <remarks></remarks>
		private static object ReadFaultDetail(MessageFault reply, Type faultType)
		{
			using (var reader = reply.GetReaderAtDetailContents())
			{
				var serializer = new DataContractSerializer(faultType);
				return serializer.ReadObject(reader);
			}
		}
	}
}