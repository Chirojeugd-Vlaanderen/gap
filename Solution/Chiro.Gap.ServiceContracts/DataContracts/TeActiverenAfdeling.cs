// <copyright company="Chirojeugd-Vlaanderen vzw">
// Copyright (c) 2007-2010
// Mail naar informatica@chiro.be voor alle info over deze broncode
// </copyright>

using System.Runtime.Serialization;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

using Chiro.Gap.Domain;

namespace Chiro.Gap.ServiceContracts.DataContracts
{
	[DataContract]
	public class TeActiverenAfdeling
	{
		[DataMember]
		public int AfdelingID { get; set; }

		[DataMember]
		public int OfficieleAfdelingID { get; set; }
		
		[DataMember]
		[Verplicht]
		[DisplayName(@"Geboortejaar van")]
		[Range(1940, 2100, ErrorMessage = "{0} is beperkt van {1} tot {2}.")]
		public int GeboorteJaarVan { get; set; }

		[DataMember]
		[Verplicht]
		[DisplayName(@"Geboortejaar tot")]
		[Range(1940, 2100, ErrorMessage = "{0} is beperkt van {1} tot {2}.")]
		public int GeboorteJaarTot { get; set; }

		[DataMember]
		[Verplicht]
		public GeslachtsType Geslacht { get; set; }
	}
}