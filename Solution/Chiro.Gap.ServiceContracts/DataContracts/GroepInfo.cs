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

using System.ComponentModel;
using System.Runtime.Serialization;

namespace Chiro.Gap.ServiceContracts.DataContracts
{
	/// <summary>
	/// Datacontract voor beperkte informatie over een groep
	/// </summary>
	[DataContract]
	public class GroepInfo
	{
		/// <summary>
		/// GroepID van de groep
		/// </summary>
		[DataMember]
		public int ID { get; set; }

		/// <summary>
		/// Naam van de groep
		/// </summary>
		[DataMember]
		public string Naam { get; set; }

		/// <summary>
		/// Plaats van de groep, indien van toepassing
		/// </summary>
		/// <remarks>Enkel Chirogroepen hebben een plaats</remarks>
		[DataMember]
		public string Plaats { get; set; }

		/// <summary>
		/// Stamnummer, heeft enkel nog nut als zoeksleutel.
		/// </summary>
		[DataMember]
		[DisplayName(@"Stamnummer")]
		public string StamNummer { get; set; }
	}
}