﻿// <copyright company="Chirojeugd-Vlaanderen vzw">
// Copyright (c) 2007-2012
// Mail naar informatica@chiro.be voor alle info over deze broncode
// </copyright>

using System.Collections.Generic;

using Chiro.Gap.ServiceContracts.DataContracts;

namespace Chiro.Gap.WebApp.Models
{
	/// <summary>
	/// Dit model bevat een lijstje categorieën, een lijstje GelieerdePersoonIDs, en een lijstje
	/// ID's van geselecteerde categorieën
	/// </summary>
	public class CategorieModel : MasterViewModel
	{
        public CategorieModel()
        {
            GeselecteerdeCategorieIDs = new List<int>();
			GelieerdePersoonNamen = new List<string>();
            GelieerdePersoonIDs = new List<int>();
            Categorieen = new List<CategorieInfo>();
        }

		/// <summary>
		/// Nieuwe categorieën voor de gegeven gelieerde personen
		/// </summary>
		public IEnumerable<CategorieInfo> Categorieen { get; set; }
		public List<int> GeselecteerdeCategorieIDs { get; set; }

		public IList<int> GelieerdePersoonIDs { get; set; }
		public IList<string> GelieerdePersoonNamen { get; set; }
	}
}