using System;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace Chiro.Gap.ServiceContracts.DataContracts
{
    /// <summary>
    /// Basisinfo over gebruikersrechten
    /// </summary>
    [DataContract]
    public class GebruikersRechtInfo
    {
        /// <summary>
        /// Vervaldatum gebruikersrecht
        /// </summary>
        [DataMember]
        [DataType(DataType.Date)]
        public DateTime? VervalDatum { get; set; }

        /// <summary>
        /// Geeft aan of het gebruikersrecht verlengbaar is
        /// </summary>
        [DataMember]
        public bool Verlengbaar { get; set; }
    }
}
