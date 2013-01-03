// <copyright company="Chirojeugd-Vlaanderen vzw">
// Copyright (c) 2007-2012
// Mail naar informatica@chiro.be voor alle info over deze broncode
// </copyright>

namespace Chiro.Gap.WebApp.Models
{
    /// <summary>
    /// Model dat vraagt om bevestiging ivm een betalende operatie op een persoon
    /// </summary>
    public class BevestigingsModel : MasterViewModel
    {
        /// <summary>
        /// ID van een gelieerde persoon
        /// </summary>
        public int GelieerdePersoonID { get; set; }

        /// <summary>
        /// Indien relevant, een LidID van een lid van de gelieerde persoon.
        /// </summary>
        public int LidID { get; set; }

        /// <summary>
        /// Volledige naam van de persoon
        /// </summary>
        public string VolledigeNaam { get; set; }

        /// <summary>
        /// Wettegijwel wadatakost?
        /// </summary>
        public decimal Prijs { get; set; }

        /// <summary>
        /// Extra waarschuwing die getoond wordt indien niet leeg
        /// </summary>
        public string ExtraWaarschuwing { get; set; }
    }
}
