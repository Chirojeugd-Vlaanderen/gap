/*
 * Copyright 2008-2013 the GAP developers. See the NOTICE file at the 
 * top-level directory of this distribution, and at
 * https://develop.chiro.be/gap/wiki/copyright
 * 
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 * 
 *     http://www.apache.org/licenses/LICENSE-2.0
 * 
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */
using System;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.ServiceModel.Dispatcher;

namespace Chiro.Adf.ServiceModel
{
	/// <summary>
	/// Makes sure that expected or unexpected exceptions are properly shielded from the client and translated to SOAP faults.
	/// </summary>
	public class ShieldExceptionsAttribute : Attribute, IContractBehavior
	{
		private readonly Type[] _knownFaultTypes;
		private readonly Type[] _exceptionTypes;

		/// <summary>
		/// Creates a new instance of the ShieldExceptionsAttribute class.
		/// </summary>
		/// <param name="exceptionTypes">An array of possible types of exceptions to shield.</param>
		/// <param name="knownFaultTypes">An array of corresponding fault types to which the exceptions should be translated.</param>
		public ShieldExceptionsAttribute(Type[] exceptionTypes, Type[] knownFaultTypes)
		{
			this._knownFaultTypes = knownFaultTypes;
			this._exceptionTypes = exceptionTypes;
		}

		///<summary>
		/// Adds the specified fault contracts to the service contract.
		/// </summary>
		public void AddBindingParameters(ContractDescription contractDescription, ServiceEndpoint endpoint, BindingParameterCollection bindingParameters)
		{
			foreach (var op in contractDescription.Operations)
				foreach (var knownFaultType in this._knownFaultTypes)
				{
					// Add fault contract if it is not yet present
					var type = knownFaultType;
					if (op.Faults.All(f => f.DetailType != type))
						op.Faults.Add(new FaultDescription(knownFaultType.Name) { DetailType = knownFaultType, Name = knownFaultType.Name });
				}
		}

		///<summary>
		/// Adds the ExceptionShieldingMessageInspector to the client runtime.
		/// </summary>
		public void ApplyClientBehavior(ContractDescription contractDescription, ServiceEndpoint endpoint, ClientRuntime clientRuntime)
		{
			clientRuntime.MessageInspectors.Add(new ExceptionShieldingMessageInspector(this._knownFaultTypes));
		}

		/// <summary>
		/// Adds the ExceptionShieldingErrorHandler to the channel dispatcher.
		/// </summary>
		public void ApplyDispatchBehavior(ContractDescription contractDescription, ServiceEndpoint endpoint, DispatchRuntime dispatchRuntime)
		{
			dispatchRuntime.ChannelDispatcher.ErrorHandlers.Add(new ExceptionShieldingErrorHandler(this._knownFaultTypes, this._exceptionTypes));
		}

		///<summary>
		/// Ensures the specified exception and fault contract types are valid.
		/// </summary>
		public void Validate(ContractDescription contractDescription, ServiceEndpoint endpoint)
		{
			if (this._knownFaultTypes.Length != this._exceptionTypes.Length)
				throw new ArgumentException("The ShieldExceptions behavior needs a corresponding exception type for each possible fault to shield.");

			var badType = this._knownFaultTypes.FirstOrDefault(t => !t.IsDefined(typeof(DataContractAttribute), true));
			if (badType != null)
				throw new ArgumentException(string.Format("The specified fault '{0}' is no data contract. Did you forget to decorate the class with the DataContractAttirbute attribute?", badType));

			var badExceptionType = this._exceptionTypes.FirstOrDefault(t => t != typeof(Exception) && !t.IsSubclassOf(typeof(Exception)));
			if (badExceptionType != null)
				throw new ArgumentException(string.Format("The specified type '{0}' is not an Exception-derived type.", badExceptionType));
		}
	}
}