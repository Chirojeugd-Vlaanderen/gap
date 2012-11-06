using System.Collections.Generic;

namespace Chiro.Gap.Poco.Model
{
    public partial class AfdelingsJaar
    {
        public AfdelingsJaar()
        {
            this.Leiding = new HashSet<Leiding>();
            this.Kind = new HashSet<Kind>();
        }
    
        public int GeboorteJaarTot { get; set; }
        public int GeboorteJaarVan { get; set; }
        public int ID { get; set; }
        public byte[] Versie { get; set; }
        private int GeslachtsInt { get; set; }
    
        public virtual GroepsWerkJaar GroepsWerkJaar { get; set; }
        public virtual Afdeling Afdeling { get; set; }
        public virtual OfficieleAfdeling OfficieleAfdeling { get; set; }
        public virtual ICollection<Leiding> Leiding { get; set; }
        public virtual ICollection<Kind> Kind { get; set; }
    }
    
}
