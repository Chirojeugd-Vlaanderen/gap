// <copyright company="Chirojeugd-Vlaanderen vzw">
// Copyright (c) 2007-2012
// Mail naar informatica@chiro.be voor alle info over deze broncode
// </copyright>

using System.Collections.Generic;

using Chiro.Gap.ServiceContracts.DataContracts;

namespace Chiro.Gap.WebApp.Models
{
    /// <summary>
    /// Model voor het overzicht van actieve GAVs.
    /// </summary>
    public class GavOverzichtModel : MasterViewModel
    {
        public IEnumerable<GebruikersDetail> GebruikersDetails { get; set; }
    }
}