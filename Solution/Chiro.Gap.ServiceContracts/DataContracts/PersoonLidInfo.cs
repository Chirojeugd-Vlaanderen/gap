// <copyright company="Chirojeugd-Vlaanderen vzw">
// Copyright (c) 2007-2010
// Mail naar informatica@chiro.be voor alle info over deze broncode
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

using Chiro.Gap.Orm;
using System.ComponentModel;

namespace Chiro.Gap.ServiceContracts
{
	[DataContract]
	public class PersoonLidInfo
	{
		[DataMember]
		public PersoonInfo PersoonInfo { get; set; }

		//TODO booleans voor wat er geladen is

		[DataMember]
		public LidInfo LidInfo { get; set; }

		[DataMember]
		public IEnumerable<PersoonsAdresInfo> PersoonsAdresInfo { get; set; }

		[DataMember]
		public IEnumerable<CommunicatieInfo> CommunicatieInfo { get; set; }
	}
}
