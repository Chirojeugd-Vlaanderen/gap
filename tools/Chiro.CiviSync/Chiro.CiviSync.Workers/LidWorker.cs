﻿/*
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

using System.Diagnostics;
using System.Linq;
using Chiro.Cdf.ServiceHelper;
using Chiro.CiviCrm.Api;
using Chiro.CiviCrm.Api.DataContracts;
using Chiro.CiviCrm.Api.DataContracts.Entities;
using Chiro.CiviCrm.Api.DataContracts.Requests;
using Chiro.CiviSync.Logic;
using Chiro.Gap.Log;

namespace Chiro.CiviSync.Workers
{
    /// <summary>
    /// Api-handigheidjes voor leden
    /// </summary>
    public class LidWorker: BaseWorker
    {
        private readonly RelationshipLogic _relationshipLogic;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="serviceHelper"></param>
        /// <param name="log"></param>
        /// <param name="relationshipLogic"></param>
        public LidWorker(ServiceHelper serviceHelper, IMiniLog log, RelationshipLogic relationshipLogic,
            ICiviCache cache) : base(serviceHelper, log, cache)
        {
            _relationshipLogic = relationshipLogic;
        }

        /// <summary>
        /// Haalt de actieve lidrelatie op tussen de persoon met ID <paramref name="contactIdPersoon"/>
        /// en de groep (ploeg) met ID <paramref name="contactIdGroep"/>.
        /// </summary>
        /// <param name="contactIdPersoon">Civi-ID van persoon</param>
        /// <param name="contactIdGroep">Civi-ID van groep</param>
        /// <returns>Actieve lidrelatie op tussen de persoon met ID <paramref name="contactIdPersoon"/>
        /// en de groep (ploeg) met ID <paramref name="contactIdGroep"/>.
        /// <c>null</c> als er zo geen is gevonden.
        /// </returns>
        public Relationship ActiefLidOphalen(int? contactIdPersoon, int? contactIdGroep)
        {
            var request = new RelationshipRequest
            {
                ContactIdA = contactIdPersoon,
                ContactIdB = contactIdGroep,
                RelationshipTypeId = (int)(RelatieType.LidVan),
                IsActive = true,
            };
            var result =
                ServiceHelper.CallService<ICiviCrmApi, ApiResultValues<Relationship>>(
                    svc => svc.RelationshipGet(ApiKey, SiteKey, request));
            result.AssertValid();
            // Hoogstens 1 keer lid van dezelfde ploeg in hetzelfde werkjaar.
            Debug.Assert(result.Count <= 1);

            return result.Count == 0 ? null : result.Values.First();
        }
    }
}
