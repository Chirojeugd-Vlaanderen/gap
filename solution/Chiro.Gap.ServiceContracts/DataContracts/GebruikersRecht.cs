using System.Runtime.Serialization;
using Chiro.Gap.Domain;

namespace Chiro.Gap.ServiceContracts.DataContracts
{
    /// <summary>
    /// Een gebruikersrecht is niet veel meer dan een combinatie van rollen voor een groep.
    /// </summary>
    [DataContract]
    public class GebruikersRecht
    {
        /// <summary>
        /// ID van een groep
        /// </summary>
        [DataMember]
        public int GroepID { get; set; }

        [DataMember]
        public Rol Rol { get; set; }
    }
}
