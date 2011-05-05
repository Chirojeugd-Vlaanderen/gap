using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

using Chiro.Gap.Domain;

namespace Chiro.Gap.ServiceContracts.DataContracts
{
	/// <summary>
	/// Datacontract voor beperkte info ivm een deelnemer van een uitstap.
	/// </summary>
	[DataContract]
	public class UitstapDeelnemerInfo
	{
		[DataMember]
		public int DeelnemerID { get; set; }

		[DataMember]
		public int GelieerdePersoonID { get; set; }

		[DataMember]
		public string VoorNaam { get; set; }

		[DataMember]
		public string FamilieNaam { get; set; }

		[DataMember]
		public DeelnemerType Type { get; set; }

		[DataMember]
		public bool HeeftBetaald { get; set; }

		[DataMember]
		public bool MedischeFicheOk { get; set; }

		[DataMember]
		public string Opmerkingen { get; set; }

		[DataMember]
		public IList<AfdelingInfo> Afdelingen { get; set; }

		[DataMember]
		public bool IsContact { get; set; }
	}
}
