using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using Chiro.Adf.ServiceModel;
using Chiro.Gap.Orm;
using Chiro.Gap.ServiceContracts;

namespace Chiro.Gap.WebApp.Models
{
    public class LedenModel : MasterViewModel 
    {
        public IList<Lid> Ledenlijst { get; set; }

        public Lid HuidigLid { get; set; }

        public LedenModel() : base() { }
    }
}
