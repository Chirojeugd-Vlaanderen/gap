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
using Chiro.Gap.Domain;

namespace Chiro.Gap.ServiceContracts.DataContracts
{
	/// <summary>
	/// AdresInfo, met als extra informatie het AdresType (thuisadres, werkadres,...)
	/// </summary>
	[DataContract]
	public class PersoonsAdresInfo : AdresInfo
	{
		/// <summary>
		/// Enumwaarde die de relatie van de persoon tot het adres beschrijft (bv. 'kotadres', 'thuis')
		/// </summary>
		[DataMember]
		public AdresTypeEnum AdresType { get; set; }

		/// <summary>
		/// Het PersoonsAdresID
		/// </summary>
		[DataMember]
		public int PersoonsAdresID { get; set; }
	}

	/// <summary>
	/// Flauw datacontractje dat enkel een PersoonID en een AdresID bevat.  Wordt gebruikt om informatie
	/// mee te geven over al bestaande adressen
	/// (Helaas had ik een datacontract geschreven met dezelfde naam als een ander datacontract van Broes.)
	/// </summary>
	[DataContract]
	public class PersoonsAdresInfo2
	{
		/// <summary>
		/// De ID van de persoon
		/// </summary>
		[DataMember]
		public int PersoonID { get; set; }

		/// <summary>
		/// De ID van het adres
		/// </summary>
		[DataMember]
		public int AdresID { get; set; }
	}
}
