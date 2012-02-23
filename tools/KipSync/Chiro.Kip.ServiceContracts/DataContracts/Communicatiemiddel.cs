// <copyright company="Chirojeugd-Vlaanderen vzw">
//   Copyright (c) 2007-2012 Mail naar informatica@chiro.be voor alle info over deze broncode
// </copyright>

using System.Runtime.Serialization;

namespace Chiro.Kip.ServiceContracts.DataContracts
{
    /// <summary>
    /// Datacontract voor een communicatietype
    /// </summary>
    [DataContract]
    public enum CommunicatieType
    {
        /// <summary>
        /// Aanduiding voor een telefoon- of gsm-nummer
        /// </summary>
        [EnumMember] TelefoonNummer = 1, 

        /// <summary>
        /// Aanduiding voor een faxnummer
        /// </summary>
        [EnumMember] Fax = 2, 

        /// <summary>
        /// Aanduiding voor een e-mailadres
        /// </summary>
        [EnumMember] Email = 3, 

        /// <summary>
        /// Aanduiding voor een (persoonlijke) website
        /// </summary>
        [EnumMember] WebSite = 4, 

        /// <summary>
        /// Aanduiding voor een MSN-adres
        /// </summary>
        [EnumMember] Msn = 5, 

        /// <summary>
        /// Aanduiding voor een XMPP-account
        /// </summary>
        [EnumMember] Xmpp = 6, 

        /// <summary>
        /// Aanduiding voor een Twitteraccount
        /// </summary>
        [EnumMember] Twitter = 7, 

        /// <summary>
        /// Aanduiding voor een StatusNet-account
        /// </summary>
        [EnumMember] StatusNet = 8
    }

    /// <summary>
    /// Datacontract voor een communicatiemiddel
    /// </summary>
    [DataContract]
    public class CommunicatieMiddel
    {
        /// <summary>
        /// Geeft aan over welk soort communicatiemiddel het gaat
        /// </summary>
        [DataMember]
        public CommunicatieType Type { get; set; }

        /// <summary>
        /// Het adres, het nummer of de accountnaam
        /// </summary>
        [DataMember]
        public string Waarde { get; set; }

        /// <summary>
        /// De persoon wenst via dit Communicatiemiddel al dan niet mailings te ontvangen,
        /// afhankelijk van de opgegeven waarde. Met andere woorden: dit bepaalt of het
        /// Communicatiemiddel gebruikt mag worden in bulkcommunicatie, bijvoorbeeld
        /// via de Snelleberichtenlijsten.
        /// </summary>
        [DataMember]
        public bool GeenMailings { get; set; }
    }
}