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
using System.Runtime.Serialization;

using Chiro.Gap.Domain;

namespace Chiro.Gap.ServiceContracts.DataContracts
{
    /// <summary>
    /// Details van een uitstap
    /// </summary>
    /// <remarks>Startdatum en einddatum zijn <c>DateTime?</c>, opdat we dit
    /// datacontract ook als model zouden kunnen gebruiken in de webappl.  Als
    /// Startdatum en einddatum nullable zijn, dan zijn ze bij het aanmaken
    /// van een nieuwe uitstap gewoon leeg, ipv een nietszeggende datum in het
    /// jaar 1 als ze niet nullable zijn.</remarks>
    [DataContract]
    public class UitstapDetail : UitstapInfo
    {
        [DataMember]
        [DisplayName(@"Naam van de bivakplaats")]
        public string PlaatsNaam { get; set; }

        // Een datacontract moet normaalgezien 'plat' zijn.  Maar het lijkt me
        // zo raar om hier gewoon over te tikken wat er al in AdresInfo staat.
        [DataMember]
        public AdresInfo Adres { get; set; }

        [DataMember]
        public int GroepsWerkJaarID { get; set; }
    }
}
