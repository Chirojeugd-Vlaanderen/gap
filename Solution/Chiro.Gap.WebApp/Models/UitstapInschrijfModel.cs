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
using System.ComponentModel;

using Chiro.Gap.ServiceContracts.DataContracts;

namespace Chiro.Gap.WebApp.Models
{
    /// <summary>
    /// Model voor het inschrijven van personen voor een uitstap.
    /// </summary>
    public class UitstapInschrijfModel : MasterViewModel
    {
        /// <summary>
        /// Informatie over alle gelieerde personen die zullen worden ingeschreven.  Dit enkel ter
        /// verificatie.
        /// </summary>
        public IEnumerable<PersoonInfo> GelieerdePersonen;

        /// <summary>
        /// Bij postback zal deze lijst de GelieerdePersoonIDs bevatten, zodat de gelieerde
        /// personen in kwestie ingeschreven kunnen worden.
        /// </summary>
        public IList<int> GelieerdePersoonIDs { get; set; }

        /// <summary>
        /// Alle uitstappen waarvoor ingeschreven kan worden
        /// </summary>
        public IEnumerable<UitstapInfo> Uitstappen { get; set; }

		/// <summary>
		/// ID van de geselecteerde uitstap (voor postback)
		/// </summary>
		[DisplayName("Uitstap/bivak")]
		public int GeselecteerdeUitstapID { get; set; }

        /// <summary>
        /// Status van vinkje 'Is logistieke deelnemer'
        /// </summary>
        [DisplayName("Logistiek deelnemer, bv. kookploeg")]
        public bool LogistiekDeelnemer { get; set; }
    }
}