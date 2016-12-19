/*
 * Copyright 2008-2013 the GAP developers. See the NOTICE file at the 
 * top-level directory of this distribution, and at
 * https://gapwiki.chiro.be/copyright
 * Bijgewerkte authenticatie Copyright 2014,2015 Chirojeugd-Vlaanderen vzw
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
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using Chiro.Gap.Domain;

namespace Chiro.Gap.ServiceContracts.DataContracts
{
    /// <summary>
    /// Datacontract voor beperkte informatie over gebruikersrechten
    /// </summary>
    [DataContract]
    public class GebruikersInfo
    {
        /// <summary>
        /// Rol van de gebruiker
        /// </summary>
        [DataMember]
        public GebruikersRecht GebruikersRecht { get; set; }

        /// <summary>
        /// Vervaldatum gebruikersrecht
        /// </summary>
        [DataMember]
        [DataType(DataType.Date)]
        public DateTime? VervalDatum { get; set; }

        /// <summary>
        /// Login voor de gebruiker
        /// </summary>
        [DataMember]
        public string Login { get; set; }

        /// <summary>
        /// Geeft aan of het gebruikersrecht verlengbaar is
        /// </summary>
        [DataMember]
        public bool IsVerlengbaar { get; set; }
    }
}
