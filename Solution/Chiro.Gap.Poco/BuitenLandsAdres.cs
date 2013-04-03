using System;

namespace Chiro.Gap.Poco.Model
{
    public partial class BuitenLandsAdres : Adres
    {
        public string PostCode { get; set; }
        public string Straat { get; set; }
        public string WoonPlaats { get; set; }
        public Nullable<int> PostNummer { get; set; }
    
        public virtual Land Land { get; set; }
    }
    
}
