using System;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace Chiro.Gap.ServiceContracts.DataContracts
{
    /// <summary>
    /// Basisinfo over gebruikersrechten
    /// </summary>
    [DataContract]
    public class GebruikersInfo
    {
        /// <summary>
        /// ID van het gebruikersrecht
        /// </summary>
        [DataMember]
        public int ID { get; set; }

        /// <summary>
        /// Vervaldatum gebruikersrecht
        /// </summary>
        [DataMember]
        [DataType(DataType.Date)]
        public DateTime? VervalDatum { get; set; }

        /// <summary>
        /// Login voor de gebruiker
        /// </summary>
        [DataMember]
        public string GavLogin { get; set; }

        /// <summary>
        /// Geeft aan of het gebruikersrecht verlengbaar is
        /// </summary>
        [DataMember]
        public bool Verlengbaar { get; set; }
    }
}
