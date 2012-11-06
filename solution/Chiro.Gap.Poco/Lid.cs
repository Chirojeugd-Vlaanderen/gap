using System;
using System.Collections.Generic;

namespace Chiro.Gap.Poco.Model
{
    public partial class Lid
    {
        public Lid()
        {
            this.Functie = new HashSet<Functie>();
        }
    
        public Nullable<bool> LidgeldBetaald { get; set; }
        public bool NonActief { get; set; }
        public bool Verwijderd { get; set; }
        public short VolgendWerkjaarInt { get; set; }
        public int ID { get; set; }
        public byte[] Versie { get; set; }
        public Nullable<System.DateTime> EindeInstapPeriode { get; set; }
        public Nullable<System.DateTime> UitschrijfDatum { get; set; }
    
        public virtual GroepsWerkJaar GroepsWerkJaar { get; set; }
        public virtual GelieerdePersoon GelieerdePersoon { get; set; }
        public virtual ICollection<Functie> Functie { get; set; }
    }
    
}
