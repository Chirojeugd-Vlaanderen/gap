// <copyright company="Chirojeugd-Vlaanderen vzw">
// Copyright (c) 2007-2010
// Mail naar informatica@chiro.be voor alle info over deze broncode
// </copyright>

using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Properties;
using System.Runtime.Serialization;

namespace Chiro.Gap.Domain
{
	/// <summary>
	/// Generieke adresgegevens.  Wordt gebruikt om adressen op te zoeken, maar ook als 
	/// datacontract.  Omdat de workers deze klasse gebruiken, heb ik ze verplaatst van
	/// Chiro.Gap.ServiceContracts naar Chiro.Gap.Domain.
	/// 
	/// Het ID wordt enkel gebruikt bij het ophalen van een adres.
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
		// De upper limit is verhoogd van 9999 naar 99999, om dit datacontract ook te kunnen gebruiken voor
		// buitenlandse adressen.  Een beetje een hack dus.
		[Range(1000, 99999, ErrorMessageResourceType = typeof(Resources), ErrorMessageResourceName = "RangeError")]
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
		// Woonplaats is eigenlijk wel verplicht, maar ik zet dat hier toch af,
		// omdat ik anders problemen krijg in de UI bij het ingeven van een
		// buitenlands adres.  Daar wordt de drop down voor woonplaats dan 
		// niet gebruikt, waardoor bij een verplichte woonplaats het formulier niet 
		// gepost kan worden.  (hack)
		//[Verplicht]
		[StringLengte(80)]
		public String WoonPlaatsNaam { get; set; }

		[DataMember]
		[DisplayName(@"Land")]
		[Verplicht]
		[StringLengte(80)]
		public String LandNaam { get; set; }
	}
}
