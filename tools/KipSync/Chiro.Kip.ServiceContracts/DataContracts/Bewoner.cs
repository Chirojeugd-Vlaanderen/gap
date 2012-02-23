// <copyright company="Chirojeugd-Vlaanderen vzw">
//   Copyright (c) 2007-2012 Mail naar informatica@chiro.be voor alle info over deze broncode
// </copyright>

using System.Runtime.Serialization;

namespace Chiro.Kip.ServiceContracts.DataContracts
{
    /// <summary>
    /// Datacontract voor een 'bewoner'
    /// </summary>
    [DataContract]
    public class Bewoner
    {
        /// <summary>
        /// De persoon die woont
        /// </summary>
        [DataMember]
        public Persoon Persoon { get; set; }

        /// <summary>
        /// Hoedanigheid van het adres mbt de persoon (kot, thuis, werk,...)
        /// </summary>
        [DataMember]
        public AdresTypeEnum AdresType { get; set; }
    }
}