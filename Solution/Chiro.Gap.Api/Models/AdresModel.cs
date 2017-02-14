/*
 * Copyright 2017 Chirojeugd-Vlaanderen vzw. See the NOTICE file at the 
 * top-level directory of this distribution, and at
 * https://gapwiki.chiro.be/copyright
 * 
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 * 
 *     http://www.apache.org/licenses/LICENSE-2.0
 * 
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

using Chiro.Gap.Domain;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Chiro.Gap.Api.Models
{
    public class AdresModel
    {
        public int AdresId { get; set; }
        public string Straat { get; set; }
        public int? Huisnr { get; set; }
        public string Bus { get; set; }
        public string Postcode { get; set; }
        public string Woonplaats { get; set; }
        public string Land { get; set; }
        public bool IsVoorkeur { get; set; }
        [JsonConverter(typeof(StringEnumConverter))]
        public AdresTypeEnum Adrestype { get; set; }
    }
}