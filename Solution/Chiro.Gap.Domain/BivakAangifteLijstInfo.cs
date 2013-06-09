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
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace Chiro.Gap.Domain
{
    /// <summary>
    /// DataContract voor info over de status van een bivakaangifte
    /// </summary>
    [DataContract]
    public class BivakAangifteLijstInfo
    {
        /// <summary>
        /// Constructor. Instantieert een leeg lijstje van bivakken.
        /// </summary>
        public BivakAangifteLijstInfo()
        {
            AlgemeneStatus = BivakAangifteStatus.Onbekend;
            Bivakinfos = new List<BivakAangifteInfo>();
        }

        /// <summary>
        /// Het lijstje met geregistreerde bivakken
        /// </summary>
        [Verplicht]
        [DataMember]
        public IList<BivakAangifteInfo> Bivakinfos { get; set; }

        /// <summary>
        /// Geeft aan wat er op dit moment moet gebeuren voor de bivakaangifte
        /// om in orde te zijn met de Chiroadministratie
        /// </summary>
        [Verplicht]
        [DataMember]
        public BivakAangifteStatus AlgemeneStatus { get; set; }

        /// <summary>
        /// Geeft stringrepresentatie van Versie weer (hex).
        /// Nodig om versie te bewaren in MVC view, voor concurrencycontrole.
        /// </summary>
        [DataMember]
        public string VersieString { get; set; }
    }
}