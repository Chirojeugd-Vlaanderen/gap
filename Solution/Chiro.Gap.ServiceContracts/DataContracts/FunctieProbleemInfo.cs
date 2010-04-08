// <copyright company="Chirojeugd-Vlaanderen vzw">
// Copyright (c) 2007-2010
// Mail naar informatica@chiro.be voor alle info over deze broncode
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Chiro.Gap.ServiceContracts
{
	/// <summary>
	/// Datacontract voor problemen met aantallen en functies
	/// </summary>
	[DataContract]
	public class FunctieProbleemInfo
	{
		[DataMember]
		public int ID { get; set; }
		[DataMember]
		public string Code { get; set; }
		[DataMember]
		public string Naam { get; set; }
		[DataMember]
		public int? MaxAantal { get; set; }
		[DataMember]
		public int MinAantal { get; set; }
		[DataMember]
		public int EffectiefAantal { get; set; }
	}
}
