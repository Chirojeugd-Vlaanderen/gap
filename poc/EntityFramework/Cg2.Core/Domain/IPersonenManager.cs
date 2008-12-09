using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cg2.Core.Domain
{
    /// <summary>
    /// Deze interface biedt de operaties aan die met een persoon kunnen
    /// gebeuren.
    /// </summary>
    public interface IPersonenManager
    {
        /// <summary>
        /// Updatet een persoon
        /// </summary>
        /// <param name="p">Te updaten persoon</param>
        /// <param name="origineel">Originele persoon, als die beschikbaar is.
        /// Anders null.</param>
        /// <returns>De bewaarde persoon</returns>
        Persoon Updaten(Persoon p, Persoon origineel);
    }
}
