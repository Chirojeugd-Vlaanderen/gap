using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Cg2.Fouten.FaultContracts
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
