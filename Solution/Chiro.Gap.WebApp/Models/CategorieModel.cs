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
	/// Dit model bevat een lijstje categorieën, een lijstje GelieerdePersoonIDs, en een lijstje
	/// ID's van geselecteerde categorieën
	/// </summary>
	public class CategorieModel : MasterViewModel
	{
        public CategorieModel()
        {
            GeselecteerdeCategorieIDs = new List<int>();
			GelieerdePersoonNamen = new List<string>();
            GelieerdePersoonIDs = new List<int>();
            Categorieen = new List<CategorieInfo>();
        }

		/// <summary>
		/// Nieuwe categorieën voor de gegeven gelieerde personen
		/// </summary>
		public IEnumerable<CategorieInfo> Categorieen { get; set; }
		public List<int> GeselecteerdeCategorieIDs { get; set; }

		public IList<int> GelieerdePersoonIDs { get; set; }
		public IList<string> GelieerdePersoonNamen { get; set; }
	}
}
