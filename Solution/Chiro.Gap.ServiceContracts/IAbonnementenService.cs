using System.ServiceModel;
using Chiro.Gap.ServiceContracts.FaultContracts;

namespace Chiro.Gap.ServiceContracts
{
    /// <summary>
    /// Functionaliteit voor abonnementen krijgt nu ook een eigen service
    /// </summary>
    [ServiceContract]
    public interface IAbonnementenService
    {
        /// <summary>
        /// Bestelt Dubbelpunt voor de persoon met GelieerdePersoonID <paramref name="gelieerdePersoonID"/>.
        /// </summary>
        /// <param name="gelieerdePersoonID">ID van gelieerde persoon van persoon die Dubbelpunt wil</param>
        [OperationContract]
        [FaultContract(typeof(GapFault))]
        [FaultContract(typeof(FoutNummerFault))]
        void DubbelPuntBestellen(int gelieerdePersoonID);

        /// <summary>
        /// Controleert of de gelieerde persoon met gegeven <paramref name="id"/> recht heeft op gratis Dubbelpunt.
        /// </summary>
        /// <param name="id">GelieerdePersoonID</param>
        /// <returns><c>true</c> als de persoon zeker een gratis dubbelpuntabonnement krijgt voor jouw groep. <c>false</c>
        /// als de persoon zeker geen gratis dubbelpuntabonnement krijgt voor jouw groep. En <c>null</c> als het niet
        /// duidelijk is. (In praktijk: als de persoon contactpersoon is, maar als er meerdere contactpersonen zijn.)
        /// </returns>
        [OperationContract]
        bool? HeeftGratisDubbelpunt(int id);
    }
}
