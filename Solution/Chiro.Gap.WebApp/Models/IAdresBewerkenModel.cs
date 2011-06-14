// <copyright company="Chirojeugd-Vlaanderen vzw">
// Copyright (c) 2007-2011
// Mail naar informatica@chiro.be voor alle info over deze broncode
// </copyright>

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

		// De upper limit is verhoogd van 9999 naar 99999, om dit datacontract ook te kunnen gebruiken voor
		// buitenlandse adressen.  Een beetje een hack dus.
		[Range(1000, 99999, ErrorMessageResourceType = typeof(System.ComponentModel.DataAnnotations.Properties.Resources), ErrorMessageResourceName = "RangeError")]
		[Verplicht]
		[DisplayName(@"Postnr.")]
		int PostNr { get; set;  }

		[StringLengte(10)]
		[DisplayName(@"Postcode")]
		string PostCode { get; set; }

		[Verplicht]
		[StringLengte(80)]
		string Straat { get; set; }

		[Range(0, int.MaxValue, ErrorMessageResourceType = typeof(System.ComponentModel.DataAnnotations.Properties.Resources), ErrorMessageResourceName = "RangeError")]
		[DisplayName(@"Nr.")]
		int? HuisNr { get; set; }

		string Bus { get; set; }

		[DisplayName(@"Woonplaats")]
		[StringLengte(80)]
		string WoonPlaats { get; set; }

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