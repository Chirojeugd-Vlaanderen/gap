using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Chiro.Gap.Domain
{
    /// <summary>
    /// Interface voor een periode (van-tot), die we zullen gebruiken
    /// om te verifieren of een begindatum voor een einddatum ligt.
    /// </summary>
    public interface IPeriode
    {
        DateTime? DatumVan { get; }
        DateTime? DatumTot { get; }
    }
}
