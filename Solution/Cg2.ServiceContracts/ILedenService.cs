using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using Cg2.Orm;

namespace Cg2.ServiceContracts
{
    // NOTE: If you change the interface name "ILedenService" here, you must also update the reference to "ILedenService" in Web.config.
    [ServiceContract]
    public interface ILedenService
    {
        /// <summary>
        /// Een gelieerde persoon ophalen en die lid in het huidige werkjaar
        /// en bewaren in database.
        /// </summary>
        /// <param name="gelieerdePersoonID">ID van de gelieerde persoon</param>
        /// <returns>nieuw lidobject</returns>
        [OperationContract]
        Lid LidMakenEnBewaren(int gelieerdePersoonID);

        /// <summary>
        /// Haalt pagina met leden op uit bepaald groepswerkjaar
        /// </summary>
        /// <param name="groepsWerkJaarID">gevraagde groepswerkjaar</param>
        /// <param name="pagina">paginanr (1 of hoger)</param>
        /// <param name="paginaGrootte">aantal leden per pagina</param>
        /// <param name="aantalOpgehaald">hierin wordt bewaard hoeveel
        /// records er effectief opgehaald zijn</param>
        /// <returns>lijst met leden, inclusief info gelieerde personen
        /// en personen</returns>
        [OperationContract]
        IList<Lid> PaginaOphalen(int groepsWerkJaarID, int pagina, int paginaGrootte, out int aantalOpgehaald);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="?"></param>
        /// <returns></returns>
        [OperationContract]
        IList<Lid> LedenOphalenMetInfo(string name, IList<LidInfo> gevraagd); //andere searcharg


        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="?"></param>
        /// <returns></returns>
        [OperationContract]
        IList<Lid> LidOphalenMetInfo(int lidID, string name, IList<LidInfo> gevraagd); //andere searcharg


        /// <summary>
        /// ook om te maken en te deleten
        /// </summary>
        /// <param name="persoon"></param>
        [OperationContract]
        void LidBewaren(Lid lid);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="lid"></param>
        [OperationContract]
        void LidOpNonactiefZetten(Lid lid);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="lid"></param>
        [OperationContract]
        void LidActiveren(Lid lid);
    }
}
