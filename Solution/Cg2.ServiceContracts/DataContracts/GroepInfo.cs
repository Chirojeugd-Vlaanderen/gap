using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Cg2.ServiceContracts
{
    [DataContract]
    public class GroepInfo
    {
        [DataMember]
        public string Groepsnaam;

        [DataMember]
        public string Plaats;

        [DataMember]
        public string StamNummer;

    }
}