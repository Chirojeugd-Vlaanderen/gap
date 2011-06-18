// <copyright company="Chirojeugd-Vlaanderen vzw">
// Copyright (c) 2007-2011
// Mail naar informatica@chiro.be voor alle info over deze broncode
// </copyright>

using System.Collections.Generic;

using Chiro.Gap.Domain;
using Chiro.Gap.ServiceContracts.DataContracts;

namespace Chiro.Gap.WebApp.Models
{
    public class LedenLijstModel : MasterViewModel
    {
        public IList<LidOverzicht> LidInfoLijst { get; set; }
        public LidEigenschap GekozenSortering { get; set; }

        /// <summary>
        /// Een lijst met ID's van gelieerde personen, gebruikt voor selecties
        /// </summary>
        public IEnumerable<int> SelectieGelieerdePersoonIDs { get; set; }

        /// <summary>
        /// Kunnen de gegevens van de leden gewijzigd worden?
        /// (i.e. zijn ze lid in huidig werkjaar)
        /// </summary>
        public bool KanLedenBewerken { get; set; }
    }
}
