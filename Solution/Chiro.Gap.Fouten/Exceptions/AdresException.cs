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
	public class AdresException : System.Exception, ISerializable
	{
		private AdresFault _fault = null;

		/// <summary>
		/// 
		/// </summary>
		public AdresException()
			: base()
		{
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="message"></param>
		public AdresException(string message)
			: base(message)
		{
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="message"></param>
		/// <param name="inner"></param>
		public AdresException(string message, Exception inner)
			: base(message, inner)
		{
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="info"></param>
		/// <param name="context"></param>
		public AdresException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="vf"></param>
		public AdresException(AdresFault vf)
			: base()
		{
			_fault = vf;
		}

		/// <summary>
		/// 
		/// </summary>
		public AdresFault Fault
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
