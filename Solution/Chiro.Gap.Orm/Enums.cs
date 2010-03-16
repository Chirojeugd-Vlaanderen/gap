// <copyright company="Chirojeugd-Vlaanderen vzw">
// Copyright (c) 2007-2010
// Mail naar informatica@chiro.be voor alle info over deze broncode
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Chiro.Gap.Orm
{
	/// <summary>
	/// Enum die aangeeft welke extra's er meegeleverd kunnen worden met een groep.
	/// </summary>
	/// <remarks>Er zullen nog extra members aan deze enum toegevoegd moeten worden</remarks>
	[DataContract]
	[Flags]
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

	/// <summary>
	/// Enum om aan te geven welke extra informatie er met een lid mee opgehaald moet worden.
	/// </summary>
	[DataContract]
	[Flags]
	public enum LidExtras
	{
		[EnumMember]
		Geen = 0x00,
		/// <summary>
		/// Haalt groepswerkjaar en groep mee op
		/// </summary>
		[EnumMember]
		Groep = 0x01,
		/// <summary>
		/// Haalt afdelingsjaren en afdelingen mee op
		/// </summary>
		[EnumMember]
		Afdelingen = 0x02,
		[EnumMember]
		Functies = 0x04,
		[EnumMember]
		Alles = Groep | Afdelingen | Functies
	}
}
