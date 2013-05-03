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

using Chiro.Gap.ServiceContracts.DataContracts;

namespace Chiro.Gap.WebApp.Models
{
	/// <summary>
	/// In het algemeen bevat het GelieerdePersonenModel informatie over slechts 1 persoon.
	/// Deze informatie zit dan in <c>HuidigePersoon</c>.
	/// <para />
	/// Wanneer dit model gebruikt wordt voor het toevoegen van een nieuwe persoon, dan
	/// bevat het ook mogelijke gelijkaardige personen (<c>GelijkaardigePersonen</c>) en
	/// een boolean <c>Forceer</c> die aangeeft of een nieuwe persoon geforceerd moet worden
	/// ondanks gevonden gelijkaardige personen.
	/// </summary>
	public class GelieerdePersonenModel : MasterViewModel
	{
        public GelieerdePersonenModel()
        {
            GelijkaardigePersonen = new List<PersoonDetail>();
            HuidigePersoon = new PersoonDetail();
        }

		/// <summary>
		/// Informatie over een te tonen of te wijzigen persoon
		/// </summary>
		public PersoonDetail HuidigePersoon { get; set; }

		/// <summary>
		/// Lijst met info over eventueel gelijkaardige personen
		/// </summary>
		public IEnumerable<PersoonDetail> GelijkaardigePersonen { get; set; }

		/// <summary>
		/// Geeft aan of een persoon ook toegevoegd moet worden als er al gelijkaardige
		/// personen bestaan.
		/// </summary>
		public bool Forceer { get; set; }

		/// <summary>
		/// Een eventuele ID als een broer zus waarvan de NIEUWE persoon gemaakt wordt gekend is.
		/// </summary>
		public int BroerzusID { get; set; }

        /// <summary>
        /// ID van het groepswerkjaar waarin we deze persoon mogelijk lid willen maken.
        /// </summary>
        public int GroepsWerkJaarID { get; set; }
	}
}