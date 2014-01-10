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
    /// Class die een persoon voorstelt en daar de basiseigenschappen van bevat
    /// </summary>
    [DataContract]
    public class Persoon
    {
        /// <summary>
        /// Een uniek identificatienummer
        /// </summary>
        [DataMember]
        public int ID { get; set; }

        /// <summary>
        /// Het administratief identificatienummer dat aan die persoon toegekend werd
        /// door het administratieprogramma van Chirojeugd Vlaanderen.
        /// </summary>
        /// <remarks>
        /// Wordt toegekend op het moment dat er een reden is om de persoon te registreren in de
        /// nationale Chiroadministratie: bij aansluiting, bij inschrijving voor een evenement of 
        /// bij een gift waar de persoon een fiscaal attest voor wil/moet krijgen.
        /// </remarks>
        [DataMember]
        public int? AdNummer { get; set; }

        /// <summary>
        /// De voornaam van de persoon
        /// </summary>
        [DataMember]
        public string VoorNaam { get; set; }

        /// <summary>
        /// De familienaam van de persoon
        /// </summary>
        [DataMember]
        public string Naam { get; set; }

        /// <summary>
        /// De geboortedatum van de persoon
        /// </summary>
        [DataMember]
        public DateTime? GeboorteDatum { get; set; }

        /// <summary>
        /// De datum waarop de persoon overleden is
        /// </summary>
        [DataMember]
        public DateTime? SterfDatum { get; set; }

        /// <summary>
        /// Een aanduiding voor de sekse van de persoon
        /// </summary>
        [DataMember]
        public GeslachtsEnum Geslacht { get; set; }

        /// <summary>
        /// Leesbare identiticatie
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return this.Naam + ' ' + this.VoorNaam + " (" + this.AdNummer + ')';
        }
    }
}