using System.Collections.Generic;
using Chiro.Cdf.Poco;

namespace Chiro.Gap.Poco.Model
{
    public partial class Gav: BasisEntiteit
    {
        public Gav()
        {
            this.GebruikersRecht = new HashSet<GebruikersRecht>();
            this.Persoon = new HashSet<Persoon>();
        }
    
        public override int ID { get; set; }
        public string Login { get; set; }
        public override byte[] Versie { get; set; }
    
        public virtual ICollection<GebruikersRecht> GebruikersRecht { get; set; }
        public virtual ICollection<Persoon> Persoon { get; set; }
    }
    
}
