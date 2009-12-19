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
        public Dictionary<int, AfdelingInfo> AfdelingsInfoDictionary { get; set; }
        //bevat de huidige afdelingen van een lid, of de geselecteerde na de ui, voor leiding
        public List<int> AfdelingIDs { get; set; }
        //bevat de huidige of de nieuwe gewenste afdeling voor een kind
        public int AfdelingID { get; set; }

        public LidInfo HuidigLid { get; set; }

        public LedenModel() : base() { }
    }
}
