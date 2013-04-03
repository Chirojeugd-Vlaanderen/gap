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

using System.Runtime.Serialization;

namespace Chiro.Gap.ServiceContracts.DataContracts
{
	/// <summary>
	/// Datacontract voor info over officiele afdeling
	/// </summary>
	[DataContract]
	public class OfficieleAfdelingDetail
	{
		/// <summary>
		/// Naam van de officiele afdeling
		/// </summary>
		[DataMember]
		public string Naam { get; set; }

		/// <summary>
		/// De OfficieleAfdelingID
		/// </summary>
		[DataMember]
		public int ID { get; set; }

		/// <summary>
		/// Standaard oudste leeftijd voor deze afdeling
		/// </summary>
		[DataMember]
		public int LeefTijdVan { get; set; }

		/// <summary>
		/// Standaard jongste leeftijd voor deze afdeling
		/// </summary>
		[DataMember]
		public int LeefTijdTot { get; set; }

		// TODO (#704): StandaardGeboorteJaarVan en StandaardGeboorteJaarTot mogen wat mij betreft gerust
		// in het datacontract zitten, maar dan gewoon als 'data member'.  Het berekenen van deze
		// jaren hoort thuis in de business, en mag niet door het datacontract zelf gebeuren

		/// <summary>
		/// Standaard 'geboortejaar van' voor gegeven werkJaar
		/// </summary>
		/// <param name="werkJaar">Het werkJaar waar het over gaat</param>
		/// <returns>Een jaartal</returns>
		public int StandaardGeboorteJaarVan(int werkJaar)
		{
			return werkJaar - LeefTijdTot;
		}

		/// <summary>
		/// Standaard 'geboortejaar van' voor dit werkJaar
		/// </summary>
		/// <param name="werkJaar">Het werkJaar waar het over gaat</param>
		/// <returns>Een jaartal</returns>
		public int StandaardGeboorteJaarTot(int werkJaar)
		{
			return werkJaar - LeefTijdVan;
		}
	}
}
