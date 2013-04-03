using System.Collections.Generic;
using Chiro.Cdf.Poco;

namespace Chiro.Gap.Poco.Model
{
    public partial class Uitstap: BasisEntiteit
    {
        public Uitstap()
        {
            this.Deelnemer = new HashSet<Deelnemer>();
        }
    
        public override int ID { get; set; }
        public string Naam { get; set; }
        public bool IsBivak { get; set; }
        public System.DateTime DatumVan { get; set; }
        public System.DateTime DatumTot { get; set; }
        public string Opmerkingen { get; set; }
        public override byte[] Versie { get; set; }
    
        public virtual Plaats Plaats { get; set; }
        public virtual GroepsWerkJaar GroepsWerkJaar { get; set; }
        public virtual ICollection<Deelnemer> Deelnemer { get; set; }
        public virtual Deelnemer ContactDeelnemer { get; set; }
    }
    
}
