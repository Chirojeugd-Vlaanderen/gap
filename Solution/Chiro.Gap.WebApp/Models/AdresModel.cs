// <copyright company="Chirojeugd-Vlaanderen vzw">
// Copyright (c) 2007-2010
// Mail naar informatica@chiro.be voor alle info over deze broncode
// </copyright>

using System.Collections.Generic;
using System.ComponentModel;

using Chiro.Gap.WebApp.HtmlHelpers;
using Chiro.Gap.ServiceContracts.DataContracts;

namespace Chiro.Gap.WebApp.Models
{
	/// <summary>
	/// Model voor verhuis:
	///   . op een adres A wonen personen
	///   . een aantal van die personen verhuizen naar een nieuw adres B
	/// Model voor een nieuw adres
	/// </summary>
	public class AdresModel : MasterViewModel
	{
        /// <summary>
        /// De standaardconstructor voor AdresModel
        /// </summary>
        public AdresModel()
        {
            Bewoners = new List<CheckBoxListInfo>();
            GelieerdePersoonIDs = new List<int>();
            PersoonsAdresInfo = new PersoonsAdresInfo();
            WoonPlaatsen = new List<WoonPlaatsInfo>();
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
		[DisplayName("Voorkeursadres van de bewoner(s)")]
		public bool Voorkeur { get; set; }

		/// <summary>
		/// Lijstje woonplaatsen dat overeenkomt met Adres.PostNr
		/// </summary>
		public IEnumerable<WoonPlaatsInfo> WoonPlaatsen { get; set; }
	}
}