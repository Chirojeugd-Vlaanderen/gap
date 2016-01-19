/*
 * Copyright 2008-2013 the GAP developers. See the NOTICE file at the 
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

using System.ComponentModel;
using System.Runtime.Serialization;
using Chiro.Gap.Domain;

namespace Chiro.Gap.ServiceContracts.DataContracts
{
    /// <summary>
    /// Details van een uitstap
    /// </summary>
    [DataContract]
    public class UitstapOverzicht : UitstapInfo
    {
        /// <summary>
        /// De naam van de bivakplaats
        /// </summary>
        [DataMember]
        [DisplayName(@"Naam van de bivakplaats")]
        public string PlaatsNaam { get; set; }

        // Een datacontract moet normaal gezien 'plat' zijn.  Maar het lijkt me
        // zo raar om hier gewoon over te tikken wat er al in AdresInfo staat.

        /// <summary>
        /// Het adres van de bivakplaats
        /// </summary>
        [DataMember]
        public AdresInfo Adres { get; set; }

        /// <summary>
        /// De ID van het groepswerkjaar waarin de uitstap georganiseerd werd
        /// </summary>
        [DataMember]
        public int GroepsWerkJaarID { get; set; }
    }
}
