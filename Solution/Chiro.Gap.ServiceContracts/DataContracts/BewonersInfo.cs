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
using System.Runtime.Serialization;
using Chiro.Gap.Domain;

namespace Chiro.Gap.ServiceContracts.DataContracts
{
	/// <summary>
	/// Datacontract dat gebruikt wordt voor informatie over een bewoner.  Het kan ook dienen voor
	/// minimale persoonsinfo, als je <c>AdresType</c> negeert.
	/// </summary>
	/// <remarks>De namen van de property's zijn zodanig gekozen, dat AutoMapper niet
	/// speciaal geconfigureerd moet worden om te mappen van PersoonsAdres.</remarks>
	[DataContract]
	public class BewonersInfo
	{
		/// <summary>
		/// AD-nummer van de bewoner
		/// </summary>
		[DataMember]
		public int? PersoonAdNummer { get; set; }

		/// <summary>
		/// De ID van de bewoner als gelieerde persoon
		/// </summary>
		[DataMember]
		public int GelieerdePersoonID { get; set; }

		/// <summary>
		/// De ID van de bewoner als persoon
		/// </summary>
		[DataMember]
		public int PersoonID { get; set; }

		/// <summary>
		/// Voornaam en naam van de bewoner
		/// </summary>
		[DataMember]
		public string PersoonVolledigeNaam { get; set; }

		/// <summary>
		/// Geboortedatum van de bewoner
		/// </summary>
		[DataMember]
		public DateTime? PersoonGeboorteDatum { get; set; }

		/// <summary>
		/// Geslacht van de bewoner
		/// </summary>
		[DataMember]
		public GeslachtsType PersoonGeslacht { get; set; }

		/// <summary>
		/// Het type dat de relatie van de bewoner met het adres beschrijft
		/// (bv. 'kotadres')
		/// </summary>
		[DataMember]
		public AdresTypeEnum AdresType { get; set; }
	}
}
