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
        public async void CommunicatieToevoegen(Persoon persoon, CommunicatieMiddel communicatieMiddel)
        {
            if (persoon.AdNummer == null)
            {
                persoon.AdNummer = AdNummerZoeken(persoon);
            }

            if (persoon.AdNummer == null)
            {
                _log.Loggen(Niveau.Error,
                    String.Format("Kan {0} {1} voor {2} niet bewaren; persoon niet gevonden.", communicatieMiddel.Type,
                        communicatieMiddel.Waarde, persoon),
                    null, null, null);
                return;
            }

            int? contactId = _contactWorker.ContactIdGet(persoon.AdNummer.Value);

            if (contactId == null)
            {
                _log.Loggen(Niveau.Error, String.Format("Onbestaand AD-nummer {0} voor te bewaren lid - als dusdanig terug naar GAP.", persoon.AdNummer),
                    null, persoon.AdNummer, null);

                await _gapUpdateClient.OngeldigAdNaarGap(persoon.AdNummer.Value);
                return;
            }

            var communicatieRequest = CommunicatieLogic.RequestMaken(communicatieMiddel, contactId, true);
            var bestaandeCommunicatie =
                ServiceHelper.CallService<ICiviCrmApi, ApiResult>(
                    svc =>
                        svc.GenericCall(_apiKey, _siteKey, communicatieRequest.EntityType, ApiAction.Get,
                            communicatieRequest));

            bestaandeCommunicatie.AssertValid();
            if (bestaandeCommunicatie.Count != 0)
            {
                _log.Loggen(Niveau.Info,
                    String.Format("Nieuwe communicatievorm ({0}) {1} voor {2} {3} (AD {4}) bestond al.",
                        communicatieMiddel.Type, communicatieMiddel.Waarde, persoon.VoorNaam, persoon.Naam,
                        persoon.AdNummer), null, persoon.AdNummer, persoon.ID);
                return;
            }

            // Maak alleen aan als communicatievorm nog niet bestond.
            var createResult = ServiceHelper.CallService<ICiviCrmApi, ApiResult>(
                svc =>
                    svc.GenericCall(_apiKey, _siteKey, communicatieRequest.EntityType, ApiAction.Create,
                        communicatieRequest));
            createResult.AssertValid();

            _log.Loggen(Niveau.Info,
                String.Format("Nieuwe communicatievorm ({0}) {1} bewaard voor {2} {3} (AD {4}).",
                    communicatieMiddel.Type, communicatieMiddel.Waarde, persoon.VoorNaam, persoon.Naam,
                    persoon.AdNummer), null, persoon.AdNummer, persoon.ID);

        }
    }
}