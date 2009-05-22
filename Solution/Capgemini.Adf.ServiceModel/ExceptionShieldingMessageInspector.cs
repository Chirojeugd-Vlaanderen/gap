using System;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Dispatcher;

namespace Cg2.Adf.ServiceModel
{
	public class ExceptionShieldingMessageInspector : IClientMessageInspector
	{
		private readonly Type[] knownFaultTypes;

		public ExceptionShieldingMessageInspector(Type[] knownFaultTypes)
		{
			this.knownFaultTypes = knownFaultTypes;
		}

		public object BeforeSendRequest(ref Message request, IClientChannel channel)
		{
			return null; // no correlation required
		}

		public void AfterReceiveReply(ref Message reply, object correlationState)
		{
			if (!reply.IsFault) return;

			var action = reply.Headers.Action;
			var faultType = knownFaultTypes.FirstOrDefault(t => t.Name == action);

			if (faultType != null)
			{
				var detail = ReadFaultDetail(MessageFault.CreateFault(reply, int.MaxValue), faultType);
				var exceptionType = typeof(FaultException<>).MakeGenericType(faultType);
				var faultException = Activator.CreateInstance(exceptionType, detail, "Server exception has been shielded.") as Exception;

				throw faultException;
			}
		}

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