using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Chiro.Gap.Fouten.Exceptions
{
    /// <summary>
    /// Exceptie voor onbekende subgemeente
    /// </summary>
    public class BestaatAlException : System.Exception, ISerializable
    {

        public BestaatAlException() : base() { }
        public BestaatAlException(string message) : base(message) { }
        public BestaatAlException(string message, Exception inner) : base(message, inner) { }
        public BestaatAlException(SerializationInfo info, StreamingContext context) : base(info, context) { }
    }
}
