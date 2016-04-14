/*
 * Copyright 2016 Chirojeugd-Vlaanderen vzw. See the NOTICE file at the 
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

using Chiro.Ad.Domain;

namespace Chiro.Ad.DirectoryInterface
{
    /// <summary>
    /// Interface voor Active Directory of gelijkaardige service.
    /// </summary>
    public interface IDirectoryAccess
    {
        /// <summary>
        /// Reset het wachtwoord van de gegeven <paramref name="login"/>.
        /// </summary>
        /// <param name="login"></param>
        /// <param name="wachtwoord">Te gebruiken wachtwoord.</param>
        void PasswordReset(Chirologin login, string wachtwoord);

        /// <summary>
        /// Activeert de gegeven <paramref name="login"/>.
        /// </summary>
        /// <param name="login"></param>
        void GebruikerActiveren(Chirologin login);

        /// <summary>
        /// Zoekt de login van een bestaande Chirogebruiker op.
        /// </summary>
        /// <param name="ldapRoot"></param>
        /// <param name="adNr"></param>
        /// <returns></returns>
        Chirologin GebruikerZoeken(string ldapRoot, int adNr);

        /// <summary>
        /// Zoekt de login van een bestaande Chirogebruiker op.
        /// </summary>
        /// <param name="ldapRoot"></param>
        /// <param name="voornaam"></param>
        /// <param name="familienaam"></param>
        /// <returns></returns>
        Chirologin GebruikerZoeken(string ldapRoot, string voornaam, string familienaam);

        /// <summary>
        /// Bewaart de login van een nieuwe gebruiker.
        /// </summary>
        /// <param name="login"></param>
        /// <param name="gebruikerOu">OU waarin de gebruiker gemaakt moet worden.</param>
        void NieuweGebruikerBewaren(Chirologin login, string gebruikerOu);

        /// <summary>
        /// Voegt gegeven <paramref name="gebruiker" /> toe aan de security groep
        /// <paramref name="groep" />.
        /// </summary>
        /// <param name="gebruiker">Gebruiker toe te voegen aan <paramref name="groep"/>.</param>
        /// <param name="groep">Groep waaraan <paramref name="gebruiker"/> toegevoegd moet worden.</param>
        /// <param name="groepOu">OU van de security group.</param>
        void GebruikerToevoegenAanGroep(Chirologin gebruiker, string groep, string groepOu);
    }
}
