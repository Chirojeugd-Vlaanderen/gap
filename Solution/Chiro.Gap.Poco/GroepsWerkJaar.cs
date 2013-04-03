using System.Collections.Generic;
using Chiro.Cdf.Poco;

namespace Chiro.Gap.Poco.Model
{
    public partial class GroepsWerkJaar: BasisEntiteit
    {
        public GroepsWerkJaar()
        {
            Lid = new HashSet<Lid>();
            AfdelingsJaar = new HashSet<AfdelingsJaar>();
            Uitstap = new HashSet<Uitstap>();
            Abonnement = new HashSet<Abonnement>();
        }
    
        public int WerkJaar { get; set; }
        public override int ID { get; set; }
        public override byte[] Versie { get; set; }
    
        public virtual Groep Groep { get; set; }
        public virtual ICollection<Lid> Lid { get; set; }
        public virtual ICollection<AfdelingsJaar> AfdelingsJaar { get; set; }
        public virtual ICollection<Uitstap> Uitstap { get; set; }
        public virtual ICollection<Abonnement> Abonnement { get; set; }
    }
    
}
