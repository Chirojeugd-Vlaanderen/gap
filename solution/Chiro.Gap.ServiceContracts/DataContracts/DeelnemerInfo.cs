// <copyright company="Chirojeugd-Vlaanderen vzw">
//   Copyright (c) 2007-2012 Mail naar informatica@chiro.be voor alle info over deze broncode
// </copyright>

using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace Chiro.Gap.ServiceContracts.DataContracts
{
    /// <summary>
    /// Datacontract voor beperkte informatie over een deelnemer
    /// </summary>
    [DataContract]
    [KnownType(typeof(DeelnemerDetail))]
    public class DeelnemerInfo
    {
        /// <summary>
        /// De ID van de deelnemer
        /// </summary>
        [DataMember]
        public int DeelnemerID { get; set; }

        /// <summary>
        /// Geeft aan of de deelnemer logistiek medewerker is
        /// </summary>
        [DataMember]
        [DisplayName(@"Logistieke ploeg")]
        public bool IsLogistieker { get; set; }

        /// <summary>
        /// Geeft aan of de deelnemer betaald heeft
        /// </summary>
        [DataMember]
        [DisplayName(@"Inschrijvingsgeld betaald")]
        public bool HeeftBetaald { get; set; }

        /// <summary>
        /// Geeft aan of de medische fiche volledig ingevuld 
        /// en aan de organisatie bezorgd is
        /// </summary>
        [DataMember]
        [DisplayName(@"Medische fiche OK")]
        public bool MedischeFicheOk { get; set; }

        /// <summary>
        /// Eventuele extra info over de deelname
        /// </summary>
        [DataMember]
        [DataType(DataType.MultilineText)]
        public string Opmerkingen { get; set; }
    }
}