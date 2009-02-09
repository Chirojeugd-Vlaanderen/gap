using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using Cg2.Orm;

namespace WebServices
{
    // NOTE: If you change the interface name "IPersonenService" here, you must also update the reference to "IPersonenService" in Web.config.
    [ServiceContract]
    [ServiceKnownType(typeof(Persoon))]
    [ServiceKnownType(typeof(GeslachtsType))]
    [ServiceKnownType(typeof(CommunicatieVorm))]
    public interface IPersonenService
    {
        /// <summary>
        /// Haalt persoon op uit database
        /// </summary>
        /// <param name="persoonID">ID op te halen persoon</param>
        /// <returns>Het gevraagde persoonsobject, null als niets gevonden
        /// is.</returns>
        [OperationContract]
        Persoon Ophalen(int persoonID);

        /// <summary>
        /// Haalt persoon op uit database, inclusief communicatiegegevens
        /// </summary>
        /// <param name="persoonID">ID op te halen persoon</param>
        /// <returns>Persoonsobject waarbij member Communicatie de
        /// communicatievormen van de persoon bevat.  Als de persoon niet
        /// werd gevonden, is het resultaat null.</returns>
        [OperationContract]
        Persoon OphalenMetCommunicatie(int persoonID);

        /// <summary>
        /// Enkel om te testen
        /// </summary>
        /// <returns>Een teststring</returns>
        [OperationContract]
        string Hallo();
    }
}
