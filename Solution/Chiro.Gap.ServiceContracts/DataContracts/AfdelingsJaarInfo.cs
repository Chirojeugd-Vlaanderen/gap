using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace Chiro.Gap.ServiceContracts.DataContracts
{
    /// <summary>
    /// Basisafdelingsinfo met geboortejaren.
    /// </summary>
    [DataContract]
    public class AfdelingsJaarInfo: AfdelingInfo
    {
        [DataMember]
        public int GeboorteJaarVan { get; set; }

        [DataMember]
        public int GeboorteJaarTot { get; set; }
    }
}
