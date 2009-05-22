using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using Capgemini.Adf.ServiceModel;
using Cg2.Orm;
using Cg2.ServiceContracts;

namespace MvcWebApp2.Models
{
    public class GelieerdePersonenModel : MasterViewModel
    {
        public IList<GelieerdePersoon> GelieerdePersonenLijst { get; set; }

        public GelieerdePersoon HuidigePersoon { get; set; }

        public GelieerdePersonenModel() : base() { }
    }
}