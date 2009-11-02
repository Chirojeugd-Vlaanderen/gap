using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Chiro.Gap.ServiceContracts
{
    [DataContract]
    public class GroepInfo
    {
        [DataMember]
        public int ID;

        [DataMember]
        public string Naam;

        [DataMember]
        public string Plaats;

        [DataMember]
        public string StamNummer;

    }
}