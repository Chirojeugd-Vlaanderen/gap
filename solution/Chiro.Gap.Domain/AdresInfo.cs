﻿// <copyright company="Chirojeugd-Vlaanderen vzw">
// Copyright (c) 2007-2012
// Mail naar informatica@chiro.be voor alle info over deze broncode
// </copyright>

using System;
using System.Runtime.Serialization;

namespace Chiro.Gap.Domain
{
	/// <summary>
	/// Generieke adresgegevens.  Wordt gebruikt om adressen op te zoeken, maar ook als 
	/// datacontract.  Omdat de workers deze klasse gebruiken, heb ik ze verplaatst van
	/// Chiro.Gap.ServiceContracts naar Chiro.Gap.Domain.
	/// <para />
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
		[DataMember]

		public int PostNr { get; set; }

		/// <summary>
		/// Postcode, voor buitenlandse adressen.
		/// (dit gaan de gebruikers weer langs geen kanten snappen)
		/// </summary>
		[DataMember]
		public string PostCode { get; set; }

		/// <summary>
		/// Het huisnummer
		/// </summary>
		[DataMember]
		public int? HuisNr { get; set; }

		/// <summary>
		/// Naam van de straat
		/// </summary>
		[DataMember]
		public String StraatNaamNaam { get; set; }

		/// <summary>
		/// Naam van de stad of gemeente
		/// </summary>
		[DataMember]
		// Woonplaats is eigenlijk wel verplicht, maar ik zet dat hier toch af,
		// omdat ik anders problemen krijg in de UI bij het ingeven van een
		// buitenlands adres.  Daar wordt de drop down voor woonplaats dan 
		// niet gebruikt, waardoor bij een verplichte woonplaats het formulier niet 
		// gepost kan worden.  (hack)
		// [Verplicht]
		public String WoonPlaatsNaam { get; set; }

	    /// <summary>
	    /// Naam van het land
	    /// </summary>
	    [DataMember]
		public String LandNaam { get; set; }
	}
}