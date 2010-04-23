// <copyright company="Chirojeugd-Vlaanderen vzw">
// Copyright (c) 2007-2010
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
		[DataMember]
		public int FoutNummer { get; set; }
		[DataMember]
		public string Bericht { get; set; }     // omschrijving
	}
}
