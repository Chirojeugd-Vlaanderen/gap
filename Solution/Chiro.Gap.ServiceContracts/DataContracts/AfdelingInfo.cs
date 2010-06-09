// <copyright company="Chirojeugd-Vlaanderen vzw">
// Copyright (c) 2007-2010
// Mail naar informatica@chiro.be voor alle info over deze broncode
// </copyright>

using System.ComponentModel;
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
		[DataMember]
		public int ID { get; set; }

        [Verplicht]
        [StringLengte(50), StringMinimumLengte(2)]
		[DataMember]
		public string Naam { get; set; }

		[DataMember]
		[DisplayName(@"Code")]
        [Verplicht]
        [StringLengte(10), StringMinimumLengte(1)]
		public string Afkorting { get; set; }
	}
}
