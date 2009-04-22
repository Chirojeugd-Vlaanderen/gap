using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using Cg2.Fouten.FaultContracts;

namespace Cg2.Fouten.Exceptions
{
    /// <summary>
    /// Exceptie voor onbekende subgemeente
    /// </summary>
    public class VerhuisException : System.Exception, ISerializable
    {
        private VerhuisFault _fault = null;

        public VerhuisException() : base() { }
        public VerhuisException(string message) : base(message) { }
        public VerhuisException(string message, Exception inner) : base(message, inner) { }
        public VerhuisException(SerializationInfo info, StreamingContext context) : base(info, context) { }
        public VerhuisException(VerhuisFault vf)
            : base()
        {
            _fault = vf;
        }


        public VerhuisFault Fault
        {
            get { return _fault; }
            set { _fault = value; }
        }
    }
}
