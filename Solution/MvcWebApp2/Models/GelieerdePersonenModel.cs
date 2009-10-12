using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using Cg2.Adf.ServiceModel;
using Cg2.Orm;
using Cg2.ServiceContracts;

namespace MvcWebApp2.Models
{
    public class GelieerdePersonenModel : MasterViewModel
    {
        public IList<GelieerdePersoon> GelieerdePersonenLijst { get; set; }

        public GelieerdePersoon HuidigePersoon { get; set; }

        public GelieerdePersonenModel() : base() { }

        /// <summary>
        /// Creeert een nieuwe gelieerde persoon, en stockeert in
        /// HuidigePersoon.
        /// </summary>
        /// <returns>de nieuw gecreeerde gelieerde persoon</returns>
        /// <remarks>Er is precies iets niet juist in deze code...</remarks>
        public GelieerdePersoon NieuweHuidigePersoon()
        {
            HuidigePersoon = new GelieerdePersoon();
            HuidigePersoon.Persoon = new Persoon();
            return HuidigePersoon;
        }
    }
}