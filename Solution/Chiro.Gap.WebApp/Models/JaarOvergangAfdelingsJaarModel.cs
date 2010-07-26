// <copyright company="Chirojeugd-Vlaanderen vzw">
// Copyright (c) 2007-2010
// Mail naar informatica@chiro.be voor alle info over deze broncode
// </copyright>

using System.Collections.Generic;

using Chiro.Gap.ServiceContracts.DataContracts;

namespace Chiro.Gap.WebApp.Models
{
	/// <summary>
	/// Model voor het lijstje van actieve en niet-actieve afdelingen
	/// </summary>
	public class JaarOvergangAfdelingsJaarModel : MasterViewModel
	{
		public JaarOvergangAfdelingsJaarModel()
		{
			Afdelingen = new List<AfdelingDetail>();
		}

		/// <summary>
		/// Afdelingen die al actief zijn dit werkjaar (met afdelingsjaar dus)
		/// </summary>
		public IEnumerable<AfdelingDetail> Afdelingen { get; set; }
		public IEnumerable<OfficieleAfdelingDetail> OfficieleAfdelingen { get; set; }

		public List<string> AfdelingsIDs { get; set; }
		public List<string> OfficieleAfdelingsIDs { get; set; }
		public List<string> VanLijst { get; set; }
		public List<string> TotLijst { get; set; }
		public List<string> GeslLijst { get; set; }
	}
}
