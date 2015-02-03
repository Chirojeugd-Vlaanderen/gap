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
using System.Linq;
using Chiro.CiviCrm.Api;
using Chiro.CiviCrm.Api.DataContracts;
using Chiro.CiviCrm.Api.DataContracts.Entities;
using Chiro.CiviCrm.Api.DataContracts.EntityRequests;
using Chiro.CiviCrm.Api.DataContracts.Requests;
using Chiro.CiviSync.Helpers;
using Chiro.Gap.Log;
using Chiro.Kip.ServiceContracts.DataContracts;

namespace Chiro.CiviSync.Services
{
    public partial class SyncService
    {
        /// <summary>
        /// Maakt een persoon met gekend ad-nummer lid, of updatet een bestaand lid
        /// </summary>
        /// <param name="adNummer">
        /// AD-nummer van het opgegeven lid
        /// </param>
        /// <param name="gedoe">
        /// De nodige info voor het lid.
        /// </param>
        public void LidBewaren(int adNummer, LidGedoe gedoe)
        {
            RelationshipRequest relationshipRequest;

            int? civiGroepId = _contactHelper.ContactIdGet(gedoe.StamNummer);
            if (civiGroepId == null)
            {
                _log.Loggen(Niveau.Error, String.Format("Onbestaande groep {0} - lid niet bewaard.", gedoe.StamNummer),
                    gedoe.StamNummer, adNummer, null);
                return;
            }

            // Haal de persoon op met gegeven AD-nummer en zijn recentste lidrelatie in de gevraagde groep.
            var contactRequest = new ContactRequest
            {
                ExternalIdentifier = adNummer.ToString(),
                RelationshipGetRequest = new RelationshipRequest
                {
                    RelationshipTypeId = (int)RelatieType.LidVan,
                    ContactIdAValueExpression = "$value.id",
                    ContactIdB = civiGroepId,
                    ApiOptions = new ApiOptions{Sort = "start_date DESC", Limit = 1}
                }
            };

            var contact =
                ServiceHelper.CallService<ICiviCrmApi, Contact>(
                    svc => svc.ContactGetSingle(_apiKey, _siteKey, contactRequest));

            if (contact == null)
            {
                _log.Loggen(Niveau.Error, String.Format("Onbestaand AD-nummer {0} - lid niet bewaard.", adNummer),
                    gedoe.StamNummer, adNummer, null);
                return;
            }

            if (contact.RelationshipResult.Count == 1)
            {
                var bestaandeRelatie = contact.RelationshipResult.Values.First();
                if (_relationshipHelper.WerkjaarGet(bestaandeRelatie) == gedoe.WerkJaar)
                {
                    // Recupereer (vooral ID) van bestaande relatie.
                }
            }
        }
    }
}