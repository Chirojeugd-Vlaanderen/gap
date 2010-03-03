// <copyright company="Chirojeugd-Vlaanderen vzw">
// Copyright (c) 2007-2010
// Mail naar informatica@chiro.be voor alle info over deze broncode
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using Chiro.Gap.Orm;
using Chiro.Gap.ServiceContracts;

namespace Chiro.Gap.WebApp.Models
{
	/// <summary>
	/// Dit model bevat een lijstje categorieën, een lijstje GelieerdePersoonIDs, en een lijstje
	/// ID's van geselecteerde categorieën
	/// </summary>
	public class CategorieModel : MasterViewModel
	{
		/// <summary>
		/// Nieuwe categorieën voor de gegeven gelieerde personen
		/// </summary>
		public IEnumerable<Categorie> Categorieen { get; set; }
		public List<int> GeselecteerdeCategorieIDs { get; set; }

		public IList<int> GelieerdePersoonIDs { get; set; }

		/// <summary>
		/// Standaardconstructor - creeert lege
		/// </summary>
		public CategorieModel()
		{
		}
	}
}
