// <copyright company="Chirojeugd-Vlaanderen vzw">
// Copyright (c) 2007-2012
// Mail naar informatica@chiro.be voor alle info over deze broncode
// </copyright>

using System.Runtime.Serialization;

namespace Chiro.Gap.ServiceContracts.DataContracts
{
    /// <summary>
    /// DataContract voor summiere info over aangesloten leden/leiding en hun afdeling(en)
    /// </summary>
    [DataContract]
    public class InTeSchrijvenLid
    {
        /// <summary>
        /// De volledige naam van de gelieerde persoon
        /// </summary>
        [DataMember]
        public string VolledigeNaam { get; set; }

        /// <summary>
        /// De id van de gelieerde persoon die we leiding willen maken
        /// </summary>
        [DataMember]
        public int GelieerdePersoonID { get; set; }

        /// <summary>
        /// Geeft aan of het een leiding moet worden ipv een kind
        /// </summary>
        [DataMember]
        public bool LeidingMaken { get; set; }

        /// <summary>
        /// De IDs van de eventuele gekozen afdelingsjaars.
        /// </summary>
        [DataMember]
        public int[] AfdelingsJaarIDs { get; set; }

        /// <summary>
        /// Boolean die aangeeft of het afdelingsjaar aangepast moet worden.
        /// </summary>
        public bool AfdelingsJaarIrrelevant;
    }
}
