// <copyright company="Chirojeugd-Vlaanderen vzw">
//   Copyright (c) 2007-2012 Mail naar informatica@chiro.be voor alle info over deze broncode
// </copyright>

using System.Collections.Generic;

using Chiro.Gap.Orm;

namespace Chiro.Gap.Workers
{
    /// <summary>
    /// TODO (#190): documenteren
    /// </summary>
    public interface IVeelGebruikt
    {
        /// <summary>
        /// Verwijdert het recentste groepswerkjaar van groep met ID <paramref name="groepID"/>
        /// uit de cache.
        /// </summary>
        /// <param name="groepID">
        /// ID van de groep waarvan groepswerkjaarcache te resetten
        /// </param>
        void GroepsWerkJaarResetten(int groepID);

        /// <summary>
        /// Haalt van de groep met gegeven <paramref name="groepID"/> het recentste
        /// groepswerkjaar op, inclusief de groep zelf.
        /// </summary>
        /// <param name="groepID">
        /// ID van de groep waarvan groepswerkjaar gevraagd
        /// </param>
        /// <returns>
        /// Het groepswerkjaar, met daaraan gekoppeld de groep
        /// </returns>
        GroepsWerkJaar GroepsWerkJaarOphalen(int groepID);

        /// <summary>
        /// Haalt alle nationale functies op
        /// </summary>
        /// <returns>
        /// Lijstje nationale functies
        /// </returns>
        IEnumerable<Functie> NationaleFunctiesOphalen();

        /// <summary>
        /// Haalt het groepID van de groep met gegeven stamnummer op uit de cache.
        /// </summary>
        /// <param name="code">
        /// Stamnummer van groep waarvan groepID te bepalen is
        /// </param>
        /// <returns>
        /// GroepID van de groep met stamnummer <paramref name="code"/>.
        /// </returns>
        int CodeNaarGroepID(string code);
    }
}