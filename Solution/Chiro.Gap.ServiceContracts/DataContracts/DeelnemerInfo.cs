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

using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace Chiro.Gap.ServiceContracts.DataContracts
{
    /// <summary>
    /// Datacontract voor beperkte informatie over een deelnemer
    /// </summary>
    [DataContract]
    [KnownType(typeof(DeelnemerDetail))]
    public class DeelnemerInfo
    {
        /// <summary>
        /// De ID van de deelnemer
        /// </summary>
        [DataMember]
        public int DeelnemerID { get; set; }

        /// <summary>
        /// Geeft aan of de deelnemer logistiek medewerker is
        /// </summary>
        [DataMember]
        [DisplayName(@"Logistieke ploeg")]
        public bool IsLogistieker { get; set; }

        /// <summary>
        /// Geeft aan of de deelnemer betaald heeft
        /// </summary>
        [DataMember]
        [DisplayName(@"Inschrijvingsgeld betaald")]
        public bool HeeftBetaald { get; set; }

        /// <summary>
        /// Geeft aan of de medische fiche volledig ingevuld 
        /// en aan de organisatie bezorgd is
        /// </summary>
        [DataMember]
        [DisplayName(@"Medische fiche OK")]
        public bool MedischeFicheOk { get; set; }

        /// <summary>
        /// Eventuele extra info over de deelname
        /// </summary>
        [DataMember]
        [DataType(DataType.MultilineText)]
        public string Opmerkingen { get; set; }
    }
}