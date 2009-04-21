using System;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.ServiceModel.Dispatcher;

namespace Capgemini.Adf.ServiceModel
{
	/// <summary>
	/// Makes sure that expected or unexpected exceptions are properly shielded fro; the client and translated to SOAP faults.
	/// </summary>
	public class ShieldExceptionsAttribute : Attribute, IContractBehavior
	{
		private readonly Type[] knownFaultTypes;
		private readonly Type[] exceptionTypes;

		/// <summary>
		/// Creates a new instance of the ShieldExceptionsAttribute class.
		/// </summary>
		/// <param name="exceptionTypes">An array of possible types of exceptions to shield.</param>
		/// <param name="knownFaultTypes">An array of corresponding fault types to which the exceptions should be translated.</param>
		public ShieldExceptionsAttribute(Type[] exceptionTypes, Type[] knownFaultTypes)
		{
			this.knownFaultTypes = knownFaultTypes;
			this.exceptionTypes = exceptionTypes;
		}

		///<summary>
		/// Adds the specified fault contracts to the service contract.
		///</summary>
		public void AddBindingParameters(ContractDescription contractDescription, ServiceEndpoint endpoint, BindingParameterCollection bindingParameters)
		{
			foreach (var op in contractDescription.Operations)
				foreach (var knownFaultType in knownFaultTypes)
				{
					// Add fault contract if it is not yet present
					if (!op.Faults.Any(f => f.DetailType == knownFaultType))
						op.Faults.Add(new FaultDescription(knownFaultType.Name) { DetailType = knownFaultType, Name = knownFaultType.Name });
				}
		}

		///<summary>
		/// Adds the ExceptionShieldingMessageInspector to the client runtime.
		///</summary>
		public void ApplyClientBehavior(ContractDescription contractDescription, ServiceEndpoint endpoint, ClientRuntime clientRuntime)
		{
			clientRuntime.MessageInspectors.Add(new ExceptionShieldingMessageInspector(knownFaultTypes));
		}

		/// <summary>
		/// Adds the ExceptionShieldingErrorHandler to the channel dispatcher.
		/// </summary>
		public void ApplyDispatchBehavior(ContractDescription contractDescription, ServiceEndpoint endpoint, DispatchRuntime dispatchRuntime)
		{
			dispatchRuntime.ChannelDispatcher.ErrorHandlers.Add(new ExceptionShieldingErrorHandler(knownFaultTypes, exceptionTypes));
		}

		///<summary>
		/// Ensures the specified exception and fault contract types are valid.
		///</summary>
		public void Validate(ContractDescription contractDescription, ServiceEndpoint endpoint)
		{
			if (knownFaultTypes.Length != exceptionTypes.Length)
				throw new ArgumentException("The ShieldExceptions behavior needs a corresponding exception type for each possible fault to shield.");

			var badType = knownFaultTypes.FirstOrDefault(t => !t.IsDefined(typeof(DataContractAttribute), true));
			if (badType != null)
				throw new ArgumentException(string.Format("The specified fault '{0}' is no data contract. Did you forget to decorate the class with the DataContractAttirbute attribute?", badType));

			var badExceptionType = exceptionTypes.FirstOrDefault(t => t != typeof(Exception) && !t.IsSubclassOf(typeof(Exception)));
			if (badExceptionType != null)
				throw new ArgumentException(string.Format("The specified type '{0}' is not an Exception-derived type.", badExceptionType));
		}
	}
}