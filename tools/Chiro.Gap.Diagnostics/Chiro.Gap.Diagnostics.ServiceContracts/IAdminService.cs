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

        /// <summary>
        /// Haalt het aantal adressen op dat niet doorgekomen is naar Kipadmin.
        /// </summary>
        /// <returns>Het aantal (voorkeurs)adressen in GAP waarvoor de persoon in Kipadmin geen
        /// adres heeft.</returns>
        [OperationContract]
        int AantalVerdwenenAdressenOphalen();

        /// <summary>
        /// Synct de adressen die niet doorkwamen naar Kipadmin opnieuw
        /// </summary>
        [OperationContract]
        void OntbrekendeAdressenSyncen();

        /// <summary>
        /// Haalt het aantal functie-inconsistenties op voor het huidige werkjaar
        /// </summary>
        /// <returns>Het aantal functies in GAP dat niet in Kipadmin gevonden wordt, plus
        /// het aantal functies in Kipadmin dat niet in GAP gevonden wordt.</returns>
        [OperationContract]
        int AantalFunctieFoutenOphalen();

        /// <summary>
        /// Hersynchroniseert de functies van de leden met functieproblemen (huidig werkjaar)
        /// </summary>
        [OperationContract]
        void FunctieProbleemLedenOpnieuwSyncen();

        /// <summary>
        /// Haalt het aantal verdwenen bivakken voor dit werkjaar op
        /// </summary>
        /// <returns>Het aantal verdwenen bivakken.</returns>
        [OperationContract]
        int AantalVerdwenenBivakkenOphalen();
    }
}
