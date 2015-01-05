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
using Chiro.Gap.Domain;

namespace Chiro.Gap.ServiceContracts.DataContracts
{
	/// <summary>
	/// Datacontract voor beperkte info ivm een deelnemer van een uitstap.
	/// </summary>
	[DataContract]
	public class DeelnemerDetail : DeelnemerInfo
	{
	    /// <summary>
	    /// De ID van de gelieerde persoon die deelneemt
	    /// </summary>
	    [DataMember]
		public int GelieerdePersoonID { get; set; }

	    /// <summary>
	    /// Gedetailleerde persoonsinfo over de deelnemer
	    /// </summary>
	    [DataMember]
		public PersoonOverzicht PersoonOverzicht { get; set; }

	    /// <summary>
	    /// De ID van de uitstap waar die persoon aan deelneemt
	    /// </summary>
	    [DataMember]
        public int UitstapID { get; set; }

	    /// <summary>
	    /// De voornaam van de deelnemer
	    /// </summary>
	    [DataMember]
		public string VoorNaam { get; set; }

	    /// <summary>
	    /// De familienaam van de deelnemer
	    /// </summary>
	    [DataMember]
		public string FamilieNaam { get; set; }

	    /// <summary>
	    /// Geeft aan of de deelnemer deelneemt, begeleidt of meegaat als logistiek medewerker
	    /// </summary>
	    [DataMember]
		public DeelnemerType Type { get; set; }

	    /// <summary>
	    /// TODO (#190): documenteren
	    /// </summary>
	    [DataMember]
		public IEnumerable<AfdelingInfo> Afdelingen { get; set; }

	    /// <summary>
	    /// Is de deelnemer contactpersoon voor de uitstap in kwestie?
	    /// </summary>
	    [DataMember]
		public bool IsContact { get; set; }
	}
}