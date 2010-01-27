using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Chiro.Gap.ServiceContracts.FaultContracts
{
	/// <summary>
	/// Foutcodes voor een BestaatAlFault
	/// </summary>
	[DataContract]
	public enum BestaatAlFaultCode
	{
		[EnumMember]
		AlgemeneFout = 0,	// standaardwaarde
		[EnumMember]
		CategorieBestaatAl	// poging tot aanmaken dubbele categorie
		// TODO: foutcodes voor andere dingen die al kunnen bestaan.
	}

	/// <summary>
	/// Deze fault zal over de service gestuurd worden als de gebruiker probeert een dubbele entiteit te
	/// maken, terwijl dat niet mag.
	[DataContract]
	public class BestaatAlFault
	{
		[DataMember]
		public BestaatAlFaultCode FoutCode { get; set; }
	}
}
