using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using Chiro.Gap.Orm;

namespace Chiro.Gap.ServiceContracts
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
        public IList<int> AfdelingIdLijst { get; set; }

    }
}
