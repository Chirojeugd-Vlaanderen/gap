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

using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

using Chiro.Gap.ServiceContracts.DataContracts;

namespace Chiro.Gap.WebApp.Models
{
	/// <summary>
	/// Model voor een partial view om adressen te bewerken
	/// </summary>
	public interface IAdresBewerkenModel
	{
		IEnumerable<LandInfo> AlleLanden { get; set; }
		IEnumerable<WoonPlaatsInfo> BeschikbareWoonPlaatsen { get; set; }

		[Verplicht]
		[StringLengte(80)]
		string Land { get; set; }

        // TODO: Rangerestrictie terug aanzetten. Uitgezet voor fix #1723.
        // (eventueel aanzetten in implementatie ipv in interface)
        //[Range(1000, 99999, ErrorMessageResourceType = typeof(System.ComponentModel.DataAnnotations.Properties.Resources), ErrorMessageResourceName = "RangeError")]
		[Verplicht]
		[DisplayName(@"Postnr.")]
		int PostNr { get; set;  }

		[StringLengte(10)]
		[DisplayName(@"Postcode")]
		string PostCode { get; set; }

        // TODO: Opnieuw verplicht maken. Uitgezet voor #1723.
		//[Verplicht]
        [DisplayName(@"Straat")]
		[StringLengte(80)]
		string StraatNaamNaam { get; set; }

		[Range(0, int.MaxValue, ErrorMessageResourceType = typeof(System.ComponentModel.DataAnnotations.Properties.Resources), ErrorMessageResourceName = "RangeError")]
		[DisplayName(@"Nr.")]
		int? HuisNr { get; set; }

		string Bus { get; set; }

		[DisplayName(@"Woonplaats")]
		[StringLengte(80)]
		string WoonPlaatsNaam { get; set; }

		[DisplayName(@"Woonplaats")]
		[StringLengte(80)]
		string WoonPlaatsBuitenLand { get; set; }

		// 'Straat' wordt gedeeld voor binnenlandse en buitenlandse adressen.
		// Voor woonplaats zijn er verschillende property's (WoonPlaats en WoonPlaatsBuitenland)
		// Dit onderscheid is vooral implementatietechnisch:
		// Omdat voor binnenlandse en buitenlandse adressen de straatnaam een vrij in te vullen tekst
		// is, kan 'Straat' hergebruikt worden.
		// De woonplaats is echter voor binnenlandse adressen uit te kiezen uit een lijstje, terwijl
		// een buitenlandse woonplaats vrij in te vullen is.  Om dit onderscheid op te
		// vangen, heb ik gemakshalve de property WoonPlaatsBuitenland toegevoegd.
	}
}