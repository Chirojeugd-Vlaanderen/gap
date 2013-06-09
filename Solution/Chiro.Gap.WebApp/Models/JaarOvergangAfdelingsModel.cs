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
using System.ComponentModel.DataAnnotations;

using Chiro.Gap.ServiceContracts.DataContracts;

namespace Chiro.Gap.WebApp.Models
{
	/// <summary>
	/// Model voor het lijstje van actieve en niet-actieve afdelingen
	/// </summary>
	public class JaarOvergangAfdelingsModel : MasterViewModel
	{
		public JaarOvergangAfdelingsModel()
		{
			Afdelingen = new List<AfdelingInfo>();
			
            // LET OP: GezokenAfdelingsIDs moet hier null zijn, en geen lege lijst,
            // omdat ik het [Verplicht] attribuut gebruik om te controleren of de groep
            // niet vergeten is afdelingen aan te klikken.
            
            GekozenAfdelingsIDs = null;
		}

		/// <summary>
		/// Afdelingen die al actief zijn dit werkJaar (met afdelingsjaar dus)
		/// </summary>
		public IEnumerable<AfdelingInfo> Afdelingen { get; set; }

        /// <summary>
        /// Gekozen afdelingen die er volgend jaar zullen zijn.  Er moet er minstens 1 gekozen
        /// zijn.  (Wel opletten dat je dit model dan niet gebruikt voor kaderploegen! Zie ook TODO #1124)
        /// </summary>
        [Verplicht]
        [DisplayName(@"Geselecteerde afdelingen")]
		public IEnumerable<int> GekozenAfdelingsIDs { get; set; }
	}
}
