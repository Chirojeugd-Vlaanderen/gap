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
using System.Linq;
using System.ServiceModel;
using Chiro.CiviCrm.Api;
using Chiro.CiviCrm.Api.DataContracts;
using Chiro.CiviCrm.Api.DataContracts.Entities;
using Chiro.CiviCrm.Api.DataContracts.Requests;
using Chiro.CiviSync.Logic;
using Chiro.Gap.Log;

namespace Chiro.CiviSync.Services
{
    public partial class SyncService
    {
        /// <summary>
        /// Sluit het huidige groepswerkjaar af van de groep met gegeven <paramref name="stamNummer"/>.
        /// </summary>
        /// <remarks>
        /// Dat komt er eigenlijk op neer dat alle actieve lidrelaties worden stopgezet.
        /// Dit is tamelijk gevaarlijk. Als dit faalt, dan loopt de ledensync voor het volgende
        /// werkjaar in het honderd.
        /// </remarks>
        /// <param name="stamNummer"></param>
        [OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
        public void GroepsWerkjaarAfsluiten(string stamNummer)
        {
            int? civiGroepId = _contactWorker.ContactIdGet(stamNummer);
            if (civiGroepId == null)
            {
                _log.Loggen(Niveau.Error, String.Format("Onbestaande groep {0} - geen werkjaar afgesloten.", stamNummer),
                    stamNummer, null, null);
                return;
            }

            var request = new RelationshipRequest
            {
                ContactIdB = civiGroepId,
                IsActive = true,
                RelationshipTypeId = (int)RelatieType.LidVan,
                RelationshipSaveRequest = new[]
                {
                    new RelationshipRequest
                    {
                        IdValueExpression = "$value.id",
                        IsActive = false,
                        EndDate = DateTime.Now
                    }
                }
            };

            var result = ServiceHelper.CallService<ICiviCrmApi, ApiResultValues<Relationship>>(
                svc =>
                    svc.RelationshipGet(_apiKey, _siteKey, request));
            result.AssertValid();
            _log.Loggen(Niveau.Info,
                String.Format(
                    "Werkjaar afgesloten voor groep {0}. {1} lidrelatie(s) beeindigd.",
                    stamNummer, result.Count),
                stamNummer, null, null);
        }
    }
}