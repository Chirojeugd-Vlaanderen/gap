using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Chiro.Gap.Domain;

namespace Chiro.Gap.Orm
{
    /// <summary>
    /// Klasse voor kadergroep, die het meeste gewoon erft van Groep.
    /// </summary>
    public partial class KaderGroep
    {
        /// <summary>
        /// Converteert de 'NiveauInt' uit de database naar een enum Niveau.
        /// </summary>
        public override Niveau Niveau
        {
            get { return (Niveau)NiveauInt; }
        }
    }
}
