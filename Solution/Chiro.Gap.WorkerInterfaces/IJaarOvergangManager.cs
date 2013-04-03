using System.Collections.Generic;

using Chiro.Gap.Poco.Model.Exceptions;
using Chiro.Gap.ServiceContracts.DataContracts;

namespace Chiro.Gap.WorkerInterfaces
{
    public interface IJaarOvergangManager
    {
        /// <summary>
        /// Maakt voor de groep met de opgegeven <paramref name="groepID"/> een nieuw werkJaar aan
        /// en maakt daarin de opgegeven afdelingen aan, met hun respectieve leeftijdsgrenzen (geboortejaren).
        /// </summary>
        /// <param name="teActiveren">
        /// De afdelingen die geactiveerd moeten worden, met ingestelde geboortejaren 
        /// </param>
        /// <param name="groepID">
        /// De ID van de groep die de jaarovergang uitvoert
        /// </param>
        /// <exception cref="GapException">
        /// Komt voor wanneer de jaarvergang te vroeg gebeurt.
        /// </exception>
        /// <exception cref="FoutNummerException">
        /// Komt voor als er een afdeling bij zit die niet gekend is in de groep, of als er een afdeling gekoppeld is
        /// aan een onbestaande nationale afdeling. Ook validatiefouten worden op deze manier doorgegeven.
        /// </exception>
        /// <remarks>Er worden geen leden gemaakt door deze method.</remarks>
        void JaarOvergangUitvoeren(IEnumerable<AfdelingsJaarDetail> teActiveren, int groepID);
    }
}