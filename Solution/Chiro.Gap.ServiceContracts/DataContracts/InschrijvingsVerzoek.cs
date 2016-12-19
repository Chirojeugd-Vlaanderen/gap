/*
 * Copyright 2008-2014 the GAP developers. See the NOTICE file at the 
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

using System.Runtime.Serialization;

namespace Chiro.Gap.ServiceContracts.DataContracts
{
    /// <summary>
    /// Datacontract met alle nodige informatie om een gelieerde persoon in te schrijven.
    /// </summary>
    [DataContract]
    public class InschrijvingsVerzoek
    {
        /// <summary>
        /// Boolean die aangeeft of het afdelingsjaar aangepast moet worden.
        /// </summary>
        [DataMember]
        public bool AfdelingsJaarIrrelevant;

        /// <summary>
        /// De id van de gelieerde persoon die we leiding willen maken
        /// </summary>
        [DataMember]
        public int GelieerdePersoonID { get; set; }

        /// <summary>
        /// Gaat het om leiding of een kind
        /// </summary>
        [DataMember]
        public bool LeidingMaken { get; set; }

        /// <summary>
        /// De IDs van de eventuele gekozen afdelingsjaren.
        /// </summary>
        [DataMember]
        public int[] AfdelingsJaarIDs { get; set; }
    }
}
