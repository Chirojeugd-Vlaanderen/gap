// <copyright company="Chirojeugd-Vlaanderen vzw">
// Copyright (c) 2007-2010
// Mail naar informatica@chiro.be voor alle info over deze broncode
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Chiro.Gap.Fouten.Exceptions
{
	/// <summary>
	/// Exception die opgeworpen kan worden als er ergens een groepswerkjaar niet klopt.
	/// </summary>
	public class GroepsWerkJaarException : System.Exception, ISerializable
	{
		/// <summary>
		/// 
		/// </summary>
		public GroepsWerkJaarException()
			: base()
		{
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="message"></param>
		public GroepsWerkJaarException(string message)
			: base(message)
		{
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="message"></param>
		/// <param name="inner"></param>
		public GroepsWerkJaarException(string message, Exception inner)
			: base(message, inner)
		{
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="info"></param>
		/// <param name="context"></param>
		public GroepsWerkJaarException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}
	}
}
