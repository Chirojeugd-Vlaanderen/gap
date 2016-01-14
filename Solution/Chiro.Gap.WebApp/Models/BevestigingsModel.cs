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

namespace Chiro.Gap.WebApp.Models
{
    /// <summary>
    /// Model dat vraagt om bevestiging ivm een betalende operatie op een persoon
    /// </summary>
    public class BevestigingsModel : MasterViewModel
    {
        /// <summary>
        /// ID van een gelieerde persoon
        /// </summary>
        public int GelieerdePersoonID { get; set; }

        /// <summary>
        /// Indien relevant, een LidID van een lid van de gelieerde persoon.
        /// </summary>
        public int LidID { get; set; }

        /// <summary>
        /// Volledige naam van de persoon
        /// </summary>
        public string VolledigeNaam { get; set; }

        /// <summary>
        /// Wettegijwel wadatakost?
        /// </summary>
        public decimal Prijs { get; set; }

        /// <summary>
        /// Extra waarschuwing die getoond wordt indien niet leeg
        /// </summary>
        public string ExtraWaarschuwing { get; set; }
    }
}
