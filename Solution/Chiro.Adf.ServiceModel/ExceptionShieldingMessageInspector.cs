using System;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Dispatcher;

namespace Chiro.Adf.ServiceModel
{
    /// <summary>
    /// TODO (#190): Documenteren!
    /// </summary>
	public class ExceptionShieldingMessageInspector : IClientMessageInspector
	{
		private readonly Type[] _knownFaultTypes;

        /// <summary>
        /// TODO (#190): Documenteren
        /// </summary>
        /// <param name="knownFaultTypes"></param>
		public ExceptionShieldingMessageInspector(Type[] knownFaultTypes)
		{
			_knownFaultTypes = knownFaultTypes;
		}

        /// <summary>
        /// TODO (#190): Documenteren!
        /// </summary>
        /// <param name="request"></param>
        /// <param name="channel"></param>
        /// <returns></returns>
		public object BeforeSendRequest(ref Message request, IClientChannel channel)
		{
			return null; // no correlation required
		}

        /// <summary>
        /// TODO (#190): Documenteren!
        /// </summary>
        /// <param name="reply"></param>
        /// <param name="correlationState"></param>
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
        /// TODO (#190): Documenteren!
        /// </summary>
        /// <param name="reply"></param>
        /// <param name="faultType"></param>
        /// <returns></returns>
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