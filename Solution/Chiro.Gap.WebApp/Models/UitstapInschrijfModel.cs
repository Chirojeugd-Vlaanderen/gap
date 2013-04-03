// <copyright company="Chirojeugd-Vlaanderen vzw">
// Copyright (c) 2007-2012
// Mail naar informatica@chiro.be voor alle info over deze broncode
// </copyright>

using System.Collections.Generic;
using System.ComponentModel;

using Chiro.Gap.ServiceContracts.DataContracts;

namespace Chiro.Gap.WebApp.Models
{
    /// <summary>
    /// Model voor het inschrijven van personen voor een uitstap.
    /// </summary>
    public class UitstapInschrijfModel : MasterViewModel
    {
        /// <summary>
        /// Informatie over alle gelieerde personen die zullen worden ingeschreven.  Dit enkel ter
        /// verificatie.
        /// </summary>
        public IEnumerable<PersoonInfo> GelieerdePersonen;

        /// <summary>
        /// Bij postback zal deze lijst de GelieerdePersoonIDs bevatten, zodat de gelieerde
        /// personen in kwestie ingeschreven kunnen worden.
        /// </summary>
        public IList<int> GelieerdePersoonIDs { get; set; }

        /// <summary>
        /// Alle uitstappen waarvoor ingeschreven kan worden
        /// </summary>
        public IEnumerable<UitstapInfo> Uitstappen { get; set; }

		/// <summary>
		/// ID van de geselecteerde uitstap (voor postback)
		/// </summary>
		[DisplayName("Uitstap/bivak")]
		public int GeselecteerdeUitstapID { get; set; }

        /// <summary>
        /// Status van vinkje 'Is logistieke deelnemer'
        /// </summary>
        [DisplayName("Logistiek deelnemer, bv. kookploeg")]
        public bool LogistiekDeelnemer { get; set; }
    }
}