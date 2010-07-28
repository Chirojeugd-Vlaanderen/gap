// <copyright company="Chirojeugd-Vlaanderen vzw">
// Copyright (c) 2007-2010
// Mail naar informatica@chiro.be voor alle info over deze broncode
// </copyright>

using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Properties;
using System.Runtime.Serialization;

using Chiro.Gap.Domain;
using System.ComponentModel.DataAnnotations;

namespace Chiro.Gap.ServiceContracts.DataContracts
{
	/// <summary>
	/// Datacontract voor gegevens van het afdelingsjaar.
	/// </summary>
	[DataContract]
	public class AfdelingsJaarDetail
	{
		[DataMember]
		public int AfdelingsJaarID { get; set; }

		[DataMember]
		public int AfdelingID { get; set; }

		[DataMember]
		[DisplayName(@"Officiele afdeling")]
		public int OfficieleAfdelingID { get; set; }

		// TODO: onder- en bovengrens van geboortejaren mag hier niet hard gecodeerd zijn (ticket #595)
		// Verplaatsen naar settings lukt niet, dus hebben we een attribuut nodig met eigen logica.

		[DataMember]
		[Verplicht]
		[DisplayName(@"Geboortejaar van")]
        [Range(1940, 2100, ErrorMessageResourceType = typeof (Resources), ErrorMessageResourceName = "AfdelingsJaarDetail_GeboorteJaar_RangeFout")]
		public int GeboorteJaarVan { get; set; }

		[DataMember]
		[Verplicht]
		[DisplayName(@"Geboortejaar tot")]
        [Range(1940, 2100, ErrorMessageResourceType = typeof (Resources), ErrorMessageResourceName = "AfdelingsJaarDetail_GeboorteJaar_RangeFout")]
		public int GeboorteJaarTot { get; set; }

		[DataMember]
        [Verplicht]
		public GeslachtsType Geslacht { get; set; }

		[DataMember]
		public string VersieString { get; set; }
	}
}
