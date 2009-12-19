using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Chiro.Gap.Orm;
using System.ServiceModel;

namespace Chiro.Gap.ServiceContracts
{
    [ServiceContract]
    interface IPersonenService
    {
        /// <summary>
        /// Eens ingelogd, kan je hiermee je huidige GAV entity opvragen
        /// </summary>
        /// <returns></returns>
        [OperationContract]
        Gav OphalenGAV();

        /// <summary>
        /// Vanuit je huidige GAV entity, alle groepen opvragen waarvan je admin bent
        /// </summary>
        /// <param name="g"></param>
        /// <returns></returns>
        [OperationContract]
        Gav OphalenGavMetGroepen(Gav g);

        /// <summary>
        /// Een groep toevoegen aan de rechten van de gegeven gav
        /// </summary>
        /// <param name="g"></param>
        [OperationContract]
        void GroepAanGavToevoegen(Gav g);

        /// <summary>
        /// Zorgen dat een gav geen rechten meer heeft voor een gegeven gav
        /// </summary>
        /// <param name="g"></param>
        /// <param name="groepID"></param>
        [OperationContract]
        void GroepVanGavVerwijderen(Gav g, int groepID);
    }
}
