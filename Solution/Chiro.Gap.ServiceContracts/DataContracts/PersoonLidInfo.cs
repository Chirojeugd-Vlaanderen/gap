// <copyright company="Chirojeugd-Vlaanderen vzw">
// Copyright (c) 2007-2010
// Mail naar informatica@chiro.be voor alle info over deze broncode
// </copyright>

using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Chiro.Gap.ServiceContracts.DataContracts
{
	/// <summary>
	/// 
	/// </summary>
	[DataContract]
	public class PersoonLidInfo
	{
		[DataMember]
		public PersoonDetail PersoonDetail { get; set; }

		// TODO booleans voor wat er geladen is

		[DataMember]
		public LidInfo LidInfo { get; set; }

		[DataMember]
		public IEnumerable<PersoonsAdresInfo> PersoonsAdresInfo { get; set; }

		[DataMember]
		public IEnumerable<CommunicatieDetail> CommunicatieInfo { get; set; }
	}
}
