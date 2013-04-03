using System.Collections.Generic;

namespace Chiro.Gap.Poco.Model
{
    public partial class Leiding : Lid
    {
        public Leiding()
        {
            this.AfdelingsJaar = new HashSet<AfdelingsJaar>();
        }
    
    
        public virtual ICollection<AfdelingsJaar> AfdelingsJaar { get; set; }
    }
    
}
