// <copyright company="Chirojeugd-Vlaanderen vzw">
// Copyright (c) 2007-2010
// Mail naar informatica@chiro.be voor alle info over deze broncode
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Chiro.Gap.Fouten.Exceptions
{
    /// <summary>
    /// Exception voor een 'Groep Mismatch', bijv. bij het koppelen
    /// van een categorie van groep A aan een gelieerd persoon van
    /// groep B.
    /// </summary>
    public class FoutieveGroepException: System.Exception, ISerializable
    {
        public FoutieveGroepException(string message) : base(message) { }
    }
}
