// <copyright file="GebruikersDetail.cs" company="Chirojeugd-Vlaanderen vzw">
//   Copyright (c) 2007-2012 Mail naar informatica@chiro.be voor alle info over deze broncode
// </copyright>

using System.Runtime.Serialization;

namespace Chiro.Gap.ServiceContracts.DataContracts
{
    /// <summary>
    /// TODO (#190): documenteren
    /// </summary>
    [DataContract]
    public class GebruikersDetail : GebruikersInfo
    {
        /// <summary>
        /// Voornaam van de GAV (als geweten)
        /// </summary>
        [DataMember]
        public string VoorNaam { get; set; }

        /// <summary>
        /// Naam van de GAV (als geweten)
        /// </summary>
        [DataMember]
        public string FamilieNaam { get; set; }

        /// <summary>
        /// PersoonID van de GAV (als geweten)
        /// </summary>
        [DataMember]
        public int? PersoonID { get; set; }

        /// <summary>
        /// GelieerdePersoonID van combinatie persoon-groep
        /// </summary>
        [DataMember]
        public int? GelieerdePersoonID { get; set; }
    }
}
