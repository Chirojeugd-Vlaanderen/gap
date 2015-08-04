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
    public class BivakWorker: BaseWorker
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="serviceHelper">Helper to be used for WCF service calls</param>
        public BivakWorker(ServiceHelper serviceHelper, IMiniLog log, ICiviCache cache)
            : base(serviceHelper, log, cache)
        {
        }

        /// <summary>
        /// Haalt een bivak op (event) uit CiviCRM, samen met organiserende
        /// ploeg en adres.
        /// </summary>
        /// <param name="uitstapId">GAP-uitstapID van bivak.</param>
        /// <returns>Het bivak uit CiviCRM, samen met organiserende ploeg en adres.</returns>
        public Event BivakDetailsOphalen(int uitstapId)
        {
            var apiResult = ServiceHelper.CallService<ICiviCrmApi, ApiResultValues<Event>>(
                svc =>
                    svc.EventGet(ApiKey, SiteKey,
                        new EventRequest
                        {
                            EventTypeId = (int)EvenementType.Bivak,
                            GapUitstapId = uitstapId,
                            // Haal ook organiserende ploeg op, zodat we het stamnummer
                            // hebben om te loggen.
                            ContactGetRequest = new ContactRequest
                            {
                                IdValueExpression = "$value.custom_47_id",
                                ReturnFields = "external_identifier"
                            },
                            LocBlockGetRequest = new LocBlockRequest
                            {
                                IdValueExpression = "$value.loc_block_id",
                                AddressGetRequest = new AddressRequest
                                {
                                    IdValueExpression = "$value.address_id"
                                }
                            }
                        }));
            apiResult.AssertValid();

            return apiResult.Count > 0 ? apiResult.Values.First() : null;
        }
    }
}
