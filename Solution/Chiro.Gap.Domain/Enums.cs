// <copyright company="Chirojeugd-Vlaanderen vzw">
// Copyright (c) 2007-2012
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
    /// Deelnemers, begeleiding, logistiek.  Wordt voorlopig enkel gebruikt voor uitstappen van een groep,
    /// waar dit zich vertaalt als kind, leiding, logistiek.
    /// </summary>
    [DataContract]
    [Flags]
    public enum DeelnemerType
    {
        [EnumMember]
        Onbekend = 0x00,
        [EnumMember]
        Deelnemer = 0x01,
        [EnumMember]
        Begeleiding = 0x02,
        [EnumMember]
        Logistiek = 0x04
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
    /// Niveau van een lid/groep/functie
    /// </summary>
    [DataContract]
    [Flags]
    public enum Niveau
    {
        // FUTURE
        //        [EnumMember]
        //        Satelliet = 0x01,
        [EnumMember] LidInGroep = 0x02,
        [EnumMember] LeidingInGroep = 0x04,
        [EnumMember] Groep = LidInGroep | LeidingInGroep,
        [EnumMember] Gewest = 0x08,
        [EnumMember] Verbond = 0x20,
        [EnumMember] Nationaal = 0x80,
        [EnumMember] KaderGroep = Gewest | Verbond | Nationaal,
        [EnumMember] Alles = /*Satelliet |*/ Groep | KaderGroep
    };

    public static class NiveauExtensions
    {
        public static bool HeeftNiveau(this Niveau orig, Niveau n)
        {
            return (n & orig) != 0;
        }
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
        [EnumMember]
        TelefoonNummer = 1,
        [EnumMember]
        Fax = 2,
        [EnumMember]
        Email = 3,
        [EnumMember]
        WebSite = 4,
        [EnumMember]
        Msn = 5,
        [EnumMember]
        Xmpp = 6,
        [EnumMember]
        Twitter = 7,
        [EnumMember]
        StatusNet = 8
    }

    /// <summary>
    /// Enum voor verschillende publicaties.
    /// Momenteel ondersteunen we enkel Dubbelpunt.
    /// </summary>
    public enum PublicatieID
    {
        Onbekend = 0,
        Dubbelpunt = 1
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
	/// Geeft aan of de bivakaangifte al is ingevuld
	/// </summary>
	[DataContract]
	public enum BivakAangifteStatus
	{
		[EnumMember]
		Onbekend,
		[EnumMember]
		Ingevuld,
		[EnumMember]
		NogNietVanBelang,
		[EnumMember]
		DringendInTeVullen,
		[EnumMember]
		PersoonInTeVullen,
		[EnumMember]
		PlaatsInTeVullen,
		[EnumMember]
		PlaatsEnPersoonInTeVullen
	}

    /// <summary>
    /// Mogelijke problemen bij gegevens van leden
    /// </summary>
    [DataContract]
    public enum LidProbleem
    {
        [EnumMember]
        Onbekend,
        [EnumMember]
        AdresOntbreekt,
        [EnumMember]
        TelefoonNummerOntbreekt,
        [EnumMember]
        EmailOntbreekt
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
        Proost = 7,
        GroepsLeidingsBijeenkomsten = 196 + 1,
        SomVerantwoordelijke = 196 + 2,
        IkVerantwoordelijke = 196 + 3,
        RibbelVerantwoordelijke = 196 + 4,
        SpeelclubVerantwoordelijke = 196 + 5,
        RakwiVerantwoordelijke = 196 + 6,
        TitoVerantwoordelijke = 196 + 7,
        KetiVerantwoordelijke = 196 + 8,
        AspiVerantwoordelijke = 196 + 9,
        SomGewesten = 196 + 10,
        OpvolgingStadsGroepen = 196 + 11,
        Verbondsraad = 196 + 12,
        Verbondskern = 196 + 13,
        StartDagVerantwoordelijker = 196 + 14,
        SbVerantwoordelijke = 196 + 15
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