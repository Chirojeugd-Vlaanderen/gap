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
using Chiro.CiviCrm.Api.DataContracts.Requests;
using Chiro.CiviSync.Logic;
using Chiro.Gap.Log;
using Chiro.Kip.ServiceContracts.DataContracts;

namespace Chiro.CiviSync.Services
{
    public partial class SyncService
    {
        /// <summary>
        /// Verwijdert een communicatiemiddel uit ChiroCivi.
        /// </summary>
        /// <param name="persoon">
        /// Persoonsgegevens van de persoon waarvan het communicatiemiddel moet verdwijnen.
        /// </param>
        /// <param name="communicatieMiddel">
        /// Gegevens over het te verwijderen communicatiemiddel
        /// </param>
        [OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
        public async void CommunicatieVerwijderen(Persoon persoon, CommunicatieMiddel communicatieMiddel)
        {
            if (persoon.AdNummer == null)
            {
                persoon.AdNummer = AdNummerZoeken(persoon);
            }

            if (persoon.AdNummer == null)
            {
                _log.Loggen(Niveau.Error,
                    String.Format("Kan {0} {1} voor {2} niet verwijderen; persoon niet gevonden.", communicatieMiddel.Type,
                        communicatieMiddel.Waarde, persoon),
                    null, null, null);
                return;
            }

            int? contactId = _contactWorker.ContactIdGet(persoon.AdNummer.Value);

            if (contactId == null)
            {
                _log.Loggen(Niveau.Error, String.Format("Onbestaand AD-nummer {0} voor te verwijderen communicatie - als dusdanig terug naar GAP.", persoon.AdNummer),
                    null, persoon.AdNummer, null);

                await _gapUpdateClient.OngeldigAdNaarGap(persoon.AdNummer.Value);
                return;
            }

            var communicatieRequest = CommunicatieLogic.RequestMaken(communicatieMiddel, contactId, true);

            // Zoek er hoogstens 1. Dan hebben we een een Id in het result.
            communicatieRequest.ApiOptions = new ApiOptions {Limit = 1};
            var bestaandeCommunicatie =
                ServiceHelper.CallService<ICiviCrmApi, ApiResult>(
                    svc =>
                        svc.GenericCall(_apiKey, _siteKey, communicatieRequest.EntityType, ApiAction.Get,
                            communicatieRequest));

            bestaandeCommunicatie.AssertValid();
            if (bestaandeCommunicatie.Count == 0)
            {
                _log.Loggen(Niveau.Info,
                    String.Format("Kon niet-gevonden communicatievorm ({0}) {1} voor {2} {3} (AD {4}) niet verwijderen.",
                        communicatieMiddel.Type, communicatieMiddel.Waarde, persoon.VoorNaam, persoon.Naam,
                        persoon.AdNummer), null, persoon.AdNummer, persoon.ID);
                return;
            }
            Debug.Assert(bestaandeCommunicatie.Id.HasValue);

            // Verwijder de communicatievorm
            var createResult = ServiceHelper.CallService<ICiviCrmApi, ApiResult>(
                svc =>
                    svc.GenericCall(_apiKey, _siteKey, communicatieRequest.EntityType, ApiAction.Delete,
                        new IdRequest(bestaandeCommunicatie.Id.Value)));
            createResult.AssertValid();

            _log.Loggen(Niveau.Info,
                String.Format("Communicatievorm ({0}) {1} verwijderd voor {2} {3} (AD {4}).",
                    communicatieMiddel.Type, communicatieMiddel.Waarde, persoon.VoorNaam, persoon.Naam,
                    persoon.AdNummer), null, persoon.AdNummer, persoon.ID);
        }
    }
}