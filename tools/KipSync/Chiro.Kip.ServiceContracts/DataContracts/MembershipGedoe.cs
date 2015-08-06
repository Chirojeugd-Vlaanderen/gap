/*
 * Copyright 2008-2013 Chirojeugd-Vlaanderen vzw. 
 * See the NOTICE file at the top-level directory of this distribution, 
 * and at https://develop.chiro.be/gap/wiki/copyright
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

using System.Runtime.Serialization;

namespace Chiro.Kip.ServiceContracts.DataContracts
{
    /// <summary>
    /// Relevante informatie voor het maken of bijwerken van een membership.
    /// </summary>
    [DataContract]
    public class MembershipGedoe
    {
        /// <summary>
        /// Stamnummer van de groep die het membership aanvraagt.
        /// </summary>
        [DataMember]
        public string StamNummer { get; set; }
        /// <summary>
        /// Moet er meteen een verzekering loonverlies bij?
        /// </summary>
        [DataMember]
        public bool MetLoonVerlies { get; set; }
        /// <summary>
        /// Gaat het om een gratis membership? (Typisch voor kaderploegen.)
        /// </summary>
        [DataMember]
        public bool Gratis { get; set; }
    }
}
