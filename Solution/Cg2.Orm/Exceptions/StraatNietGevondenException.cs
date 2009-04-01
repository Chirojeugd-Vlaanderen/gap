using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Cg2.Orm.Exceptions
{
    /// <summary>
    /// Exceptie voor onbekende straat
    /// </summary>
    public class StraatNietGevondenException : System.Exception, ISerializable
    {
        public StraatNietGevondenException() : base() { }
        public StraatNietGevondenException(string message) : base(message) { }
        public StraatNietGevondenException(string message, Exception inner) : base(message, inner) { }
        public StraatNietGevondenException(SerializationInfo info, StreamingContext context) : base(info, context) { }
    }
}
