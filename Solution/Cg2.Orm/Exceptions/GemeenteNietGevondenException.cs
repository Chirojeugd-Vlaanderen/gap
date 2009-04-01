using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Cg2.Orm.Exceptions
{
    /// <summary>
    /// Exceptie voor onbekende subgemeente
    /// </summary>
    public class GemeenteNietGevondenException : System.Exception, ISerializable
    {
        public GemeenteNietGevondenException() : base() { }
        public GemeenteNietGevondenException(string message) : base(message) { }
        public GemeenteNietGevondenException(string message, Exception inner) : base(message, inner) { }
        public GemeenteNietGevondenException(SerializationInfo info, StreamingContext context) : base(info, context) { }
    }
}
