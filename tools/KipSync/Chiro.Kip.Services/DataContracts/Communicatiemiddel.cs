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
	    [EnumMember]
	    TelefoonNummer = 1,
	    [EnumMember]
	    Fax = 2,
	    [EnumMember]
	    Email = 3,
	    [EnumMember]
	    WebSite = 4,
	    [EnumMember]
	    Msn = 5,
	    [EnumMember]
	    Xmpp = 6,
	    [EnumMember]
	    Twitter = 7,
	    [EnumMember]
	    StatusNet = 8
    }

    [DataContract]
    public class CommunicatieMiddel
    {
        [DataMember]
        public CommunicatieType Type { get; set; }

        [DataMember]
        public string Waarde { get; set; }

        /// <summary>
        /// De persoon wenst via dit Communicatiemiddel geen mailings te ontvangen
        /// </summary>
        [DataMember]
        public bool GeenMailings { get; set; }

    }
}
