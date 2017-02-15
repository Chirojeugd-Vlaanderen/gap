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
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using Chiro.Gap.Domain;

namespace Chiro.Gap.ServiceContracts.DataContracts
{
	/// <summary>
	/// DataContract voor summiere info over leden/leiding
	/// </summary>
	[DataContract]
	public class LidInfo
	{
		/// <summary>
		/// De ID van het lid
		/// </summary>
		[DataMember]
		public int LidID { get; set; }

		/// <summary>
		/// Kind of leiding
		/// </summary>
		[DataMember]
		public LidType Type { get; set; }

		/// <summary>
		/// Geeft aan of het lidgeld voor dat lid al betaald is of niet
		/// </summary>
		[DataMember]
		[DisplayName(@"Lidgeld betaald?")]
		public bool LidgeldBetaald { get; set; }

		/// <summary>
		/// De datum van het einde van de instapperiode
		/// enkel voor kinderen en niet aanpasbaar
		/// </summary>
		[DataMember]
		[DisplayName(@"Probeerperiode")]
		[DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ConvertEmptyStringToNull = true)]
		public DateTime EindeInstapperiode { get; set; }

		/// <summary>
		/// Geeft aan of het lid inactief is of niet
		/// </summary>
		[DataMember]
		[DisplayName(@"Non-actief")]
		public bool NonActief { get; set; }

        // FIXME: het is wat raar dat zowel afdeling-ID's als afdelingsafkortingen worden meegegeven
        // in aparte arrays.

		/// <summary>
		/// De lijst van afdelingIDs waarin het lid zit (1 voor een kind)
		/// </summary>
		[DataMember]
		public IList<int> AfdelingIdLijst { get; set; }

        /// <summary>
        /// Lijst van afdelingsafkortingen.
        /// </summary>
        [DataMember]
        public IList<string> AfdelingAfkortingLijst { get; set; }

        /// <summary>
        /// Functies van het lid
        /// </summary>
        [DataMember]
		public IList<FunctieDetail> Functies { get; set; }

		/// <summary>
		/// Groepswerkjaar waarvoor het lid ingeschreven is
		/// </summary>
		[DataMember]
		public int GroepsWerkJaarID { get; set; }

		/// <summary>
		/// Geeft aan of het lid verzekerd is tegen loonverlies
		/// </summary>
		[DataMember]
		public bool VerzekeringLoonVerlies { get; set; }

	}
}
