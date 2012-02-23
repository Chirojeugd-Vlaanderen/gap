// <copyright company="Chirojeugd-Vlaanderen vzw">
//   Copyright (c) 2007-2012 Mail naar informatica@chiro.be voor alle info over deze broncode
// </copyright>

using System.ServiceModel;

namespace Chiro.Gap.Workers
{
    /// <summary>
    /// Worker die alle businesslogica i.v.m. authenticatie bevat
    /// </summary>
    public class AuthenticatieManager : IAuthenticatieManager
    {
        #region IAuthenticatieManager Members

        /// <summary>
        /// Bepaalt de gebruikersnaam van de huidig aangemelde gebruiker.
        /// (lege string indien niet van toepassing)
        /// </summary>
        /// <returns>
        /// Username aangemelde gebruiker
        /// </returns>
        public string GebruikersNaamGet()
        {
            return ServiceSecurityContext.Current == null
                       ? string.Empty
                       : ServiceSecurityContext.Current.WindowsIdentity.Name;
        }

        #endregion
    }
}