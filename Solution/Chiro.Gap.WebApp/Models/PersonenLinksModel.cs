using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using Chiro.Gap.ServiceContracts;

namespace Chiro.Gap.WebApp.Models
{
	/// <summary>
	/// Model dat gebruikt kan worden voor een lijstje links naar personen
	/// </summary>
	public class PersonenLinksModel: MasterViewModel
	{
		/// <summary>
		/// Informatie op te lijsten personen
		/// </summary>
		public IEnumerable<PersoonInfo> Personen { get; set; }
		/// <summary>
		/// Indien niet leeg, wordt de lijst als onvolledig beschouwd, en wordt
		/// een link toegevoegd naar de volledige lijst (link naar gegeven url)
		/// </summary>
		public string VolledigeLijstUrl { get; set; }
		/// <summary>
		/// Indien relevant: ID van categorie waar alle personen toe behoren.
		/// </summary>
		public int CategorieID { get; set; }
	}
}
