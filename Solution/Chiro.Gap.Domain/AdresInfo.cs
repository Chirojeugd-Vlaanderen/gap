/*
 * Copyright 2008-2013 the GAP developers. See the NOTICE file at the 
 * top-level directory of this distribution, and at
 * https://develop.chiro.be/gap/wiki/copyright
 * 
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 * 
 *     http://www.apache.org/licenses/LICENSE-2.0
 * 
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

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

        /// <summary>
        /// True als het om een Belgisch adres gaat
        /// </summary>
        [DataMember]
        public bool IsBelgisch { get; set; }

	}
}
