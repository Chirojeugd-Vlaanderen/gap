using System.Collections.Generic;

namespace Chiro.Gap.Poco.Model
{
    public partial class GroepsWerkJaar
    {
        public GroepsWerkJaar()
        {
            this.Lid = new HashSet<Lid>();
            this.AfdelingsJaar = new HashSet<AfdelingsJaar>();
            this.Uitstap = new HashSet<Uitstap>();
            this.Abonnement = new HashSet<Abonnement>();
        }
    
        public int WerkJaar { get; set; }
        public int ID { get; set; }
        public byte[] Versie { get; set; }
    
        public virtual Groep Groep { get; set; }
        public virtual ICollection<Lid> Lid { get; set; }
        public virtual ICollection<AfdelingsJaar> AfdelingsJaar { get; set; }
        public virtual ICollection<Uitstap> Uitstap { get; set; }
        public virtual ICollection<Abonnement> Abonnement { get; set; }
    }
    
}
