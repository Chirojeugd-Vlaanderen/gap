/*
 * Copyright 2008-2013 the GAP developers. See the NOTICE file at the 
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
using System;
using System.Runtime.Serialization;

namespace Chiro.Kip.ServiceContracts.DataContracts
{
    /// <summary>
    /// Datacontract voor een bivakobject
    /// </summary>
    [DataContract]
    public class Bivak
    {
        /// <summary>
        /// ID van de uitstap in GAP.
        /// </summary>
        [DataMember]
        public int UitstapID { get; set; }

        /// <summary>
        /// Stamnummer voor groep die op bivak gaat
        /// </summary>
        [DataMember]
        public string StamNummer { get; set; }

        /// <summary>
        /// Werkjaar van het bivak.
        /// </summary>
        [DataMember]
        public int WerkJaar { get; set; }

        /// <summary>
        /// Begindatum van het bivak
        /// </summary>
        [DataMember]
        public DateTime DatumVan { get; set; }

        /// <summary>
        /// Einddatum van het bivak
        /// </summary>
        [DataMember]
        public DateTime DatumTot { get; set; }

        /// <summary>
        /// Naam van het bivak
        /// </summary>
        [DataMember]
        public string Naam { get; set; }

        /// <summary>
        /// Opmerkingen bij het bivak
        /// </summary>
        [DataMember]
        public string Opmerkingen { get; set; }
    }
}