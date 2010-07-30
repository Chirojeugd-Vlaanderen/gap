// <copyright company="Chirojeugd-Vlaanderen vzw">
// Copyright (c) 2007-2010
// Mail naar informatica@chiro.be voor alle info over deze broncode
// </copyright>

using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Properties;
using System.Runtime.Serialization;

using Chiro.Gap.Domain;

namespace Chiro.Gap.ServiceContracts.DataContracts
{
	/// <summary>
	/// Datacontract voor gegevens van het afdelingsjaar.
	/// </summary>
	[DataContract]
	public class AfdelingsJaarDetail
	{
		/// <summary>
		/// De ID van het afdelingsjaar
		/// </summary>
		[DataMember]
		public int AfdelingsJaarID { get; set; }

		/// <summary>
		/// De ID van de afdeling zelf
		/// </summary>
		[DataMember]
		public int AfdelingID { get; set; }

		/// <summary>
		/// De ID van de corresponderende officiële afdeling
		/// </summary>
		[DataMember]
		[DisplayName(@"Officiele afdeling")]
		public int OfficieleAfdelingID { get; set; }

		// TODO: onder- en bovengrens van geboortejaren mag hier niet hard gecodeerd zijn (ticket #595)
		// Verplaatsen naar settings lukt niet, dus hebben we een attribuut nodig met eigen logica.
		// Bij de range van postnummers ligt de situatie anders, die is niet arbitrair en ze schuift ook niet op.

		/// <summary>
		/// Het jaar waarin de jongste leden van die afdeling geboren mogen zijn
		/// </summary>
		[DataMember]
		[Verplicht]
		[DisplayName(@"Geboortejaar van")]
		[Range(1940, 2100, ErrorMessageResourceType = typeof(Resources), ErrorMessageResourceName = "AfdelingsJaarDetail_GeboorteJaar_RangeFout")]
		public int GeboorteJaarVan { get; set; }

		/// <summary>
		/// Het jaar waarin de oudste leden van die afdeling geboren mogen zijn
		/// </summary>
		[DataMember]
		[Verplicht]
		[DisplayName(@"Geboortejaar tot")]
		[Range(1940, 2100, ErrorMessageResourceType = typeof(Resources), ErrorMessageResourceName = "AfdelingsJaarDetail_GeboorteJaar_RangeFout")]
		public int GeboorteJaarTot { get; set; }

		/// <summary>
		/// Het 'geslacht' van de afdeling: gemengd, alleen voor meisjes of alleen voor jongens
		/// </summary>
		[DataMember]
		[Verplicht]
		public GeslachtsType Geslacht { get; set; }

		/// <summary>
		/// Geeft stringrepresentatie van Versie weer (hex).
		/// Nodig om versie te bewaren in MVC view, voor concurrencycontrole.
		/// </summary>
		[DataMember]
		public string VersieString { get; set; }
	}
}
