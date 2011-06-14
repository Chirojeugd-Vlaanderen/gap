// <copyright company="Chirojeugd-Vlaanderen vzw">
// Copyright (c) 2007-2011
// Mail naar informatica@chiro.be voor alle info over deze broncode
// </copyright>

using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace Chiro.Gap.ServiceContracts.DataContracts
{
	/// <summary>
	/// Zeer beperkt datacontract voor afdelingsnaam en -code.
	/// </summary>
	[DataContract]
	public class AfdelingInfo
	{
		/// <summary>
		/// De ID van de afdeling
		/// </summary>
		[DataMember]
		public int ID { get; set; }

		/// <summary>
		/// Naam van de afdeling
		/// </summary>
        [Verplicht]
        [StringLengte(50), StringMinimumLengte(2)]
		[DataMember]
		public string Naam { get; set; }

		/// <summary>
		/// Afkorting voor de afdeling
		/// </summary>
		[DataMember]
        [Verplicht]
        [StringLengte(10), StringMinimumLengte(1)]
		public string Afkorting { get; set; }

		/// <summary>
		/// Naam van de corresponderende officiële afdeling
		/// </summary>
		[DataMember]
		public string OfficieleAfdelingNaam { get; set; }
	}
}
