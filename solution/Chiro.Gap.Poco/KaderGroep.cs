using System.Collections.Generic;

namespace Chiro.Gap.Poco.Model
{
    public partial class KaderGroep : Groep
    {
        public KaderGroep()
        {
            this.ChiroGroep = new HashSet<ChiroGroep>();
            this.Children = new HashSet<KaderGroep>();
        }
    
        public int NiveauInt { get; set; }
    
        public virtual ICollection<ChiroGroep> ChiroGroep { get; set; }
        public virtual ICollection<KaderGroep> Children { get; set; }
        public virtual KaderGroep Parent { get; set; }
    }
    
}
