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
using Chiro.CiviSync.Logic;
using Chiro.Gap.Log;
using Chiro.Kip.ServiceContracts.DataContracts;

namespace Chiro.CiviSync.Services
{
    public partial class SyncService
    {
        /// <summary>
        /// Gaat op zoek naar de gegeven <paramref name="persoon"/>, en zoekt daarvan de communicatie van
        /// type <c>communicatieMiddel.Type</c> en nummer <paramref name="nummerBijTeWerken"/>. Dat
        /// gevonden communicatiemiddel wordt vervangen door <paramref name="communicatieMiddel"/>.
        /// </summary>
        /// <param name="persoon">persoon met te vervangen communicatiemiddel</param>
        /// <param name="nummerBijTeWerken">huidig nummer van te vervangen communicatiemiddel</param>
        /// <param name="communicatieMiddel">nieuwe info voor te vervangen communicatiemiddel</param>
        [OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
        public void CommunicatieBijwerken(Persoon persoon, string nummerBijTeWerken, CommunicatieMiddel communicatieMiddel)
        {
            int? contactId = CiviIdGet(persoon, "Communicatie bijwerken.");
            if (contactId == null) return;

            var teVervangen = new CommunicatieMiddel
            {
                Type = communicatieMiddel.Type,
                Waarde = nummerBijTeWerken,
                IsBulk = communicatieMiddel.IsBulk,
            };

            // Zoek de te vervangen communicatie
            var oudeCommunicatieRequest = CommunicatieLogic.RequestMaken(teVervangen, contactId, true);

            // Zoek er hoogstens 1. Dan hebben we een een Id in het result.
            oudeCommunicatieRequest.ApiOptions = new ApiOptions { Limit = 1 };
            var bestaandeCommunicatie =
                ServiceHelper.CallService<ICiviCrmApi, ApiResult>(
                    svc =>
                        svc.GenericCall(_apiKey, _siteKey, oudeCommunicatieRequest.EntityType, ApiAction.Get,
                            oudeCommunicatieRequest));
            bestaandeCommunicatie.AssertValid();

            // Maak een request voor de nieuwe
            var nieuweCommunicatieRequest = CommunicatieLogic.RequestMaken(communicatieMiddel, contactId, false);

            if (bestaandeCommunicatie.Count == 0)
            {
                _log.Loggen(Niveau.Info,
                    String.Format("Kon bestaande communicatievorm ({0}) {1} voor {2} {3} (AD {4}) niet vinden. Nieuwe {5} gewoon toegevoegd.",
                        communicatieMiddel.Type, nummerBijTeWerken, persoon.VoorNaam, persoon.Naam,
                        persoon.AdNummer, communicatieMiddel.Waarde), null, persoon.AdNummer, persoon.ID);
            }
            else
            {
                Debug.Assert(bestaandeCommunicatie.Id.HasValue);
                _log.Loggen(Niveau.Info,
                    String.Format(
                        "Bestaande communicatievorm ({0}) {1} voor {2} {3} (AD {4}) vervangen door {5}.",
                        communicatieMiddel.Type, nummerBijTeWerken, persoon.VoorNaam, persoon.Naam,
                        persoon.AdNummer, communicatieMiddel.Waarde), null, persoon.AdNummer, persoon.ID);
                CommunicatieLogic.RequestIdZetten(nieuweCommunicatieRequest, bestaandeCommunicatie.Id.Value);
            }

            // Bewaar de communicatievorm
            var createResult = ServiceHelper.CallService<ICiviCrmApi, ApiResult>(
                svc =>
                    svc.GenericCall(_apiKey, _siteKey, nieuweCommunicatieRequest.EntityType, ApiAction.Create,
                        nieuweCommunicatieRequest));
            createResult.AssertValid();
        }
    }
}