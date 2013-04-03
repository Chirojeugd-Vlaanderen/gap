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
	/// Model gebruikt om iemand een nieuwe communicatievorm te geven:
	/// telefoonnummer, mailadres, enz.
	/// </summary>
	public class NieuweCommVormModel : MasterViewModel
	{
        /// <summary>
        /// De standaardconstructor - creëert leeg NieuweCommVorm
        /// </summary>
        public NieuweCommVormModel()
        {
            Aanvrager = new PersoonDetail();
            NieuweCommVorm = new CommunicatieDetail();
            Types = new List<CommunicatieTypeInfo>();
        }

        public NieuweCommVormModel(PersoonDetail aanvrager, IEnumerable<CommunicatieTypeInfo> types) : this()
        {
            Aanvrager = aanvrager;
            Types = types;
            NieuweCommVorm = new CommunicatieDetail();
        }

		/// <summary>
		/// ID van GelieerdePersoon waarvoor aangeklikt dat
		/// hij/zij een extra adres nodig heeft
		/// </summary>
		public PersoonDetail Aanvrager { get; set; }

		/// <summary>
		/// Nieuwe communicatievorm (telefoonnummer, mailadres, ...)
		/// voor de gegeven gelieerde personen
		/// </summary>
		public CommunicatieDetail NieuweCommVorm { get; set; }

		public IEnumerable<CommunicatieTypeInfo> Types { get; set; }
	}
}
