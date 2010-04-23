// <copyright company="Chirojeugd-Vlaanderen vzw">
// Copyright (c) 2007-2010
// Mail naar informatica@chiro.be voor alle info over deze broncode
// </copyright>

using System;
using System.Runtime.Serialization;

namespace Chiro.Gap.Domain
{
	/// <summary>
	/// Maakt een onderscheid tussen kinderen en leiding
	/// </summary>
	[DataContract]
	[Flags]
	public enum LidType
	{
		[EnumMember]
		Kind = 0x01,
		[EnumMember]
		Leiding = 0x02,
		[EnumMember]
		Alles = Kind | Leiding
	}

	/// <summary>
	/// Enum om informatie over het geslacht over te brengen
	/// </summary>
	/// <remarks>Kan zowel over personen als over groepen/afdelingen gaan</remarks>
	[DataContract]
	[Flags]
	public enum GeslachtsType
	{
		[EnumMember]
		Onbekend = 0x00,
		[EnumMember]
		Man = 0x01,
		[EnumMember]
		Vrouw = 0x02,
		[EnumMember]
		Gemengd = Man | Vrouw	// interessant voor gemengde groepen/afdelingen
	}

	/// <summary>
	/// Bepaalt de 'status' van het adres
	/// </summary>
	[DataContract]
	public enum AdresTypeEnum
	{
		[EnumMember]
		Thuis = 1,
		[EnumMember]
		Kot = 2,
		[EnumMember]
		Werk = 3,
		[EnumMember]
		Overig = 4
	}
}