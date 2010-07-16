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
	[DataContract]
	public class CommunicatieInfo: ICommunicatie
	{
		[DataMember]
		public int ID { get; set; }

		[Verplicht]
		[DataMember]
        [StringLengte(160)]
		public string Nummer { get; set; }

		[DataMember]
		[DisplayName(@"Mag gebruikt worden voor Snelleberichtenlijsten?")]
		public bool IsVoorOptIn { get; set; }

		[DataMember]
		[DisplayName(@"Gebruiken om persoon te contacteren?")]
		public bool Voorkeur { get; set; }

		[DataMember]
		[StringLengte(320)]
		[DataType(DataType.MultilineText)]
		public string Nota { get; set; }

		[DataMember]
		[DisplayName(@"Voor heel het gezin?")]
		public bool IsGezinsGebonden { get; set; }

		[DataMember]
		public string VersieString { get; set; }

		[DataMember]
        [Verplicht]
		public int CommunicatieTypeID { get; set; }

		[DataMember]
		public bool CommunicatieTypeIsOptIn { get; set; }

		[DataMember]
		public string CommunicatieTypeOmschrijving { get; set; }

		[DataMember]
		public string CommunicatieTypeValidatie { get; set; }

		[DataMember]
		public string CommunicatieTypeVoorbeeld { get; set; }
	}
}
