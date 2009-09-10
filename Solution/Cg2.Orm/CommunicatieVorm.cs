using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using Cg2.EfWrapper;

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
        Mail = 3,
        [EnumMember]
        WebSite = 4,
        [EnumMember]
        Msn = 5,
        [EnumMember]
        Jabber = 6
    }

    public partial class CommunicatieVorm: IBasisEntiteit
    {
        private bool _teVerwijderen = false;

        public bool TeVerwijderen
        {
            get { return _teVerwijderen; }
            set { _teVerwijderen = value; }
        }

        public string VersieString
        {
            get { return this.VersieStringGet(); }
            set { this.VersieStringSet(value); }
        }

        public CommunicatieType Type
        {
            get { return (CommunicatieType)this.CommunicatieTypeInt; }
            set { this.CommunicatieTypeInt = (int)value; }
        }
    }
}
