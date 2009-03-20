using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Cg2.Orm
{
    /// <summary>
    /// stelt voor welke extra info er in een lid object opgeslagen is dat terug wordt gegeven.
    /// PERSOONSINFO
    /// VRIJEVELDEN
    /// AFDELINGSINFO
    /// FUNCTIES
    /// BIVAKINFO
    /// </summary>
    [DataContract]
    public enum LidInfo
    {
        [EnumMember]
        PERSOONSINFO,
        [EnumMember]
        VRIJEVELDEN,
        [EnumMember]
        AFDELINGSINFO,
        [EnumMember]
        FUNCTIES,
        [EnumMember]
        BIVAKINFO
    }

    public partial class Lid: IBasisEntiteit
    {
        public Lid()
        {
            BusinessKey = Guid.NewGuid();
        }

        public override bool Equals(object obj)
        {
            return this.MyEquals(obj);
        }

        public override int GetHashCode()
        {
            return this.MyGetHashCode();
        }

        public string VersieString
        {
            get { return this.VersieStringGet(); }
            set { this.VersieStringSet(value); }
        }
    }
}
