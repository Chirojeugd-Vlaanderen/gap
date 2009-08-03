using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cg2.Validatie
{
    /// <summary>
    /// Interface voor een dictionary die een foutbericht koppelt
    /// aan een component (key)
    /// </summary>
    public interface IValidatieDictionary
    {
        /// <summary>
        /// Voegt een bericht toe
        /// </summary>
        /// <param name="key">component waarop fout van toepassing</param>
        /// <param name="bericht">foutbericht</param>
        void BerichtToevoegen(string key, string bericht);
        /// <summary>
        /// Geeft enkel true als er geen fouten in de dictionary
        /// zitten.
        /// </summary>
        bool IsGeldig { get; }
    }
}
