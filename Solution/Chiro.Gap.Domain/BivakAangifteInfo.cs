// <copyright company="Chirojeugd-Vlaanderen vzw">
// Copyright (c) 2007-2012
// Mail naar informatica@chiro.be voor alle info over deze broncode
// </copyright>

using System;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace Chiro.Gap.Domain
{
	/// <summary>
	/// DataContract voor info over de status van een bivakaangifte
	/// </summary>
	[DataContract]
	public class BivakAangifteInfo
	{
		/// <summary>
		/// Het unieke ID van het bivak
		/// </summary>
		[Verplicht]
		[DataMember]
		public int ID { get; set; }

		/// <summary>
		/// De omschrijving van het bivak
		/// </summary>
		[Verplicht]
		[DataMember]
		public String Omschrijving { get; set; }

		/// <summary>
		/// De huidige status van de bivakaangifte
		/// </summary>
		[Verplicht]
		[DataMember]
		public BivakAangifteStatus Status { get; set; }

		/// <summary>
		/// Geeft stringrepresentatie van Versie weer (hex).
		/// Nodig om versie te bewaren in MVC view, voor concurrencycontrole.
		/// </summary>
		[DataMember]
		public string VersieString { get; set; }
	}
}