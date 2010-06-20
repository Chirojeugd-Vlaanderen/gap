// <copyright company="Chirojeugd-Vlaanderen vzw">
// Copyright (c) 2007-2010
// Mail naar informatica@chiro.be voor alle info over deze broncode
// </copyright>

using System.Collections.Generic;

using Chiro.Gap.ServiceContracts;
using Chiro.Gap.ServiceContracts.DataContracts;

namespace Chiro.Gap.WebApp.Models
{
	public class LidInfoModel : MasterViewModel
	{
        public LidInfoModel()
        {
            WerkJaarInfos = new List<WerkJaarInfo>();
            LidInfoLijst = new List<PersoonLidInfo>();
            AfdelingsInfoDictionary = new Dictionary<int, AfdelingDetail>();
        }

		public int PageHuidig { get; set; }
		public int PageTotaal { get; set; }

		public int HuidigeAfdeling { get; set; }

		public Dictionary<int, AfdelingDetail> AfdelingsInfoDictionary { get; set; }
		public Dictionary<int, FunctieInfo> FunctieInfoDictionary { get; set; }

		public int IDGetoondGroepsWerkJaar { get; set; }
		public int JaartalGetoondGroepsWerkJaar { get; set; }
		public IEnumerable<WerkJaarInfo> WerkJaarInfos { get; set; }

		public IList<PersoonLidInfo> LidInfoLijst { get; set; }

		public int GekozenAfdeling { get; set; }
		public int GekozenFunctie { get; set; }
	}
}
