using System.Collections.Generic;

namespace Chiro.Gap.Poco.Model
{
    public partial class Gav
    {
        public Gav()
        {
            this.GebruikersRecht = new HashSet<GebruikersRecht>();
            this.Persoon = new HashSet<Persoon>();
        }
    
        public int ID { get; set; }
        public string Login { get; set; }
        public byte[] Versie { get; set; }
    
        public virtual ICollection<GebruikersRecht> GebruikersRecht { get; set; }
        public virtual ICollection<Persoon> Persoon { get; set; }
    }
    
}
