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
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Chiro.Kip.ServiceContracts.DataContracts
{
    /// <summary>
    /// Gegevens die je nodig hebt om een lid te maken, met uitzondering van de persoonsgegevens.
    /// </summary>
    [DataContract]
    public class LidGedoe
    {
        /// <summary>
        /// Stamnummer van de groep waarbij aan te sluiten
        /// </summary>
        [DataMember]
        public string StamNummer { get; set; }

        /// <summary>
        /// Werkjaar waarin de persoon lid moet worden
        /// </summary>
        [DataMember]
        public int WerkJaar { get; set; }

        /// <summary>
        /// Type van het lid (kind, leiding)
        /// </summary>
        [DataMember]
        public LidTypeEnum LidType { get; set; }

        /// <summary>
        /// Nationale functies die het lid moet krijgen
        /// </summary>
        [DataMember]
        public IEnumerable<FunctieEnum> NationaleFuncties { get; set; }

        /// <summary>
        /// Officiele afdelingen van het lid
        /// </summary>
        [DataMember]
        public IEnumerable<AfdelingEnum> OfficieleAfdelingen { get; set; }

        /// <summary>
        /// Einde van de instapperiode
        /// </summary>
        [DataMember]
        public DateTime? EindeInstapPeriode { get; set; }

        /// <summary>
        /// Als het lid uitgeschreven is: een uitschrijfdatum.
        /// </summary>
        [DataMember]
        public DateTime? UitschrijfDatum { get; set; }

        /// <summary>
        /// Returns a System.String that represents the current LidGedoe
        /// </summary>
        /// <returns>System.String that represents the current LidGedoe</returns>
        public override string ToString()
        {
            return this.LidType.ToString() + this.WerkJaar + this.StamNummer;
        }
    }
}