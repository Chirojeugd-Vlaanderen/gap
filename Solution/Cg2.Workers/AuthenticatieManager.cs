using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;

namespace Chiro.Gap.Workers
{
    public class AuthenticatieManager: IAuthenticatieManager
    {
        #region IAuthenticatieManager Members

        /// <summary>
        /// Bepaalt de gebruikersnaam van de huidig aangemelde gebruiker.
        /// (lege string indien niet van toepassing)
        /// </summary>
        /// <returns>Username aangemelde gebruiker</returns>
        public string GebruikersNaamGet()
        {
            return ServiceSecurityContext.Current == null ? ""
                : ServiceSecurityContext.Current.WindowsIdentity.Name;
        }

        #endregion
    }
}
