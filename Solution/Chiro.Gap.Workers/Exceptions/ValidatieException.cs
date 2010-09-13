// <copyright company="Chirojeugd-Vlaanderen vzw">
// Copyright (c) 2007-2010
// Mail naar informatica@chiro.be voor alle info over deze broncode
// </copyright>

using System;
using System.Runtime.Serialization;

using Chiro.Gap.Domain;

namespace Chiro.Gap.Workers.Exceptions
{
	/// <summary>
	/// Exceptie voor fouten tegen validatieregels.
	/// </summary>
	public class ValidatieException : GapException
	{
		/// <summary>
		/// Enumwaarde die meer info geeft over de aard van de exceptie
		/// </summary>
		public FoutNummer FoutNummer { get; set; }

		/// <summary>
		/// Instantieert een lege ValidatieException
		/// </summary>
		public ValidatieException()
		{
		}

		/// <summary>
		/// Instantieert een ValidatieException met een opgegeven foutboodschap
		/// </summary>
		/// <param name="message">De foutboodschap die doorgegeven moet worden</param>
		public ValidatieException(string message) : base(message)
		{
		}

		/// <summary>
		/// Instantieert een ValidatieException met een opgegeven foutboodschap en een foutnummer
		/// </summary>
		/// <param name="message">De foutboodschap die doorgegeven moet worden</param>
		/// <param name="foutNummer">Het foutnummer</param>
		public ValidatieException(string message, FoutNummer foutNummer) : base(message)
		{
			FoutNummer = foutNummer;
		}

		/// <summary>
		/// Instantieert een ValidatieException met een opgegeven foutboodschap en 'inner exception'
		/// </summary>
		/// <param name="message">De foutboodschap die doorgegeven moet worden</param>
		/// <param name="inner">De 'inner exception'</param>
		public ValidatieException(string message, Exception inner) : base(message, inner)
		{
		}

		/// <summary>
		/// Instantieert een ValidatieException met een opgegeven SerializationInfo en StreamingContext
		/// </summary>
		/// <param name="info">De SerializationInfo</param>
		/// <param name="context">De StreamingContext</param>
		public ValidatieException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
