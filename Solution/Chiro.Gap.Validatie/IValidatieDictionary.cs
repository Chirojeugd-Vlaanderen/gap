// <copyright company="Chirojeugd-Vlaanderen vzw">
// Copyright (c) 2007-2010
// Mail naar informatica@chiro.be voor alle info over deze broncode
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Chiro.Gap.Validatie
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
        /// <param name="key">Component waarop fout van toepassing</param>
        /// <param name="bericht">Foutbericht dat toegevoegd moet worden</param>
        void BerichtToevoegen(string key, string bericht);

        /// <summary>
        /// Geeft enkel true als er geen fouten in de dictionary
        /// zitten.
        /// </summary>
        bool IsGeldig { get; }
    }
}
