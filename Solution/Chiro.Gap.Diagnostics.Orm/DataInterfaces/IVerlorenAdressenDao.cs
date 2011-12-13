using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Chiro.Cdf.Data;

namespace Chiro.Gap.Diagnostics.Orm.DataInterfaces
{
    /// <summary>
    /// Data access voor verloren adressen
    /// </summary>
    public interface IVerlorenAdressenDao: IDao<VerlorenAdres>
    {
        // Op dit moment niets speciaals, maar we hebben deze definitie wel nodig
        // om via dependency injection de juiste objectcontext te kiezen
        // (diagnosticEntities ipv chiroGroepEntitities)
    }
}
