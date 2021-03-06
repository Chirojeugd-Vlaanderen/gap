/*
 * Copyright 2008-2016 the GAP developers. See the NOTICE file at the 
 * top-level directory of this distribution, and at
 * https://gapwiki.chiro.be/copyright
 * Verfijnen gebruikersrechten Copyright 2015 Chirojeugd-Vlaanderen vzw
 * 
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 * 
 *     http://www.apache.org/licenses/LICENSE-2.0
 * 
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

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
        // Opgelet: Niveau.LidInGroep = 2*LidType.Kind
        // en Niveau.LeidingInGroep = 2*LidType.Leiding
        // Origineel was dit toevallig, maar dit wordt gebruikt in
        // GroepenService.svc.cs voor conversie.
        // Let dus goed op als je die zaken zou wijzigen.

        [EnumMember] Geen = 0x0,
        [EnumMember] Kind = 0x01,
        [EnumMember] Leiding = 0x02,
        [EnumMember] Alles = Kind | Leiding,
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
        X = 0x04,               // het derde geslacht
        [EnumMember]
        Gemengd = Man | Vrouw | X,	// interessant voor gemengde groepen/afdelingen
    }

    /// <summary>
    /// Soorten abonnement
    /// </summary>
    [DataContract]
    public enum AbonnementType
    {
        /// <summary>
        /// <c>AbonnementType.Geen</c> mag niet in de database bewaard worden; in dat geval
        /// moet het abonnement gewoon verdwijnen. <c>Geen</c> is enkel van toepassing
        /// in een webform.
        /// </summary>
        [EnumMember]
        Geen = 0,
        [EnumMember]
        Digitaal = 1,
        [EnumMember]
        Papier = 2,
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

        // Opgelet: Niveau.LidInGroep = 2*LidType.Kind
        // en Niveau.LeidingInGroep = 2*LidType.Leiding
        // Origineel was dit toevallig, maar dit wordt gebruikt in
        // GroepenService.svc.cs voor conversie.
        // Let dus goed op als je die zaken zou wijzigen.
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
    /// Types verzekering; moet overeenkomen met database!
    /// </summary>
    [DataContract]
    public enum Verzekering
    {
        [EnumMember]
        LoonVerlies = 1
    }

    /// <summary>
    /// Geeft aan of een werkJaar voorbij is, bezig is, of op zijn einde loopt (in overgang)
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2217:DoNotMarkEnumsWithFlags"), DataContract]
    [Flags]
    public enum WerkJaarStatus
    {
        [EnumMember]
        Onbekend = 0x00,
        /// <summary>
        /// Het werkjaar is voorbij.
        /// </summary>
        [EnumMember]
        Voorbij = 0x01,
        /// <summary>
        /// Het werkjaar is bezig.
        /// </summary>
        [EnumMember]
        Bezig = 0x02,
        /// <summary>
        /// De mogelijkheid bestaat om de jaarovergang te doen.
        /// </summary>
        [EnumMember]
        InOvergang = 0x06,	// bewust 0x06, omdat een werkJaar in overgang dan ook bezig is.
        /// <summary>
        /// Het werkjaar is bezig, maar er zijn nog geen leden waarvan de probeerperiode voorbij is.
        /// </summary>
        [EnumMember]
        KanTerugDraaien = 0x12
    }

	/// <summary>
	/// Geeft aan of de bivakaangifte al is ingevuld
	/// </summary>
	[DataContract]
    [Flags]
	public enum BivakAangifteStatus
	{
		[EnumMember]
		Ok = 0x00,
		[EnumMember]
		NogNietVanBelang = 0x01,
		[EnumMember]
		Ontbrekend = 0x02,
		[EnumMember]
		ContactOntbreekt = 0x04,
		[EnumMember]
		PlaatsOntbreekt = 0x08,
		[EnumMember]
		PlaatsEnContactOntbreekt = ContactOntbreekt|PlaatsOntbreekt,
        [EnumMember]
        Onbekend = 0x10
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
        EmailOntbreekt,
        [EnumMember]
        EmailIsVerdacht
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

    [DataContract]
    [Flags]
    public enum Permissies
    {
        /// <summary>
        /// Geen rechten
        /// </summary>
        [EnumMember] Geen = 0x00,
        /// <summary>
        /// Leesrechten
        /// </summary>
        [EnumMember] Lezen = 0x01,
        /// <summary>
        /// Lezen en schrijven
        /// </summary>
        [EnumMember] Bewerken = 0x3,
    }

    /// <summary>
    /// Zaken waarop je permissies kunt hebben.
    /// </summary>
    [Flags]
    public enum SecurityAspect
    {
        [EnumMember] PersoonlijkeGegevens = 0x01,
        [EnumMember] GroepsGegevens = 0x02,
        [EnumMember] PersonenInAfdeling = 0x10,
        [EnumMember] PersonenInGroep = 0x90,
        [EnumMember] AllesVanGroep = (GroepsGegevens|PersonenInGroep)
    }

    public static class Nieuwebackend
    {
        public static string Info
        {
            get { return "Gewoon een marker om bij te houden wat ik weggooide voor de nieuwe backend"; }
        }
    }
}