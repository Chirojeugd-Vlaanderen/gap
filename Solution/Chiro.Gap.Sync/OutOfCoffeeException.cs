// <copyright company="Chirojeugd-Vlaanderen vzw">
// Copyright (c) 2007-2012
// Mail naar informatica@chiro.be voor alle info over deze broncode
// </copyright>

using System;

namespace Chiro.Gap.Sync
{
    /// <summary>
    /// Dom grapje voor KipSync.
    /// </summary>
    public class OutOfCoffeeException : Exception
    {
        /// <summary>
        /// Domme summary voor de constructor
        /// </summary>
        /// <param name="message">
        /// De boodschap die de exception aan de gebruiker moet geven
        /// </param>
        public OutOfCoffeeException(string message) : base(message)
        {
        }
    }
}
