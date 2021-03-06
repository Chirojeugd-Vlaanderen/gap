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

using System.ComponentModel.DataAnnotations;

namespace Chiro.Gap.ServiceContracts.DataContracts
{
	/// <summary>
	/// Relevante informatie over een categorie
	/// </summary>
	public class CategorieInfo
	{
		/// <summary>
		/// CategorieID van de categorie
		/// </summary>
		public int ID { get; set; }

		/// <summary>
		/// Naam van de categorie
		/// </summary>
        [Verplicht]
        [StringLength(80, MinimumLength = 2)]
		public string Naam { get; set; }

		/// <summary>
		/// Code voor de categorie
		/// </summary>
        [Verplicht]
        [StringLength(10, MinimumLength = 2)]
        public string Code { get; set; }
	}
}
