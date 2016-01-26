/*
 * Copyright 2008-2013 the GAP developers. See the NOTICE file at the 
 * top-level directory of this distribution, and at
 * https://gapwiki.chiro.be/copyright
 * 
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 * 
 *     http://www.apache.org/licenses/LICENSE-2.0
 * 
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

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
        /// <param name="straatNamen">queryable voor alle beschikbare straatnamen</param>
        /// <param name="woonPlaatsen">queryable voor alle beschikbare woonplaatsen</param>
        /// <param name="landen">queryable voor alle beschikbare landen</param>
        /// <returns>
        /// Gevonden adres
        /// </returns>
        /// <remarks>
        /// Ieder heeft het recht adressen op te zoeken.
        /// Voor een Belgisch adres levert het zoeken ook een resultaat op als enkel de woonplaats
        /// verschilt. De combinatie straat-postnummer is immers uniek.
        /// Omdat ik er niet zeker van ben of je dat in het buitenland ook mag doen, neem ik daar
        /// de woonplaats wel mee in de zoekquery. Fusiegemeenten lijkt me vooral iets Belgisch.
        /// </remarks>
        Adres ZoekenOfMaken(AdresInfo adresInfo, IQueryable<Adres> adressen, IQueryable<StraatNaam> straatNamen,
                            IQueryable<WoonPlaats> woonPlaatsen, IQueryable<Land> landen);
    }
}
