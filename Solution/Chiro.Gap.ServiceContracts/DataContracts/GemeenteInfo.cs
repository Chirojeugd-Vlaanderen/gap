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

namespace Chiro.Gap.ServiceContracts
{
	[DataContract]
	public class GemeenteInfo
	{
		[DataMember]
		public int PostNummer { get; set; }

		[DataMember]
		public String Naam { get; set; }
	}
}
