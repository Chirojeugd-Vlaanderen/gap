using System;
using System.Collections.Generic;

namespace Chiro.Gap.Poco.Model
{
    public partial class Persoon
    {
        public Persoon()
        {
            this.GelieerdePersoon = new HashSet<GelieerdePersoon>();
            this.PersoonsAdres = new HashSet<PersoonsAdres>();
            this.PersoonsVerzekering = new HashSet<PersoonsVerzekering>();
            this.Gav = new HashSet<Gav>();
        }
    
        public Nullable<int> AdNummer { get; set; }
        public string Naam { get; set; }
        public string VoorNaam { get; set; }
        public Nullable<System.DateTime> GeboorteDatum { get; set; }
        internal int GeslachtsInt { get; set; }
        public Nullable<System.DateTime> SterfDatum { get; set; }
        public int ID { get; set; }
        public byte[] Versie { get; set; }
        public bool AdInAanvraag { get; set; }
    
        public virtual ICollection<GelieerdePersoon> GelieerdePersoon { get; set; }
        public virtual ICollection<PersoonsAdres> PersoonsAdres { get; set; }
        public virtual ICollection<PersoonsVerzekering> PersoonsVerzekering { get; set; }
        public virtual ICollection<Gav> Gav { get; set; }
    }
    
}
