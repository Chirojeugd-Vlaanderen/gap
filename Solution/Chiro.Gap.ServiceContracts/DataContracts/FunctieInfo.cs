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

using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace Chiro.Gap.ServiceContracts.DataContracts
{
	/// <summary>
	/// Datacontract voor beperkte informatie over een functie
	/// </summary>
	[DataContract]
	[KnownType(typeof(FunctieDetail))]
	public class FunctieInfo
	{
		/// <summary>
		/// Uniek identificatienummer
		/// </summary>
		[DataMember]
		public int ID { get; set; }

		/// <summary>
		/// De afkorting van de functie
		/// </summary>
		[Verplicht, StringLengte(10), StringMinimumLengte(2)]
		[DataMember]
		public string Code { get; set; }

		/// <summary>
		/// De volledige naam van de functie
		/// </summary>
		[Verplicht, StringLengte(80), StringMinimumLengte(2)]
		[DataMember]
		public string Naam { get; set; }
	}
}