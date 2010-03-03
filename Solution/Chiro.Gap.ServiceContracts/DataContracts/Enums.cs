// <copyright company="Chirojeugd-Vlaanderen vzw">
// Copyright (c) 2007-2010
// Mail naar informatica@chiro.be voor alle info over deze broncode
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

using Chiro.Gap.Orm;

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
