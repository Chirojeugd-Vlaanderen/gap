using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Chiro.Gap.WebApp.Models
{
	/// <summary>
	/// Model voor de aanvraag van een verzekering loonverlies
	/// </summary>
	public class LoonVerliesModel: MasterViewModel
	{
		/// <summary>
		/// ID van het lid dat verzekerd moet worden tegen loonverlies (enkel leden kunnen deze verzekering krijgen)
		/// </summary>
		public int LidID { get; set; }

		/// <summary>
		/// ID van de overeenkomstige gelieerde persoon
		/// </summary>
		public int GelieerdePersoonID { get; set; }

		/// <summary>
		/// Volledige naam van het lid dat verzekerd moet worden
		/// </summary>
		public string VolledigeNaam { get; set; }

		/// <summary>
		/// Wettegijwel wadatakost?
		/// </summary>
		public decimal Prijs { get; set; }
	}
}
