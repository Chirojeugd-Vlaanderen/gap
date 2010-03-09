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
	/// Exceptie voor fouten tegen validatieregels.
	/// </summary>
	public class ValidatieException : System.Exception, ISerializable
	{
		/// <summary>
		/// Instantieert een lege ValidatieException
		/// </summary>
		public ValidatieException()
			: base()
		{
		}

		/// <summary>
		/// Instantieert een ValidatieException met een opgegeven foutboodschap
		/// </summary>
		/// <param name="message">De foutboodschap die doorgegeven moet worden</param>
		public ValidatieException(string message)
			: base(message)
		{
		}

		/// <summary>
		/// Instantieert een ValidatieException met een opgegeven foutboodschap en 'inner exception'
		/// </summary>
		/// <param name="message"></param>
		/// <param name="inner"></param>
		public ValidatieException(string message, Exception inner)
			: base(message, inner)
		{
		}

		/// <summary>
		/// Instantieert een ValidatieException met een opgegeven SerializationInfo en StreamingContext
		/// </summary>
		/// <param name="info"></param>
		/// <param name="context"></param>
		public ValidatieException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}
	}
}
