using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using Chiro.Gap.Orm;

namespace Cg2.ServiceContracts
{
    [DataContract]
    public class LidInfo
    {
        [DataMember]
        public int LidID { get; set; }

        [DataMember]
        public PersoonInfo PersoonInfo { get; set; }

        [DataMember]
        public bool LidgeldBetaald { get; set; }

        [DataMember]
        public LidType Type { get; set; }

        [DataMember]
        public String AfdelingString { get; set; }

    }
}
