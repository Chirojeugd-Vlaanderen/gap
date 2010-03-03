// <copyright company="Chirojeugd-Vlaanderen vzw">
// Copyright (c) 2007-2010
// Mail naar informatica@chiro.be voor alle info over deze broncode
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace Chiro.Gap.ServiceContracts.FaultContracts
{
    [DataContract]
    public enum AdresFaultCode
    {
        [EnumMember]
        AlgemeneFout = 0,   // standaardwaarde
        [EnumMember]
        OnbekendeStraat,
        [EnumMember]
        OnbekendeGemeente
    }

    [DataContract]
    public class AdresFault : DataContractFault<AdresFaultCode> { }
}
