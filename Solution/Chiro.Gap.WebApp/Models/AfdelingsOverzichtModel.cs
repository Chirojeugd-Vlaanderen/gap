using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using Chiro.Gap.ServiceContracts;

namespace Chiro.Gap.WebApp.Models
{
	/// <summary>
	/// Model voor het lijstje van actieve en niet-actieve afdelingen
	/// </summary>
	public class AfdelingsOverzichtModel: MasterViewModel
	{
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
