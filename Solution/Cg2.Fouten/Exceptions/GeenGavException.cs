using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Chiro.Gap.Fouten.Exceptions
{
    /// <summary>
    /// Exceptie voor operatie op objecten waarvoor de
    /// gebruiker geen GAV-rechten heeft.
    /// </summary>
    public class GeenGavException: System.Exception, ISerializable
    {
        public GeenGavException() : base() { }
        public GeenGavException(string message): base(message){}
        public GeenGavException(string message, Exception inner) : base(message, inner) { }
        public GeenGavException(SerializationInfo info, StreamingContext context) : base(info, context) { }
    }
}
