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

        /// <summary>
        /// Haalt persoonsinfo op uit database
        /// </summary>
        /// <param name="persoonID">ID van de persoon met de gevraagde info.
        /// </param>
        /// <returns>Gevonden persoonsobject, null als niet gevonden</returns>
        Persoon Ophalen(int persoonID);

        /// <summary>
        /// Haalt persoonsinfo op uit database, incl. communicatievormen
        /// </summary>
        /// <param name="persoonID">ID op te halen persoon</param>
        /// <returns>Gevraagde persoonsobject, inclusief communicatievormen
        /// in member Communicatie.  Null indien de persoon niet gevonden.
        /// </returns>
        Persoon OphalenMetCommunicatie(int persoonID);
    }
}
