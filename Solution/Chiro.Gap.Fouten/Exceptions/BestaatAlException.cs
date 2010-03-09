// <copyright company="Chirojeugd-Vlaanderen vzw">
// Copyright (c) 2007-2010
// Mail naar informatica@chiro.be voor alle info over deze broncode
// </copyright>

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

		/// <summary>
		/// 
		/// </summary>
		public BestaatAlException()
			: base()
		{
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="message"></param>
		public BestaatAlException(string message)
			: base(message)
		{
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="message"></param>
		/// <param name="inner"></param>
		public BestaatAlException(string message, Exception inner)
			: base(message, inner)
		{
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="info"></param>
		/// <param name="context"></param>
		public BestaatAlException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="fault"></param>
		public BestaatAlException(BestaatAlFault fault)
			: base()
		{
			_fault = fault;
		}

		/// <summary>
		/// 
		/// </summary>
		public BestaatAlFault Fault
		{
			get
			{
				return _fault;
			}
			set
			{
				_fault = value;
			}
		}
	}
}
