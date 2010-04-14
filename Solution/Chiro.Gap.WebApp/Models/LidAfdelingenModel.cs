using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Chiro.Gap.ServiceContracts;

namespace Chiro.Gap.WebApp.Models
{
	/// <summary>
	/// Model dat gebruikt wordt om de afdeling(en) van een lid te wijzigen
	/// </summary>
	public class LidAfdelingenModel: MasterViewModel
	{
		/// <summary>
		/// Informatie over de afdelingsjaren gekoppeld aan een lid.
		/// </summary>
		public LidAfdelingInfo Info { get; set; }

		/// <summary>
		/// Lijst met de actieve afdelingen dit werkjaar
		/// </summary>
		public IEnumerable<ActieveAfdelingInfo> BeschikbareAfdelingen { get; set; }
	}
}
