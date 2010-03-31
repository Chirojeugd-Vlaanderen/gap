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
	/// <typeparam name="T">Type van de foutcode</typeparam>
	[DataContract]
	public class FoutCodeFault<T>
	{
		/// <summary>
		/// De foutcode
		/// </summary>
		[DataMember]
		public T FoutCode { get; set; }
	}
}
