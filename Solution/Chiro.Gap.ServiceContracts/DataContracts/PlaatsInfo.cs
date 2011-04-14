using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

using Chiro.Gap.Domain;

namespace Chiro.Gap.ServiceContracts.DataContracts
{
	[DataContract]
	public class PlaatsInfo: AdresInfo
	{
		[DataMember]
		public string Naam { get; set; }
	}
}
