// <copyright company="Chirojeugd-Vlaanderen vzw">
// Copyright (c) 2007-2010
// Mail naar informatica@chiro.be voor alle info over deze broncode
// </copyright>

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;

using Chiro.Gap.Orm;
using Chiro.Gap.ServiceContracts;

namespace Chiro.Gap.WebApp.Models
{
	/// <summary>
	/// Dit model wordt her en der gebruikt voor afdelingsgerelateerde dingen.
	/// </summary>
	public class AfdelingInfoModel : MasterViewModel
	{
		public Afdeling HuidigeAfdeling { get; set; }

		public AfdelingsJaar HuidigAfdelingsJaar { get; set; }

		public IList<AfdelingInfo> GebruikteAfdelingLijst { get; set; }

		public IList<AfdelingInfo> OngebruikteAfdelingLijst { get; set; }

		public IList<OfficieleAfdeling> OfficieleAfdelingenLijst { get; set; }

		[DisplayName("Officiële afdeling")]
		public int OfficieleAfdelingID { get; set; }

		public AfdelingInfoModel() : base() { }
	}
}
