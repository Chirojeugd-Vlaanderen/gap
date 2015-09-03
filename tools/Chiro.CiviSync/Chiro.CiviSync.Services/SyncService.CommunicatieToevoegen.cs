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
using System.ServiceModel;
using Chiro.CiviCrm.Api;
using Chiro.CiviCrm.Api.DataContracts;
using Chiro.CiviSync.Logic;
using Chiro.Gap.Log;
using Chiro.Kip.ServiceContracts.DataContracts;

namespace Chiro.CiviSync.Services
{
    public partial class SyncService
    {
        /// <summary>
        /// Voegt 1 communicatiemiddel toe aan de communicatiemiddelen van een persoon
        /// </summary>
        /// <param name="persoon">
        /// Persoon die het nieuwe communicatiemiddel krijgt
        /// </param>
        /// <param name="communicatieMiddel">
        /// Het nieuwe communicatiemiddel
        /// </param>
        [OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
        public void CommunicatieToevoegen(Persoon persoon, CommunicatieMiddel communicatieMiddel)
        {
            int? contactId = CiviIdGet(persoon, "Communicatie toevoegen.");
            if (contactId == null) return;

            var communicatieRequest = CommunicatieLogic.RequestMaken(communicatieMiddel, contactId, true);
            var bestaandeCommunicatie =
                ServiceHelper.CallService<ICiviCrmApi, ApiResult>(
                    svc =>
                        svc.GenericCall(_apiKey, _siteKey, communicatieRequest.EntityType, ApiAction.Get,
                            communicatieRequest));

            bestaandeCommunicatie.AssertValid();

            // We bewaren sowieso opnieuw. Maar dan moeten we een request hebben dat wel
            // voor saven gebruikt kan worden (dus inclusief de IsBulkMail)
            communicatieRequest = CommunicatieLogic.RequestMaken(communicatieMiddel, contactId, false);

            if (bestaandeCommunicatie.Count != 0)
            {
                // Overschrijf bestaande als dat van toepassing is.
                communicatieRequest.Id = bestaandeCommunicatie.Id;
            }

            // Maak alleen aan als communicatievorm nog niet bestond.
            // Pak wel de voorkeur mee.
            var createResult = ServiceHelper.CallService<ICiviCrmApi, ApiResult>(
                svc =>
                    svc.GenericCall(_apiKey, _siteKey, communicatieRequest.EntityType, ApiAction.Create,
                        communicatieRequest));
            createResult.AssertValid();

            _log.Loggen(Niveau.Info,
                String.Format("{5} communicatievorm ({0}) {1} bewaard voor {2} {3} (AD {4}).",
                    communicatieMiddel.Type, communicatieMiddel.Waarde, persoon.VoorNaam, persoon.Naam,
                    persoon.AdNummer, bestaandeCommunicatie.Count > 0 ? "Bestaande" : "Nieuwe"), null, persoon.AdNummer,
                persoon.ID);

        }
    }
}