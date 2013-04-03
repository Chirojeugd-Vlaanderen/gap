// <copyright company="Chirojeugd-Vlaanderen vzw">
// Copyright (c) 2007-2012
// Mail naar informatica@chiro.be voor alle info over deze broncode
// </copyright>

using System;

using Chiro.Gap.ServiceContracts.DataContracts;

namespace Chiro.Gap.WebApp.Models
{
    /// <summary>
    /// Gegevens relevant voor een inschrijving, aangevuld met een boolean die aangeeft
    /// of de gelieerde persoon wel of niet ingeschreven moet worden
    /// </summary>
    [Serializable]
    public class InschrijfbaarLid : InTeSchrijvenLid
    {
        /// <summary>
        /// Enkel true als de persoon wel degelijk ingeschreven moet worden.
        /// </summary>
        public bool InTeSchrijven { get; set; }
    }
}