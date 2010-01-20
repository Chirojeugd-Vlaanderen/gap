using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Chiro.Gap.ServiceContracts
{
	/// <summary>
	/// Enum die aangeeft welke extra's er meegeleverd kunnen worden met de gewone GroepInfo.
	/// </summary>
	/// <remarks>Er zullen nog extra members aan deze enum toegevoegd moeten worden</remarks>
	[DataContract]
	public enum GroepsExtras
	{
		[EnumMember]
		Geen = 0x00,
		[EnumMember]
		AfdelingenHuidigWerkJaar = 0x01,
		[EnumMember]
		Categorieen = 0x02,
		[EnumMember]
		Alles = AfdelingenHuidigWerkJaar | Categorieen
	}
}
