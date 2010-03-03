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
    /// <summary>
    /// Klasse voor een foutboodschap.
    /// </summary>
    /// <typeparam name="T">Type voor foutcodes (waarschijnlijk meestal enum)</typeparam>
    [DataContract]
    public class FoutBericht<T>
    {
        [DataMember]
        public T FoutCode { get; set; }
        [DataMember]
        public string Bericht { get; set; }     // omschrijving
    }
}
