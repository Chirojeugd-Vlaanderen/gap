// <copyright company="Chirojeugd-Vlaanderen vzw">
// Copyright (c) 2007-2011
// Mail naar informatica@chiro.be voor alle info over deze broncode
// </copyright>

using System.Collections.Generic;

using Chiro.Gap.ServiceContracts.DataContracts;

namespace Chiro.Gap.WebApp.Models
{
	/// <summary>
	/// Model dat gebruikt kan worden voor een lijstje links naar personen
	/// </summary>
	public class LedenLinksModel : MasterViewModel
	{
        public LedenLinksModel()
        {
            Leden = new List<PersoonLidInfo>();
        }

		/// <summary>
		/// Informatie op te lijsten leden
		/// </summary>
		public IEnumerable<PersoonLidInfo> Leden { get; set; }

		/// <summary>
		/// Indien niet leeg, wordt de lijst als onvolledig beschouwd, en wordt
		/// een link toegevoegd naar de volledige lijst (link naar gegeven url)
		/// </summary>
		public string VolledigeLijstUrl { get; set; }

		/// <summary>
		/// Totaal aantal personen in volledige lijst
		/// </summary>
		public int TotaalAantal { get; set; }

		/// <summary>
		/// Indien relevant: ID van functie die al die personen hebben.
		/// </summary>
		public int FunctieID { get; set; }
	}
}
