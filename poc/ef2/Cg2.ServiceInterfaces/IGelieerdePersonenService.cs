using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using Cg2.Orm;

namespace Cg2.ServiceContracts
{
    // NOTE: If you change the interface name "IGelieerdePersonenService" here, you must also update the reference to "IGelieerdePersonenService" in Web.config.
    [ServiceContract]
    public interface IGelieerdePersonenService
    {
        /// <summary>
        /// Haalt een pagina met persoonsgegevens op van gelieerde personen van een groep
        /// </summary>
        /// <param name="groepID">ID van de betreffende groep</param>
        /// <param name="pagina">paginanummer (1 of hoger)</param>
        /// <param name="paginaGrootte">aantal records per pagina (1 of meer)</param>
        /// <param name="aantalOpgehaald">outputparameter; geeft effectief aantal opgehaalde personen weer</param>
        /// <returns>lijst van gelieerde personen met persoonsinfo</returns>
        [OperationContract]
        IList<GelieerdePersoon> PaginaOphalen(int groepID, int pagina, int paginaGrootte, out int aantalOpgehaald);

        /// <summary>
        /// Haalt een pagina met persoonsgegevens op van gelieerde personen van een groep,
        /// inclusief eventueel lidobject voor het recentste werkjaar.
        /// </summary>
        /// <param name="groepID">ID van de betreffende groep</param>
        /// <param name="pagina">paginanummer (1 of hoger)</param>
        /// <param name="paginaGrootte">aantal records per pagina (1 of meer)</param>
        /// <param name="aantalOpgehaald">outputparameter; geeft effectief aantal opgehaalde personen weer</param>
        /// <returns>lijst van gelieerde personen met persoonsinfo</returns>
        [OperationContract]
        IList<GelieerdePersoon> PaginaOphalenMetLidInfo(int groepID, int pagina, int paginaGrootte, out int aantalOpgehaald);

        /// <summary>
        /// Haalt gelieerd persoon op, incl. persoonsgegevens, communicatievormen en adressen
        /// </summary>
        /// <param name="gelieerdePersoonID">ID op te halen GelieerdePersoon</param>
        /// <returns>GelieerdePersoon met persoonsgegevens, communicatievorm en adressen</returns>
        [OperationContract]
        GelieerdePersoon DetailsOphalen(int gelieerdePersoonID);

        /// <summary>
        /// Bewaart nieuwe/gewijzigde gelieerde persoon
        /// </summary>
        /// <param name="persoon">Te bewaren persoon</param>
        [OperationContract]
        void Bewaren(GelieerdePersoon persoon);
    }
}
