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

namespace Chiro.Gap.ServiceContracts.DataContracts
{
	/// <summary>
	/// DataContract waarin een gelieerde persoon, zijn/haar LidInfo, zijn/haar adressen 
	/// en zijn/haar communicatievormen aan elkaar koppeld zijn
	/// </summary>
	[DataContract]
	public class PersoonLidInfo
	{
		/// <summary>
		/// Uitgebreid info-object over de persoon
		/// </summary>
		[DataMember]
		public PersoonDetail PersoonDetail { get; set; }

        // TODO (#1134): booleans voor wat er geladen is

		/// <summary>
		/// Info-object van het lidmaatschap van de persoon
		/// </summary>
		[DataMember]
		public LidInfo LidInfo { get; set; }

		/// <summary>
		/// De lijst van adressen waar de persoon zoal verblijft
		/// </summary>
		[DataMember]
		public IEnumerable<PersoonsAdresInfo> PersoonsAdresInfo { get; set; }

		/// <summary>
		/// De lijst van communicatievormen die de persoon gebruikt
		/// </summary>
		[DataMember]
		public IEnumerable<CommunicatieDetail> CommunicatieInfo { get; set; }

	    /// <summary>
	    /// Info over eventueel gebruikersrecht van deze gelieerde persoon op zijn eigen groep.
	    /// (null als er geen gebruikersrecht is)
	    /// </summary>
        [DataMember]
        public GebruikersInfo GebruikersInfo { get; set; }
	}
}
