using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using Cg2.Adf.ServiceModel;
using Cg2.Orm;
using Cg2.ServiceContracts;

namespace MvcWebApp2.Models
{
    public class LidInfoModel : MasterViewModel 
    {
        public IList<LidInfo> LidInfoLijst { get; set; }

        public LidInfoModel() : base() { }
    }
}
