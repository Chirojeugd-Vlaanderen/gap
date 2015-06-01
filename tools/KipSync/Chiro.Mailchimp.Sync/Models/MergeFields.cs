/*
 * Copyright 2015 Chirojeugd-Vlaanderen vzw. See the NOTICE file at the 
 * top-level directory of this distribution, and at
 * https://develop.chiro.be/gap/wiki/copyright
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

namespace Chiro.Mailchimp.Sync.Models
{
    public class MergeFields
    {
        public string FNAME { get; set; }
        public string LNAME { get; set; }
        public string STAMNUMMER { get; set; }
        public string HOE { get; set; }
        public string STRAAT_NUM { get; set; }
        public string POSTCODE { get; set; }
        public string GEMEENTE { get; set; }
        public string LAND { get; set; }
    }
}
