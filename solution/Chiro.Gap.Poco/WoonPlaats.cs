using System;
using System.Collections.Generic;
using Chiro.Cdf.Poco;

namespace Chiro.Gap.Poco.Model
{
    public partial class WoonPlaats: BasisEntiteit
    {
        public WoonPlaats()
        {
            this.BelgischAdres = new HashSet<BelgischAdres>();
        }
    
        public override int ID { get; set; }
        public int PostNummer { get; set; }
        public string Naam { get; set; }
        public int TaalID { get; set; }
        public Nullable<int> CrabPostKantonID { get; set; }
        public override byte[] Versie { get; set; }
    
        public virtual ICollection<BelgischAdres> BelgischAdres { get; set; }
    }
    
}