using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Chiro.Kip.Services.DataContracts
{
	/// <summary>
	/// Adrestypes uit Kipadmin
	/// </summary>
	[DataContract]
	public enum AdresTypeEnum
	{
		[EnumMember(Value = "THUIS")]
		Thuis = 1,
		[EnumMember(Value = "KOT")]
		Kot = 2,
		[EnumMember(Value = "WERK")]
		Werk = 3,
		[EnumMember(Value = "ANDER")]
		Overige = 4
	}

	/// <summary>
	/// Lidtypes uit Kipadmin
	/// </summary>
	[DataContract]
	public enum LidTypeEnum {[EnumMember]Kind, [EnumMember]Leiding, [EnumMember]Kader}

	/// <summary>
	/// Functies uit Kipadmin
	/// </summary>
	[DataContract]
	public enum FunctieEnum
	{
		[EnumMember] ContactPersoon = 168,
		[EnumMember] GroepsLeiding = 169,
		[EnumMember] Vb = 170,
		[EnumMember] FinancieelVerantwoordelijke = 153,
		[EnumMember] JeugdRaad = 17,
		[EnumMember] KookPloeg = 185,
		[EnumMember] Proost = 172
	}

	/// <summary>
	/// Afdelingen uit Kipadmin
	/// </summary>
	[DataContract]
	public enum AfdelingEnum
	{
		[EnumMember] Ribbels = 1,
		[EnumMember] Speelclub = 2,
		[EnumMember] Rakwis = 3,
		[EnumMember] Titos = 4,
		[EnumMember] Ketis = 5,
		[EnumMember] Aspis = 6,
		[EnumMember] Speciaal = 7
	}
}
