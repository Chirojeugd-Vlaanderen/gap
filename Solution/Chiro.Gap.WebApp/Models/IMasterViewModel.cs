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
		int GroepID { get; }
		/// <summary>
		/// ID van de Chirogroep
		/// </summary>
		string GroepsNaam { get; }
		/// <summary>
		/// Plaats van de Chirogroep
		/// </summary>
		string Plaats { get; }
		/// <summary>
		/// Het stamnummer wordt niet meer gebruikt als primary key, maar zal nog wel
		/// lang gebruikt worden als handige manier om een groep op te zoeken.
		/// </summary>
		string StamNummer { get; }
		/// <summary>
		/// Titel van de webpagina
		/// </summary>
		string Titel { get; }
	}
}
