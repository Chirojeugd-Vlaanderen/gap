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

using System;
using System.Collections.Generic;

using Chiro.Gap.Domain;
using Chiro.Gap.ServiceContracts.FaultContracts;

namespace Chiro.Gap.Validatie
{
	/// <summary>
	/// Extension methods voor IValidatieDictionary
	/// </summary>
	public static class ValidatieDictionaryMethods
	{
		/// <summary>
		/// Voegt berichten uit een BusinessFault toe aan een
		/// 'IValidatieDictionary'
		/// </summary>
		/// <param name="dst">IValidatieDictionary waaraan de berichten
		/// moeten worden toegevoegd.</param>
		/// <param name="src">BusinessFault met toe te voegen berichten</param>
		/// <param name="keyPrefix">Prefix toe te voegen aan de keys van src,
		/// alvorens ze toe te voegen aan dst</param>
		public static void BerichtenToevoegen(this IValidatieDictionary dst, OngeldigObjectFault src, string keyPrefix)
		{
			foreach (KeyValuePair<string, FoutBericht> paar in src.Berichten)
			{
				dst.BerichtToevoegen(String.Format("{0}{1}", keyPrefix, paar.Key), paar.Value.Bericht);
			}
		}
	}
}
