using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using Capgemini.Adf.ServiceModel;
using Cg2.Orm;
using Cg2.ServiceContracts;

namespace MvcWebApp2.Models
{
    public class LedenModel : MasterViewModel 
    {
        public IList<Lid> Ledenlijst { get; set; }

        public Lid HuidigLid { get; set; }

        public LedenModel() : base() { }
    }
}
