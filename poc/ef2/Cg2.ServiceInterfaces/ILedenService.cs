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
        /// Maakt een GelieerdePersoon lid voor het huidige werkjaar
        /// en bewaart in database.
        /// </summary>
        /// <param name="gelieerdePersoonID">ID van de gelieerde persoon</param>
        /// <returns>nieuw lidobject</returns>
        [OperationContract]
        Lid LidMakenEnBewaren(int gelieerdePersoonID);
    }
}
