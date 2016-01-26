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
using System.Runtime.Serialization;
using Chiro.Gap.Domain;

namespace Chiro.Gap.ServiceContracts.DataContracts
{
	/// <summary>
	/// Datacontract met informatie over een lid (kind of leiding).  In eerste instantie enkel gemaakt voor
	/// Excelexport, maar later hopelijk voor meer bruikbaar.
	/// </summary>
	[DataContract]
	public class LidOverzicht : PersoonOverzicht
	{
		/// <summary>
		/// Type lid (kind, leiding)
		/// </summary>
		[DataMember]
		public LidType Type { get; set; }

		/// <summary>
		/// Afdelingen van lid
		/// </summary>
		[DataMember]
		public List<AfdelingsJaarInfo> Afdelingen { get; set; }
		
		/// <summary>
		/// Functies van lid
		/// </summary>
		[DataMember]
		public List<FunctieInfo> Functies { get; set; }

		/// <summary>
		/// Heeft deze persoon al lidgeld betaald?
		/// </summary>
		[DataMember]
		public bool LidgeldBetaald { get; set; }

		/// <summary>
		/// De ID van het Lid
		/// </summary>
		[DataMember]
		public int LidID { get; set; }

		/// <summary>
		/// Einde instapperiode
		/// </summary>
		[DataMember]
		public DateTime? EindeInstapPeriode { get; set; }
	}
}
