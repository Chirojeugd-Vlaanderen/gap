/*
 * Copyright 2008-2013 the GAP developers. See the NOTICE file at the 
 * top-level directory of this distribution, and at
 * https://gapwiki.chiro.be/copyright
 * Bijgewerkt gebruikersbeheer Copyright 2014, 2017 Chirojeugd-Vlaanderen
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
using Chiro.Gap.WorkerInterfaces;
using Chiro.Gap.Poco.Model;
using System;
using Chiro.Cdf.Authentication;

namespace Chiro.Gap.Workers
{
    /// <summary>
    /// Worker die alle businesslogica i.v.m. authenticatie bevat
    /// </summary>
    public class AuthenticatieManager : IAuthenticatieManager
    {
        private readonly IVeelGebruikt _veelGebruikt;
        private readonly IAuthenticator _authenticator;

        /// <summary>
        /// Creeert een nieuwe authenticatiemanager.
        /// </summary>
        /// <param name="veelGebruikt">Gecachete veelgebruikte zaken</param>
        public AuthenticatieManager(IVeelGebruikt veelGebruikt, IAuthenticator authenticator)
        {
            _veelGebruikt = veelGebruikt;
            _authenticator = authenticator;
        }

        /// <summary>
        /// Bepaalt de gebruikersnaam van de huidig aangemelde gebruiker.
        /// (lege string indien niet van toepassing)
        /// </summary>
        /// <returns>
        /// Username aangemelde gebruiker
        /// </returns>
        public string GebruikersNaamGet()
        {
            return AdNummerGet() == null ? String.Empty : _veelGebruikt.GebruikersNaamOphalen(AdNummerGet().Value);
        }

        /// <summary>
        /// <c>true</c> als de gebruiker developer is, en bijv. rechten kan krijgen op
        /// een willekeurige groep.
        /// </summary>
        /// <returns></returns>
        /// <remarks>Dit zit qua architectuur niet helemaal juist. De authenticatiemanager
        /// zou beter een lijstje met rollen opleveren.</remarks>
        public bool IsDeveloper()
        {
            return _authenticator.WieBenIk().DeveloperMode;
        }

        /// <summary>
        /// Opvragen AD-nummer huidige gebruiker.
        /// </summary>
        /// <returns>Het AD-nummer van de momenteel aangemelde gebruiker.</returns>
        public int? AdNummerGet()
        {
            var adnrHeader = _authenticator.WieBenIk();
            return adnrHeader == null ? null : (int?)adnrHeader.AdNr;
        }

        /// <summary>
        /// Vraagt de gebruikersnaam op van de gegeven <paramref name="persoon"/>.
        /// </summary>
        /// <param name="persoon">Persoon wiens gebruikersnaam gezocht is.</param>
        /// <returns>De gebruikersnaam van de persoon. <c>null</c> als die niet bestaat.</returns>
        public string GebruikersNaamGet(Persoon persoon)
        {
            // Als het GAP geen (al dan niet vervallen) gebruikersrechten kent, dan
            // gaat het ervan uit dat er geen gebruiker bestaat. Dat spaart wel wat rekenwerk
            // uit, en als de user dan toch een gebruiker probeert aan te maken, zal de AD-service
            // de bestaande account wel opleveren.
            // Personen zonder AD-nummer hebben sowieso geen gebruiker.
            if (!persoon.GebruikersRechtV2.Any() || persoon.AdNummer == null)
            {
                return null;
            }

            return _veelGebruikt.GebruikersNaamOphalen(persoon.AdNummer.Value);
        }
    }
}