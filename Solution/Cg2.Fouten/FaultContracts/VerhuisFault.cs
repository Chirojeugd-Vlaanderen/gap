using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;

namespace Cg2.Fouten.FaultContracts
{
    [DataContract]
    public enum VerhuisFoutCode
    {
        [EnumMember]
        AlgemeneFout = 0,   // standaardwaarde
        [EnumMember]
        OnbekendeStraat,
        [EnumMember]
        OnbekendeGemeente
    }

    [DataContract]
    public class VerhuisFault : BusinessFault<VerhuisFoutCode> { }
}
