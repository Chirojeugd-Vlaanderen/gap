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
	/// DataContract voor een afdeling die geactiveerd moet worden in een bepaald groepswerkjaar
	/// </summary>
	[DataContract]
	public class TeActiverenAfdeling
	{
		/// <summary>
		/// ID van de afdeling
		/// </summary>
		[DataMember]
		public int AfdelingID { get; set; }

		/// <summary>
		/// ID van de officieel erkende afdeling waar deze afdeling op gemapt wordt
		/// </summary>
		[DataMember]
		public int OfficieleAfdelingID { get; set; }

		/// <summary>
		/// Geboortejaar van de oudste leden
		/// </summary>
		[DataMember]
		[Verplicht]
		[DisplayName(@"Geboortejaar van")]
		[Range(1940, 2100, ErrorMessageResourceType = typeof(Resources), ErrorMessageResourceName = "RangeError")]
		public int GeboorteJaarVan { get; set; }

		/// <summary>
		/// Geboortejaar van de oudste leden
		/// </summary>
		[DataMember]
		[Verplicht]
		[DisplayName(@"Geboortejaar tot")]
		[Range(1940, 2100, ErrorMessageResourceType = typeof(Resources), ErrorMessageResourceName = "RangeError")]
		public int GeboorteJaarTot { get; set; }

		/// <summary>
		/// Het 'geslacht' van de afdeling
		/// </summary>
		[DataMember]
		[Verplicht]
		public GeslachtsType Geslacht { get; set; }
	}
}