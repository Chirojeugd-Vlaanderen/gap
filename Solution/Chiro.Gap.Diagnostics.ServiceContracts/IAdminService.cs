using System.ServiceModel;

using Chiro.Gap.Diagnostics.ServiceContracts.DataContracts;

namespace Chiro.Gap.Diagnostics.ServiceContracts
{
    /// <summary>
    /// Webservice voor diagnostische en administratieve zaken
    /// </summary>
    [ServiceContract]
    public interface IAdminService
    {
        /// <summary>
        /// Haalt basisgegevens van de groep met stamnr <paramref name="code"/> op, 
        /// samen met de e-mailadressen van contactpersoon en gekende GAV's
        /// </summary>
        /// <param name="code">stamnummer van de groep</param>
        /// <returns>basisgegevens van de groep, en e-mailadressen van contactpersoon
        /// en gekende GAV's</returns>
        [OperationContract]
        GroepContactInfo ContactInfoOphalen(string code);
    }
}
