using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Chiro.Gap.ServiceContracts
{
	/// <summary>
	/// Zeer beperkt datacontract voor afdelingsnaam en -code.
	/// </summary>
	[DataContract]
	public class AfdelingInfo
	{
		[DataMember]
		public int ID { get; set; }
		[DataMember]
		public string Naam { get; set; }
		[DataMember]
		[DisplayName("Code")]
		public string Afkorting { get; set; }
	}
}
