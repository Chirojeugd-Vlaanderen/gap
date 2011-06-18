// <copyright company="Chirojeugd-Vlaanderen vzw">
// Copyright (c) 2007-2011
// Mail naar informatica@chiro.be voor alle info over deze broncode
// </copyright>

using System.Collections.Generic;
using System.Runtime.Serialization;

using Chiro.Gap.Domain;

namespace Chiro.Gap.ServiceContracts.DataContracts
{
	/// <summary>
	/// Datacontract voor beperkte info ivm een deelnemer van een uitstap.
	/// </summary>
	[DataContract]
	public class DeelnemerDetail : DeelnemerInfo
	{
	    [DataMember]
		public int GelieerdePersoonID { get; set; }

		[DataMember]
		public PersoonLidInfo PersoonLidInfo { get; set; }

        [DataMember]
        public int UitstapID { get; set; }

		[DataMember]
		public string VoorNaam { get; set; }

		[DataMember]
		public string FamilieNaam { get; set; }

		[DataMember]
		public DeelnemerType Type { get; set; }

	    [DataMember]
		public IList<AfdelingInfo> Afdelingen { get; set; }

		[DataMember]
		public bool IsContact { get; set; }
	}
}
