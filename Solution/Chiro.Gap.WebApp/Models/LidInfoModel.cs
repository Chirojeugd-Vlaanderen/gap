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
	public class LidInfoModel : MasterViewModel
	{
		public int PageHuidig { get; set; }
		public int PageTotaal { get; set; }

		public int HuidigeAfdeling { get; set; }

		public Dictionary<int, AfdelingInfo> AfdelingsInfoDictionary { get; set; }

		public int GroepsWerkJaarIdZichtbaar { get; set; }
		public int GroepsWerkJaartalZichtbaar { get; set; }
		public IList<GroepsWerkJaar> GroepsWerkJaarLijst { get; set; }

		public IList<LidInfo> LidInfoLijst { get; set; }

		public LidInfoModel() : base() { }
	}
}
