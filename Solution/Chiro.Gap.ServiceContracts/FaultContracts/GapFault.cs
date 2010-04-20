using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Chiro.Gap.ServiceContracts.FaultContracts
{
	/// <summary>
	/// Faultcontract dat een foutcode van type <typeparamref name="T"/> bevat
	/// </summary>
	[DataContract]
	public class GapFault
	{
		/// <summary>
		/// De foutcode
		/// </summary>
		[DataMember]
		public int FoutNummer { get; set; }
	}
}
