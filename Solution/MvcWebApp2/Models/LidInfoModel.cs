using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using Chiro.Adf.ServiceModel;
using Cg2.Orm;
using Cg2.ServiceContracts;

namespace Chiro.Gap.WebApp.Models
{
    public class LidInfoModel : MasterViewModel 
    {
        public int GroepsWerkJaarIdZichtbaar { get; set; }
        public List<GroepsWerkJaar> GroepsWerkJaarLijst { get; set; }

        public IList<LidInfo> LidInfoLijst { get; set; }

        public LidInfoModel() : base() { }
    }
}
