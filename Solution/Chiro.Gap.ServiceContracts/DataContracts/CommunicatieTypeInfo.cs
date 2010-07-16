// <copyright company="Chirojeugd-Vlaanderen vzw">
// Copyright (c) 2007-2010
// Mail naar informatica@chiro.be voor alle info over deze broncode
// </copyright>

using System.Runtime.Serialization;

namespace Chiro.Gap.ServiceContracts.DataContracts
{
	[DataContract]
	public class CommunicatieTypeInfo
	{
		[DataMember]
		public int ID { get; set; }

		[DataMember]
		public string Omschrijving { get; set; }

		[DataMember]
		public bool IsOptIn { get; set; }

		[DataMember]
		public string Validatie { get; set; }

		[DataMember]
		public string Voorbeeld { get; set; }
	}
}
