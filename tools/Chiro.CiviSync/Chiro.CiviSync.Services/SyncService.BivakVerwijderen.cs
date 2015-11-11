/*
   Copyright 2015 Chirojeugd-Vlaanderen vzw

   Licensed under the Apache License, Version 2.0 (the "License");
   you may not use this file except in compliance with the License.
   You may obtain a copy of the License at

       http://www.apache.org/licenses/LICENSE-2.0

   Unless required by applicable law or agreed to in writing, software
   distributed under the License is distributed on an "AS IS" BASIS,
   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
   See the License for the specific language governing permissions and
   limitations under the License.
 */

using System;
using System.Diagnostics;
using System.ServiceModel;
using Chiro.CiviCrm.Api;
using Chiro.CiviCrm.Api.DataContracts;
using Chiro.CiviCrm.Api.DataContracts.Entities;
using Chiro.CiviCrm.Api.DataContracts.Requests;
using Chiro.CiviSync.Logic;
using Chiro.Gap.Log;

namespace Chiro.CiviSync.Services
{
    public partial class SyncService
    {
        /// <summary>
        /// Verwijdert een bivak met GAP-ID <paramref name="uitstapId"/>
        /// </summary>
        /// <param name="uitstapId">GAP-ID van te verwijderen uitstap</param>
        [OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
        public void BivakVerwijderen(int uitstapId)
        {
            Event bivak;
            string stamNr;

            ValideerBivak(uitstapId, out bivak, out stamNr);
            if (bivak == null)
            {
                return;
            }

            var result =
                ServiceHelper.CallService<ICiviCrmApi, ApiResult>(
                    svc => svc.EventDelete(_apiKey, _siteKey, new DeleteRequest(bivak.Id)));
            result.AssertValid();

            _log.Loggen(Niveau.Info,
                String.Format("Bivakaangifte verwijderd voor {0}: {1}. Gap-ID {2}, Civi-ID {3}.", stamNr, bivak.Title,
                    uitstapId, bivak.Id), stamNr, null, null);
        }
    }
}