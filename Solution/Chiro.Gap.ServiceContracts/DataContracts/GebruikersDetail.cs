using System;
using System.Runtime.Serialization;

namespace Chiro.Gap.ServiceContracts.DataContracts
{
    [DataContract]
    public class GebruikersDetail: GebruikersInfo
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
