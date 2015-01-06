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

using System.Runtime.Serialization;
using Chiro.CiviCrm.Api.DataContracts;
using Chiro.CiviCrm.BehaviorExtension;
using Newtonsoft.Json;

namespace Chiro.ChiroCivi.ServiceContracts.DataContracts
{
    /// <summary>
    /// Een CiviCRM-contact met de custom fields van ChiroCivi.
    /// </summary>
    [JsonConvertible]
    [DataContract]
    public class ChiroContact : Contact
    {
        /// <summary>
        /// Bind the member 'GapId' to the custom field custom_1.
        /// </summary>
        [DataMember(Name = "custom_10"), JsonProperty]
        public int? GapId { get; set; }

        public ChiroContact() : base()
        {
        }

        public ChiroContact(Contact c) : base()
        {
            
        }
    }
}
