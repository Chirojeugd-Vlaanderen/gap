using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Chiro.Gap.ServiceContracts
{
    [DataContract]
    public class AfdelingInfo
    {
        [DataMember]
        public int ID;

        [DataMember]
        public string Naam;

        [DataMember]
        public string Afkorting;

    }
}