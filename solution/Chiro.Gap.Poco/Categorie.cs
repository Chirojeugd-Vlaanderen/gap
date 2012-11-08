using System.Collections.Generic;
using Chiro.Cdf.Poco;

namespace Chiro.Gap.Poco.Model
{
    public partial class Categorie: BasisEntiteit
    {
        public Categorie()
        {
            this.GelieerdePersoon = new HashSet<GelieerdePersoon>();
        }
    
        public override int ID { get; set; }
        public string Naam { get; set; }
        public string Code { get; set; }
        public override byte[] Versie { get; set; }
    
        public virtual Groep Groep { get; set; }
        public virtual ICollection<GelieerdePersoon> GelieerdePersoon { get; set; }
    }
    
}
