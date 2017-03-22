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
using System.Linq;
using System.ServiceModel;
using Chiro.CiviCrm.Api;
using Chiro.CiviCrm.Api.DataContracts;
using Chiro.CiviCrm.Api.DataContracts.Entities;
using Chiro.CiviCrm.Api.DataContracts.Requests;
using Chiro.CiviSync.Logic;
using Chiro.Gap.Log;
using Chiro.Kip.ServiceContracts.DataContracts;

namespace Chiro.CiviSync.Services
{
    public partial class SyncService
    {
        /// <summary>
        /// Bewaart een bivak, zonder contactpersoon of adres
        /// </summary>
        /// <param name="bivak">
        /// Gegevens voor de bivakaangifte
        /// </param>
        [OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
        public void BivakBewaren(Bivak bivak)
        {
            Event oudEvent;
            int? contactIdPloeg = _contactWorker.ContactIdGet(bivak.StamNummer);

            if (contactIdPloeg == null)
            {
                _log.Loggen(Niveau.Error,
                    String.Format("Onbestaand stamnummer {0} voor te bewaren bivak.", bivak.StamNummer),
                    bivak.StamNummer, null, null);
                return;
            }

            // Haal bivak op, met OrganiserendePloeg1Id.
            var apiResult = ServiceHelper.CallService<ICiviCrmApi, ApiResultValues<Event>>(
                svc =>
                    svc.EventGet(_apiKey, _siteKey,
                        new EventRequest {GapUitstapId = bivak.UitstapID, ReturnFields = "id,custom_48"}));
            apiResult.AssertValid();

            if (apiResult.Count == 0)
            {
                oudEvent = null;
            }
            else
            {
                if (apiResult.Count > 1)
                {
                    _log.Loggen(Niveau.Error,
                        String.Format(
                            "Meerdere bivakken met zelfde GAP-uitstapID {1} (stamnr {0}). We doen maar iets.",
                            bivak.StamNummer, bivak.UitstapID),
                        bivak.StamNummer, null, null);
                }
                oudEvent = apiResult.Values.First();
            }

            var eventRequest = new EventRequest
            {
                OrganiserendePloeg1Id = _contactWorker.ContactIdGet(bivak.StamNummer)
            };

            MappingHelper.Map(bivak, eventRequest);

            if (oudEvent != null && oudEvent.Id != 0)
            {
                // Een stamnummer van een bivakaangifte mag niet veranderen.
                Debug.Assert(oudEvent.OrganiserendePloeg1Id == contactIdPloeg);
                
                // Als er al een event bestond, dan nemen we het ID over. De API-call overschrijft
                // straks enkel wat gegeven is in 'bivak'.
                eventRequest.Id = oudEvent.Id;

                _log.Loggen(Niveau.Info,
                    String.Format("Bestaande bivakaangifte bijwerken voor {0}: {1}. Gap-ID {2}, Civi-ID {3}.",
                        bivak.StamNummer, bivak.Naam,
                        bivak.UitstapID, eventRequest.Id), bivak.StamNummer,
                    null, null);
            }
            // Overschrijf nieuwe/bestaande met aangeleverde informatie.

            var result =
                ServiceHelper.CallService<ICiviCrmApi, ApiResultValues<Event>>(
                    svc => svc.EventSave(_apiKey, _siteKey, eventRequest));
            result.AssertValid();

            _log.Loggen(Niveau.Info,
                String.Format("Bivakaangifte maken voor {0}: {1} bewaard. Gap-ID {2}, Civi-ID {3}.", bivak.StamNummer,
                    bivak.Naam,
                    bivak.UitstapID, result.Id), bivak.StamNummer,
                null, null);
        }
    }
}