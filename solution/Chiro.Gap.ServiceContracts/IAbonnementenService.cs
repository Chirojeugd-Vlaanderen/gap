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
    }
}
