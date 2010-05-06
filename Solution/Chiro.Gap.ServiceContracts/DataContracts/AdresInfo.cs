// <copyright company="Chirojeugd-Vlaanderen vzw">
// Copyright (c) 2007-2010
// Mail naar informatica@chiro.be voor alle info over deze broncode
// </copyright>

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

using Chiro.Gap.Domain;

namespace Chiro.Gap.ServiceContracts
{
	/// <summary>
	/// Datacontract voor een adres.  Het ID wordt enkel gebruikt bij het ophalen van een adres.
	/// Voor de rest zijn alle members strings (geen straatIDs of woonplaatsIDs), zodat hetzelfde
	/// contract gebruikt kan worden voor binnenlandse en buitenlandse adressen.
	/// </summary>
	[DataContract]
	public class AdresInfo
	{
		private string _bus;

		/// <summary>
		/// Het AdresID
		/// </summary>
		[DataMember]
		public int ID { get; set; }

		[DataMember]
		public string Bus
		{
			// Vervangt eventuele null door String.Empty
			// Zie ticket #202: https://develop.chiro.be/trac/cg2/ticket/202
            // TODO: toch terug kolom nullable maken, zie ticket
			get { return _bus; }
			set { _bus = value ?? String.Empty; }
		}

        [Verplicht]
		[DataMember]
        [Range(1000, 9999, ErrorMessage = "{0} is beperkt van {1} tot {2}.")]
		public int PostNr { get; set; }

		[DataMember]
        [Range(0, int.MaxValue, ErrorMessage = "{0} is beperkt van {1} tot {2}.")]
		public int? HuisNr { get; set; }

		[DataMember]
		[DisplayName(@"Straat")]
        [Verplicht]
        [StringLengte(80)]
		public String StraatNaamNaam { get; set; }

		[DataMember]
		[DisplayName(@"Woonplaats")]
        [Verplicht]
        [StringLengte(80)]
		public String WoonPlaatsNaam { get; set; }
	}
}
