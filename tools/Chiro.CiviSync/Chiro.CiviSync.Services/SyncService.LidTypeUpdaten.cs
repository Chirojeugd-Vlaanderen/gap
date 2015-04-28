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
using System.Linq;
using System.ServiceModel;
using System.Web;
using Chiro.CiviCrm.Api;
using Chiro.CiviCrm.Api.DataContracts.Requests;
using Chiro.Gap.Log;
using Chiro.Kip.ServiceContracts.DataContracts;
using Chiro.CiviCrm.Api.DataContracts;
using Chiro.CiviCrm.Api.DataContracts.Entities;
using Chiro.CiviSync.Logic;

namespace Chiro.CiviSync.Services
{   
    public partial class SyncService
    {
        /// <summary>
        /// Stelt het lidtype van het lid in, bepaald door <paramref name="persoon"/>, <paramref name="stamNummer"/>
        /// en <paramref name="werkJaar"/>.
        /// </summary>
        /// <param name="persoon">
        /// Persoon van wie het lidtype aangepast moet worden
        /// </param>
        /// <param name="stamNummer">
        /// Stamnummer van groep waarin de persoon lid is
        /// </param>
        /// <param name="werkJaar">
        /// Werkjaar waarvoor het lidtype moet aangepast worden
        /// </param>
        /// <param name="lidType">
        /// Nieuw lidtype
        /// </param>
        [OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
        public void LidTypeUpdaten(Persoon persoon, string stamNummer, int werkJaar, LidTypeEnum lidType)
        {
            int? contactIdPersoon = CiviIdGet(persoon, "LidType updaten");
            if (contactIdPersoon == null) return;

            int? contactIdGroep = _contactWorker.ContactIdGet(stamNummer);
            if (contactIdGroep == null)
            {
                _log.Loggen(Niveau.Error,
                    String.Format("Onbestaande groep {0} - lid {1} niet bewaard.", stamNummer, persoon),
                    stamNummer, persoon.AdNummer, null);
                return;
            }

            var lid = _lidWorker.LidOphalen(contactIdPersoon, contactIdGroep, werkJaar);
            if (lid == null)
            {
                _log.Loggen(Niveau.Error,
                    String.Format("Lid {1} niet gevonden in groep {0}, werkjaar {2}. Kon afdelingen niet updaten.",
                        stamNummer, persoon, werkJaar),
                    stamNummer, persoon.AdNummer, null);
                return;
            }

            // Als het lidtype niet verandert, dan doen we niets.
            if (lid.Afdeling == Afdeling.Leiding && (lidType == LidTypeEnum.Leiding || lidType == LidTypeEnum.Kader)
                || lid.Afdeling != Afdeling.Leiding && (lidType == LidTypeEnum.Kind))
            {
                return;
            }

            var request = new RelationshipRequest
            {
                Id = lid.Id
            };

            if (lidType == LidTypeEnum.Kind)
            {
                // Als de leid(st)er afdelingen had, behoud dan de eerste.
                // Als er geen afdeling is, dan kiezen we 'Speciaal'. Wat niet juist is. Maar
                // als we niets meegeven, gaat civicrm.net de afdeling niet aanpassen, en dan
                // blijft er 'Leiding' staan, en dat is nog erger.
                // In praktijk zal een call naar het veranderen van een lidtype naar Kind toch
                // steeds gevolgd worden door een call naar AfdelingZetten.
                request.Afdeling = lid.LeidingVan.Any() ? lid.LeidingVan.First() : Afdeling.Speciaal;
                request.LeidingVan = new Afdeling[0];
            }
            else
            {
                if (lid.Afdeling.HasValue)
                {
                    request.LeidingVan = new Afdeling[] {lid.Afdeling.Value};
                }
                request.Afdeling = Afdeling.Leiding;
            }

            var result =
                ServiceHelper.CallService<ICiviCrmApi, ApiResultValues<Relationship>>(
                    svc => svc.RelationshipSave(_apiKey, _siteKey, request));
            result.AssertValid();

            _log.Loggen(Niveau.Info,
                String.Format(
                    "{0} stamnr {1} werkjaar {2} - nieuw lidtype {3}, lidid {4}.",
                    persoon, stamNummer, werkJaar, lidType, result.Id),
                stamNummer, persoon.AdNummer, persoon.ID);
        }
    }
}