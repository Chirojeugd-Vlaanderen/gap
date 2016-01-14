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

using System;
using System.ServiceModel;
using Chiro.Gap.Log;
using Chiro.Kip.ServiceContracts.DataContracts;

namespace Chiro.CiviSync.Services
{
    public partial class SyncService
    {
        /// <summary>
        /// Werkt een dubbelpuntabonnement bij in Mailchimp. Spannend.
        /// </summary>
        /// <param name="abonnementInfo">Alle informatie over het abonnement.</param>
        [OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
        public void AbonnementNaarMailchimp(AbonnementInfo abonnementInfo)
        {
            _chimpSyncHelper.AbonnementSyncen(abonnementInfo);
            _log.Loggen(Niveau.Debug,
                String.Format("DubbelpuntAbonnement voor {0} {1} ({2}, {3}) bijgewerkt. Type {4}.",
                    abonnementInfo.VoorNaam, abonnementInfo.Naam, abonnementInfo.EmailAdres, abonnementInfo.Adres,
                    abonnementInfo.AbonnementType), abonnementInfo.StamNr, null, abonnementInfo.GapPersoonId);
        }

        /// <summary>
        /// Verwijdert Dubbelpuntabonnement voor persoon met gegeven <paramref name="eMail"/>.
        /// </summary>
        /// <param name="eMail">E-mailadres (of dummy-e-mailadres) van te verwijderen abonnement.</param>
        [OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
        public void AbonnementVerwijderen(string eMail)
        {
            _chimpSyncHelper.AbonnementVerwijderen(eMail);
            _log.Loggen(Niveau.Debug,
                string.Format("DubbelpuntAbonnement verwijderd: {0}.",
                    eMail), null, null, null);
        }
    }
}
