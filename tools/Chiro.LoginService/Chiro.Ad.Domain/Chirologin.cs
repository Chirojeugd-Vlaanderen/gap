/*
 * Copyright 2008-2015 the GAP developers. See the NOTICE file at the 
 * top-level directory of this distribution, and at
 * https://gapwiki.chiro.be/copyright
 * Refactoring AD-access (#4938) Copyright 2016, Chirojeugd-Vlaanderen vzw.
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

namespace Chiro.Ad.Domain
{
    /// <summary>
    /// Algemene class voor beheer van Chirologins
    /// </summary>
    public class Chirologin
    {
        /// <summary>
        /// Het domein in Active Directory waar de account zich bevindt
        /// </summary>
        public String Domein { get; set; }

        /// <summary>
        /// De login van de gebruiker
        /// </summary>
        public String Login { get; set; }

        /// <summary>
        /// De voornaam van de gebruiker
        /// </summary>
        public String Voornaam { get; set; }

        /// <summary>
        /// De familienaam van de gebruiker
        /// </summary>
        public String Familienaam { get; set; }

        /// <summary>
        /// Het AD-nummer van de gebruiker
        /// </summary>
        public int? AdNr { get; set; }

        /// <summary>
        /// Het mailadres van de gebruiker
        /// </summary>
        public String Mailadres { get; set; }
        
        /// <summary>
        /// De beschrijving (Description) van de gebruiker
        /// </summary>
        public String Beschrijving { get; set; }

        /// <summary>
        /// Waar bevindt de account zich precies in Active Directory?
        /// </summary>
        public String Path { get; set; }

        /// <summary>
        /// Is de account enabled?
        /// </summary>
        public bool IsActief { get; set; }

        /// <summary>
        /// Securitygroepen waar de account member van is
        /// </summary>
        public List<string> SecurityGroepen { get; set; }

        /// <summary>
        /// Geeft aan of de account al bestond of dat hij nieuw aangemaakt is
        /// </summary>
        public bool BestondAl { get; set; }

        public string Naam
        {
            get { return String.Format("{0} {1}", Voornaam, Familienaam); }
        }
    }
}