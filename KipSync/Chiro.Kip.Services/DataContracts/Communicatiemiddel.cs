using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Chiro.Kip.Services.DataContracts
{
    [DataContract]
    public enum CommunicatieType
    {
        [EnumMember] Telefoon = 1,
        [EnumMember] Fax = 2,
        [EnumMember] Email = 3,
        [EnumMember] Website = 4,
        [EnumMember] Msn = 5,
        [EnumMember] Jabber = 6
    }

    [DataContract]
    public class Communicatiemiddel
    {
        [DataMember]
        public CommunicatieType Type { get; set; }

        [DataMember]
        public string Waarde { get; set; }

        /// <summary>
        /// De persoon wenst op dit Communicatiemiddel geen mailings te ontvangen
        /// </summary>
        [DataMember]
        public bool GeenMailings { get; set; }

    }
}
