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
	public class AfdelingsOverzichtModel : MasterViewModel
	{
		public AfdelingsOverzichtModel()
		{
			NietActief = new List<AfdelingInfo>();
			Actief = new List<AfdelingDetail>();
		}

		/// <summary>
		/// Afdelingen niet-actief dit werkjaar
		/// </summary>
		public IEnumerable<AfdelingInfo> NietActief { get; set; }

		/// <summary>
		/// Afdelingen die al actief zijn dit werkjaar (met afdelingsjaar dus)
		/// </summary>
		public IEnumerable<AfdelingDetail> Actief { get; set; }
	}
}
