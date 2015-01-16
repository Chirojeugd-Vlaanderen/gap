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

using Chiro.CiviCrm.Api.DataContracts;
using Chiro.CiviCrm.BehaviorExtension;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Chiro.ChiroCivi.ServiceContracts.DataContracts.Requests
{
    [JsonConvertible]
    public class GapIdRequest: BaseRequest
    {
        [JsonProperty("custom_10")]
        public int GapId { get; set; }

        [JsonProperty("external_identifier", NullValueHandling = NullValueHandling.Ignore)]
        public string ExternalIdentifier { get; set; }

        [JsonConverter(typeof (StringEnumConverter))]
        [JsonProperty("contact_type", NullValueHandling = NullValueHandling.Ignore)]
        public ContactType ContactType
        {
            get { return ContactType.Individual; }
        }

        public GapIdRequest() : base()
        {
        }

        public GapIdRequest(int gapId) : this()
        {
            this.GapId = gapId;
        }
    }
}
