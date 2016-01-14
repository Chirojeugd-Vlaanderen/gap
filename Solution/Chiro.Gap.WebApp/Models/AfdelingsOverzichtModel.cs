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
	/// Model voor het lijstje van actieve en niet-actieve afdelingen
	/// </summary>
	public class AfdelingsOverzichtModel : MasterViewModel
	{
		public AfdelingsOverzichtModel()
		{
			NietActief = new List<AfdelingInfo>();
			Actief = new List<AfdelingDetail>();
		}

		/// <summary>
		/// Afdelingen niet-actief dit werkJaar
		/// </summary>
		public IEnumerable<AfdelingInfo> NietActief { get; set; }

		/// <summary>
		/// Afdelingen die al actief zijn dit werkJaar (met afdelingsjaar dus)
		/// </summary>
		public IEnumerable<AfdelingDetail> Actief { get; set; }
	}
}
