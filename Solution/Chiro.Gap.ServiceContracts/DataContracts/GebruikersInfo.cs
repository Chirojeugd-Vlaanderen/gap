// <copyright company="Chirojeugd-Vlaanderen vzw">
//   Copyright (c) 2007-2012 Mail naar informatica@chiro.be voor alle info over deze broncode
// </copyright>

using System;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using Chiro.Gap.Domain;

namespace Chiro.Gap.ServiceContracts.DataContracts
{
    /// <summary>
    /// Datacontract voor beperkte informatie over gebruikersrechten
    /// </summary>
    [DataContract]
    public class GebruikersInfo
    {
        /// <summary>
        /// Rol van de gebruiker
        /// </summary>
        /// <remarks>In praktijk heeft een niet-vervallen gebruiker voorlopig altijd
        /// de rol GAV. Zie ook #844</remarks>
        [DataMember]
        public Rol Rol { get; set; }

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
        public bool IsVerlengbaar { get; set; }
    }
}
