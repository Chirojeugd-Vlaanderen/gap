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

	/// <summary>
	/// Sorteringsopties van een lijst van personen
	/// </summary>
	[DataContract]
	public enum PersoonSorteringsEnum
	{
		[EnumMember]
		Naam = 1,
		[EnumMember]
		Leeftijd = 2,
		[EnumMember]
		Categorie = 3
	}

	/// <summary>
	/// Enum voor (een aantal) eigenschappen relevant voor een lid.  In eerste instantie wordt deze enum
	/// gebruikt om te bepalen op welke kolom gesorteerd moet worden.
	/// </summary>
	[DataContract]
	public enum LidEigenschap
	{
		[EnumMember]
		Naam = 1,
		[EnumMember]
		Leeftijd = 2,
		[EnumMember]
		Afdeling = 3,
		[EnumMember]
		InstapPeriode = 4,
		[EnumMember]
		Adres = 5,
		[EnumMember]
		Verjaardag = 6
	}

	/// <summary>
	/// Enum voor ondersteunde communicatietypes
	/// </summary>
	/// <remarks>Moet overeenkomen met de database!</remarks>
	[DataContract]
	public enum CommunicatieTypeEnum
	{
		[EnumMember] TelefoonNummer = 1,
		[EnumMember] Fax = 2,
		[EnumMember] Email = 3,
		[EnumMember] WebSite = 4,
		[EnumMember] Msn = 5,
		[EnumMember] Xmpp = 6,
		[EnumMember] Twitter = 7,
		[EnumMember] StatusNet = 8
	}

	/// <summary>
	/// Types verzekering; moet overeenkomen met database!
	/// </summary>
	[DataContract]
	public enum Verzekering
	{
		[EnumMember]
		LoonVerlies = 1
	}

	/// <summary>
	/// Geeft aan of een werkjaar voorbij is, bezig is, of op zijn einde loopt (in overgang)
	/// </summary>
	[DataContract]
	[Flags]
	public enum WerkJaarStatus
	{
		[EnumMember]
		Onbekend = 0x00,
		[EnumMember]
		Voorbij = 0x01,
		[EnumMember]
		Bezig = 0x02,
		[EnumMember]
		InOvergang = 0x06	// bewust 0x06, omdat een werkjaar in overgang dan ook bezig is.
	}

	/// <summary>
	/// Sommige functies zijn gepredefinieerd, en hun codes moeten constant zijn.
	/// (Dit is enkel van toepassing op nationaal gedefinieerde functies)
	/// </summary>
	public enum NationaleFunctie
	{
		ContactPersoon = 1,
		GroepsLeiding = 2,
		Vb = 3,
		FinancieelVerantwoordelijke = 4,
		JeugdRaad = 5,
		KookPloeg = 6,
		Proost = 7
	};

	/// <summary>
	/// De ID's van de officiele afdelingen in GAP
	/// </summary>
	public enum NationaleAfdeling
	{
		Ribbels = 1,
		Speelclub = 2,
		Rakwis = 3,
		Titos = 4,
		Ketis = 5,
		Aspis = 6,
		Speciaal = 7
	}
}