namespace Chiro.Gap.WebApp.Models
{
	/// <summary>
	/// Model dat vraagt om bevestiging ivm een betalende operatie op een persoon
	/// </summary>
	public class BevestigingsModel: MasterViewModel
	{
		/// <summary>
		/// ID van een gelieerde persoon
		/// </summary>
		public int GelieerdePersoonID { get; set; }

		/// <summary>
		/// Indien relevant, een LidID van een lid van de gelieerde persoon.
		/// </summary>
		public int LidID { get; set; }


		/// <summary>
		/// Volledige naam van de persoon
		/// </summary>
		public string VolledigeNaam { get; set; }

		/// <summary>
		/// Wettegijwel wadatakost?
		/// </summary>
		public decimal Prijs { get; set; }
	}
}
