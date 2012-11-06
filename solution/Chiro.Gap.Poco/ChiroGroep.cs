using System.Collections.Generic;

namespace Chiro.Gap.Poco.Model
{
    public partial class ChiroGroep : Groep
    {
        public ChiroGroep()
        {
            this.Afdeling = new HashSet<Afdeling>();
        }
    
        public string Plaats { get; set; }
    
        public virtual KaderGroep KaderGroep { get; set; }
        public virtual ICollection<Afdeling> Afdeling { get; set; }
    }
    
}
