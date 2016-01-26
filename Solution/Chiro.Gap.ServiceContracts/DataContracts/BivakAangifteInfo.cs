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
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using Chiro.Gap.Domain;

namespace Chiro.Gap.ServiceContracts.DataContracts
{
	/// <summary>
	/// DataContract voor info over de status van een bivakaangifte
	/// </summary>
	[DataContract]
	public class BivakAangifteInfo
	{
		/// <summary>
		/// Het unieke ID van het bivak
		/// </summary>
		[Verplicht]
		[DataMember]
		public int ID { get; set; }

		/// <summary>
		/// De omschrijving van het bivak
		/// </summary>
		[Verplicht]
		[DataMember]
		public String Omschrijving { get; set; }

		/// <summary>
		/// De huidige status van de bivakaangifte
		/// </summary>
		[Verplicht]
		[DataMember]
		public BivakAangifteStatus Status { get; set; }

		/// <summary>
		/// Geeft stringrepresentatie van Versie weer (hex).
		/// Nodig om versie te bewaren in MVC view, voor concurrencycontrole.
		/// </summary>
		[DataMember]
		public string VersieString { get; set; }
	}
}