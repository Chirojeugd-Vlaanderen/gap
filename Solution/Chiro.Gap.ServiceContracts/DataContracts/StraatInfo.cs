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

using System;
using System.Runtime.Serialization;

namespace Chiro.Gap.ServiceContracts.DataContracts
{
	/// <summary>
	/// DataContract voor summiere info over straten
	/// </summary>
	[DataContract]
	public class StraatInfo
	{
		/// <summary>
		/// Het postnummer van de gemeente waar de straat ligt
		/// </summary>
		[DataMember]
		public int PostNummer { get; set; }

		/// <summary>
		/// De straatnaam zelf
		/// </summary>
		[DataMember]
		public String Naam { get; set; }
	}
}
