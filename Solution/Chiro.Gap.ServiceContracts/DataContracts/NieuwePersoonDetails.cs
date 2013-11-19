/*
 * Copyright 2013 Chirojeugd-Vlaanderen vzw.
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

using System.Collections.Generic;
using System.Runtime.Serialization;
using Chiro.Gap.Domain;

namespace Chiro.Gap.ServiceContracts.DataContracts
{
    /// <summary>
    /// Datacontract voor een nieuwe persoon, inclusief gegevens om hem/haar meteen in te schrijven
    /// </summary>
    [DataContract]
    public class NieuwePersoonDetails
    {
        [DataMember]
        public PersoonInfo PersoonInfo { get; set; }
        [DataMember]
        public AdresInfo Adres { get; set; }
        [DataMember]
        public AdresTypeEnum AdresType { get; set; }
        [DataMember]
        public CommunicatieInfo EMail { get; set; }
        [DataMember]
        public CommunicatieInfo TelefoonNummer { get; set; }
        /// <summary>
        /// Geen als niet inschrijven, anders Kind of Leiding.
        /// </summary>
        [DataMember]
        public LidType InschrijvenAls { get; set; }
        [DataMember]
        public List<int> AfdelingsJaarIDs { get; set; } 
    }
}
