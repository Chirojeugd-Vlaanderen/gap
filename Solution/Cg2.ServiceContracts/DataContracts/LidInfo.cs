using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using Cg2.Orm;

namespace Cg2.ServiceContracts
{
    [DataContract]
    public class LidInfo
    {
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
