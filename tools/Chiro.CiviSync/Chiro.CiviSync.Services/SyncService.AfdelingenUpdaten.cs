﻿/*
   Copyright 2015, 2016 Chirojeugd-Vlaanderen vzw

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
        /// Updatet de afdelingen van een actief lid.
        /// </summary>
        /// <param name="persoon">
        /// Persoon waarvan de afdelingen geupdatet moeten worden
        /// </param>
        /// <param name="stamNummer">
        /// Stamnummer van de groep waarin de persoon lid is
        /// </param>
        /// <param name="afdelingen">
        /// Toe te kennen (officiele) afdelingen.  Eventuele andere reeds toegekende afdelingen worden verwijderd.
        /// </param>
        /// <remarks>
        /// Er is in Kipadmin maar plaats voor 2 afdelingen/lid
        /// </remarks>
        [OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
        public void AfdelingenUpdaten(Persoon persoon, string stamNummer, AfdelingEnum[] afdelingen)
        {
            int? contactIdPersoon = CiviIdGet(persoon, "Afdeling updaten");
            if (contactIdPersoon == null) return;

            int? contactIdGroep = _contactWorker.ContactIdGet(stamNummer);
            if (contactIdGroep == null)
            {
                _log.Loggen(Niveau.Error,
                    String.Format("Onbestaande groep {0} - lid {1} niet bewaard.", stamNummer, persoon),
                    stamNummer, persoon.AdNummer, null);
                return;
            }

            var lid = _lidWorker.ActiefLidOphalen(contactIdPersoon, contactIdGroep);
            if (lid == null)
            {
                _log.Loggen(Niveau.Error,
                    String.Format("Geen actieve lidrelatie gevonden voor {1} in ploeg {0}. Kon afdelingen niet updaten.",
                        stamNummer, persoon),
                    stamNummer, persoon.AdNummer, null);
                return;
            }

            // Nieuw request
            var request = new RelationshipRequest
            {
                Id = lid.Id
            };

            // Het lidtype verandert niet. Dus de afdeling van het opgehaalde
            // lid bepaalt of het om leiding gaat of kinderen.

            if (lid.Afdeling == Afdeling.Leiding)
            {
                request.LeidingVan = MappingHelper.Map<IEnumerable<AfdelingEnum>, Afdeling[]>(afdelingen);
            }
            else
            {
                request.Afdeling = MappingHelper.Map<AfdelingEnum, Afdeling>(afdelingen.First());
            }

            var result =
                ServiceHelper.CallService<ICiviCrmApi, ApiResultValues<Relationship>>(
                    svc => svc.RelationshipSave(_apiKey, _siteKey, request));
            result.AssertValid();

            _log.Loggen(Niveau.Info,
                String.Format(
                    "{0} stamnr {1} - nieuwe afdeling(en) {3} voor relatie met startdatum {2:dd/MM/yyyy} relID {4}",
                    persoon, stamNummer, lid.StartDate, String.Join(",", afdelingen.Select(afd => afd.ToString())), result.Id),
                stamNummer, persoon.AdNummer, persoon.ID);
        }
    }
}