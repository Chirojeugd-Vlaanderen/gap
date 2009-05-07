using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Cg2.Orm
{
    [DataContract]
    public enum GeslachtsType
    {
        [EnumMember] Man = 1
        , [EnumMember] Vrouw = 2
        , [EnumMember] Onbekend = 0
    }

    public partial class Persoon : IBasisEntiteit
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

        public GeslachtsType Geslacht
        {
            get { return (GeslachtsType)this.GeslachtsInt; }
            set { this.GeslachtsInt = (int)value; }
        }

        public string VolledigeNaam
        {
            get { return String.Format("{0} {1}", VoorNaam, Naam); }
        }

    }
}
