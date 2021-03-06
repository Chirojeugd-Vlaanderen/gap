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

using System.Collections.Generic;

using Chiro.Gap.ServiceContracts.DataContracts;

namespace Chiro.Gap.WebApp.Models
{
	public class PersonenLedenModel : MasterViewModel
	{
		public PersonenLedenModel()
		{
			AlleAfdelingen = new List<AfdelingDetail>();
			PersoonLidInfo = new PersoonLidInfo();
		}

		/// <summary>
		/// Informatie over een te tonen of te wijzigen persoon
		/// </summary>
		public PersoonLidInfo PersoonLidInfo { get; set; }

		public IEnumerable<AfdelingDetail> AlleAfdelingen { get; set; }

		/// <summary>
		/// Geef de gebruiker de optie om een verzekering te geven voor loonverlies
		/// </summary>
		public bool KanVerzekerenLoonVerlies { get; set; }

		/// <summary>
		/// De kost om iemand bij te verzekeren tegen loonverlies
		/// </summary>
		public decimal PrijsVerzekeringLoonVerlies { get; set; }

		/// <summary>
		/// De kost voor een dubbelpuntabonnement van een jaar
		/// </summary>
		public decimal PrijsDubbelPunt { get; set; }
	}
}