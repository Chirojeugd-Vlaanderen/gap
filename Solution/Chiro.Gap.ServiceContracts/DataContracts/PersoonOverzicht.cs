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

using System.Runtime.Serialization;

namespace Chiro.Gap.ServiceContracts.DataContracts
{
	/// <summary>
	/// Datacontract met informatie over (gelieerde) personen dat in eerste instantie enkel gebruikt
	/// zal worden door de Excelexport.  Gegevens van voorkeursadres, voorkeurstelefoonnummer en
	/// voorkeursmailadres worden mee opgenomen
	/// </summary>
	[DataContract]
	[KnownType(typeof(LidOverzicht))]
	public class PersoonOverzicht : PersoonInfo
	{
		[DataMember]
		public string StraatNaam;

		[DataMember]
		public int? HuisNummer;
		
		[DataMember]
		public string Bus;
		
		[DataMember]
		public int? PostNummer;

		[DataMember] public string PostCode;
		
		[DataMember]
		public string WoonPlaats;

		[DataMember] public string Land;
		
		[DataMember]
		public string TelefoonNummer;
		
		[DataMember]
		public string Email;
	}
}
