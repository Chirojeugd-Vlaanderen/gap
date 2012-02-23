// <copyright company="Chirojeugd-Vlaanderen vzw">
// Copyright (c) 2007-2012
// Mail naar informatica@chiro.be voor alle info over deze broncode
// </copyright>

using System;

namespace Chiro.Ad.ServiceContracts
{
    /// <summary>
    /// Dummy-implementatie van de AdService, voor gebruik bij testen.
    /// </summary>
    public class AdServiceMock : IAdService
    {
        /// <summary>
        /// Doet alsof het een login aanvraagt
        /// </summary>
        /// <param name="adNr">
        /// Het AD-nummer van de persoon die een login moet krijgen
        /// </param>
        /// <param name="voornaam">
        /// De voornaam van die persoon
        /// </param>
        /// <param name="familienaam">
        /// De familienaam van die persoon
        /// </param>
        /// <param name="mailadres">
        /// Het mailadres van die persoon
        /// </param>
        /// <returns>
        /// Een string die de login (accountnaam) voorstelt
        /// </returns>
        public string GapLoginAanvragen(int adNr, string voornaam, string familienaam, string mailadres)
        {
            return String.Format("ONGELDIG-{0}", adNr);
        }
    }
}
