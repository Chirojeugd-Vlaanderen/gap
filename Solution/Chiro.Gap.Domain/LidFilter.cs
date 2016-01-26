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

namespace Chiro.Gap.Domain
{
	/// <summary>
	/// Filter die gebruikt kan worden om te zoeken op leden
	/// </summary>
	[DataContract]
	public class LidFilter
	{
		/// <summary>
		/// Indien niet <c>null</c>, zoek leden uit gegeven groep
		/// </summary>
		[DataMember]
		public int? GroepID { get; set; }

		/// <summary>
		/// Indien niet <c>null</c>, zoek leden uit gegeven groepswerkjaar
		/// </summary>
		[DataMember]
		public int? GroepsWerkJaarID { get; set; }

		/// <summary>
		/// Indien niet <c>null</c>, zoek leden uit gegeven afdeling
		/// </summary>
		[DataMember]
		public int? AfdelingID { get; set; }

		/// <summary>
		/// Indien niet <c>null</c>, zoek leden met gegeven functie
		/// </summary>
		[DataMember]
		public int? FunctieID { get; set; }

		/// <summary>
		/// Indien niet <c>null</c>, lever enkel leden op waarbij de
		/// probeerperiode na de gegeven datum eindigt.
		/// </summary>
		[DataMember]
		public DateTime? ProbeerPeriodeNa { get; set; }

		/// <summary>
		/// Indien <c>true</c>, lever enkel leden op met voorkeuradres,
		/// indien <c>false</c>, lever enkel leden op zonder voorkeuradres,
		/// indien <c>null</c>, negeer.
		/// </summary>
		[DataMember]
		public bool? HeeftVoorkeurAdres { get; set; }

		/// <summary>
		/// Indien <c>true</c>, lever enkel leden op met telefoonnummer,
		/// indien <c>false</c>, lever enkel leden op zonder telefoonnummer,
		/// indien <c>null</c>, negeer.
		/// </summary>
		[DataMember]
		public bool? HeeftTelefoonNummer { get; set; }

		/// <summary>
		/// Indien <c>true</c>, lever enkel leden op met e-mailadres,
		/// indien <c>false</c>, lever enkel leden op zonder e-mailadres,
		/// indien <c>null</c>, negeer.
		/// </summary>
		[DataMember]
		public bool? HeeftEmailAdres { get; set; }

		/// <summary>
		/// Als <c>LidType.Kind</c> of <c>LidType.Leiding</c>, dan worden enkel leden van het gevraagde type
		/// opgeleverd.
		/// </summary>
		[DataMember]
		public LidType LidType { get; set; }
	}
}
