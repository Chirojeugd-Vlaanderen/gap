using System.Collections.Generic;

namespace Chiro.Gap.Poco.Model
{
    public partial class OfficieleAfdeling
    {
        public OfficieleAfdeling()
        {
            this.AfdelingsJaar = new HashSet<AfdelingsJaar>();
        }
    
        public string Naam { get; set; }
        public int ID { get; set; }
        public byte[] Versie { get; set; }
        public int LeefTijdVan { get; set; }
        public int LeefTijdTot { get; set; }
    
        public virtual ICollection<AfdelingsJaar> AfdelingsJaar { get; set; }
    }
    
}
