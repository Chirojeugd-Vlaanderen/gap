// <copyright company="Chirojeugd-Vlaanderen vzw">
// Copyright (c) 2007-2010
// Mail naar informatica@chiro.be voor alle info over deze broncode
// </copyright>

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

using Chiro.Gap.Orm;

namespace Chiro.Gap.ServiceContracts
{
	[DataContract]
	public class PersoonsAdresInfo
	{
		[DataMember]
		public AdresInfo AdresInfo { get; set; }

		[DataMember]
		public AdresTypeEnum AdresType { get; set; }
	}
}
