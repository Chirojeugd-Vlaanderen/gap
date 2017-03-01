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

namespace Chiro.Gap.ServiceContracts.DataContracts
{
	/// <summary>
	/// DataContract voor summiere info over communicatievormen
	/// </summary>
	[DataContract]
	public class CommunicatieDetail : CommunicatieInfo, ICommunicatie
	{
		/// <summary>
		/// De 'naam' van het communicatietype
		/// </summary>
		/// <remarks>Overgenomen van geassocieerde CommunicatieType</remarks>
		[DataMember]
		public string CommunicatieTypeOmschrijving { get; set; }

		/// <summary>
		/// Een regular expression die aangeeft welke vorm de waarde voor dat type moet hebben
		/// </summary>
		/// <remarks>Overgenomen van geassocieerde CommunicatieType</remarks>
		[DataMember]
		public string CommunicatieTypeValidatie { get; set; }

		/// <summary>
		/// Een voorbeeld van een communicatievorm die volgens de validatieregels gestructureerd is
		/// </summary>
		/// <remarks>Overgenomen van geassocieerde CommunicatieType</remarks>
		[DataMember]
		public string CommunicatieTypeVoorbeeld { get; set; }
        public object IsVerdacht { get; internal set; }
        public object LaatsteControle { get; internal set; }
    }
}
