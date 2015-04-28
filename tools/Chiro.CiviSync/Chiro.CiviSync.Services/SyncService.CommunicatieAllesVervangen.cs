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
using System.Collections.Generic;
using System.ServiceModel;
using Chiro.CiviCrm.Api;
using Chiro.CiviCrm.Api.DataContracts;
using Chiro.CiviCrm.Api.DataContracts.Requests;
using Chiro.CiviSync.Logic;
using Chiro.Gap.Log;
using Chiro.Kip.ServiceContracts.DataContracts;

namespace Chiro.CiviSync.Services
{
    public partial class SyncService
    {
        /// <summary>
        /// Verwijdert alle bestaande contactinfo, en vervangt door de contactinfo meegegeven in 
        /// <paramref name="communicatieMiddelen"/>.
        /// </summary>
        /// <param name="persoon">
        /// Persoon voor wie we contactinfo updaten
        /// </param>
        /// <param name="communicatieMiddelen">
        /// Te updaten contactinfo
        /// </param>
        /// <remarks>
        /// Dit wordt niet (meer) door GAP gebruikt, maar we zullen het behouden. Lijkt me wel nuttig om
        /// zaken te fixen als er iets misgelopen is met de communicatiesync.
        /// </remarks>
        [OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
        public void AlleCommunicatieBewaren(Persoon persoon, IEnumerable<CommunicatieMiddel> communicatieMiddelen)
        {
            // Verwijder eerst alle bestaande communicatie. Ik vermoed dat dat
            // één voor één moet gebeuren.

            int? contactId = CiviIdGet(persoon, "Alle communicatie overnemen.");
            if (contactId == null) return;

            _communicatieWorker.AlleTelefoonEnFaxVerwijderen(contactId.Value);
            _communicatieWorker.AlleEmailVerwijderen(contactId.Value);
            _communicatieWorker.AlleWebsitesVerwijderen(contactId.Value);
            _communicatieWorker.AlleImVerwijderen(contactId.Value);

            var saveRequest = new ContactRequest {Id = contactId};
            CommunicatieLogic.RequestsChainen(saveRequest, communicatieMiddelen);

            ServiceHelper.CallService<ICiviCrmApi, EmptyResult>(
                svc => svc.ContactSaveWorkaroundCrm15815(_apiKey, _siteKey, saveRequest));

            _log.Loggen(Niveau.Info, String.Format("Communicatievormen vervangen voor {0}).", persoon), null,
                persoon.AdNummer, persoon.ID);
        }
    }
}