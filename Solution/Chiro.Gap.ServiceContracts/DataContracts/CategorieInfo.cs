using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;

namespace Chiro.Gap.ServiceContracts
{
	/// <summary>
	/// Relevante informatie over een categorie
	/// </summary>
	public class CategorieInfo
	{
		/// <summary>
		/// CategorieID van de categorie
		/// </summary>
		public int ID { get; set; }

		/// <summary>
		/// Naam van de categorie
		/// </summary>
		[Verplicht(), StringLengte(80), StringMinimumLengte(2)]
		public string Naam { get; set; }

		[Verplicht(), StringLengte(10), StringMinimumLengte(2)]
		/// <summary>
		/// Code voor de categorie
		/// </summary>
		public string Code { get; set; }
	}
}
