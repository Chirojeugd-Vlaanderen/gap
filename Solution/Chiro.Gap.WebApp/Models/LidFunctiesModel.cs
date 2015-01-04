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
	/// Model voor het tonen/bewerken van lidinfo.  Vrij omslachtig; mogelijk kan dat beter (TODO)
	/// </summary>
	public class LidFunctiesModel : MasterViewModel
	{
		/// <summary>
		/// De standaardconstructor; er zijn geen afdelingen geselecteerd.
		/// </summary>
		public LidFunctiesModel()
		{
			AlleFuncties = new List<FunctieDetail>();
			FunctieIDs = new List<int>();
			Persoon = new PersoonInfo();
		}

		/// <summary>
		/// Bevat de huidige of de nieuwe gewenste afdeling voor een kind
		/// </summary>
		public int AfdelingID { get; set; }

		/// <summary>
		/// Alle functies die relevant zouden kunnen zijn voor Persoon. (afhankelijk van lidsoort
		/// en groepswerkjaar.)
		/// </summary>
		public IEnumerable<FunctieDetail> AlleFuncties { get; set; }

		/// <summary>
		/// ID's van geselecteerde functies in de 'functiecheckboxlist'
		/// </summary>
		public IEnumerable<int> FunctieIDs { get; set; }

        /// <summary>
        /// Basisgegevens van de persoon
        /// </summary>
        public PersoonInfo Persoon { get; set; }

        /// <summary>
        /// Beperkte gegevens over het lid. Optioneel.
        /// </summary>
        public LidInfo LidInfo { get; set; }
	}
}
