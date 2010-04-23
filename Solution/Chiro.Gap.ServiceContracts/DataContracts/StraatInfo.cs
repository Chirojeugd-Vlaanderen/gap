// <copyright company="Chirojeugd-Vlaanderen vzw">
// Copyright (c) 2007-2010
// Mail naar informatica@chiro.be voor alle info over deze broncode
// </copyright>

using System;
using System.Runtime.Serialization;

namespace Chiro.Gap.ServiceContracts
{
	[DataContract]
	public class StraatInfo
	{
		[DataMember]
		public int PostNummer { get; set; }

		[DataMember]
		public String Naam { get; set; }
	}
}
