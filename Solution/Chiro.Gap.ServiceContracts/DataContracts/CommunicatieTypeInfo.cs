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

using System.Runtime.Serialization;

namespace Chiro.Gap.ServiceContracts.DataContracts
{
	/// <summary>
	/// DataContract voor summiere info over communicatietypes
	/// </summary>
	[DataContract]
	public class CommunicatieTypeInfo
	{
		/// <summary>
		/// Uniek identificatienummer
		/// </summary>
		[DataMember]
		public int ID { get; set; }

		/// <summary>
		/// De 'naam' van het communicatietype
		/// </summary>
		[DataMember]
		public string Omschrijving { get; set; }

		/// <summary>
		/// Geeft aan of iemand toestemming moet geven voor Chirojeugd Vlaanderen
		/// waarden voor dit communicatietype mag gebruiken
		/// </summary>
		[DataMember]
		public bool IsOptIn { get; set; }

		/// <summary>
		/// Een regular expression die aangeeft welke vorm de waarde voor dat type moet hebben
		/// </summary>
		[DataMember]
		public string Validatie { get; set; }

		/// <summary>
		/// Een voorbeeld van een communicatievorm die volgens de validatieregels gestructureerd is
		/// </summary>
		[DataMember]
		public string Voorbeeld { get; set; }
	}
}
