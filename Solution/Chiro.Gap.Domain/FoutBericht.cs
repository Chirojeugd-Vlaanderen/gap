// <copyright company="Chirojeugd-Vlaanderen vzw">
// Copyright (c) 2007-2011
// Mail naar informatica@chiro.be voor alle info over deze broncode
// </copyright>

using System.Runtime.Serialization;

namespace Chiro.Gap.Domain
{
	/// <summary>
	/// Klasse voor een foutboodschap.
	/// </summary>
	[DataContract]
	public class FoutBericht
	{
		/// <summary>
		/// Identificatiecode van de fout
		/// </summary>
		[DataMember]
		public FoutNummer FoutNummer { get; set; }

		/// <summary>
		/// Omschrijving van wat er foutgelopen is
		/// </summary>
		[DataMember]
		public string Bericht { get; set; }
	}
}
