using System.Collections.Generic;
using Chiro.Gap.Domain;

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

        /// <summary>
        /// Het niveau van een Chirogroep is altijd Niveau.Groep
        /// </summary>
        public override Niveau Niveau
        {
            get { return Niveau.Groep; }
        }
    }
    
}
