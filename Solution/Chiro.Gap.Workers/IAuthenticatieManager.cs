// <copyright company="Chirojeugd-Vlaanderen vzw">
// Copyright (c) 2007-2010
// Mail naar informatica@chiro.be voor alle info over deze broncode
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Chiro.Gap.Workers
{
    /// <summary>
    /// Interface voor klasse die gebruiker moet authenticeren.
    /// Deze interface zal gebruikt worden om bij het automatisch
    /// testen via mocking een user te emuleren.
    /// </summary>
    public interface IAuthenticatieManager
    {
        /// <summary>
        /// Opvragen gebruikersnaam huidige gebruiker
        /// </summary>
        /// <returns>De gebruikersnaam in een string,  de lege string
        /// indien geen gebruiker aangemeld.</returns>
        string GebruikersNaamGet();
    }
}
