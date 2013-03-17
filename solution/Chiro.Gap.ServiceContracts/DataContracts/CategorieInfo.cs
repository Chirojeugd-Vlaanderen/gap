// <copyright company="Chirojeugd-Vlaanderen vzw">
// Copyright (c) 2007-2012
// Mail naar informatica@chiro.be voor alle info over deze broncode
// </copyright>

using System.ComponentModel.DataAnnotations;

namespace Chiro.Gap.ServiceContracts.DataContracts
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
		[Verplicht, StringLengte(80), StringMinimumLengte(2)]
		public string Naam { get; set; }

		/// <summary>
		/// Code voor de categorie
		/// </summary>
		[Verplicht, StringLengte(10), StringMinimumLengte(2)]
		public string Code { get; set; }
	}
}
