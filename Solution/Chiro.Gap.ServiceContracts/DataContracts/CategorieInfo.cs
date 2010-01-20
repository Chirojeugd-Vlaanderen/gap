using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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
		public string Naam { get; set; }
		/// <summary>
		/// Code voor de categorie
		/// </summary>
		public string Code { get; set; }
	}
}
