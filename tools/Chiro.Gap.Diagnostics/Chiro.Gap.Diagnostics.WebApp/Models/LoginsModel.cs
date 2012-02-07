using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;

using Chiro.Gap.ServiceContracts.DataContracts;

namespace Chiro.Gap.Diagnostics.WebApp.Models
{
    /// <summary>
    /// Model voor het overzichtje van tijdelijke logins
    /// </summary>
    public class LoginsModel
    {
        /// <summary>
        /// Url voor GAP, als formatstring, waar {0} vervangen moet worden door de GroepID.
        /// </summary>
        public string GapUrl { get; set; }
        /// <summary>
        /// Lijst met informatie over groepen waar een gebruiker toegang toe heeft
        /// </summary>
        public IEnumerable<GroepInfo> Groepen { get; set; }
        /// <summary>
        /// Stamnummer van een eventuele nieuwe groep waartoe een gebruiker toegang wil
        /// </summary>
        [DisplayName(@"Stamnummer")]
        public string StamNummer { get; set; }
    }
}