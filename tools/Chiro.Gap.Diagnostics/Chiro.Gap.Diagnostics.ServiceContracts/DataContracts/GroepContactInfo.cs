using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Chiro.Gap.Diagnostics.ServiceContracts.DataContracts
{
    /// <summary>
    /// Datacontractje met groepsinformatie en e-mailadressen van contactpersonen
    /// </summary>
    [DataContract]
    public class GroepContactInfo
    {
        /// <summary>
        /// Naam van de groep
        /// </summary>
        [DataMember]
        public string Naam { get; set; }

        /// <summary>
        /// Stamnummer van de groep
        /// </summary>
        [DataMember]
        public string Code { get; set; }

        /// <summary>
        /// Plaats (gemeente, parochie,...) van de groep
        /// </summary>
        [DataMember]
        public string Plaats { get; set; }

        /// <summary>
        /// Contactpersonen met e-mailadres
        /// </summary>
        [DataMember]
        public IEnumerable<MailContactInfo> Contacten { get; set; }
    }
}
