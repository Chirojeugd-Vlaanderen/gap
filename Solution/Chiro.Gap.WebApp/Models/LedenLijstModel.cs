using System.Collections.Generic;

using Chiro.Gap.Domain;
using Chiro.Gap.ServiceContracts.DataContracts;

namespace Chiro.Gap.WebApp.Models
{
	public class LedenLijstModel: MasterViewModel
	{
		public IList<LidOverzicht> LidInfoLijst { get; set; }
		public LidEigenschap GekozenSortering { get; set; }

		/// <summary>
		/// Kunnen de gegevens van de leden gewijzigd worden?
		/// </summary>
		public bool KanLedenBewerken { get; set; }
	}
}
