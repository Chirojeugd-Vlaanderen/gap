using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Chiro.Kip.Workers
{
    public class WerkJaarManager
    {
        /// <summary>
        /// Bepaalt het werkjaar dat gekoppeld is aan een datum.  De maand waarop het nieuwe werkjaar begint,
        /// zit in de settings.
        /// </summary>
        /// <param name="datum">Datum waarvan werkjaar te bepalen is</param>
        /// <returns>Beginjaar van het werkjaar.  Bijv. 2011 voor 2011-2012</returns>
        public int DatumNaarWerkJaar(DateTime datum)
        {
            return datum.Month >= Properties.Settings.Default.WerkJaarStartMaand ? datum.Year : datum.Year - 1;
        }
    }
}
