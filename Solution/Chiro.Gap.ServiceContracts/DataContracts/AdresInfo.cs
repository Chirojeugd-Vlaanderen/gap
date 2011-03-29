// <copyright company="Chirojeugd-Vlaanderen vzw">
// Copyright (c) 2007-2010
// Mail naar informatica@chiro.be voor alle info over deze broncode
// </copyright>

using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Properties;
using System.Runtime.Serialization;

namespace Chiro.Gap.ServiceContracts.DataContracts
{
	/// <summary>
	/// Datacontract voor een adres.  Het ID wordt enkel gebruikt bij het ophalen van een adres.
	/// Voor de rest zijn alle members strings (geen straatIDs of woonplaatsIDs), zodat hetzelfde
	/// contract gebruikt kan worden voor binnenlandse en buitenlandse adressen.
	/// </summary>
	[DataContract]
	public class AdresInfo
	{
		/// <summary>
		/// Het AdresID
		/// </summary>
		[DataMember]
		public int ID { get; set; }

		/// <summary>
		/// Het busnummer of de buscode
		/// </summary>
		[DataMember]
		public string Bus { get; set; }

		/// <summary>
		/// Het postnummer
		/// </summary>
		[Verplicht]
		[DataMember]
		[Range(1000, 9999, ErrorMessageResourceType = typeof(Resources), ErrorMessageResourceName = "RangeError")]
		public int PostNr { get; set; }

		/// <summary>
		/// Postcode, voor buitenlandse adressen.
		/// (dit gaan de gebruikers weer langs geen kanten snappen)
		/// </summary>
		[DataMember]
		[StringLengte(10)]
		public string PostCode { get; set; }

		/// <summary>
		/// Het huisnummer
		/// </summary>
		[DataMember]
		[Range(0, 2147483647, ErrorMessageResourceType = typeof(Resources), ErrorMessageResourceName = "RangeError")]
		public int? HuisNr { get; set; }

		/// <summary>
		/// Naam van de straat
		/// </summary>
		[DataMember]
		[DisplayName(@"Straat")]
		[Verplicht]
		[StringLengte(80)]
		public String StraatNaamNaam { get; set; }

		/// <summary>
		/// Naam van de stad of gemeente
		/// </summary>
		[DataMember]
		[DisplayName(@"Woonplaats")]
		[Verplicht]
		[StringLengte(80)]
		public String WoonPlaatsNaam { get; set; }

		[DataMember]
		[DisplayName(@"Land")]
		[Verplicht]
		[StringLengte(80)]
		public String LandNaam { get; set; }
	}
}
