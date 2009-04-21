using System;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Dispatcher;

namespace Capgemini.Adf.ServiceModel
{
	public class ExceptionShieldingErrorHandler : IErrorHandler
	{
		private readonly Type[] knownFaultTypes;
		private readonly Type[] exceptionTypes;

		public ExceptionShieldingErrorHandler(Type[] knownFaultTypes, Type[] exceptionTypes)
		{
			this.knownFaultTypes = knownFaultTypes;
			this.exceptionTypes = exceptionTypes;
		}

		public bool HandleError(Exception error)
		{
			return true; // session should not be killed, or should it?
		}

		public void ProvideFault(Exception error, MessageVersion version, ref Message fault)
		{
			if (error is FaultException) return;

			var exceptionType = exceptionTypes.FirstOrDefault(t => error.GetType() == t || error.GetType().IsSubclassOf(t));
			if (exceptionType != null)
			{
				var faultType = knownFaultTypes[Array.IndexOf(exceptionTypes, exceptionType)];
				fault = Message.CreateMessage(version, CreateFaultException(faultType, error).CreateMessageFault(), faultType.Name);
			}
		}

		private static FaultException CreateFaultException(Type faultType, Exception exception)
		{
			var ctor = faultType.GetConstructor(new[] { exception.GetType() })   // .ctor with specific exception?
			           ?? faultType.GetConstructor(new[] { typeof(Exception) }); // .ctor with generic exception?

			var detail = ctor != null ? ctor.Invoke(new[] { exception }) : faultType.GetConstructor(Type.EmptyTypes).Invoke(null); // fall back to default .ctor

			// Create generic fault exception with detail and reason
			return Activator.CreateInstance(typeof(FaultException<>).MakeGenericType(faultType), detail, "Unhandled exception has been shielded.") as FaultException;
		}
	}
}