using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Objects.DataClasses;

namespace Cg2.Core.Domain
{
    /// <summary>
    /// Een Chirogroep is een 'echte' groep met leden en leiding.
    /// </summary>
    ///
    [Serializable]
    [EdmEntityTypeAttribute
        (NamespaceName="Cg2.Core.Domain", Name="ChiroGroep")]
    public class ChiroGroep: Groep
    {
    }
}
