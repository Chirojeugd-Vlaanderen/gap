/*
 * Copyright 2015 Chirojeugd-Vlaanderen vzw. See the NOTICE file at the 
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

using Chiro.Kip.ServiceContracts.DataContracts;

namespace Chiro.Mailchimp.Sync
{
    // TODO: Ondersteunen van meerdere lijsten.
    // Je zou het list-ID kunnen meegeven.

    public interface IChimpSyncHelper
    {
        /// <summary>
        /// Maakt of updatet het mailchimpabonnement met gegeven <paramref name="abonnementInfo"/>.
        /// </summary>
        /// <param name="abonnementInfo">Info over te maken/updaten abonnement.</param>
        void AbonnementSyncen(AbonnementInfo abonnementInfo);
        /// <summary>
        /// Verwijdert het mailchimpabonnement voor het gegeven <paramref name="eMailAdres"/>
        /// </summary>
        /// <param name="eMailAdres">E-mailadres dat van Mailchimp verwijderd moet worden.</param>
        void AbonnementVerwijderen(string eMailAdres);
    }
}