using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using Capgemini.Adf.ServiceModel;
using Cg2.Orm;
using Cg2.ServiceContracts;

namespace MvcWebApp2.Models
{
    public class PersoonInfoModel : MasterViewModel
    {
        public IList<PersoonInfo> PersoonInfoLijst { get; set; }

        // public PersoonInfo HuidigePersoon { get; set; }

        public PersoonInfoModel() : base() { }
    }
}