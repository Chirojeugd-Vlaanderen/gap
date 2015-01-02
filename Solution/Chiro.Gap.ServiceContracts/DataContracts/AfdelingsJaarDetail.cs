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

using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Properties;
using System.Runtime.Serialization;
using Chiro.Gap.Domain;

namespace Chiro.Gap.ServiceContracts.DataContracts
{
	/// <summary>
	/// Datacontract voor gegevens van het afdelingsjaar.
	/// </summary>
	[DataContract]
	public class AfdelingsJaarDetail
	{
		/// <summary>
		/// De ID van het afdelingsjaar
		/// </summary>
		[DataMember]
		public int AfdelingsJaarID { get; set; }

		/// <summary>
		/// De ID van de afdeling zelf
		/// </summary>
		[DataMember]
		public int AfdelingID { get; set; }

		/// <summary>
		/// De ID van de corresponderende officiële afdeling
		/// </summary>
		[DataMember]
		[DisplayName(@"Officiële afdeling")]
		public int OfficieleAfdelingID { get; set; }

		// TODO (#595): onder- en bovengrens van geboortejaren mag hier niet hard gecodeerd zijn
		// Verplaatsen naar settings lukt niet, dus hebben we een attribuut nodig met eigen logica.
		// Bij de range van postnummers ligt de situatie anders, die is niet arbitrair en ze schuift ook niet op.

		/// <summary>
		/// Het jaar waarin de jongste leden van die afdeling geboren mogen zijn
		/// </summary>
		[DataMember]
		[Verplicht]
		[DisplayName(@"Geboortejaar van")]
        [Range(1940, 2100, ErrorMessageResourceType = typeof(Resources), ErrorMessageResourceName = "RangeError")]
		public int GeboorteJaarVan { get; set; }

		/// <summary>
		/// Het jaar waarin de oudste leden van die afdeling geboren mogen zijn
		/// </summary>
		[DataMember]
		[Verplicht]
		[DisplayName(@"Geboortejaar tot")]
		[Range(1940, 2100, ErrorMessageResourceType = typeof(Resources), ErrorMessageResourceName = "RangeError")]
		public int GeboorteJaarTot { get; set; }

		/// <summary>
		/// Het 'geslacht' van de afdeling: gemengd, alleen voor meisjes of alleen voor jongens
		/// </summary>
		[DataMember]
		[Verplicht]
		public GeslachtsType Geslacht { get; set; }

		/// <summary>
		/// Geeft stringrepresentatie van Versie weer (hex).
		/// Nodig om versie te bewaren in MVC view, voor concurrencycontrole.
		/// </summary>
		[DataMember]
		public string VersieString { get; set; }
	}
}
