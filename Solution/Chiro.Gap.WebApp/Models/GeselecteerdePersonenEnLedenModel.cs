// <copyright company="Chirojeugd-Vlaanderen vzw">
// Copyright (c) 2007-2012
// Mail naar informatica@chiro.be voor alle info over deze broncode
// </copyright>

using System.Collections.Generic;

using Chiro.Gap.ServiceContracts.DataContracts;

namespace Chiro.Gap.WebApp.Models
{
    /// <summary>
    /// Wordt voor de jaarovergang gebruikt; lijst met leden die al dan niet over te zetten
    /// zijn naar het nieuwe werkjaar
    /// </summary>
    public class GeselecteerdePersonenEnLedenModel : MasterViewModel
    {
        /// <summary>
        /// Een lijst met alle nodige persoons en leden informatie.
        /// </summary>
        public InschrijfbaarLid[] PersoonEnLidInfos { get; set; }

        /// <summary>
        /// Lijst met de actieve afdelingen dit werkjaar
        /// </summary>
        public IEnumerable<ActieveAfdelingInfo> BeschikbareAfdelingen { get; set; }
    }
}