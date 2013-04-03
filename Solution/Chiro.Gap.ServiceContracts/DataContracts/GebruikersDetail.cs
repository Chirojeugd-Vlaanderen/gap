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
using System.Runtime.Serialization;

namespace Chiro.Gap.ServiceContracts.DataContracts
{
    /// <summary>
    /// Details van een gebruiker.
    /// </summary>
    [DataContract]
    public class GebruikersDetail : GebruikersInfo
    {
        /// <summary>
        /// Voornaam van de GAV (als geweten)
        /// </summary>
        [DataMember]
        public string VoorNaam { get; set; }

        /// <summary>
        /// Naam van de GAV (als geweten)
        /// </summary>
        [DataMember]
        public string FamilieNaam { get; set; }

        /// <summary>
        /// PersoonID van de GAV (als geweten)
        /// </summary>
        [DataMember]
        public int? PersoonID { get; set; }

        /// <summary>
        /// GelieerdePersoonID van combinatie persoon-groep
        /// </summary>
        [DataMember]
        public int? GelieerdePersoonID { get; set; }
    }
}
