using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using Chiro.Cdf.EfWrapper;

namespace Cg2.Orm
{
    [DataContract]
    public enum AdresTypeEnum
    {
        [EnumMember]
        Onbekend = 0
        ,
        [EnumMember]
        Thuis = 1
        ,
        [EnumMember]
        Kot = 2
            ,
        [EnumMember]
        Werk = 3
        ,
        [EnumMember]
        Overig = 4
    }

    public partial class PersoonsAdres: IBasisEntiteit
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

        public AdresTypeEnum AdresType
        {
            get { return (AdresTypeEnum)this.AdresTypeInt; }
            set { this.AdresTypeInt = (int)value; }
        }

        public override int GetHashCode()
        {
            return 2;
        }
    }
}
