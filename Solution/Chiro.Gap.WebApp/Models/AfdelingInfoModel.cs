using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using Chiro.Adf.ServiceModel;
using Chiro.Gap.Orm;
using Chiro.Gap.ServiceContracts;

namespace Chiro.Gap.WebApp.Models
{
    public class AfdelingInfoModel : MasterViewModel 
    {
        public Afdeling HuidigeAfdeling { get; set; }

        public AfdelingsJaar HuidigAfdelingsJaar { get; set; }

        public IList<AfdelingInfo> GebruikteAfdelingLijst { get; set; }

        public IList<AfdelingInfo> OngebruikteAfdelingLijst { get; set; }

        public IList<OfficieleAfdeling> OfficieleAfdelingenLijst { get; set; }

        public int OfficieleAfdelingID { get; set; }

        public AfdelingInfoModel() : base() { }
    }
}
