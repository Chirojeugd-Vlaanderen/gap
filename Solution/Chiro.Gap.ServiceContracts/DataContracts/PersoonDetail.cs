// <copyright company="Chirojeugd-Vlaanderen vzw">
// Copyright (c) 2007-2010
// Mail naar informatica@chiro.be voor alle info over deze broncode
// </copyright>

using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Chiro.Gap.ServiceContracts.DataContracts
{
	[DataContract]
	public class PersoonDetail : PersoonInfo
	{
		[DataMember]
		public int PersoonID{get;set;}

		[DataMember]
		public Boolean IsLid{get;set;}

		///Geeft aan of er plaats is in een van de afdelingen om iemand lid te maken
		[DataMember]
		public Boolean KanLidWorden { get; set; }

		[DataMember]
		public IList<CategorieInfo> CategorieLijst{ get; set; }

		[DataMember]
		public int? VoorkeursAdresID { get; set; }

		public string VolledigeNaam
		{
			get
			{
				return VoorNaam + " " + Naam;
			}
		}
	}
}
