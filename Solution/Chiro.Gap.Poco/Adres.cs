using System;
using System.Collections.Generic;
using Chiro.Cdf.Poco;

namespace Chiro.Gap.Poco.Model
{
    public abstract partial class Adres: BasisEntiteit
    {
        public Adres()
        {
            this.PersoonsAdres = new HashSet<PersoonsAdres>();
            this.BivakPlaats = new HashSet<Plaats>();
        }
    
        public string Bus { get; set; }
        public Nullable<int> HuisNr { get; set; }
        public override int ID { get; set; }
        public override byte[] Versie { get; set; }
    
        public virtual ICollection<PersoonsAdres> PersoonsAdres { get; set; }
        public virtual ICollection<Plaats> BivakPlaats { get; set; }
    }
    
}
