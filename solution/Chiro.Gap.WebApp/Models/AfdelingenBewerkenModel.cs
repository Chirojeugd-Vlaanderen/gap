// <copyright company="Chirojeugd-Vlaanderen vzw">
// Copyright (c) 2007-2012
// Mail naar informatica@chiro.be voor alle info over deze broncode
// </copyright>

using System.Collections.Generic;

using Chiro.Gap.ServiceContracts.DataContracts;

namespace Chiro.Gap.WebApp.Models
{
    /// <summary>
    /// Model voor het bewerken van afdelingen van meer dan 1 persoon.
    /// </summary>
    public class AfdelingenBewerkenModel : MasterViewModel
    {
        public IEnumerable<ActieveAfdelingInfo> BeschikbareAfdelingen { get; set; }

        /// <summary>
        /// Details van de personen (om te laten zien)
        /// </summary>
        public IList<PersoonDetail> Personen { get; set; }

        /// <summary>
        /// ID's van de leden, voor postback
        /// </summary>
        public IEnumerable<int> LidIDs { get; set; }

        // AfdelingsJaarID of AfdelingsJaarIDs wordt gebruikt, alnaargelang de gebruiker
        // 1 of meerdere afdelingen in kon geven.  (Dit laaste is het geval als er enkel
        // leiding geselecteerd was.)

        public int AfdelingsJaarID { get; set; }
        public IEnumerable<int> AfdelingsJaarIDs { get; set; }
    }
}