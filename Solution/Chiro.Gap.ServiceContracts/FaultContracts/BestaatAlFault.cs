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

namespace Chiro.Gap.ServiceContracts.FaultContracts
{
	/// <summary>
	/// Faultcontract dat gebruikt wordt als een bestaand(e) entiteit/object verhindert
	/// dat bepaalde gegevens opgeslagen worden
	/// </summary>
	/// <typeparam name="TObject">Het type van het object dat al bestaat</typeparam>
	[DataContract]
	public class BestaatAlFault<TObject> : GapFault
	{
		/// <summary>
		/// Het bestaande object
		/// </summary>
		[DataMember]
		public TObject Bestaande { get; set; }
	}
}
