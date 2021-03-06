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
	/// <summary>
	/// Model dat gebruikt kan worden voor een lijstje links naar personen
	/// </summary>
	public class LedenLinksModel : MasterViewModel
	{
        public LedenLinksModel()
        {
            Leden = new List<PersoonLidInfo>();
        }

		/// <summary>
		/// Informatie op te lijsten leden
		/// </summary>
		public IEnumerable<PersoonLidInfo> Leden { get; set; }

		/// <summary>
		/// Indien niet leeg, wordt de lijst als onvolledig beschouwd, en wordt
		/// een link toegevoegd naar de volledige lijst (link naar gegeven url)
		/// </summary>
		public string VolledigeLijstUrl { get; set; }

		/// <summary>
		/// Totaal aantal personen in volledige lijst
		/// </summary>
		public int TotaalAantal { get; set; }

		/// <summary>
		/// Indien relevant: ID van functie die al die personen hebben.
		/// </summary>
		public int FunctieID { get; set; }
	}
}
