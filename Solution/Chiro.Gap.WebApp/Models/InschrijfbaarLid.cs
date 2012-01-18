using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using Chiro.Gap.ServiceContracts.DataContracts;

namespace Chiro.Gap.WebApp.Models
{
    /// <summary>
    /// Gegevens relevant voor een inschrijving, aangevuld met een boolean die aangeeft
    /// of de gelieerde persoon wel of niet ingeschreven moet worden
    /// </summary>
    [Serializable]
	public class InschrijfbaarLid: InTeSchrijvenLid
	{
        /// <summary>
        /// Enkel true als de persoon wel degelijk ingeschreven moet worden.
        /// </summary>
        public bool InTeSchrijven { get; set; }
	}
}