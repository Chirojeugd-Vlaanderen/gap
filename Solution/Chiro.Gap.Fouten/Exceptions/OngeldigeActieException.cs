// <copyright company="Chirojeugd-Vlaanderen vzw">
// Copyright (c) 2007-2010
// Mail naar informatica@chiro.be voor alle info over deze broncode
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Chiro.Gap.Fouten.Exceptions
{
	/// <summary>
	/// Exceptie voor onbekende subgemeente
	/// </summary>
	public class OngeldigeActieException : System.Exception, ISerializable
	{
		/// <summary>
		/// 
		/// </summary>
		public OngeldigeActieException()
			: base()
		{
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="message"></param>
		public OngeldigeActieException(string message)
			: base(message)
		{
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="message"></param>
		/// <param name="inner"></param>
		public OngeldigeActieException(string message, Exception inner)
			: base(message, inner)
		{
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="info"></param>
		/// <param name="context"></param>
		public OngeldigeActieException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}
	}
}
