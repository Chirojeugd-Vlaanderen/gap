using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

using Chiro.Cdf.EfWrapper;

namespace Chiro.Gap.Orm
{
    [DataContract]
    public enum LidType {
        [EnumMember] Kind = 1,
        [EnumMember] Leiding = 2
    }

    public partial class Lid : IBasisEntiteit
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

        public override int GetHashCode()
        {
            return 5;
        }
    }
}
