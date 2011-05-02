// <copyright company="Chirojeugd-Vlaanderen vzw">
// Copyright (c) 2007-2010
// Mail naar informatica@chiro.be voor alle info over deze broncode
// </copyright>

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

using Chiro.Gap.ServiceContracts.DataContracts;
using Chiro.Gap.WebApp.HtmlHelpers;

namespace Chiro.Gap.WebApp.Models
{
	/// <summary>
	/// Model voor verhuis:
	///   . op een adres A wonen personen
	///   . een aantal van die personen verhuizen naar een nieuw adres B
	/// Model voor een nieuw adres
	/// </summary>
	public class AdresModel : MasterViewModel, IAdresBewerkenModel
	{
		/// <summary>
		/// De standaardconstructor voor AdresModel
		/// </summary>
		public AdresModel()
		{
			Bewoners = new List<CheckBoxListInfo>();	// mensen die nu op dit adres wonen
			GelieerdePersoonIDs = new List<int>();		// ID's van de geselecteerde bewoners
			PersoonsAdresInfo = new PersoonsAdresInfo();	// het adres zelf, in combinatie met het type
			BeschikbareWoonPlaatsen = new List<WoonPlaatsInfo>();	// woonplaatsen met het postnummer van het adres
			AlleLanden = new List<LandInfo>();			// alle gekende landen
		}

		/// <summary>
		/// ID van GelieerdePersoon die graag zou verhuizen.
		/// Wordt bewaard om achteraf terug naar de details van de
		/// aanvrager te kunnen redirecten.
		/// </summary>
		public int AanvragerID { get; set; }

		/// <summary>
		/// Lijst met bewoners van het huidige adres
		/// </summary>
		public IEnumerable<CheckBoxListInfo> Bewoners { get; set; }

		/// <summary>
		/// Adresinfo in combinatie met adrestype
		/// </summary>
		public PersoonsAdresInfo PersoonsAdresInfo { get; set; }

		/// <summary>
		/// Het ID van het oude adres
		/// </summary>
		public int OudAdresID { get; set; }

		/// <summary>
		/// De IDs van de gekozen gelieerdepersonen die mee verhuizen (subset van de bewoners).
		/// </summary>
		public List<int> GelieerdePersoonIDs { get; set; }

		/// <summary>
		/// Is dit nieuwe adres iedereen zijn voorkeursadres
		/// </summary>
		[DisplayName(@"Voorkeursadres van de bewoner(s)")]
		public bool Voorkeur { get; set; }

		/// <summary>
		/// Lijstje woonplaatsen dat overeenkomt met Adres.PostNr
		/// </summary>
		public IEnumerable<WoonPlaatsInfo> BeschikbareWoonPlaatsen { get; set; }

		/// <summary>
		/// Lijstje beschikbare landen
		/// </summary>
		public IEnumerable<LandInfo> AlleLanden { get; set; }

		/// <summary>
		/// Apart invulveld voor woonplaats in het buitenland
		/// </summary>
		[DisplayName(@"Woonplaats")]
		[StringLengte(80)]
		public string WoonPlaatsBuitenLand { get; set; }

		#region Implementatie IAdresBewerkenModel

		// Dit mapt gewoon de velden van IAdresBewerkenModel naar de goeie
		// velden van dit model.  Op die manierhoop ik een partial view
		// te kunnen gebruiken om adressen op te vragen.

		// Ik had deze interface liever expliciet geïmplementeerd, maar
		// dat werkt blijkbaar niet goed samen met de model binding.

		public string Land
		{
			get { return PersoonsAdresInfo.LandNaam; }
			set { PersoonsAdresInfo.LandNaam = value; }
		}

		public int PostNr
		{
			get { return PersoonsAdresInfo.PostNr; }
			set { PersoonsAdresInfo.PostNr = value; }
		}

		public string PostCode
		{
			get { return PersoonsAdresInfo.PostCode; }
			set { PersoonsAdresInfo.PostCode = value; }
		}

		public string Straat
		{
			get { return PersoonsAdresInfo.StraatNaamNaam; }
			set { PersoonsAdresInfo.StraatNaamNaam = value; }
		}

		public int? HuisNr
		{
			get { return PersoonsAdresInfo.HuisNr; }
			set { PersoonsAdresInfo.HuisNr = value; }
		}

		public string Bus
		{
			get { return PersoonsAdresInfo.Bus; }
			set { PersoonsAdresInfo.Bus = value; }
		}

		public string WoonPlaats
		{
			get { return PersoonsAdresInfo.WoonPlaatsNaam; }
			set { PersoonsAdresInfo.WoonPlaatsNaam = value; }
		}

		#endregion
	}
}