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
	/// Exceptie voor operatie op objecten waarvoor de
	/// gebruiker geen GAV-rechten heeft.
	/// </summary>
	public class GeenGavException : System.Exception, ISerializable
	{
		/// <summary>
		/// 
		/// </summary>
		public GeenGavException()
			: base()
		{
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="message"></param>
		public GeenGavException(string message)
			: base(message)
		{
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="message"></param>
		/// <param name="inner"></param>
		public GeenGavException(string message, Exception inner)
			: base(message, inner)
		{
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="info"></param>
		/// <param name="context"></param>
		public GeenGavException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}
	}
}
