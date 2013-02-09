using System.Collections.Generic;
using Chiro.Cdf.Poco;

namespace Chiro.Gap.Poco.Model
{
    public partial class Publicatie: BasisEntiteit
    {
        public Publicatie()
        {
            this.Abonnement = new HashSet<Abonnement>();
        }
    
        public override int ID { get; set; }
        public string Naam { get; set; }
        public override byte[] Versie { get; set; }
        public bool Actief { get; set; }
    
        public virtual ICollection<Abonnement> Abonnement { get; set; }
    }
    
}
