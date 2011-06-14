// <copyright company="Chirojeugd-Vlaanderen vzw">
// Copyright (c) 2007-2011
// Mail naar informatica@chiro.be voor alle info over deze broncode
// </copyright>

using System.Collections.Generic;

using Chiro.Gap.ServiceContracts.DataContracts;

namespace Chiro.Gap.WebApp.Models
{
    /// <summary>
    /// Model met een rij uitstappen
    /// </summary>
    public class UitstapOverzichtModel : MasterViewModel
    {
        /// <summary>
        /// Een rij uitstappen
        /// </summary>
        public IEnumerable<UitstapInfo> Uitstappen { get; set; }
    }
}