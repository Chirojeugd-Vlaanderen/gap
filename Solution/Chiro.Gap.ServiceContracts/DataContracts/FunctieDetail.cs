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
using System.Runtime.Serialization;
using Chiro.Gap.Domain;

namespace Chiro.Gap.ServiceContracts.DataContracts
{
	/// <summary>
	/// DataContract voor uitgebreide informatie mbt functies
	/// </summary>
	[DataContract]
	public class FunctieDetail : FunctieInfo
	{
		/// <summary>
		/// Het type lid waar de functie aan toegekend kan worden.
		/// </summary>
		/// <remarks>Voorbeeld: groepsleiding kan enkel bij leiding</remarks>
		[Verplicht]
		[DataMember]
		[DisplayName(@"Kan toegekend worden aan")]
		public LidType Type { get; set; }

		/// <summary>
		/// Het maximum aantal leden dat deze functie mag hebben per groep
		/// </summary>
		/// <remarks>Voorbeeld: er mag maar één contactpersoon zijn (nationaal bepaald), 
		/// maar de groep mag zelf kiezen hoeveel materiaalmeesters ze aanstelt.
		/// In dat laatste geval is het maximumaantal niet ingevuld (null).</remarks>
		[DataMember]
		[DisplayName(@"Maximumaantal per groep")]
		public int? MaxAantal { get; set; }

		/// <summary>
		/// Het minimum aantal leden dat deze functie moet hebben per groep
		/// </summary>
		/// <remarks>Voorbeeld: er moet iemand contactpersoon zijn (nationaal bepaald),
		/// maar de groep is niet verplicht om materiaalmeesters aan te stellen. In dat laatste
		/// geval is het minimumaantal 0.</remarks>
		[Verplicht]
		[DataMember]
		[DisplayName(@"Minimumaantal per groep")]
		public int MinAantal { get; set; }
		
		/// <summary>
		/// Het eerste werkJaar waarin de functie gebruikt werd in deze groep
		/// </summary>
		[DataMember]
		[DisplayName(@"Ingevoerd in het werkjaar")]
		public int? WerkJaarVan { get; set; }
		
		/// <summary>
		/// Het laatste werkJaar waarin de functie gebruikt werd in deze groep
		/// </summary>
		[DataMember]
		[DisplayName(@"Afgeschaft in het werkjaar")]
		public int? WerkJaarTot { get; set; }
		
		/// <summary>
		/// Geeft aan of de functie officieel erkend is (<c>true</c>)
		/// of dat het iets is dat de groep zelf invoerde 
		/// of waar Chirojeugd Vlaanderen niet in geïnteresseerd is (<c>false</c>)
		/// </summary>
		[DataMember]
		public bool IsNationaal { get; set; }
	}
}
