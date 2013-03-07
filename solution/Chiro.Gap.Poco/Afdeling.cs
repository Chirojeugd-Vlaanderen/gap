using System.Collections.Generic;
using Chiro.Cdf.Poco;

namespace Chiro.Gap.Poco.Model
{
    public partial class Afdeling : BasisEntiteit
    {
        public Afdeling()
        {
            this.AfdelingsJaar = new HashSet<AfdelingsJaar>();
        }
    
        public string Naam { get; set; }
        public string Afkorting { get; set; }
        public override int ID { get; set; }
        public override byte[] Versie { get; set; }
    
        public virtual ICollection<AfdelingsJaar> AfdelingsJaar { get; set; }
        public virtual ChiroGroep ChiroGroep { get; set; }
    }
    
}
