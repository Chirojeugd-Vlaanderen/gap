using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using Chiro.Adf.ServiceModel;
using Cg2.Orm;
using Cg2.ServiceContracts;

namespace Chiro.Gap.WebApp.Models
{
    public class PersoonInfoModel : MasterViewModel
    {
        public int PageHuidig { get; set; }
        public int PageTotaal { get; set; }
        public int Totaal { get; set; }

        public IList<PersoonInfo> PersoonInfoLijst { get; set; }

        public PersoonInfoModel() : base() { }
    }
}