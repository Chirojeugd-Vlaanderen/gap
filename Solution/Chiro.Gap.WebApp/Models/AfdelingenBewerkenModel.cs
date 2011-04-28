using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using Chiro.Gap.ServiceContracts.DataContracts;

namespace Chiro.Gap.WebApp.Models
{
	/// <summary>
	/// Model voor het bewerken van afdelingen van meer dan 1 persoon.
	/// </summary>
	public class AfdelingenBewerkenModel: MasterViewModel
	{
		public IEnumerable<ActieveAfdelingInfo> BeschikbareAfdelingen { get; set; }

		/// <summary>
		/// Details van de personen (om te laten zien)
		/// </summary>
		public IList<PersoonDetail> Personen { get; set; }

		/// <summary>
		/// ID's van de leden, voor postback
		/// </summary>
		public IEnumerable<int> LidIDs { get; set; }

		// AfdelingsJaarID of AfdelingsJaarIDs wordt gebruikt, alnaargelang de gebruiker
		// 1 of meerdere afdelingen in kon geven.  (Dit laaste is het geval als er enkel
		// leiding geselecteerd was.)

		public int AfdelingsJaarID { get; set; }
		public IEnumerable<int> AfdelingsJaarIDs { get; set; }

	}
}