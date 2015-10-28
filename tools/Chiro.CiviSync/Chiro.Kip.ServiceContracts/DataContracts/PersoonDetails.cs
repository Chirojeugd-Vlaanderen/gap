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
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Chiro.Kip.ServiceContracts.DataContracts
{
    /// <summary>
    /// Datacontract dat typisch gebruikt zal worden om een onbekende persoon naar kipadmin te sturen.
    /// Het bevat een hoop informatie over de persoon, in de hoop dat we op die manier kunnen uitvissen
    /// over wie in Kipadmn het gaat.
    /// </summary>
    [DataContract]
    public class PersoonDetails
    {
        /// <summary>
        /// Een persoonobject dat basiseigenschappen als naam en geboortedatum bevat
        /// </summary>
        [DataMember]
        public Persoon Persoon { get; set; }

        /// <summary>
        /// Een adresobject dat gegevens bevat over een woon- of werkplaats
        /// </summary>
        [DataMember]
        public Adres Adres { get; set; }

        /// <summary>
        /// Een aanduiding over welk soort adres het gaat
        /// </summary>
        [DataMember]
        public AdresTypeEnum AdresType { get; set; }

        /// <summary>
        /// Een opsomming van telefoonnummers, mailadressen, enz.
        /// </summary>
        [DataMember]
        public IEnumerable<CommunicatieMiddel> Communicatie { get; set; }

        /// <summary>
        /// Returns a System.String that represents the current PersoonDetails
        /// </summary>
        /// <returns>System.String that represents the current PersoonDetails</returns>
        public override string ToString()
        {
            return this.Persoon.ToString() + ' ' + this.AdresType.ToString() + ' ' + this.Adres + ' ' + this.Communicatie;
        }
    }
}