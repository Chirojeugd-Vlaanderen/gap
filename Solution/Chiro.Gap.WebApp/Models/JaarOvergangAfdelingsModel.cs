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
	public class JaarOvergangAfdelingsModel : MasterViewModel
	{
		public JaarOvergangAfdelingsModel()
		{
			Afdelingen = new List<AfdelingInfo>();
			GekozenAfdelingsIDs = new List<int>();
		}

		/// <summary>
		/// Afdelingen die al actief zijn dit werkjaar (met afdelingsjaar dus)
		/// </summary>
		public IEnumerable<AfdelingInfo> Afdelingen { get; set; }

		public IEnumerable<int> GekozenAfdelingsIDs { get; set; }
	}
}
