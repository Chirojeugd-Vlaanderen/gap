using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using Chiro.Gap.Orm;
using Chiro.Gap.ServiceContracts;


namespace Chiro.Gap.WebApp.Models
{
	/// <summary>
	/// Model gebruikt om iemand een nieuw adres te geven.
	/// </summary>
	public class CategorieModel : MasterViewModel
	{
		/// <summary>
		/// Nieuwe categorieen voor de gegeven gelieerde personen
		/// </summary>
		public IEnumerable<Categorie> Categorieen { get; set; }
		public List<int> GeselecteerdeCategorieen { get; set; }

		public List<int> GelieerdePersonenIDs { get; set; }
		public List<string> GelieerdePersonenNaam { get; set; }

		/// <summary>
		/// Standaardconstructor - creeert lege
		/// </summary>
		public CategorieModel()
		{
		}
	}
}
