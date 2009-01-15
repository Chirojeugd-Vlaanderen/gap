using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cg2.Core.Domain
{
    /// <summary>
    /// Interface voor manager van Chirogroepen
    /// </summary>
    public interface IChiroGroepenManager
    {
        /// <summary>
        /// Ophalen groepsgegevens uit database, op basis van ID
        /// </summary>
        /// <param name="groepID">ID op te halen ChiroGroep</param>
        /// <returns>ChiroGroep, zonder relaties</returns>
        ChiroGroep Ophalen(int groepID);
    }
}
