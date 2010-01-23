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
    public class OngeldigeActieException : System.Exception, ISerializable
    {

        public OngeldigeActieException() : base() { }
        public OngeldigeActieException(string message) : base(message) { }
        public OngeldigeActieException(string message, Exception inner) : base(message, inner) { }
		public OngeldigeActieException(SerializationInfo info, StreamingContext context) : base(info, context) { }
    }
}
