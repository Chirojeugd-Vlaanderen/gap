using System.Collections.Generic;
using Chiro.Cdf.Poco;

namespace Chiro.Gap.Poco.Model
{
    public partial class Land: BasisEntiteit
    {
        public Land()
        {
            this.BuitenLandsAdres = new HashSet<BuitenLandsAdres>();
        }
    
        public override int ID { get; set; }
        public string Naam { get; set; }
    
        public virtual ICollection<BuitenLandsAdres> BuitenLandsAdres { get; set; }

        public override byte[] Versie { get; set; }
    }
    
}
