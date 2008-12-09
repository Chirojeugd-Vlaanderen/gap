using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cg2.Core.Domain
{
    /// <summary>
    /// Deze interface bevat operaties van toepassing op groepen.
    /// </summary>
    public interface IGroepenManager
    {
        /// <summary>
        /// Persisteert een groep in de database
        /// </summary>
        /// <param name="g">Groep met te persisteren info</param>
        /// <param name="origineel">Origineel uit database, indien
        /// beschikbaar.  Anders null.</param>
        /// <returns>De bewaarde groep</returns>
        Groep Updaten(Groep g, Groep origineel);
    }
}
