using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Chiro.Gap.ServiceContracts.FaultContracts
{
	/// <summary>
	/// Faultcontract dat gebruikt wordt als een bestaande entiteit/object een operatie verhindert
	/// </summary>
	[DataContract]
	public class BlokkerendeObjectenFault<TFoutCode, TObject> : FoutCodeFault<TFoutCode>
	{
		[DataMember]
		public IEnumerable<TObject> Objecten { get; set; }
	}
}
