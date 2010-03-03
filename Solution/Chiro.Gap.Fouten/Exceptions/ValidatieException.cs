// <copyright company="Chirojeugd-Vlaanderen vzw">
// Copyright (c) 2007-2010
// Mail naar informatica@chiro.be voor alle info over deze broncode
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

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
