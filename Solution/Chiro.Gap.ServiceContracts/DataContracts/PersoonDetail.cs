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
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Chiro.Gap.ServiceContracts.DataContracts
{
	/// <summary>
	/// DataContract voor uitgebreide info over personen
	/// </summary>
	[DataContract]
	public class PersoonDetail : PersoonInfo
	{
		/// <summary>
		/// De ID van de persoon
		/// </summary>
		[DataMember]
		public int PersoonID { get; set; }

		// TODO (#546): conceptuele verwarring ivm IsLid

		/// <summary>
		/// Geeft aan of de persoon als kind ingeschreven is in een groep
		/// </summary>
		[DataMember]
		public Boolean IsLid { get; set; }

		/// <summary>
		/// Geeft aan of de persoon als leiding ingeschreven is in een groep
		/// </summary>
		[DataMember]
		public Boolean IsLeiding { get; set; }

		// TODO (#546): conceptuele verwarring bij IsLid

		/// <summary>
		/// Als de persoon lid of leiding is (beter zou zijn: kind of leiding), dan
		/// bevat LidID het LidID.
		/// </summary>
		[DataMember]
		public int? LidID { get; set; }

		/// <summary>
		/// Geeft aan of de persoon op basis van zijn/haar leeftijd in één van de afdelingen past
		/// </summary>
		[DataMember]
		public Boolean KanLidWorden { get; set; }

		/// <summary>
		/// Geeft aan of de persoon de juiste leeftijd heeft om leiding te kunnen worden
		/// </summary>
		[DataMember]
		public Boolean KanLeidingWorden { get; set; }

		/// <summary>
		/// De lijst van categorieën die aan de persoon toegekend zijn
		/// </summary>
		[DataMember]
		public IList<CategorieInfo> CategorieLijst { get; set; }

		/// <summary>
		/// De ID van het adres dat als voorkeursadres gemarkeerd is
		/// voor deze persoon
		/// </summary>
		[DataMember]
		public int? VoorkeursAdresID { get; set; }

		/// <summary>
		/// Concatenatie van voornaam en naam
		/// </summary>
		public string VolledigeNaam
		{
			get
			{
				return VoorNaam + " " + Naam;
			}
		}
	}
}
