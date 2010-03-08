// <copyright company="Chirojeugd-Vlaanderen vzw">
// Copyright (c) 2007-2010
// Mail naar informatica@chiro.be voor alle info over deze broncode
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Chiro.Gap.Orm;
using Chiro.Gap.ServiceContracts;

namespace Chiro.Gap.WebApp.Models
{
	public class GavModel : MasterViewModel
	{
		public GavModel() : base() { }

		public IEnumerable<GroepInfo> GroepenLijst { get; set; }

		public int GeselecteerdeGroepID { get; set; }
	}
}
