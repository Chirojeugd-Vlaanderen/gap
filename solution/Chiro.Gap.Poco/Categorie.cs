using System.Collections.Generic;

namespace Chiro.Gap.Poco.Model
{
    public partial class Categorie
    {
        public Categorie()
        {
            this.GelieerdePersoon = new HashSet<GelieerdePersoon>();
        }
    
        public int ID { get; set; }
        public string Naam { get; set; }
        public string Code { get; set; }
        public byte[] Versie { get; set; }
    
        public virtual Groep Groep { get; set; }
        public virtual ICollection<GelieerdePersoon> GelieerdePersoon { get; set; }
    }
    
}
