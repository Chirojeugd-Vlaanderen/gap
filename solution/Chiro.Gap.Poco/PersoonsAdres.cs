using System.Collections.Generic;

namespace Chiro.Gap.Poco.Model
{
    public partial class PersoonsAdres
    {
        public PersoonsAdres()
        {
            this.GelieerdePersoon = new HashSet<GelieerdePersoon>();
        }
    
        public string Opmerking { get; set; }
        public int AdresTypeInt { get; set; }
        public byte[] Versie { get; set; }
        public int ID { get; set; }
    
        public virtual Adres Adres { get; set; }
        public virtual Persoon Persoon { get; set; }
        public virtual ICollection<GelieerdePersoon> GelieerdePersoon { get; set; }
    }
    
}
