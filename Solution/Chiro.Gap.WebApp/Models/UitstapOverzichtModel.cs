using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using Chiro.Gap.ServiceContracts.DataContracts;

namespace Chiro.Gap.WebApp.Models
{
	/// <summary>
	/// Model met een rij uitstappen
	/// </summary>
	public class UitstapOverzichtModel: MasterViewModel
	{
		/// <summary>
		/// Een rij uitstappen
		/// </summary>
		public IEnumerable<UitstapInfo> Uitstappen { get; set; }
	}
}