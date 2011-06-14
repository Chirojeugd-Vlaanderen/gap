// <copyright company="Chirojeugd-Vlaanderen vzw">
// Copyright (c) 2007-2011
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
        public OutOfCoffeeException(string message)
            : base(message)
        {
        }
    }
}
