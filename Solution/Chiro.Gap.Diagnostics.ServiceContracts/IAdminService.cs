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
        /// Hello-world method, enkel voor testing purposes
        /// </summary>
        /// <returns>"Hello World!"</returns>
        [OperationContract]
        string Hello();

        /// <summary>
        /// Haalt basisgegevens van de groep met stamnr <paramref name="code"/> op, 
        /// samen met de e-mailadressen van contactpersoon en gekende GAV's
        /// </summary>
        /// <param name="code">stamnummer van de groep</param>
        /// <returns>basisgegevens van de groep, en e-mailadressen van contactpersoon
        /// en gekende GAV's</returns>
        [OperationContract]
        GroepContactInfo ContactInfoOphalen(string code);

        /// <summary>
        /// Geeft de momenteel aangelogde gebruiker tijdelijke rechten voor de groep 
        /// van de gelieerde persoon met ID <paramref name="notificatieGelieerdePersoonID"/>.
        /// Deze gelieerde persoon wordt hiervan via  e-mail op de hoogte gebracht.  
        /// Uiteraard moet die gelieerde persoon een e-mailadres hebben. 
        /// De tijdelijke rechten zijn geldig voor het aantal dagen gegeven in de settings van
        /// <see name="Chiro.Gap.Diagnostics.Service" />
        /// Zo nodig kan een tijdelijke gebruiker zelf zijn eigen rechten verlengen.
        /// </summary>
        /// <param name="notificatieGelieerdePersoonID">GelieerdePersoonID die de groep
        /// bepaalt, en meteen ook diegene die via e-mail
        /// verwittigd wordt over de tijdelijke login</param>
        /// <param name="reden">Extra informatie die naar de notificatie-ontvanger wordt
        /// verstuurd.</param>
        [OperationContract]
        void TijdelijkeRechtenGeven(int notificatieGelieerdePersoonID, string reden);
    }
}
