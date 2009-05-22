using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using Cg2.Adf.ServiceModel;
using Cg2.Orm;
using Cg2.ServiceContracts;

namespace MvcWebApp2.Models
{
    public class PersoonInfoModel : MasterViewModel
    {
        public int PageHuidige { get; set; }
        public int PageTotaal { get; set; }

        public IList<PersoonInfo> PersoonInfoLijst { get; set; }

        public PersoonInfoModel() : base() { }
    }
}