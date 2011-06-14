// <copyright company="Chirojeugd-Vlaanderen vzw">
// Copyright (c) 2007-2011
// Mail naar informatica@chiro.be voor alle info over deze broncode
// </copyright>

using System.Collections.Generic;

using Chiro.Gap.ServiceContracts.DataContracts;

namespace Chiro.Gap.WebApp.Models
{
	/// <summary>
	/// Model dat gebruikt wordt om de afdeling(en) van een lid te wijzigen
	/// </summary>
	public class LidAfdelingenModel : MasterViewModel
	{
		public LidAfdelingenModel()
		{
			BeschikbareAfdelingen = new List<ActieveAfdelingInfo>();
			Info = new LidAfdelingInfo();
		}

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
