using System.Collections.Generic;
using Chiro.Gap.Domain;

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

        /// <summary>
        /// Converteert de 'NiveauInt' uit de database naar een enum Niveau.
        /// </summary>
        public override Niveau Niveau
        {
            get { return (Niveau)NiveauInt; }
        }
    }
    
}
