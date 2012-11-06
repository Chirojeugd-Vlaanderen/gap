using System.Collections.Generic;

namespace Chiro.Gap.Poco.Model
{
    public partial class Land
    {
        public Land()
        {
            this.BuitenLandsAdres = new HashSet<BuitenLandsAdres>();
        }
    
        public int ID { get; set; }
        public string Naam { get; set; }
    
        public virtual ICollection<BuitenLandsAdres> BuitenLandsAdres { get; set; }
    }
    
}
