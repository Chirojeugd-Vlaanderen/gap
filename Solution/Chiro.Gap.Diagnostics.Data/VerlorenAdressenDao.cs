using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Chiro.Cdf.Data;
using Chiro.Cdf.Data.Entity;
using Chiro.Gap.Diagnostics.Orm;
using Chiro.Gap.Diagnostics.Orm.DataInterfaces;

namespace Chiro.Gap.Diagnostics.Data
{
    /// <summary>
    /// Data access wat betreft verloren adressen.
    /// </summary>
    public class VerlorenAdressenDao : Dao<VerlorenAdres, diagnosticEntities>, IVerlorenAdressenDao 
    {
        // Voorlopig implementeren we niets speciaals, maar we hebben deze definitie wel nodig
        // opdat de objectcontext juist zou zijn (diagnosticEntities en geen chiroGroepEntities)
    }
}
