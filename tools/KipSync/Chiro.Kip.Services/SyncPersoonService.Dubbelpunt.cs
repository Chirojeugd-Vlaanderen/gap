/*
 * Copyright 2015 Chirojeugd-Vlaanderen vzw. See the NOTICE file at the 
 * top-level directory of this distribution, and at
 * https://develop.chiro.be/gap/wiki/copyright
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
using Chiro.Kip.ServiceContracts.DataContracts;
using Chiro.Mailchimp.Sync;

namespace Chiro.Kip.Services
{
    public partial class SyncPersoonService
    {
        /// <summary>
        /// Werkt een dubbelpuntabonnement bij in Mailchimp. Spannend.
        /// </summary>
        /// <param name="abonnementInfo">Alle informatie over het abonnement.</param>
        [OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
        public void AbonnementNaarMailchimp(AbonnementInfo abonnementInfo)
        {
            var syncHelper = new SyncHelper();
            syncHelper.AbonnementSyncen(abonnementInfo);
            _log.BerichtLoggen(0,
                String.Format("DubbelpuntAbonnement voor {0} {1} ({2}, {3}) bijgewerkt. Type {4}.",
                    abonnementInfo.VoorNaam, abonnementInfo.Naam, abonnementInfo.EmailAdres, abonnementInfo.Adres,
                    abonnementInfo.AbonnementType));
        }
    }
}
