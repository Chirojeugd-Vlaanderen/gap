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
using System.Runtime.Serialization;

namespace Chiro.Gap.ServiceContracts.DataContracts
{
	/// <summary>
	/// Zeer beperkt datacontract voor afdelingsnaam en -code.
	/// </summary>
	[DataContract]
	public class AfdelingInfo
	{
		/// <summary>
		/// De ID van de afdeling
		/// </summary>
		[DataMember]
		public int ID { get; set; }

		/// <summary>
		/// Naam van de afdeling
		/// </summary>
        [Verplicht]
        [StringLength(50, MinimumLength = 2)]
        [DataMember]
		public string Naam { get; set; }

		/// <summary>
		/// Afkorting voor de afdeling
		/// </summary>
		[DataMember]
        [Verplicht]
        [StringLength(10, MinimumLength = 1)]
        public string Afkorting { get; set; }
	}
}
