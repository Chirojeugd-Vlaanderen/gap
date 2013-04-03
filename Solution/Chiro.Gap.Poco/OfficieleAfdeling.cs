using System.Collections.Generic;
using Chiro.Cdf.Poco;

namespace Chiro.Gap.Poco.Model
{
    public partial class OfficieleAfdeling: BasisEntiteit
    {
        public OfficieleAfdeling()
        {
            this.AfdelingsJaar = new HashSet<AfdelingsJaar>();
        }
    
        public string Naam { get; set; }
        public override int ID { get; set; }
        public override byte[] Versie { get; set; }
        public int LeefTijdVan { get; set; }
        public int LeefTijdTot { get; set; }
    
        public virtual ICollection<AfdelingsJaar> AfdelingsJaar { get; set; }
    }
    
}
