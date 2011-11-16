using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Chiro.Gap.Diagnostics.ServiceContracts.DataContracts
{
    /// <summary>
    /// Informatie over een gelieerde persoon die via e-mail te bereiken is
    /// </summary>
    [DataContract]
    public class MailContactInfo
    {
        /// <summary>
        /// GelieerdePersoonID van de gelieerde persoon
        /// </summary>
        [DataMember]
        public int GelieerdePersoonID { get; set; }

        /// <summary>
        /// Voornaam en familienaam van de gelieerde persoon
        /// </summary>
        [DataMember]
        public string VolledigeNaam { get; set; }

        /// <summary>
        /// E-mailadres van de gelieerde persoon
        /// </summary>
        [DataMember]
        public string EmailAdres { get; set; }

        /// <summary>
        /// Als (en slechts als) <c>true</c>, dan is de gelieerde persoon GAV van zijn groep.
        /// </summary>
        [DataMember]
        public bool IsGav { get; set; }

        /// <summary>
        /// Als (en slechts als) <c>true</c>, dan is de gelieerde persoon contactpersoon van zijn groep.
        /// </summary>
        [DataMember]
        public bool IsContact { get; set; }
    }
}
