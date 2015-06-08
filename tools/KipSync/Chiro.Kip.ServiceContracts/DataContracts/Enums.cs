/*
 * Copyright 2008-2013, 2015 the GAP developers. See the NOTICE file at the 
 * top-level directory of this distribution, and at
 * https://develop.chiro.be/gap/wiki/copyright
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
using System.Runtime.Serialization;

namespace Chiro.Kip.ServiceContracts.DataContracts
{
    /// <summary>
    /// Adrestypes uit Kipadmin
    /// </summary>
    [DataContract]
    public enum AdresTypeEnum
    {
        // EnumMembers hebben een remoting Value om te mappen op KipAdmin

        /// <summary>
        /// Aanduiding voor een thuisadres
        /// </summary>
        [EnumMember(Value = "THUIS")] Thuis = 1, 

        /// <summary>
        /// Aanduiding voor een kotadres
        /// </summary>
        [EnumMember(Value = "KOT")] Kot = 2, 

        /// <summary>
        /// Aanduiding voor een werkadres
        /// </summary>
        [EnumMember(Value = "WERK")] Werk = 3, 

        /// <summary>
        /// Aanduiding voor elk ander soort adres
        /// </summary>
        [EnumMember(Value = "ANDER")] Overige = 4
    }

    /// <summary>
    /// Lidtypes uit Kipadmin
    /// </summary>
    [DataContract]
    public enum LidTypeEnum
    {
        /// <summary>
        /// Aanduiding voor een 'gewoon' lid, dus een kindje dat komt spelen
        /// </summary>
        [EnumMember] Kind, 

        /// <summary>
        /// Aanduiding voor iemand die leiding geeft in een lokale groep
        /// </summary>
        [EnumMember] Leiding, 

        /// <summary>
        /// Aanduiding voor iemand die een vrijwillig engagement opneemt in een gewest,
        /// een verbond of een nationale ploeg (werkgroep, commissie, redactie, enz.)
        /// </summary>
        [EnumMember] Kader
    }

    /// <summary>
    /// Functies uit Kipadmin
    /// </summary>
    [DataContract]
    public enum FunctieEnum
    {
        /// <summary>
        /// Aanduiding voor degene die de post moet krijgen en die het secretariaat moet opbellen
        /// als ze de groep moeten bereiken
        /// </summary>
        [EnumMember] ContactPersoon = 168, 

        /// <summary>
        /// Aanduiding voor degene die eindverantwoordelijkheid draagt binnen de leidingsploeg
        /// </summary>
        [EnumMember] GroepsLeiding = 169, 

        /// <summary>
        /// Aanduiding voor de volwassen begeleid(st)er
        /// </summary>
        [EnumMember] Vb = 170, 

        /// <summary>
        /// Aanduiding voor degene die de facturen moet krijgen
        /// </summary>
        [EnumMember] FinancieelVerantwoordelijke = 153, 

        /// <summary>
        /// Aanduiding voor degene die de groep vertegenwoordigt in de plaatselijke jeugdraad
        /// </summary>
        [EnumMember] JeugdRaad = 17, 

        /// <summary>
        /// Aanduiding voor degene van de leidingsploeg die contactpersoon is voor de kookploeg.
        /// Niet noodzakelijk iemand van de kookploeg zelf.
        /// </summary>
        [EnumMember] KookPloeg = 185, 

        /// <summary>
        /// Aanduiding voor een specifiek soort volwassen begeleid(st)er
        /// </summary>
        [EnumMember] Proost = 172, 

        /// <summary>
        /// Aanduiding voor degene die binnen de gewestploeg verantwoordelijk is voor groepsleidingsbijeenkomsten
        /// </summary>
        [EnumMember] GroepsLeidingsBijeenkomsten = 7, 

        /// <summary>
        /// Aanduiding voor degene die binnen de gewest- of verbondsploeg verantwoordelijk is voor Steun Op Maat
        /// </summary>
        [EnumMember] SomVerantwoordelijke = 214, 

        /// <summary>
        /// Aanduiding voor degene die binnen de gewestploeg verantwoordelijk is voor de organisatie
        /// van de Inleidingscursus (IK). Normaal gezien de cursustrekker.
        /// </summary>
        [EnumMember] IkVerantwoordelijke = 10, 

        /// <summary>
        /// Aanduiding voor degene binnen de gewest- of verbondsploeg die verantwoordelijk is voor de ribbelwerking
        /// </summary>
        [EnumMember] RibbelVerantwoordelijke = 3, 

        /// <summary>
        /// Aanduiding voor degene binnen de gewest- of verbondsploeg die verantwoordelijk is voor de speelclubwerking
        /// </summary>
        [EnumMember] SpeelclubVerantwoordelijke = 5, 

        /// <summary>
        /// Aanduiding voor degene binnen de gewest- of verbondsploeg die verantwoordelijk is voor de rakwiwerking
        /// </summary>
        [EnumMember] RakwiVerantwoordelijke = 4, 

        /// <summary>
        /// Aanduiding voor degene binnen de gewest- of verbondsploeg die verantwoordelijk is voor de titowerking
        /// </summary>
        [EnumMember] TitoVerantwoordelijke = 6, 

        /// <summary>
        /// Aanduiding voor degene binnen de gewest- of verbondsploeg die verantwoordelijk is voor de ketiwerking
        /// </summary>
        [EnumMember] KetiVerantwoordelijke = 2, 

        /// <summary>
        /// Aanduiding voor degene binnen de gewest- of verbondsploeg die verantwoordelijk is voor de aspiwerking
        /// </summary>
        [EnumMember] AspiVerantwoordelijke = 1, 

        /// <summary>
        /// Aanduiding voor degene binnen de verbondsploeg die zich Steun Op Maat bij gewestploegen aantrekt
        /// </summary>
        [EnumMember] SomGewesten = 162, 

        /// <summary>
        /// Aanduiding voor degene binnen de verbondsploeg die zich de stadswerking aantrekt
        /// </summary>
        [EnumMember] OpvolgingStadsGroepen = 167, 

        /// <summary>
        /// Aanduiding voor degene binnen de verbondsploeg die lid is van de verbondsraad
        /// </summary>
        [EnumMember] Verbondsraad = 166, 

        /// <summary>
        /// Aanduiding voor degene binnen de verbondsploeg die lid is van de verbondskern
        /// </summary>
        [EnumMember] Verbondskern = 156, 

        /// <summary>
        /// Aanduiding voor degene binnen de verbondsploeg die verantwoordelijk is voor de verbondelijke organisatie
        /// op de Startdag
        /// </summary>
        [EnumMember] StartDagVerantwoordelijker = 160, 

        /// <summary>
        /// Aanduiding voor degene binnen de verbondsploeg die verantwoordelijk is voor het Scholingsbivak (SB),
        /// normaal gezien de cursustrekker
        /// </summary>
        [EnumMember] SbVerantwoordelijke = 152
    }

    /// <summary>
    /// Afdelingen uit Kipadmin
    /// </summary>
    [DataContract]
    public enum AfdelingEnum
    {
        /// <summary>
        /// Aanduiding voor de afdeling voor 6-7-jarigen
        /// </summary>
        [EnumMember] Ribbels = 1, 

        /// <summary>
        /// Aanduiding voor de afdeling voor 8-9-jarigen
        /// </summary>
        [EnumMember] Speelclub = 2, 

        /// <summary>
        /// Aanduiding voor de afdeling voor 10-11-jarigen
        /// </summary>
        [EnumMember] Rakwis = 3, 

        /// <summary>
        /// Aanduiding voor de afdeling voor 12-13-jarigen
        /// </summary>
        [EnumMember] Titos = 4, 

        /// <summary>
        /// Aanduiding voor de afdeling voor 14-15-jarigen
        /// </summary>
        [EnumMember] Ketis = 5, 

        /// <summary>
        /// Aanduiding voor de afdeling voor 16-17-jarigen
        /// </summary>
        [EnumMember] Aspis = 6, 

        /// <summary>
        /// Aanduiding voor een afdeling waar leeftijd geen rol speelt
        /// </summary>
        [EnumMember] Speciaal = 12
    }

    /// <summary>
    /// De sekse-aanduiding
    /// </summary>
    [DataContract]
    public enum GeslachtsEnum
    {
        /// <summary>
        /// Aanduiding voor iemand van wie we het geslacht niet kennen
        /// </summary>
        [EnumMember] Onbekend = 0, 

        /// <summary>
        /// Aanduiding voor iemand van het mannelijke geslacht
        /// </summary>
        [EnumMember] Man = 1, 

        /// <summary>
        /// Aanduiding voor iemand van het vrouwelijke geslacht
        /// </summary>
        [EnumMember] Vrouw = 2,

        /// <summary>
        /// Het derde geslacht nemen we gewoon over uit GAP. Het kan op die
        /// manier in de database worden bewaart. Wat de kipadmin-app er dan
        /// verder nog mee doet, zullen we wel zien ;-)
        /// </summary>
        [EnumMember] X = 4,
    }
}