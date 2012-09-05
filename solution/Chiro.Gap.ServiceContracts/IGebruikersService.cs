using System.ServiceModel;
using Chiro.Gap.ServiceContracts.DataContracts;
using Chiro.Gap.ServiceContracts.FaultContracts;

namespace Chiro.Gap.ServiceContracts
{
    /// <summary>
    /// Interface voor de service voor gebruikersrechtenbeheer.
    /// </summary>
    [ServiceContract]
    public interface IGebruikersService
    {
        /// <summary>
        /// Als de persoon met gegeven <paramref name="gelieerdePersoonID"/> nog geen account heeft, wordt er een
        /// account voor gemaakt. Aan die account worden dan de meegegeven <paramref name="gebruikersRechten"/>
        /// gekoppeld.  Gebruikersrechten zijn standaard 14 maanden geldig.
        /// </summary>
        /// <param name="gelieerdePersoonID">ID van gelieerde persoon die rechten moet krijgen</param>
        /// <param name="gebruikersRechten">Rechten die de account moet krijgen. Mag leeg zijn. Bestaande 
        /// gebruikersrechten worden zo mogelijk verlengd als ze in <paramref name="gebruikersRechten"/> 
        /// voorkomen, eventuele bestaande rechten niet in <paramref name="gebruikersRechten"/> blijven
        /// onaangeroerd.
        /// </param>
        [OperationContract]
        [FaultContract(typeof(GapFault))]
        [FaultContract(typeof(FoutNummerFault))]
        void RechtenToekennen(int gelieerdePersoonID, GebruikersRecht[] gebruikersRechten);

        /// <summary>
        /// Neemt de alle gebruikersrechten van de gelieerde persoon met gegeven
        /// <paramref name="gelieerdePersoonID"/> af voor de groepen met gegeven <paramref name="groepIDs"/>
        /// </summary>
        /// <param name="gelieerdePersoonID">ID van gelieerde persoon met af te nemen gebruikersrechten</param>
        /// <param name="groepIDs">ID's van groepen waarvoor gebruikersrecht afgenomen moet worden.</param>
        /// <remarks>In praktijk gebeurt dit door de vervaldatum in het verleden te leggen.</remarks>
        [OperationContract]
        [FaultContract(typeof (GapFault))]
        [FaultContract(typeof (FoutNummerFault))]
        void GebruikersRechtenAfnemen(int gelieerdePersoonID, int[] groepIDs);
    }
}
