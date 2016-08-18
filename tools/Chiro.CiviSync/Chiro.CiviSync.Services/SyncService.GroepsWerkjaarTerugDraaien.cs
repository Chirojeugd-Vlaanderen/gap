/*
   Copyright 2016 Chirojeugd-Vlaanderen vzw

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
using System.Diagnostics;
using System.Linq;
using System.Web;
using Chiro.CiviCrm.Api;
using Chiro.CiviCrm.Api.DataContracts;
using Chiro.CiviCrm.Api.DataContracts.Entities;
using Chiro.CiviCrm.Api.DataContracts.Filters;
using Chiro.CiviCrm.Api.DataContracts.Requests;
using Chiro.CiviSync.Logic;
using Chiro.Gap.Log;

namespace Chiro.CiviSync.Services
{
    public partial class SyncService
    {

        /// <summary>
        /// Herstelt lidrelaties naar de toestand voor de gegeven <paramref name="datum"/>.
        /// </summary>
        /// <param name="stamNummer">Stamnummer van ploeg waarvan lidrelaties hersteld moeten worden.</param>
        /// <param name="datum"></param>
        public void GroepsWerkjaarTerugDraaien(string stamNummer, DateTime datum)
        {
            Debug.Assert(datum > DateTime.MinValue);
            int aantal;
            int? civiGroepId = _contactWorker.ContactIdGet(stamNummer);
            if (civiGroepId == null)
            {
                _log.Loggen(Niveau.Error, String.Format("Onbestaande groep {0} - geen werkjaar afgesloten.", stamNummer),
                    stamNummer, null, null);
                return;
            }

            _log.Loggen(Niveau.Info,
                String.Format(
                    "Jaarovergang ongedaan maken voor groep {0}. Terug naar situatie op {1:dd/MM/yyyy}.",
                    stamNummer, datum),
                stamNummer, null, null);

            // Lidrelaties van het nieuwe werkjaar verwijderen
            do
            {
                var request = new RelationshipRequest
                {
                    ContactIdB = civiGroepId,
                    RelationshipTypeId = (int) RelatieType.LidVan,
                    ReturnFields = "id",
                    StartDateFilter = new Filter<DateTime?>(WhereOperator.Gte, datum),
                    RelationshipDeleteRequest = new[] {new DeleteRequest {IdValueExpression = "$value.id"}}
                };
                var result = ServiceHelper.CallService<ICiviCrmApi, ApiResultValues<Relationship>>(
                    svc =>
                        svc.RelationshipGet(_apiKey, _siteKey, request));
                result.AssertValid();
                aantal = result.Count;
                _log.Loggen(Niveau.Info,
                    String.Format(
                        "{1} lidrelatie(s) verwijderd voor groep {0}.",
                        stamNummer, aantal),
                    stamNummer, null, null);
            } while (aantal > 0);

            // Lidrelaties van het oude werkjaar opnieuw activeren
            do
            {
                var request = new RelationshipRequest
                {
                    ContactIdB = civiGroepId,
                    RelationshipTypeId = (int)RelatieType.LidVan,
                    ReturnFields = "id",
                    EndDateFilter = new Filter<DateTime?>(WhereOperator.Gte, datum),
                    IsActive = false,
                    RelationshipSaveRequest = new[]
                    {
                        new RelationshipRequest
                        {
                            IdValueExpression = "$value.id",
                            IsActive = true,
                            // Verwijder einddatum
                            EndDate = DateTime.MinValue
                        }
                    }
                };
                var result = ServiceHelper.CallService<ICiviCrmApi, ApiResultValues<Relationship>>(
                    svc =>
                        svc.RelationshipGet(_apiKey, _siteKey, request));
                result.AssertValid();
                aantal = result.Count;
                _log.Loggen(Niveau.Info,
                    String.Format(
                        "{1} lidrelatie(s) opnieuw geactiveerd voor groep {0}.",
                        stamNummer, aantal),
                    stamNummer, null, null);
            } while (aantal > 0);
        }
    }
}