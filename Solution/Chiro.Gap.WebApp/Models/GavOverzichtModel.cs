using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using Chiro.Gap.ServiceContracts.DataContracts;

namespace Chiro.Gap.WebApp.Models
{
    /// <summary>
    /// Model voor het overzicht van actieve GAVs.
    /// </summary>
    public class GavOverzichtModel: MasterViewModel
    {
        public IEnumerable<GebruikersDetail> GebruikersDetails { get; set; }
    }
}