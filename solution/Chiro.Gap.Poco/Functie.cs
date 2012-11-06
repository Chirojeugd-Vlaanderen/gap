using System;
using System.Collections.Generic;

namespace Chiro.Gap.Poco.Model
{
    public partial class Functie
    {
        public Functie()
        {
            this.Lid = new HashSet<Lid>();
        }
    
        public string Naam { get; set; }
        public string Code { get; set; }
        public int ID { get; set; }
        public byte[] Versie { get; set; }
        public Nullable<int> MaxAantal { get; set; }
        public int MinAantal { get; set; }
        public Nullable<int> WerkJaarVan { get; set; }
        public Nullable<int> WerkJaarTot { get; set; }
        public bool IsNationaal { get; set; }
        public int LidTypeInt { internal get; set; }
        public int NiveauInt { get; set; }
    
        public virtual Groep Groep { get; set; }
        public virtual ICollection<Lid> Lid { get; set; }
    }
    
}
