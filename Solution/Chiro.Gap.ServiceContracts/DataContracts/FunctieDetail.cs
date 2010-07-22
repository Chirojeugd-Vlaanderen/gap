// <copyright company="Chirojeugd-Vlaanderen vzw">
// Copyright (c) 2007-2010
// Mail naar informatica@chiro.be voor alle info over deze broncode
// </copyright>

using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

using Chiro.Gap.Domain;

namespace Chiro.Gap.ServiceContracts.DataContracts
{
	/// <summary>
	/// DataContract voor informatie mbt functies
	/// TODO: Dit datacontract zou beter 'FunctieDetail' heten.
	/// </summary>
	[DataContract]
	public class FunctieDetail: FunctieInfo
	{
		[Verplicht]
		[DataMember]
		[DisplayName(@"Kan toegekend worden aan")]
		public LidType Type { get; set; }
		[DataMember]
		[DisplayName(@"Maximumaantal per groep")]
		public int? MaxAantal { get; set; }
		[Verplicht]
		[DataMember]
		[DisplayName(@"Minimumaantal per groep")]
		public int MinAantal { get; set; }
		[DataMember]
		[DisplayName(@"Ingevoerd in het werkjaar")]
		public int? WerkJaarVan { get; set; }
		[DataMember]
		[DisplayName(@"Afgeschaft in het werkjaar")]
		public int? WerkJaarTot { get; set; }
		[DataMember]
		public bool IsNationaal { get; set; }
	}
}
