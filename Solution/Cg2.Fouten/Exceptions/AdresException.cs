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
    public class AdresException : System.Exception, ISerializable
    {
        private AdresFault _fault = null;

        public AdresException() : base() { }
        public AdresException(string message) : base(message) { }
        public AdresException(string message, Exception inner) : base(message, inner) { }
        public AdresException(SerializationInfo info, StreamingContext context) : base(info, context) { }
        public AdresException(AdresFault vf)
            : base()
        {
            _fault = vf;
        }


        public AdresFault Fault
        {
            get { return _fault; }
            set { _fault = value; }
        }
    }
}
