using System;
using System.Collections.Generic;

namespace Chiro.Gap.Poco.Model
{
    public partial class WoonPlaats
    {
        public WoonPlaats()
        {
            this.BelgischAdres = new HashSet<BelgischAdres>();
        }
    
        public int ID { get; set; }
        public int PostNummer { get; set; }
        public string Naam { get; set; }
        public int TaalID { get; set; }
        public Nullable<int> CrabPostKantonID { get; set; }
        public byte[] Versie { get; set; }
    
        public virtual ICollection<BelgischAdres> BelgischAdres { get; set; }
    }
    
}
