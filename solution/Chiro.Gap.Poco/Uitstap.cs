using System.Collections.Generic;

namespace Chiro.Gap.Poco.Model
{
    public partial class Uitstap
    {
        public Uitstap()
        {
            this.Deelnemer = new HashSet<Deelnemer>();
        }
    
        public int ID { get; set; }
        public string Naam { get; set; }
        public bool IsBivak { get; set; }
        public System.DateTime DatumVan { get; set; }
        public System.DateTime DatumTot { get; set; }
        public string Opmerkingen { get; set; }
        public byte[] Versie { get; set; }
    
        public virtual Plaats Plaats { get; set; }
        public virtual GroepsWerkJaar GroepsWerkJaar { get; set; }
        public virtual ICollection<Deelnemer> Deelnemer { get; set; }
        public virtual Deelnemer ContactDeelnemer { get; set; }
    }
    
}
