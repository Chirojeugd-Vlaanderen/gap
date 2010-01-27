using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using Chiro.Gap.ServiceContracts.FaultContracts;

namespace Chiro.Gap.Fouten.Exceptions
{
	/// <summary>
	/// Exceptie voor onbekende subgemeente
	/// </summary>
	public class BestaatAlException : System.Exception, ISerializable
	{
		/// <summary>
		/// AdresFault die informatie bevat over het probleem
		/// </summary>
		private BestaatAlFault _fault = null;

		public BestaatAlException() : base() { }
		public BestaatAlException(string message) : base(message) { }
		public BestaatAlException(string message, Exception inner) : base(message, inner) { }
		public BestaatAlException(SerializationInfo info, StreamingContext context) : base(info, context) { }

		public BestaatAlException(BestaatAlFault fault)
			: base()
		{
			_fault = fault;
		}

		public BestaatAlFault Fault
		{
			get { return _fault; }
			set { _fault = value; }
		}
	}
}
