using System;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Dispatcher;

namespace Chiro.Adf.ServiceModel
{
    /// <summary>
    /// TODO (#190): Documenteren!
    /// </summary>
	public class ExceptionShieldingErrorHandler : IErrorHandler
	{
		private readonly Type[] _knownFaultTypes;
		private readonly Type[] _exceptionTypes;

        /// <summary>
        /// TODO (#190): Documenteren!
        /// </summary>
        /// <param name="knownFaultTypes"></param>
        /// <param name="exceptionTypes"></param>
		public ExceptionShieldingErrorHandler(Type[] knownFaultTypes, Type[] exceptionTypes)
		{
			_knownFaultTypes = knownFaultTypes;
			_exceptionTypes = exceptionTypes;
		}

        /// <summary>
        /// TODO (#190): Documenteren!
        /// </summary>
        /// <param name="error"></param>
        /// <returns></returns>
		public bool HandleError(Exception error)
		{
			return true; // session should not be killed, or should it?
		}

        /// <summary>
        /// TODO (#190): Documenteren!
        /// </summary>
        /// <param name="error"></param>
        /// <param name="version"></param>
        /// <param name="fault"></param>
		public void ProvideFault(Exception error, MessageVersion version, ref Message fault)
		{
			if (error is FaultException) return;

			var exceptionType = _exceptionTypes.FirstOrDefault(t => error.GetType() == t || error.GetType().IsSubclassOf(t));
			if (exceptionType != null)
			{
				var faultType = _knownFaultTypes[Array.IndexOf(_exceptionTypes, exceptionType)];
				fault = Message.CreateMessage(version, CreateFaultException(faultType, error).CreateMessageFault(), faultType.Name);
			}
		}

        /// <summary>
        /// TODO (#190): Documenteren!
        /// </summary>
        /// <param name="faultType"></param>
        /// <param name="exception"></param>
        /// <returns></returns>
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