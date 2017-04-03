/*
 * Copyright 2008-2017 the GAP developers. See the NOTICE file at the 
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

namespace Chiro.Cdf.Intranet
{
    /// <summary>
    /// Interface voor controle van mailadressen via een intranetwebservice
    /// </summary>
    public interface IMailadrescontrole
    {
        /// <summary>
        /// Geeft aan of het mailadres vermoedelijk fout is of van iemand anders dan de opgegeven eigenaar (m/v/x).
        /// </summary>
        /// <param name="voornaam">Voornaam van de eigenaar (m/v/x)</param>
        /// <param name="naam">Naam van de eigenaar (m/v/x)</param>
        /// <param name="email">Mailadres van de eigenaar (m/v/x)</param>
        /// <returns><c>True</c> als we vermoeden dat het ofwel fout is, ofwel toebehoort aan iemand anders 
        /// (bv. een van de ouders, voogd, enz.), <c>false</c> als het adres waarschijnlijk juist geschreven is 
        /// én toebehoort aan de opgegeven eigenaar (m/v/x).</returns>
        bool MailadresIsVerdacht(string voornaam, string naam, string email);

        /// <summary>
        /// Geeft aan of het mailadres vermoedelijk fout is of van iemand anders dan de opgegeven eigenaar (m/v/x).
        /// </summary>
        /// <param name="voornaam">Voornaam van de eigenaar (m/v/x)</param>
        /// <param name="naam">Naam van de eigenaar (m/v/x)</param>
        /// <param name="geboortejaar">Geboortejaar van de eigenaar (m/v/x)</param>
        /// <param name="email">Mailadres van de eigenaar (m/v/x)</param>
        /// <returns><c>True</c> als we vermoeden dat het ofwel fout is, ofwel toebehoort aan iemand anders 
        /// (bv. een van de ouders, voogd, enz.), <c>false</c> als het adres waarschijnlijk juist geschreven is 
        /// én toebehoort aan de opgegeven eigenaar (m/v/x).</returns>
        bool MailadresIsVerdacht(string voornaam, string naam, int geboortejaar, string email);

        /// <summary>
        /// Berekent een score die een indicatie geeft of we ervan mogen uitgaan dat het mailadres van de 
        /// opgegeven eigenaar (m/v/x) is en dat het juist geschreven is.
        /// </summary>
        /// <param name="voornaam">Voornaam van de eigenaar (m/v/x)</param>
        /// <param name="naam">Naam van de eigenaar (m/v/x)</param>
        /// <param name="email">Mailadres dat we controleren</param>
        /// <returns>Een score die aangeeft hoe betrouwbaar het adres en de link met de eigenaar (m/v/x) is. 
        /// Twee en hoger is redelijk betrouwbaar.</returns>
        int BetrouwbaarheidsscoreOphalenOpNaam(string voornaam, string naam, string email);

        /// <summary>
        /// Berekent een score die een indicatie geeft of we ervan mogen uitgaan dat het mailadres van de 
        /// opgegeven eigenaar (m/v/x) is en dat het juist geschreven is.
        /// </summary>
        /// <param name="voornaam">Voornaam van de eigenaar (m/v/x)</param>
        /// <param name="naam">Naam van de eigenaar (m/v/x)</param>
        /// <p param name="geboortejaar">Geboortejaar van de eigenaar (m/v/x)</p>
        /// <param name="email">Mailadres dat we controleren</param>
        /// <returns>Een score die aangeeft hoe betrouwbaar het adres en de link met de eigenaar (m/v/x) is. 
        /// Twee en hoger is redelijk betrouwbaar.</returns>
        int BetrouwbaarheidsscoreOphalenOpNaamEnGeboortejaar(string voornaam, string naam, int geboortejaar, string email);

    }
}
