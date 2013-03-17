// <copyright company="Chirojeugd-Vlaanderen vzw">
// Copyright (c) 2007-2012
// Mail naar informatica@chiro.be voor alle info over deze broncode
// </copyright>

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
