using System.Collections.Generic;

namespace Chiro.Gap.Poco.Model
{
    public partial class Publicatie
    {
        public Publicatie()
        {
            this.Abonnement = new HashSet<Abonnement>();
        }
    
        public int ID { get; set; }
        public string Naam { get; set; }
        public byte[] Versie { get; set; }
        public bool Actief { get; set; }
    
        public virtual ICollection<Abonnement> Abonnement { get; set; }
    }
    
}
