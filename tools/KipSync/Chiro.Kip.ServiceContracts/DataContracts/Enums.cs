using System.Runtime.Serialization;

namespace Chiro.Kip.ServiceContracts.DataContracts
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
		[EnumMember]
		ContactPersoon = 168,
		[EnumMember]
		GroepsLeiding = 169,
		[EnumMember]
		Vb = 170,
		[EnumMember]
		FinancieelVerantwoordelijke = 153,
		[EnumMember]
		JeugdRaad = 17,
		[EnumMember]
		KookPloeg = 185,
		[EnumMember]
		Proost = 172,
		[EnumMember]
		GroepsLeidingsBijeenkomsten = 7,
		[EnumMember]
		SomVerantwoordelijke = 214,
		[EnumMember]
		IkVerantwoordelijke = 10,
		[EnumMember]
		RibbelVerantwoordelijke = 3,
		[EnumMember]
		SpeelclubVerantwoordelijke = 5,
		[EnumMember]
		RakwiVerantwoordelijke = 4,
		[EnumMember]
		TitoVerantwoordelijke = 6,
		[EnumMember]
		KetiVerantwoordelijke = 2,
		[EnumMember]
		AspiVerantwoordelijke = 1,
		[EnumMember]
		SomGewesten = 162,
		[EnumMember]
		OpvolgingStadsGroepen = 167,
		[EnumMember]
		Verbondsraad = 166,
		[EnumMember]
		Verbondskern = 156,
		[EnumMember]
		StartDagVerantwoordelijker = 160,
		[EnumMember]
		SbVerantwoordelijke = 152
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
		[EnumMember] Speciaal = 12
	}

	/// <summary>
	/// Geslachten
	/// </summary>
	[DataContract]
	public enum GeslachtsEnum
	{
		[EnumMember]
		Onbekend = 0,
		[EnumMember]
		Man = 1,
		[EnumMember]
		Vrouw = 2
	}

}
