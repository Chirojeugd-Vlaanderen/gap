using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Cg2.Adf.ServiceModel;
using Cg2.Orm;
using Cg2.ServiceContracts;

namespace MvcWebApp2.Models
{
    public class GavModel : MasterViewModel
    {   
        public GavModel() : base() { }

        public IEnumerable<GroepInfo> GroepenLijst { get; set; }

        public GroepInfo GeselecteerdeGroep { get; set; }

        
    }
}
