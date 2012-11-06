using System;
using System.Collections.Generic;

namespace Chiro.Gap.Poco.Model
{
    public abstract partial class Adres
    {
        public Adres()
        {
            this.PersoonsAdres = new HashSet<PersoonsAdres>();
            this.BivakPlaats = new HashSet<Plaats>();
        }
    
        public string Bus { get; set; }
        public Nullable<int> HuisNr { get; set; }
        public int ID { get; set; }
        public byte[] Versie { get; set; }
    
        public virtual ICollection<PersoonsAdres> PersoonsAdres { get; set; }
        public virtual ICollection<Plaats> BivakPlaats { get; set; }
    }
    
}
