// <copyright company="Chirojeugd-Vlaanderen vzw">
// Copyright (c) 2007-2010
// Mail naar informatica@chiro.be voor alle info over deze broncode
// </copyright>

using System.Collections.Generic;

using Chiro.Gap.ServiceContracts;

namespace Chiro.Gap.WebApp.Models
{
	/// <summary>
	/// Dit is nu een specifiek model voor het activeren van een afdeling, of het wijzigen van een
	/// geactiveerde afdeling
	/// </summary>
	public class AfdelingsJaarModel : MasterViewModel
	{
        public AfdelingsJaarModel()
        {
            OfficieleAfdelingen = new List<OfficieleAfdelingDetail>();
            AfdelingsJaar = new AfdelingsJaarDetail();
            Afdeling = new AfdelingInfo();
        }

		/// <summary>
		/// Relevante informatie over het afdelingsjaar
		/// </summary>
		public AfdelingsJaarDetail AfdelingsJaar { get; set; }

		/// <summary>
		/// Detail over de afdeling waaraan het afdelingsjaar gekoppeld is/moet worden.
		/// </summary>
		public AfdelingInfo Afdeling { get; set; }

		/// <summary>
		/// Het lijstje officiele afdelingen
		/// </summary>
		public IEnumerable<OfficieleAfdelingDetail> OfficieleAfdelingen { get; set; }
	}
}
