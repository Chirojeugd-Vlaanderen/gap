using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Chiro.Gap.Fouten.Exceptions
{
    /// <summary>
    /// Exceptie voor fouten tegen validatieregels.
    /// </summary>
    public class ValidatieException : System.Exception, ISerializable
    {
        public ValidatieException() : base() { }
        public ValidatieException(string message) : base(message) { }
        public ValidatieException(string message, Exception inner) : base(message, inner) { }
        public ValidatieException(SerializationInfo info, StreamingContext context) : base(info, context) { }
    }
}
