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

using Chiro.Gap.ServiceContracts.DataContracts;

namespace Chiro.Gap.WebApp.Models
{
	/// <summary>
	/// Model gebruikt om een communicatievorm weer te geven.
	/// </summary>
	public class CommVormModel : MasterViewModel
	{
        /// <summary>
        /// De standaardconstructor - creëert leeg CommVormModel
        /// </summary>
        public CommVormModel()
        {
            Aanvrager = new PersoonDetail();
            NieuweCommVorm = new CommunicatieDetail();
        }

        public CommVormModel(PersoonDetail aanvrager, CommunicatieDetail v) : this()
        {
            Aanvrager = aanvrager;
            NieuweCommVorm = v;
        }

		/// <summary>
		/// ID van GelieerdePersoon wiens/wier communicatievorm 
		/// we bekijken 
		/// </summary>
		public PersoonDetail Aanvrager { get; set; }

		/// <summary>
		/// Nieuwe input voor de communicatievorm voor de gegeven gelieerde personen
		/// </summary>
		public CommunicatieDetail NieuweCommVorm { get; set; }
	}
}