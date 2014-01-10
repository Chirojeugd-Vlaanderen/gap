/*
 * Copyright 2008-2013 the GAP developers. See the NOTICE file at the 
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

using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace Chiro.Gap.ServiceContracts.DataContracts
{
    /// <summary>
    /// DataContract voor summiere info over communicatievormen
    /// </summary>
    [DataContract]
    [KnownType(typeof(CommunicatieDetail))] // Laten weten welke subtypes gebruikt worden
    public class CommunicatieInfo
    {
        /// <summary>
        /// De standaardconstructor. Doet niets.
        /// </summary>
        public CommunicatieInfo()
        {
        }

        /// <summary>
        /// Draait inheritance om: mapt eigenschappen van <paramref name="detail"/>
        /// op die van een instantie van de basisclass
        /// </summary>
        /// <param name="detail">
        /// Gedetailleerde communicatie-info
        /// </param>
        public CommunicatieInfo(CommunicatieDetail detail)
        {
            ID = detail.ID;
            CommunicatieTypeID = detail.CommunicatieTypeID;
            CommunicatieTypeIsOptIn = detail.CommunicatieTypeIsOptIn;
            IsGezinsGebonden = detail.IsGezinsGebonden;
            IsVoorOptIn = detail.IsVoorOptIn;
            Nota = detail.Nota;
            Nummer = detail.Nummer;
            VersieString = detail.VersieString;
            Voorkeur = detail.Voorkeur;
        }

        /// <summary>
        /// Uniek identificatienummer
        /// </summary>
        [DataMember]
        public int ID { get; set; }

        /// <summary>
        /// De waarde voor de communicatievorm (kan ook bv. een e-mailadres zijn)
        /// </summary>
        // TODO: Opnieuw verplicht maken. Uitgezet voor #1723
        //[Verplicht]
        [DataMember]
        [StringLength(160)]
        public string Nummer { get; set; }

        /// <summary>
        /// Geeft aan of de gebruiker toestemming geeft aan Chirojeugd Vlaanderen
        /// om communicatievorm te gebruiken als dat voor het communicatietype 
        /// van toepassing is
        /// </summary>
        /// <remarks>Voorlopig is dat alleen voor e-mailadressen van belang, voor de 
        /// Snelleberichtenlijsten. Bij <c>true</c> geeft de gebruiker aan dat het adres
        /// ingeschreven mag worden op de Snelleberichtenlijst.</remarks>
        [DataMember]
        [DisplayName(@"Mag gebruikt worden voor Snelleberichtenlijsten")]
        public bool IsVoorOptIn { get; set; }

        /// <summary>
        /// Geeft aan of deze communicatievorm de voorkeur krijgt als de persoon
        /// verschillende communicatievormen van dit type heeft
        /// </summary>
        [DataMember]
        [DisplayName(@"Als standaard gebruiken?")]
        public bool Voorkeur { get; set; }

        /// <summary>
        /// Extra info over de persoon, die niet in een specifieke property thuishoort
        /// </summary>
        [DataMember]
        [StringLength(320)]
        [DataType(DataType.MultilineText)]
        [DisplayName(@"Noot")]
        public string Nota { get; set; }

        /// <summary>
        /// Geeft aan of de communicatievorm persoonlijk is (<c>false</c>) of dat ze
        /// door het hele gezin gebruikt wordt (<c>true</c>). Een vaste telefoon op het thuisadres
        /// is typisch gezinsgebonden, een gsm-nummer niet.
        /// </summary>
        [DataMember]
        [DisplayName(@"Voor heel het gezin")]
        public bool IsGezinsGebonden { get; set; }

        /// <summary>
        /// Geeft stringrepresentatie van Versie weer (hex).
        /// Nodig om versie te bewaren in MVC view, voor concurrencycontrole.
        /// </summary>
        [DataMember]
        public string VersieString { get; set; }

        /// <summary>
        /// De ID van het communicatietype
        /// </summary>
        /// <remarks>Overgenomen van geassocieerde CommunicatieType</remarks>
        [DataMember]
        [Verplicht]
        public int CommunicatieTypeID { get; set; }

        /// <summary>
        /// Geeft aan of iemand toestemming moet geven voor Chirojeugd Vlaanderen
        /// waarden voor dit communicatietype mag gebruiken
        /// </summary>
        /// <remarks>Overgenomen van geassocieerde CommunicatieType</remarks>
        [DataMember]
        public bool CommunicatieTypeIsOptIn { get; set; }
    }
}