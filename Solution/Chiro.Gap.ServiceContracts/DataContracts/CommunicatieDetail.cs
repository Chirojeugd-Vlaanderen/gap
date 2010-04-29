// <copyright company="Chirojeugd-Vlaanderen vzw">
// Copyright (c) 2007-2010
// Mail naar informatica@chiro.be voor alle info over deze broncode
// </copyright>

using System.Runtime.Serialization;

using Chiro.Gap.Domain;

namespace Chiro.Gap.ServiceContracts
{
	[DataContract]
	public class CommunicatieDetail: CommunicatieInfo, ICommunicatie
	{
		[DataMember]
		public string CommunicatieTypeOmschrijving { get; set; }

		[DataMember]
		public string CommunicatieTypeValidatie { get; set; }

		[DataMember]
		public string CommunicatieTypeVoorbeeld { get; set; }

	}
}
