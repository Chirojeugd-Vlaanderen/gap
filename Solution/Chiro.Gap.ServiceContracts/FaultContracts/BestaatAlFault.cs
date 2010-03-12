// <copyright company="Chirojeugd-Vlaanderen vzw">
// Copyright (c) 2007-2010
// Mail naar informatica@chiro.be voor alle info over deze broncode
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace Chiro.Gap.ServiceContracts.FaultContracts
{
	/// <summary>
	/// Foutcodes voor een BestaatAlFault
	/// </summary>
	[DataContract]
	public enum BestaatAlFaultCode
	{
		[EnumMember]
		AlgemeneFout = 0,	            // standaardwaarde
		[EnumMember]
		CategorieCodeBestaatAl,	        // er is al een categorie met die code
		[EnumMember]
		CategorieNaamBestaatAl,         // er is al een categorie met die naam
		[EnumMember]
		FunctieCodeBestaatAl,		// er is al een functie met die code
		[EnumMember]
		FunctieNaamBestaatAl		// er is al een functie met die naam
		// TODO: foutcodes voor andere dingen die al kunnen bestaan.
	}

	/// <summary>
	/// Deze fault zal over de service gestuurd worden als de gebruiker probeert een dubbele entiteit te
	/// maken, terwijl dat niet mag.
	/// </summary>
	[DataContract]
	public class BestaatAlFault
	{
		[DataMember]
		public BestaatAlFaultCode FoutCode { get; set; }
	}
}
