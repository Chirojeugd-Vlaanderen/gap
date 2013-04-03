// <copyright company="Chirojeugd-Vlaanderen vzw">
//   Copyright (c) 2007-2012 Mail naar informatica@chiro.be voor alle info over deze broncode
// </copyright>

using System.Collections.Generic;
using Chiro.Cdf.Poco;
using Chiro.Gap.Poco.Model;

namespace Chiro.Gap.WorkerInterfaces
{
    /// <summary>
    /// Levert veel gebruikte basislijstje op, eventueel via een cache
    /// </summary>
    public interface IVeelGebruikt
    {
        /// <summary>
        /// Verwijdert het recentste groepswerkjaar van groep met ID <paramref name="groepID"/> uit de cache.
        /// </summary>
        /// <param name="groepID">
        /// ID van de groep waarvan groepswerkjaarcache te resetten
        /// </param>
        void GroepsWerkJaarResetten(int groepID);

        /// <summary>
        /// Haalt van de groep met gegeven <paramref name="groepID"/> het ID van het recentste groepswerkjaar op.
        /// </summary>
        /// <param name="groepID">
        ///     ID van de groep waarvan groepswerkjaarID gevraagd
        /// </param>
        /// <param name="groepenRepo">groepenrepository, via dewelke het groepswerkjaar indien nodig opgehaald
        /// kan worden.</param>
        /// <returns>
        /// Het ID van het recentste groespwerkjaar van de groep
        /// </returns>
        int GroepsWerkJaarIDOphalen(int groepID, IRepository<Groep> groepenRepo);

        /// <summary>
        /// Haalt alle nationale functies op
        /// </summary>
        /// <param name="functieRepo">Repository die deze worker kan gebruiken om functies
        ///     op te vragen.</param>
        /// <returns>
        /// Lijstje nationale functies
        /// </returns>
        /// <remarks>
        /// De repository wordt bewust niet geregeld door de constructor van deze klasse,
        /// omdat we moeten vermijden dat de IOC-container hiervoor een nieuwe context aanmaakt.
        /// </remarks>
        List<Functie> NationaleFunctiesOphalen(IRepository<Functie> functieRepo);

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

        /// <summary>
        /// Haalt het beginjaar van het huidig werkjaar van de groep met gegeven <paramref name="groepID"/> op.
        /// (Bijv. 2012 voor 2012-2013)
        /// </summary>
        /// <param name="groepID">ID van de groep, waarvan het werkjaar opgezocht moet worden</param>
        /// <returns>
        /// het beginjaar van het huidig werkjaar van de groep met gegeven <paramref name="groepID"/>.
        /// (Bijv. 2012 voor 2012-2013)
        /// </returns>
        int WerkJaarOphalen(int groepID);
    }
}