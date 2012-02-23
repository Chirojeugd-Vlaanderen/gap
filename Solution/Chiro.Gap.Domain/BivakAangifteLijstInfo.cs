// <copyright company="Chirojeugd-Vlaanderen vzw">
// Copyright (c) 2007-2012
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
	    /// <summary>
	    /// Constructor. Instantieert een leeg lijstje van bivakken.
	    /// </summary>
	    public BivakAangifteLijstInfo()
		{
			AlgemeneStatus = BivakAangifteStatus.Onbekend;
			Bivakinfos = new List<BivakAangifteInfo>();
		}

		/// <summary>
		/// Het lijstje met geregistreerde bivakken
		/// </summary>
		[Verplicht]
		[DataMember]
		public IList<BivakAangifteInfo> Bivakinfos { get; set; }

		/// <summary>
		/// Geeft aan wat er op dit moment moet gebeuren voor de bivakaangifte
		/// om in orde te zijn met de Chiroadministratie
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