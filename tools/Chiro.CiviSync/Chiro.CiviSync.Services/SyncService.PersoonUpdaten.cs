﻿/*
   Copyright 2013-2015 Chirojeugd-Vlaanderen vzw

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
using AutoMapper;
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
        /// Updatet de persoonsgegevens van een bestaand <paramref name="persoon"/> in CiviCRM.
        /// </summary>
        /// <param name="persoon">Persoon wiens gegevens te updaten zijn</param>
        [OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
        public async void PersoonUpdaten(Persoon persoon)
        {
            if (persoon.AdNummer == null)
            {
                persoon.AdNummer = _contactWorker.AdNummerZoeken(persoon);

                if (persoon.AdNummer == null)
                {
                    _log.Loggen(Niveau.Error,
                        String.Format("Update voor onbekend persoon {0} (id {1}) genegeerd.", persoon, persoon.ID), null, null,
                        persoon.ID);
                    return;
                }
            }

            // Controleer de geldigheid van het AD-nummer door het contact-ID op te vragen.
            // (Dat is wat overkill, maar nodig voor #3688)
            int? contactId = _contactWorker.ContactIdGet(persoon.AdNummer.Value);

            if (contactId == null)
            {
                _log.Loggen(Niveau.Error, String.Format("Onbestaand AD-nummer {0} voor updaten persoonsinfo - als dusdanig terug naar GAP.", persoon.AdNummer),
                    null, persoon.AdNummer, null);

                await _gapUpdateClient.OngeldigAdNaarGap(persoon.AdNummer.Value);
                return;
            }

            // Ik map de persoon naar een ContactRequest.
            var request = Mapper.Map<Persoon, ContactRequest>(persoon);
            request.ApiOptions = new ApiOptions { Match = "external_identifier" };

            var result = ServiceHelper.CallService<ICiviCrmApi, ApiResult>(svc => svc.ContactSave(
                _apiKey,
                _siteKey,
                request));

            result.AssertValid();

            _log.Loggen(
                Niveau.Info,
                String.Format("Contact {0} {1} bewaard (gid {3}, AD {2}).", persoon.VoorNaam, persoon.Naam,
                    persoon.AdNummer, result.Id),
                null,
                persoon.AdNummer,
                persoon.ID);
        }
    }
}