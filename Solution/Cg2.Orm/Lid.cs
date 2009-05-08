using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

using Cg2.EfWrapper;

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
    }
}
