using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;

namespace Cg2.ServiceContracts.FaultContracts
{
    [DataContract]
    public enum FoutCode
    {
        [EnumMember]
        OnbekendeStraat,
        [EnumMember]
        OnbekendeGemeente
    }

    [DataContract]
    public class CgFaultException
    {
        [DataMember]
        public FoutCode Code;
        [DataMember]
        public string Boodschap;
    }
}
