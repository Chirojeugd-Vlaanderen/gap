// <copyright company="Chirojeugd-Vlaanderen vzw">
// Copyright (c) 2007-2011
// Mail naar informatica@chiro.be voor alle info over deze broncode
// </copyright>

using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace Chiro.Gap.Domain
{
	/// <summary>
	/// DataContract voor info over de status van een bivakaangifte
	/// </summary>
	[DataContract]
	public class BivakAangifteLijstInfo
	{
		public BivakAangifteLijstInfo()
		{
			AlgemeneStatus = BivakAangifteStatus.Onbekend;
			Bivakinfos = new List<BivakAangifteInfo>();
		}

		/// <summary>
		///
		/// </summary>
		[Verplicht]
		[DataMember]
		public IList<BivakAangifteInfo> Bivakinfos { get; set; }

		/// <summary>
		///
		/// </summary>
		[Verplicht]
		[DataMember]
		public BivakAangifteStatus AlgemeneStatus { get; set; }

		/// <summary>
		/// Geeft stringrepresentatie van Versie weer (hex).
		/// Nodig om versie te bewaren in MVC view, voor concurrencycontrole.
		/// </summary>
		[DataMember]
		public string VersieString { get; set; }
	}
}