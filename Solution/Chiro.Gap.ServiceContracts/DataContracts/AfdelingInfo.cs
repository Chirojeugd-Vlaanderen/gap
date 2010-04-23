// <copyright company="Chirojeugd-Vlaanderen vzw">
// Copyright (c) 2007-2010
// Mail naar informatica@chiro.be voor alle info over deze broncode
// </copyright>

using System.ComponentModel;
using System.Runtime.Serialization;

namespace Chiro.Gap.ServiceContracts
{
	/// <summary>
	/// Zeer beperkt datacontract voor afdelingsnaam en -code.
	/// </summary>
	[DataContract]
	public class AfdelingInfo
	{
		[DataMember]
		public int ID { get; set; }
		[DataMember]
		public string Naam { get; set; }
		[DataMember]
		[DisplayName(@"Code")]
		public string Afkorting { get; set; }
	}
}
