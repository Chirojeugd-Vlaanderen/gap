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
using System.Runtime.Serialization;

namespace Chiro.Gap.ServiceContracts.FaultContracts
{
	/// <summary>
	/// Faultcontract dat gebruikt wordt als bestaande entiteiten/objecten een operatie verhinderen
	/// </summary>
	/// <typeparam name="TObject">Het type van het object dat de operatie blokkeert</typeparam>
	[DataContract]
	public class BlokkerendeObjectenFault<TObject> : GapFault
	{
		/// <summary>
		/// De objecten die een operatie verhinderen
		/// </summary>
		[DataMember]
		public IEnumerable<TObject> Objecten { get; set; }

		/// <summary>
		/// Het aantal objecten dat de operatie verhindert
		/// </summary>
		[DataMember]
		public int Aantal { get; set; }
	}
}
