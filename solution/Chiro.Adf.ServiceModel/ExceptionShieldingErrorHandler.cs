using System;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Dispatcher;

namespace Chiro.Adf.ServiceModel
{
    /// <summary>
    /// 
    /// </summary>
    /// <remarks></remarks>
	public class ExceptionShieldingErrorHandler : IErrorHandler
	{
        /// <summary>
        /// 
        /// </summary>
		private readonly Type[] _knownFaultTypes;

        /// <summary>
        /// 
        /// </summary>
		private readonly Type[] _exceptionTypes;


        /// <summary>
        /// Initializes a new instance of the <see cref="ExceptionShieldingErrorHandler"/> class.
        /// </summary>
        /// <param name="knownFaultTypes">The known fault types.</param>
        /// <param name="exceptionTypes">The exception types.</param>
        /// <remarks></remarks>
		public ExceptionShieldingErrorHandler(Type[] knownFaultTypes, Type[] exceptionTypes)
		{
			_knownFaultTypes = knownFaultTypes;
			_exceptionTypes = exceptionTypes;
		}
        
        /// <summary>
        /// Enables error-related processing and returns a value that indicates whether the dispatcher aborts the session and the instance context in certain cases.
        /// </summary>
        /// <param name="error">The exception thrown during processing.</param>
        /// <returns>true if  should not abort the session (if there is one) and instance context if the instance context is not <see cref="F:System.ServiceModel.InstanceContextMode.Single"/>; otherwise, false. The default is false.</returns>
        /// <remarks></remarks>
		public bool HandleError(Exception error)
		{
			return true; // session should not be killed, or should it?
		}


        /// <summary>
        /// Enables the creation of a custom <see cref="T:System.ServiceModel.FaultException`1"/> that is returned from an exception in the course of a service method.
        /// </summary>
        /// <param name="error">The <see cref="T:System.Exception"/> object thrown in the course of the service operation.</param>
        /// <param name="version">The SOAP version of the message.</param>
        /// <param name="fault">The <see cref="T:System.ServiceModel.Channels.Message"/> object that is returned to the client, or service, in the duplex case.</param>
        /// <remarks></remarks>
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
        /// Creates the fault exception.
        /// </summary>
        /// <param name="faultType">Type of the fault.</param>
        /// <param name="exception">The exception.</param>
        /// <returns></returns>
        /// <remarks></remarks>
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