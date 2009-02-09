using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Cg2.Orm
{
    [DataContract]
    public enum CommunicatieType
    {
        [EnumMember]
        Telefoon = 1,
        [EnumMember]
        Fax = 2,
        [EnumMember]
        EMail = 3,
        [EnumMember]
        WebSite = 4,
        [EnumMember]
        Msn = 5,
        [EnumMember]
        Jabber = 6
    }

    public partial class CommunicatieVorm
    {
        public CommunicatieType Type
        {
            get { return (CommunicatieType)this.CommunicatieTypeInt; }
            set { this.CommunicatieTypeInt = (int)value; }
        }
    }
}
