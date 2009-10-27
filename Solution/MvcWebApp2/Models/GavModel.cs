using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Chiro.Adf.ServiceModel;
using Cg2.Orm;
using Cg2.ServiceContracts;

namespace Chiro.Gap.WebApp.Models
{
    public class GavModel : MasterViewModel
    {   
        public GavModel() : base() { }

        public IEnumerable<GroepInfo> GroepenLijst { get; set; }

        public int GeselecteerdeGroepID { get; set; }

        
    }
}
