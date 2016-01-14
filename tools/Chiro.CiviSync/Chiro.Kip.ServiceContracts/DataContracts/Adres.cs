/*
 * Copyright 2008-2013, 2016 the GAP developers. See the NOTICE file at the 
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

namespace Chiro.Kip.ServiceContracts.DataContracts
{
    /// <summary>
    /// Bevat de gegevens waaruit een adres bestaat
    /// </summary>
    [DataContract]
    public class Adres
    {
        /// <summary>
        /// De straatnaam
        /// </summary>
        [DataMember]
        public string Straat { get; set; }

        /// <summary>
        /// Het huisnummer
        /// </summary>
        /// <remarks>Moet numeriek zijn. Elke toevoeging (letters, slash-nogiets, enz.) moet bij Bus.</remarks>
        [DataMember]
        public int? HuisNr { get; set; }

        /// <summary>
        /// Toevoeging bij het huisnummer
        /// </summary>
        [DataMember]
        public string Bus { get; set; }

        /// <summary>
        /// Postnummer.
        /// </summary>
        [DataMember]
        public string PostNr { get; set; }

        /// <summary>
        /// De (deel)gemeente
        /// </summary>
        [DataMember]
        public string WoonPlaats { get; set; }

        /// <summary>
        /// Het land
        /// </summary>
        [DataMember]
        public string Land { get; set; }

        /// <summary>
        /// ISO-Code voor het land.
        /// </summary>
        [DataMember]
        public string LandIsoCode { get; set; }

        /// <summary>
        /// Returns a System.String that represents the current Persoon
        /// </summary>
        /// <returns>System.String that represents the current Persoon</returns>
        public override string ToString()
        {
            return this.Straat + ' ' + this.HuisNr + ' ' + this.WoonPlaats;
        }
    }
}