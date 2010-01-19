using System;
namespace Chiro.Gap.WebApp.Models
{
	/// <summary>
	/// Elk model dat door de master page gebruikt wordt, moet deze interface implementeren.
	/// </summary>
	public interface IMasterViewModel
	{
		/// <summary>
		/// ID van de Chirogroep
		/// </summary>
		int GroepID { get; set; }
		/// <summary>
		/// ID van de Chirogroep
		/// </summary>
		string GroepsNaam { get; set; }
		/// <summary>
		/// Plaats van de Chirogroep
		/// </summary>
		string Plaats { get; set; }
		/// <summary>
		/// Het stamnummer wordt niet meer gebruikt als primary key, maar zal nog wel
		/// lang gebruikt worden als handige manier om een groep op te zoeken.
		/// </summary>
		string StamNummer { get; set; }
		/// <summary>
		/// Titel van de webpagina
		/// </summary>
		string Titel { get; set; }
	}
}
