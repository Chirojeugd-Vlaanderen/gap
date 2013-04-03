using System.Collections.Generic;
using Chiro.Cdf.Poco;
using Chiro.Gap.Domain;

namespace Chiro.Gap.Poco.Model
{
    public partial class AfdelingsJaar: BasisEntiteit
    {
        public AfdelingsJaar()
        {
            this.Leiding = new HashSet<Leiding>();
            this.Kind = new HashSet<Kind>();
        }
    
        public int GeboorteJaarTot { get; set; }
        public int GeboorteJaarVan { get; set; }
        public override int ID { get; set; }
        public override byte[] Versie { get; set; }
        private int GeslachtsInt { get; set; }
    
        public virtual GroepsWerkJaar GroepsWerkJaar { get; set; }
        public virtual Afdeling Afdeling { get; set; }
        public virtual OfficieleAfdeling OfficieleAfdeling { get; set; }
        public virtual ICollection<Leiding> Leiding { get; set; }
        public virtual ICollection<Kind> Kind { get; set; }

        /// <summary>
        /// Enumwaarde voor het 'geslacht' van de afdeling
        /// </summary>
        public GeslachtsType Geslacht
        {
            get { return (GeslachtsType)GeslachtsInt; }
            set { GeslachtsInt = (int)value; }
        }

    }
    
}
