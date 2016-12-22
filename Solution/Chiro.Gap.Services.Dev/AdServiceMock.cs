/*
 * Copyright 2008-2013 the GAP developers. See the NOTICE file at the 
 * top-level directory of this distribution, and at
 * https://gapwiki.chiro.be/copyright
 * Bijgewerkte authenticatie Copyright 2014 Johan Vervloet
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

using System;
using Chiro.Ad.ServiceContracts;

namespace Chiro.Gap.Services.Dev
{
    /// <summary>
    /// Dummy-implementatie van de AdService, voor gebruik bij development.
    /// </summary>
    public class AdServiceMock : IAdService
    {
        /// <summary>
        /// Doet alsof het een login aanvraagt
        /// </summary>
        /// <param name="adNr">
        /// Het AD-nummer van de persoon die een login moet krijgen
        /// </param>
        /// <param name="voornaam">
        /// De voornaam van die persoon
        /// </param>
        /// <param name="familienaam">
        /// De familienaam van die persoon
        /// </param>
        /// <param name="mailadres">
        /// Het mailadres van die persoon
        /// </param>
        /// <returns>
        /// Een string die de login (accountnaam) voorstelt
        /// </returns>
        public string GapLoginAanvragen(int adNr, string voornaam, string familienaam, string mailadres)
        {
            return String.Format("ONGELDIG-{0}", adNr);
        }

        /// <summary>
        /// Levert een dummy ad-nummer op, voor development purposes.
        /// </summary>
        /// <param name="userName">Gebruikersnaam waarvoor AD-nummer opgeleverd moet worden.</param>
        /// <returns></returns>
        public int? AdNummerOpHalen(string userName)
        {
            int result = 0;
            foreach (char c in userName)
            {
                result += (int)c;
            }
            return result;
        }

        /// <summary>
        /// Levert dummy-gebruikersnaam op, voor development purposes.
        /// </summary>
        /// <param name="adNummer">AD-nummer</param>
        /// <returns>Dummy-gebruikersnaam voor persoon met gegeven adnr.</returns>
        public string gebruikersNaamOphalen(int adNummer)
        {
            return String.Format("ONGELDIG-{0}", adNummer);
        }
    }
}
