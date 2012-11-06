using System.Collections.Generic;

namespace Chiro.Gap.Poco.Model
{
    public partial class Afdeling
    {
        public Afdeling()
        {
            this.AfdelingsJaar = new HashSet<AfdelingsJaar>();
        }
    
        public string Naam { get; set; }
        public string Afkorting { get; set; }
        public int ID { get; set; }
        public byte[] Versie { get; set; }
    
        public virtual ICollection<AfdelingsJaar> AfdelingsJaar { get; set; }
        public virtual ChiroGroep ChiroGroep { get; set; }
    }
    
}
