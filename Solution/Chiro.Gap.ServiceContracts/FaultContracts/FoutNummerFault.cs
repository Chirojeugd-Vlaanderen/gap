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

using System.Runtime.Serialization;
using Chiro.Gap.Domain;

namespace Chiro.Gap.ServiceContracts.FaultContracts
{
	/// <summary>
	/// Faultcontract voor een fout waar een foutnummer aan toegekend werd
	/// </summary>
	public class FoutNummerFault : GapFault
	{
		/// <summary>
		/// De foutcode
		/// </summary>
		[DataMember]
		public FoutNummer FoutNummer { get; set; }

		/// <summary>
		/// Meer uitleg over het probleem
		/// </summary>
		[DataMember]
		public string Bericht { get; set; }
	}
}
