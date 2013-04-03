using System.Collections.Generic;
using Chiro.Cdf.Poco;

namespace Chiro.Gap.Poco.Model
{
    public partial class Plaats: BasisEntiteit
    {
        public Plaats()
        {
            this.Uitstap = new HashSet<Uitstap>();
        }
    
        public override int ID { get; set; }
        public string Naam { get; set; }
        public override byte[] Versie { get; set; }
    
        public virtual Adres Adres { get; set; }
        public virtual GelieerdePersoon GelieerdePersoon { get; set; }
        public virtual ICollection<Uitstap> Uitstap { get; set; }
        public virtual Groep Groep { get; set; }
    }
    
}
