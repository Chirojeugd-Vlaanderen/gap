// <copyright company="Chirojeugd-Vlaanderen vzw">
// Copyright (c) 2007-2010
// Mail naar informatica@chiro.be voor alle info over deze broncode
// </copyright>

using System.Runtime.Serialization;

namespace Chiro.Gap.ServiceContracts
{
	/// <summary>
	/// DataContract voor informatie mbt functies
	/// </summary>
	[DataContract]
	public class FunctieInfo
	{
		[DataMember]
		public int ID { get; set; }
		[DataMember]
		public string Code { get; set; }
		[DataMember]
		public string Naam { get; set; }
		[DataMember]
		public int? MaxAantal { get; set; }
		[DataMember]
		public int MinAantal { get; set; }
		[DataMember]
		public int? WerkJaarVan { get; set; }
		[DataMember]
		public int? WerkJaarTot { get; set; }
		[DataMember]
		public bool IsNationaal { get; set; }
	}
}
