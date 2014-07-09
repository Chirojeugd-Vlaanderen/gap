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

using System.Collections.Generic;
using System.Runtime.Serialization;

using Chiro.Gap.Domain;

namespace Chiro.Gap.ServiceContracts.DataContracts
{
	/// <summary>
	/// Datacontract voor gedetailleerde informatie over een groep
	/// </summary>
	[DataContract]
	public class GroepDetail : GroepInfo
	{
        public GroepDetail()
        {
            Afdelingen = new List<AfdelingDetail>();
            Categorieen = new List<CategorieInfo>();
            Functies = new List<FunctieDetail>();
        }

		/// <summary>
		/// Afdelingen waarvoor er in het recentste groepswerkjaar een afdelingsjaar bestaat.
		/// </summary>
		[DataMember]
		public List<AfdelingDetail> Afdelingen { get; set; }

		/// <summary>
		/// Beschikbare categorieen
		/// </summary>
		[DataMember]
		public List<CategorieInfo> Categorieen { get; set; }

		/// <summary>
		/// Functies die de groep gebruikt
		/// </summary>
		[DataMember]
		public List<FunctieDetail> Functies { get; set; }

		/// <summary>
		/// Niveau van de groep
		/// </summary>
		[DataMember]
		public Niveau Niveau { get; set; }
	}
}
