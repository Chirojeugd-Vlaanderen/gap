// <copyright company="Chirojeugd-Vlaanderen vzw">
// Copyright (c) 2007-2012
// Mail naar informatica@chiro.be voor alle info over deze broncode
// </copyright>

using System.Linq;
using Chiro.Gap.Domain;
using Chiro.Gap.Poco.Model;

namespace Chiro.Gap.WorkerInterfaces
{
    public interface IAdressenManager
    {
        /// <summary>
        /// Zoekt adres op, op basis van de parameters.
        /// Als er zo geen adres bestaat, wordt het aangemaakt, op
        /// voorwaarde dat de straat en subgemeente geidentificeerd
        /// kunnen worden.  Als ook dat laatste niet het geval is,
        /// wordt een exception gethrowd.
        /// </summary>
        /// <param name="adresInfo">
        /// Bevat de gegevens van het te zoeken/maken adres
        /// </param>
        /// <param name="adressen">
        /// Lijst met bestaande adressen om na te kijken of het nieuwe adres al bestaat
        /// </param>
        /// <returns>
        /// Gevonden adres
        /// </returns>
        /// <remarks>
        /// Ieder heeft het recht adressen op te zoeken
        /// </remarks>
        Adres ZoekenOfMaken(AdresInfo adresInfo, IQueryable<Adres> adressen);
    }
}
