// <copyright company="Chirojeugd-Vlaanderen vzw">
// Copyright (c) 2007-2010
// Mail naar informatica@chiro.be voor alle info over deze broncode
// </copyright>

using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Chiro.Gap.ServiceContracts
{
	/// <summary>
	/// Datacontract voor gedetailleerde informatie over een groep
	/// </summary>
	[DataContract]
	public class GroepDetail : GroepInfo
	{
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
		public List<FunctieInfo> Functies { get; set; }
	}
}
